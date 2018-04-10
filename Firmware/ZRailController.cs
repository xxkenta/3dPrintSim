using System;
using Firmware;
using Hardware;
using System.Collections.Generic;

namespace Firmware
{
    public class ZRailController
    {

        private const int MaxZRailVelocity = 16080;
        private List<double> positionHistory;

        public ZRailController()
        {
            positionHistory = new List<double>();
        }

        public int ConvertZRailMMToSteps(int mmHeight) 
        {
            int steps = mmHeight * 400;

            return steps;
        }

        public void LimitVelocity(PrinterControl.StepperDir dir)
        {
      
        }

        public void GetVelocity(int initialPosition, int lastPosition, int timerTime)
        {
            int currentVelocity;

            currentVelocity = (lastPosition - initialPosition) / timerTime;

            return currentVelocity;
        }

        // Needs to track previousVelocity and generate current getVelocity()
        public void LimitAcceleration(int previousVelocity, int timerTime) 
        {
            int currentVelocity ();
            int currentAcceleration;

            currentAcceleration = (currentVelocity - previousVelocity) / timerTime;

            return currentAcceleration;
        }

        // Should be called whenever position is changed.
        public void trackHistory(double lastPosition)
        {
           positionHistory.Add(lastPosition);
        }
    }
}
