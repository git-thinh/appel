using System;
using System.Collections.Generic;
using System.Text;

namespace appel
{
    public class setTimeout : IDisposable
    {
        private System.Timers.Timer planTimer;
        private Action planAction;
        bool isRepeatedPlan = false;

        private setTimeout(int millisecondsDelay, Action planAction, bool isRepeatedPlan)
        {
            planTimer = new System.Timers.Timer(millisecondsDelay);
            planTimer.Elapsed += GenericTimerCallback;
            planTimer.Enabled = true;

            this.planAction = planAction;
            this.isRepeatedPlan = isRepeatedPlan;
        }

        public static setTimeout Delay(int millisecondsDelay, Action planAction)
        {
            return new setTimeout(millisecondsDelay, planAction, false);
        }

        public static setTimeout Repeat(int millisecondsInterval, Action planAction)
        {
            return new setTimeout(millisecondsInterval, planAction, true);
        }

        private void GenericTimerCallback(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isRepeatedPlan)
            {
                Abort();
            }
            planAction();
        }

        public void Abort()
        {
            planTimer.Enabled = false;
            planTimer.Elapsed -= GenericTimerCallback;
        }

        public void Dispose()
        {
            if (planTimer != null)
            {
                Abort();
                planTimer.Dispose();
                planTimer = null;
            }
            else
            {
                throw new ObjectDisposedException(typeof(setTimeout).Name);
            }
        }
    }
}
