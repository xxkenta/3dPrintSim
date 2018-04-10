using System;
using Firmware;
using Hardware;
using System.Collections.Generic;

namespace Firmware
{
    public class ZRailController
    {
        private List<double> positionHistory;
        public ZRailController()
        {
            positionHistory = new List<double>();
        }

        public int ConvertZ(int steps) 
        {
            return 0;
        }

        public void LimitVelocity()
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
