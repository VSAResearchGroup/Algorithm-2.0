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

        public Job(int id) {
            this.id = id;
            scheduled = false;
        }

        public bool CheckScheduled() {
            return scheduled;
        }

        public void SetScheduled(bool s) {
            scheduled = s;
        }

    }
}
