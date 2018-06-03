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

        public Job(int id) {
            this.id = id;
            scheduled = false;
            quarterScheduled = -1;
            yearScheduled = -1;
            prerequisitesScheduled = false;
        }

        public bool GetScheduled() {
            return scheduled;
        }

        public bool GetPrerequisitesScheduled() {
            return prerequisitesScheduled;
        }

        public void SetPrerequisitesScheduled(bool b) {
            prerequisitesScheduled = b;
        }

        public void SetScheduled(bool s) {
            scheduled = s;
        }

        public void SetQuarterScheduled(int x) {
            quarterScheduled = x;
        }

        public int GetQuarterScheduled() {
            return quarterScheduled;
        }

        public void SetYearScheduled(int x) {
            yearScheduled = x;
        }

        public int GetYearScheduled() {
            return yearScheduled;
        }

        public int GetID() {
            return id;
        }
    }
}
