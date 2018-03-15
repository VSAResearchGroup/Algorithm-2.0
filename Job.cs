using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Job {

        private int id;
        private ArrayList prereqs;
        private bool scheduled;

        public Job(int id) {
            this.id = id;
            prereqs = null;
            scheduled = false;
        }

        public Job(int id, ArrayList p) {
            this.id = id;
            prereqs = p;
            scheduled = false;
        }

        public ArrayList GetPrereqs() {
            return prereqs;
        }

        public void AddPrereq(Job s) {
            prereqs.Add(s);
        }

        public void DeletePrereq(Job s) {
            if (prereqs.Contains(s)) prereqs.Remove(s);
        }

        public bool CheckScheduled() {
            return scheduled;
        }

        public void SetScheduled(bool s) {
            scheduled = s;
        }

        public bool PrereqExists(Job s) {
            return prereqs.Contains(s);
        }
    }
}
