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
        PrinterControl printer;

        public ZRailController()
        {
            positionHistory = new List<double>();
        }

        public int ConvertZRailMMToSteps(int mmHeight)
        {
            int steps = mmHeight * 400;

            return steps;
        }

           public void LimitVelocity()
        {
            int currentVelocity = 0;
            while(printer.StepStepper(PrinterControl.StepperDir.STEP_UP) || 
                printer.StepStepper(PrinterControl.StepperDir.STEP_DOWN)) 
            {
                while(currentVelocity < MaxZRailVelocity)
                {
                    
                }
            }
        }

        public int GetVelocity(int initialPosition, int lastPosition, int timerTime)
        {
            int currentVelocity;

            currentVelocity = (lastPosition - initialPosition) / timerTime;

            return currentVelocity;
        }

        // Needs to track previousVelocity and generate current getVelocity()
        public int LimitAcceleration(int previousVelocity, int timerTime)
        {
            int currentVelocity = 0;
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