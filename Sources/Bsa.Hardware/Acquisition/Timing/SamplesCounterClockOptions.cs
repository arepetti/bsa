using System;

namespace Bsa.Hardware.Acquisition.Timing
{
    /// <summary>
    /// Specifies the options for the <see cref="SamplesCounterClock"/> timestamp generator.
    /// </summary>
    [Flags]
    public enum SamplesCounterClockOptions
    {
        /// <summary>
        /// Default options, clock isn't adjusted/compensated and it's strictly monotonic.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Reference time is adjusted when number of acquired samples reach the internal maximum
        /// number (see <see cref="SamplesCounterClock.MaximumNumberOfSamples"/>). Using this option
        /// lets you acquire an <em>infinte</em> number of samples (from timestamp generator point of view)
        /// but it may imply that generator isn't <see cref="TimestampGeneratorProperties.Monotonic"/>
        /// (if <see cref="SamplesCounterClockOptions.ForceMonotonic"/> isn't included) and that
        /// generator isn't <see cref="TimestampGeneratorProperties.UniformlyDistributed"/> (because
        /// it may introduce a discontinuity). Moreover error introduced by this fix is not deterministic
        /// (because it depends on run-time conditions) and it may block calling thread of
        /// <see cref="SamplesCounterClock.Increase"/> until new reference time is calculated.
        /// Note that this option may fail if <em>expected</em> time and current <em>system time</em> drifted
        /// too much (see <c>SamplesCounterClock.MaximumDelayForResynchronization</c>).
        /// <strong>Note</strong>: you probably do not need to specify this flag and you may keep
        /// the other properties of this generator; range <c>SamplesCounterClock.MaximumNumberOfSamples</c>
        /// is big enough to accomodate very large amount of samples (enough for an high frequency
        /// acquisition for many thousands years). To roughly calculate how many days you may acquire
        /// use this simple formula:
        /// <c>MaximumNumberOfSamples / Sampling rate [Hz] / 360 [seconds per hour] / 24 [hours per day]</c>.
        /// </summary>
        AdjustForOverflow = 1,

        /// <summary>
        /// When time is adjusted function must stay monotonic, if this flag is omitted and an adjustment
        /// occurs (for example because of compensation or because <see cref="SamplesCounterClockOptions.AdjustForOverflow"/>
        /// is specified) the function is forced to be strictly monotonic. If omitted, in those cases, this function
        /// may be not monotonic. Note that this flag doesn't affect if function is uniformly distributed or not.
        /// </summary>
        ForceMonotonic = 2,
    }
}
