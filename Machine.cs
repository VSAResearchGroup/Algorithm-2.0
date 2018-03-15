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
        private ArrayList dateTime;

        public Machine(int year, int quarter, ArrayList dt, ArrayList jobs) {
            this.year = year;
            this.quarter = quarter;
            dateTime = dt;
            this.jobs = jobs;
            inUse = false;
            currentJobProcessing = null;
        }

        public bool canDoJob(Job job) {
            return jobs.Contains(job);
        }

        public bool checkInUse() {
            return inUse;
        }

        public void setInUse(bool x) {
            inUse = x;
        }

        public void addJob(Job s) {
            if(!jobs.Contains(s)) jobs.Add(s);
        }

        public void deleteJob(Job s) {
            if (jobs.Contains(s)) jobs.Remove(s);
        }

        public int getYear() {
            return year;
        }

        public int getQuarter() {
            return quarter;
        }

        public Job getCurrentJobProcessing() {
            return currentJobProcessing;
        }

        public void setCurrentJobProcessing(Job s) {
            currentJobProcessing = s;
        }

        public ArrayList getDateTime() {
            return dateTime;
        }
    }
}
