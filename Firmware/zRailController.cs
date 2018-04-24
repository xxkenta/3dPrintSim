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

        private int secondsElapsedOnce = 1;
        private int waitCounterOnce = 0;
        private long waitTimeOnce = 0;

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

        public void LimitVelocity(PrinterControl.StepperDir dir)
        {

        }

        public void MoveZRail(PrinterControl.StepperDir dir, int steps)
        {
            int secondsElapsed = 1;
            int waitCounter = 0;
            long waitTime;

            for (int i = 0; i < steps; i++) 
            {
                waitTime = (1 / (secondsElapsed * 4 * 400)) * 1000000;

                if (waitTime < 62.5)
                {
                    waitTime = 63;
                }

                printer.StepStepper(dir);

                printer.WaitMicroseconds(waitTime);

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

            waitTimeOnce = (1 / (secondsElapsedOnce * 4 * 400)) * 1000000;

            if (waitTimeOnce < 62.5)
            {
                waitTimeOnce = 63;
            }

            printer.StepStepper(dir);

            printer.WaitMicroseconds(waitTimeOnce);

            waitCounterOnce += (int)waitTimeOnce;

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