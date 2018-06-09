using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Job {

        private int id;
        private bool scheduled;
        private bool prerequisitesScheduled;
        private int quarterScheduled;
        private int yearScheduled;
        //need to implement int credits for the course for preferences purposes
        //need to implement boolean if its a major course or not for preferences purposes

        //------------------------------------------------------------------------------
        // 
        // constructor
        // 
        //------------------------------------------------------------------------------
        public Job(int id) {
            this.id = id;
            scheduled = false;
            quarterScheduled = -1;
            yearScheduled = -1;
            prerequisitesScheduled = false;
        }

        //------------------------------------------------------------------------------
        // 
        // boolean for scheduled or not
        // 
        //------------------------------------------------------------------------------
        public bool GetScheduled() {
            return scheduled;
        }

        //------------------------------------------------------------------------------
        // 
        // boolean for if the prerequisites are scheduled
        // 
        //------------------------------------------------------------------------------
        public bool GetPrerequisitesScheduled() {
            return prerequisitesScheduled;
        }

        //------------------------------------------------------------------------------
        // 
        // setter for prerequisites
        // 
        //------------------------------------------------------------------------------
        public void SetPrerequisitesScheduled(bool b) {
            prerequisitesScheduled = b;
        }

        //------------------------------------------------------------------------------
        // 
        // seter for being scheduled
        // 
        //------------------------------------------------------------------------------
        public void SetScheduled(bool s) {
            scheduled = s;
        }

        //------------------------------------------------------------------------------
        // 
        // setter for quarter scheduled
        // 
        //------------------------------------------------------------------------------
        public void SetQuarterScheduled(int x) {
            quarterScheduled = x;
        }

        //------------------------------------------------------------------------------
        // 
        // getter for wuarter scheduled
        // 
        //------------------------------------------------------------------------------
        public int GetQuarterScheduled() {
            return quarterScheduled;
        }

        //------------------------------------------------------------------------------
        // 
        // setter for year scheduled
        // 
        //------------------------------------------------------------------------------
        public void SetYearScheduled(int x) {
            yearScheduled = x;
        }

        //------------------------------------------------------------------------------
        // 
        // getter for year scheduled
        // 
        //------------------------------------------------------------------------------
        public int GetYearScheduled() {
            return yearScheduled;
        }

        //------------------------------------------------------------------------------
        // 
        // getter for id of course; corresponds to what it is in DB
        // 
        //------------------------------------------------------------------------------
        public int GetID() {
            return id;
        }

        //------------------------------------------------------------------------------
        // 
        // equality
        // 
        //------------------------------------------------------------------------------
        public static bool operator ==(Job thisj, Job right) {
            if(object.ReferenceEquals(thisj, null) || object.ReferenceEquals(right, null)) {
                return false;
            }
            if(object.ReferenceEquals(thisj, right)) {
                return true;
            }
            return thisj.Equals(right);
        }

        //------------------------------------------------------------------------------
        // 
        // equality
        // 
        //------------------------------------------------------------------------------
        public static bool operator !=(Job thisj, Job right) {
            return !(thisj == right);
        }

        //------------------------------------------------------------------------------
        // 
        // equality
        // 
        //------------------------------------------------------------------------------
        public override bool Equals(object obj) {
            Job j = obj as Job;
            return this.id == j.id;
        }
    }
}
