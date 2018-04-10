using System;
using Firmware;
using Hardware;
using System.Collections.Generic;

namespace Firmware
{
    public class ZRailController
    {
<<<<<<< HEAD
        private const int MaxZRailVolocity = 16080;

=======
        private List<double> positionHistory;
>>>>>>> 32c288a3a2af9cbebaad84021d2061c74346edfb
        public ZRailController()
        {
            positionHistory = new List<double>();
        }

        public int ConvertZ(int mmHeight) 
        {
            int steps = mmHeight * 400;

            return steps;
        }

        public void LimitVelocity(PrinterControl.StepperDir dir)
        {
      
        }

        public void LimitAcceleration()
        {
            
        }

        // Should be called whenever position is changed.
        public void trackHistory(double lastPosition)
        {
           positionHistory.Add(lastPosition);
        }
    }
}
