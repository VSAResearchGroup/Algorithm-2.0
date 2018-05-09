using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Machine {

        private bool inUse;
        private int year;
        private int quarter;
        private ArrayList jobs;
        private Job currentJobProcessing;
        private ArrayList dateTime; //datetimes from class?

        public Machine() {
            this.year = 0;
            this.quarter = 0;
            dateTime = new ArrayList();
            this.jobs = new ArrayList();
            inUse = false;
            currentJobProcessing = null;
        }

        public Machine(int year, int quarter, ArrayList dt, ArrayList jobs) {
            this.year = year;
            this.quarter = quarter;
            dateTime = dt;
            this.jobs = jobs;
            inUse = false;
            currentJobProcessing = null;
        }

        public bool CanDoJob(Job job) {
            return jobs.Contains(job);
        }

        public bool CheckInUse() {
            return inUse;
        }

        public void SetInUse(bool x) {
            inUse = x;
        }

        public void SetQuarter(int q) {
            quarter = q;
        }

        public void AddJob(Job s) {
            if (!jobs.Contains(s)) jobs.Add(s);
        }

        public void DeleteJob(Job s) {
            if (jobs.Contains(s)) jobs.Remove(s);
        }

        public int GetYear() {
            return year;
        }

        public int GetQuarter() {
            return quarter;
        }

        public Job GetCurrentJobProcessing() {
            return currentJobProcessing;
        }

        public void SetCurrentJobProcessing(Job s) {
            currentJobProcessing = s;
        }

        public ArrayList GetDateTime() {
            return dateTime;
        }

        public void AddDayTime(DayTime dt) {
            if (!dateTime.Contains(dt)) {
                dateTime.Add(dt);
            }
        }

        public void Print() {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Year: " + year);
            Console.WriteLine("Quarter: " + quarter);
            Console.WriteLine("Jobs:");
            for (int i = 0; i < jobs.Count; i++) {
                Job j = (Job)jobs[i];
                Console.WriteLine(j.GetID());
            }
            Console.WriteLine("DayTimes:");
            for (int i = 0; i < dateTime.Count; i++) {
                DayTime dt = (DayTime)dateTime[i];
                Console.WriteLine("Day: " + dt.GetDay());
                Console.WriteLine("Start time: " + dt.GetStartTime());
                Console.WriteLine("End time: " + dt.GetEndTime());
            }
            Console.WriteLine("-----------------------------------");
        }

        private bool ContainsDayTime(ArrayList times, DayTime dt) {
            for(int i = 0; i < times.Count; i++) {
                DayTime time = (DayTime)times[i];
                if(time == dt) {
                    return true;
                }
            }
            return false;
        }

        public static bool operator ==(Machine thism, Machine right) {
            if (thism.quarter != right.quarter || thism.year != right.year
                || thism.dateTime.Count != right.dateTime.Count) {
                return false;
            }
            for (int i = 0; i < thism.dateTime.Count; i++) {
                if (!thism.ContainsDayTime(thism.dateTime, (DayTime)right.dateTime[i])) {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(Machine thism, Machine right) {
            return !(thism == right);
        }


    }
}
