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

        private PrinterControl printer;

        private double secondsElapsedOnce = 1;
        private double waitCounterOnce = 0;
        private double waitTimeOnce = 0;

        public ZRailController(PrinterControl printer)
        {
            positionHistory = new List<double>();
            this.printer = printer;
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

        public void MoveZRail(PrinterControl.StepperDir dir, int steps)
        {
            double secondsElapsed = 1;
            double waitCounter = 0;
            double waitTime;

            for (int i = 0; i < steps; i++) 
            {
                waitTime = (1 / (secondsElapsed * 4 * 400)) * 1000000;

                if (waitTime < 62.5)
                {
                    waitTime = 63;
                }

                printer.StepStepper(dir);

                printer.WaitMicroseconds((long) waitTime);

                waitCounter += (int) waitTime;

                if (waitCounter >= 1000000)
                {
                    secondsElapsed += 1;
                    waitCounter = 0;
                }

            }

        }

        public void StepOnce(PrinterControl.StepperDir dir)
        {
            // ResetSteps() must be called as soon as you are done using this function
            waitTimeOnce = (1 / (secondsElapsedOnce * 4 * 400)) * 1000000;

            if (waitTimeOnce < 62.5)
            {
                waitTimeOnce = 63;
            }

            printer.StepStepper(dir);

            printer.WaitMicroseconds((long)waitTimeOnce);

            waitCounterOnce += waitTimeOnce;

            if (waitCounterOnce >= 1000000)
            {
                secondsElapsedOnce += 1;
                waitCounterOnce = 0;
            }

        }

        public void ResetSteps()
        {
            secondsElapsedOnce = 1;
            waitCounterOnce = 0;
            waitTimeOnce = 0;
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