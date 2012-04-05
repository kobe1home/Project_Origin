using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Origin
{
    public class Timer
    {
        private TimeSpan previousTime;
        private TimeSpan currentTime;
        private TimeSpan ElapsedTime; 
        private TimeSpan totalTime;
        private TimeSpan timeToCount; 

        public Timer(TimeSpan startTime, int timeToCount)
        {
            this.reStartTimer(startTime, timeToCount);
        }


        public void IncreastTimer(TimeSpan currentTime)
        {
            TimeSpan time;
            this.previousTime = this.currentTime;
            this.currentTime = currentTime;
            time = this.currentTime - this.previousTime;
            

            if (this.totalTime >= this.timeToCount)
            {
                return;
            }

            if ((time + totalTime) > this.timeToCount)
            {
                this.ElapsedTime = this.timeToCount - this.totalTime;
                this.totalTime = this.timeToCount;
            }
            else
            {
                this.ElapsedTime = time;
                this.totalTime = this.totalTime + time;
            }
        }

        public Boolean IfTimeExpired()
        {
            if (this.totalTime >= this.timeToCount)
            {
                Console.WriteLine("True");
                return true;
            }
            return false;
        }
        public TimeSpan getElapsedTime()
        {
            return this.ElapsedTime;
        }


        public void reStartTimer(TimeSpan startTime, int timeToCount)
        {
            this.currentTime = startTime;
            this.previousTime = startTime;
            this.totalTime = this.currentTime - this.previousTime;
            this.timeToCount = new TimeSpan(0, 0, timeToCount);
            Console.WriteLine(""+this.timeToCount);
            this.ElapsedTime = (this.currentTime - this.previousTime);
        }
    }
}
