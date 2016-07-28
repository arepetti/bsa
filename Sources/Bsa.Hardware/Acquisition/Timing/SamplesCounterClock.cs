//
// This file is part of Biological Signals Acquisition Framework (BSA-F).
// Copyright © Adriano Repetti 2016.
//
// BSA-F is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// BSA-F is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with BSA-F.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.ComponentModel;
using System.Threading;

namespace Bsa.Hardware.Acquisition.Timing
{
    /// <summary>
    /// Implementation of <see cref="TimestampGenerator"/> which counts number of acquired samples to determine
    /// current timestamp.
    /// </summary>
    public sealed class SamplesCounterClock : TimestampGenerator
    {
        /// <summary>
        /// Maximum number of samples that can be acquired if <see cref="SamplesCounterClockOptions.AdjustForOverflow"/>
        /// isn't specified.
        /// </summary>
        /// <devdoc>
        /// This is the maximum integer number we can store in a <c>double</c> without losing precision. Note
        /// that calculations in <see cref="Current"/> are done using doubles.
        /// </devdoc>
        public const long MaximumNumberOfSamples = 9007199254740991; // 2^53 - 1

        /// <summary>
        /// Initializes a new instance of <see cref="SamplesCounterClock"/>.
        /// </summary>
        /// <param name="samplingRate">Acquired samples sampling rate.</param>
        /// <param name="options">Options for this timestamp generator, these options also affect generator properties.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="samplingRate"/> is 0 or a negative number.
        /// </exception>
        /// <exception cref="HardwareException">
        /// If requested sampling rate <paramref name="samplingRate"/> is too high for this timestamp generator
        /// and required precision cannot be guaranteed. In this case inner <see cref="HardwareError"/> will
        /// have class <see cref="HardwareErrorClass.Unsupported"/>.
        /// </exception>
        public SamplesCounterClock(double samplingRate, SamplesCounterClockOptions options)
            : base(PropertiesFromOptions(options))
        {
            if (samplingRate <= 0)
                throw new ArgumentOutOfRangeException("Sampling rate cannot be zero or negative.", "samplingRate");

            _options = options;

            unchecked
            {
                _ticksPerSample = TimeSpan.TicksPerSecond / samplingRate;
                Error = Math.Floor(_ticksPerSample) - _ticksPerSample;
            }

            EnsureCounterHasEnoughPrecision();
        }

        /// <summary>
        /// Gets the timestamp of last acquired sample.
        /// </summary>
        /// <value>
        /// Timestamp of last acquired sample or reference time if no samples have been acquired yet.
        /// </value>
        /// <remarks>
        /// Reading this value requires to know reference time, it implies that if it has not been set
        /// writing <see cref="Reference"/> then current UTC system time will be used (and <c>Reference</c>
        /// cannot be written later).
        /// </remarks>
        public override DateTimeOffset Current
        {
            get
            {
                return ResolveTime(GetReferenceTime(), _numberOfElapsedSamples);
            }
        }

        /// <summary>
        /// Gets/sets reference time.
        /// </summary>
        /// <value>
        /// The reference time used as base for calculations (summing up duration of all acquired samples at this moment)
        /// or <see langword="null"/> if reference time is unknown.
        /// When writing, if property has not been set before and specified value is <see langword="null"/> then this call has no effect.
        /// </value>
        /// <exception cref="HardwareException">
        /// When writing a new value if reference time has been explictely set before writing to <see cref="Reference"/> or implictly
        /// calling <see cref="Increase"/> or reading <see cref="Current"/>.
        /// </exception>
        public DateTimeOffset? Reference
        {
            get { return _reference; }
            set
            {
                if (_reference.HasValue)
                {
                    throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error, HardwareErrorClass.Internal, HardwareErrorCodes.Internal.InvalidOperation,
                        "Reference time cannot be changed after it has been calculated first time (after a call to Increase() or Current)."));
                }

                _reference = value;
            }
        }

        /// <summary>
        /// Increase the timestamp (<em>current time</em>) for the specified number of samples.
        /// </summary>
        /// <param name="numberOfSamples">Number of newly acquired samples</param>
        /// <returns>
        /// The timestamp of the first newly acquired sample, for example if 10 samples have been acquired
        /// (<paramref name="numberOfSamples"/> is 0) then this timestamp refers to sample #1 (the one with index
        /// 0 in an hypotetical <c>double[10]</c> array of samples). If <paramref name="numberOfSamples"/> is
        /// 0 then returned value is <see cref="Current"/>.
        /// </returns>
        /// <remarks>
        /// When maximum number of acquired samples <see cref="MaximumNumberOfSamples"/> is reached and
        /// <see cref="SamplesCounterClockOptions.AdjustForOverflow"/> specified then this function wil adjust
        /// reference time and reset samples counter, note that if <see cref="SamplesCounterClockOptions.ForceMonotonic"/>
        /// is specified then calling thread may be blocked in spin waiting until requested condition is satisfied.
        /// </remarks>
        /// <exception cref="HardwareException">
        /// If maximum number of acquired samples is reached and <see cref="SamplesCounterClockOptions.AdjustForOverflow"/>
        /// is not specified; error will be <see cref="HardwareErrorClass.Generic"/>
        /// with error code equals to <see cref="HardwareErrorCodes.Generic.LimitReached"/>.
        /// <br/>-or-<br/>
        /// If you acquired too many samples and you reached <see cref="DateTime.MaxValue"/>, it may be probably caused
        /// by a wrong setup where you acquire much faster than supposed sampling rate. In this case error cannot be
        /// managed internally and exception will always be thrown.
        /// <br/>-or-<br/>
        /// If number of acquired samples is reached and <see cref="SamplesCounterClockOptions.AdjustForOverflow"/>
        /// is specified but new reference time cannot be acquired in a reasonable amount of time; error
        /// will be <see cref="HardwareErrorClass.Generic"/> with error code <see cref="HardwareErrorCodes.Generic.Timeout"/>.
        /// </exception>
        public override DateTimeOffset Increase(uint numberOfSamples)
        {
            if (numberOfSamples == 0)
                return Current;

            // _numberOfElapsedSamples is number of already acquired samples, +1 to get the timestamp
            // of this acquired packet (with numberOfSamples samples inside).
            var firstSampleTimestamp = ResolveTime(GetReferenceTime(), _numberOfElapsedSamples + 1);

            _numberOfElapsedSamples += numberOfSamples;
            if (_numberOfElapsedSamples > MaximumNumberOfSamples)
            {
                if (!_options.HasFlag(SamplesCounterClockOptions.AdjustForOverflow))
                {
                    throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error, HardwareErrorClass.Generic, HardwareErrorCodes.Generic.LimitReached,
                        "You reached the maximum number of acquired samples you can track with this clock generator."));
                }

                CalculateNewReferenceTime();
            }

            return firstSampleTimestamp;
        }

        // This is the minimum number of ticks we require, obviously...less than this we can't count.
        private const int MinimumNumberOfTicksPerSample = 1;

        // This is the maximum % error we accept in the conversion from sampling rate to number of ticks.
        // Note that this is not the measurement error because errors may sum up! Also note that this is
        // not the value of Error property (which is measured in ticks).
        private const double MaximumErrorRatio = 0.02;

        // Maximum allowed delay (in milliseconds) when counter needs to calculate a new reference time
        // or to perform some adjustment/compensation.
        private const int MaximumDelayForResynchronization = 5;

        private SamplesCounterClockOptions _options;
        private DateTimeOffset? _reference;
        private readonly double _ticksPerSample;
        private long _numberOfElapsedSamples;

        private static TimestampGeneratorProperties PropertiesFromOptions(SamplesCounterClockOptions options)
        {
            if (options.HasFlag(SamplesCounterClockOptions.AdjustForOverflow))
            {
                if (options.HasFlag(SamplesCounterClockOptions.ForceMonotonic))
                    return TimestampGeneratorProperties.Monotonic;

                return TimestampGeneratorProperties.None;
            }

            return TimestampGeneratorProperties.Monotonic
                | TimestampGeneratorProperties.UniformlyDistributed;
        }

        private void EnsureCounterHasEnoughPrecision()
        {
            if (_ticksPerSample < MinimumNumberOfTicksPerSample)
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error, HardwareErrorClass.Unsupported, HardwareErrorCodes.Unsupported.Generic,
                    "Sampling frequency is too high to be used with this generator."));
            }

            if (Math.Abs(Error) / _ticksPerSample > MaximumErrorRatio)
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error, HardwareErrorClass.Unsupported, HardwareErrorCodes.Unsupported.Generic,
                    "Sampling frequency is too high to be used with this generator."));
            }
        }

        private DateTimeOffset ResolveTime(DateTimeOffset reference, long offsetInSamples)
        {
            // Note that we do not keep a "live" current time but we calculate offset from reference time
            // for each request. It's slightly slower and more complicate but here we're trying to minimize
            // error and its distribution (otherwise rounding errors will sum up for each call to Increase()).
            // Also note that doing this calculation with System.Double we can effectively use only 53 bit.
            try
            {
                unchecked
                {
                    return reference + new TimeSpan((long)(offsetInSamples * _ticksPerSample));
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error, HardwareErrorClass.Generic, HardwareErrorCodes.Generic.LimitReached,
                    "You acquired too many samples, timestamp cannot be represented with a DateTimeOffset object."), e);
            }
        }

        private DateTimeOffset GetReferenceTime()
        {
            // If no one set a reference time then we pick system time the first time we need it.
            // It's reasonable for calls to Increase() because (more or less) it will be called
            // when first packet has been acquired however if, for any reason, someone reads Custom
            // long before calling Increase() then there will be a (possibly) not small discrepancy.
            if (_reference == null)
                _reference = DateTimeOffset.UtcNow;

            return _reference.Value;
        }

        private void CalculateNewReferenceTime()
        {
            if (_options.HasFlag(SamplesCounterClockOptions.ForceMonotonic))
            {
                var current = ResolveTime(_reference.Value, _numberOfElapsedSamples);

                if (!SpinWait.SpinUntil(() => (_reference = DateTimeOffset.UtcNow).Value < current, MaximumDelayForResynchronization))
                {
                    throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error, HardwareErrorClass.Generic, HardwareErrorCodes.Generic.Timeout,
                        "New reference time for timestamp generator cannot be calculated in a reasonable amount of time (it may be caused by a too big drift between expected time and system time)."));
                }

                _numberOfElapsedSamples = 0;
            }
            else
            {
                _reference = DateTimeOffset.UtcNow;
                _numberOfElapsedSamples = 0;
            }
        }
    }
}
