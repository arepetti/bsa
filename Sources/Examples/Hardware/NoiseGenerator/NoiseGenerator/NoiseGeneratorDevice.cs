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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bsa.Hardware.Acquisition;
using Bsa.Instrumentation;

namespace Bsa.Examples.Hardware.NoiseGenerator
{
    public sealed class NoiseGeneratorDevice : AcquisitionDevice<PhysicalChannel>
    {
        public NoiseGeneratorDevice()
            : base(null)
        {
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                // Be careful: base.Dipose(disposing) will call DisconnectCore
                // when disposing is true (it's basic workflow to properly release device reasources)
                // then we should null these variables otherwise we will try to call .Change() on a disposed
                // object (DisconnectCore() will be called after this - as said by base class implementation).
                if (_dataTimer != null)
                {
                    _dataTimer.Dispose();
                    _dataTimer = null;
                }

                if (_ohmeterTimer != null)
                {
                    _ohmeterTimer.Dispose();
                    _ohmeterTimer = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected override void ConnectCore()
        {
        }

        protected override void DisconnectCore()
        {
            _generators = null;

            if (_dataTimer != null)
                _dataTimer.Change(Timeout.Infinite, -1);

            if (_ohmeterTimer != null)
                _ohmeterTimer.Change(Timeout.Infinite, -1);
        }

        protected override Bsa.Instrumentation.TelemetrySession CreateSession()
        {
            // We are not interested in any telemetry for this simple example
            return new NullTelemetrySession(base.CreateSession());
        }

        protected override void SetupCore()
        {
            base.SetupCore();

            _generators = Channels.Select(x => new Generator(x.Id, x.Range)).ToArray();

            // This is an example, a true device driver should not generate one sample
            // for each tick (better to pack them in bigger DataPacket) and also interval measurement
            // should be more precise. Expect floating point errors (and 1 ms rounding!) if you calculate
            // timer interval in this way.
            TimeSpan timerInterval = TimeSpan.FromSeconds(1 / Channels.First().SamplingRate);
            _dataTimer = new System.Threading.Timer(OnDataTimerTick, null, TimeSpan.Zero, timerInterval);
        }

        protected override void ChangeAcquisitionMode(AcquisitionMode mode)
        {
            if (Mode == AcquisitionMode.Ohmeter)
                _ohmeterTimer.Change(Timeout.Infinite, -1);

            base.ChangeAcquisitionMode(mode);
        }

        private static readonly TimeSpan OhmeterTick = TimeSpan.FromSeconds(5);

        private readonly Random _randomImpedanceGenerator = new Random();
        private Generator[] _generators;
        private System.Threading.Timer _dataTimer, _ohmeterTimer;

        private bool IsFeatureOhmeterAvailable()
        {
            return true;
        }

        private bool IsFeatureOhmeterEnabled()
        {
            return Mode == AcquisitionMode.Ohmeter;
        }

        private bool PerformOhmeter()
        {
            if (_ohmeterTimer == null)
                _ohmeterTimer = new Timer(OnOhmeterTimerTick, null, OhmeterTick, OhmeterTick);
            else
                _ohmeterTimer.Change(OhmeterTick, OhmeterTick);

            return true;
        }

        private bool IsFeatureCalibrationAvailable()
        {
            return true;
        }

        private bool IsFeatureCalibrationEnabled()
        {
            return Mode == AcquisitionMode.Calibration;
        }

        private void OnDataTimerTick(object state)
        {
            // Same considerations of SetupCore(), generating 1 sample each time
            // is a big performance hit and resource consuming, collect data in bigger packets (if possible).
            double[][] samples = new double[Channels.Count][];
            for (int i = 0; i < samples.Length; ++i)
                samples[i] = new double[] { NextValueForChannel(i) };

            // Note that here we're using DateTime.Now but it's a serious error in true
            // code: this clock is not monotinic increasing, it's unrelated to interval timing
            // and it's time consuming to calculate. See other examples for a more reliable implementation
            // of timestamp calculation (if it's not provided by device itself).
            OnData(new DataEventArgs(new DataPacket(Id, DateTime.Now, samples)));
        }

        private void OnOhmeterTimerTick(object state)
        {
            // For testing purposes we generate a random impedance between 1000 and 1500 Ohm. Variation is "unnatural"
            // but we use it only in our tests.
            OnOhmeter(new OhmeterEventArgs(
                Channels.Select(x => new PhysicalChannelImpedance(x.Id, new Complex(_randomImpedanceGenerator.Next(1000, 1500), 0)))));
        }

        private double NextValueForChannel(int channelIndex)
        {
            if (Mode == AcquisitionMode.Data)
                return _generators[channelIndex].Next();

            if (Mode == AcquisitionMode.Calibration)
                return (DateTime.Now.Second & 1) == 0 ? Channels[channelIndex].Range.Minimum : Channels[channelIndex].Range.Maximum;

            return 0;
        }
    }
}
