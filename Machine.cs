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

        public void AddJob(Job s) {
            if(!jobs.Contains(s)) jobs.Add(s);
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
            dateTime.Add(dt);
        }
    }
}
