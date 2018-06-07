using System;
using System.Collections;
using System.Collections.Generic;

namespace Scheduler {
    class Machine {

        private bool inUse;
        private int year;
        private int quarter;
        private List<Job> jobs;
        private Job currentJobProcessing;
        private List<DayTime> dateTime; //datetimes from class?

        public Machine() {
            this.year = 0;
            this.quarter = 0;
            dateTime = new List<DayTime>();
            this.jobs = new List<Job>();
            inUse = false;
            currentJobProcessing = null;
        }

        public Machine(int year, int quarter, List<DayTime> dt, List<Job> jobs) {
            this.year = year;
            this.quarter = quarter;
            dateTime = dt;
            this.jobs = jobs;
            inUse = false;
            currentJobProcessing = null;
        }

        public Machine(Machine m) {
            this.year = m.year;
            this.quarter = m.quarter;
            dateTime = m.dateTime;
            jobs = m.jobs;
            inUse = false;
            currentJobProcessing = null;
        }

        public bool CanDoJob(Job job) {
            for(int i = 0; i < jobs.Count; i++) {
                if(jobs[i] == job) {
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj) {
            Machine j = obj as Machine;
            if(j == null) {
                return false;
            } else {
                return j == this;
            }
        }

        public List<int> GetStartTimes() {
            List<int> start = new List<int>();
            for (int i = 0; i < dateTime.Count; i++) {
                start.Add(dateTime[i].GetStartTime());
            }
            return start;
        }

        public List<int> GetEndTimes() {
            List<int> end = new List<int>();
            for (int i = 0; i < dateTime.Count; i++) {
                end.Add(dateTime[i].GetStartTime());
            }
            return end;
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

        public void SetYear(int q) {
            year = q;
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

        public List<DayTime> GetDateTime() {
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
                Job j = jobs[i];
                Console.WriteLine(j.GetID());
            }
            Console.WriteLine("DayTimes:");
            for (int i = 0; i < dateTime.Count; i++) {
                DayTime dt = dateTime[i];
                Console.WriteLine("Day: " + dt.GetDay());
                Console.WriteLine("Start time: " + dt.GetStartTime());
                Console.WriteLine("End time: " + dt.GetEndTime());
            }
            Console.WriteLine("-----------------------------------");
        }

        public void PrintBusyMachine() {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Year: " + year);
            Console.WriteLine("Quarter: " + quarter);
            Console.WriteLine("Job:");
            Console.WriteLine(currentJobProcessing.GetID());           
            Console.WriteLine("DayTimes:");
            for (int i = 0; i < dateTime.Count; i++) {
                DayTime dt = dateTime[i];
                Console.WriteLine("Day: " + dt.GetDay());
                Console.WriteLine("Start time: " + dt.GetStartTime());
                Console.WriteLine("End time: " + dt.GetEndTime());
            }
            Console.WriteLine("-----------------------------------");
        }

        private bool ContainsDayTime(List<DayTime> times, DayTime dt) {
            for(int i = 0; i < times.Count; i++) {
                DayTime time = times[i];
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
                if (!thism.ContainsDayTime(thism.dateTime, right.dateTime[i])) {
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
