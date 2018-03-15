using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Driver {
        static void Main(string[] args) {
            Scheduler scheduler = new Scheduler();
            ArrayList schedule = new ArrayList();
            schedule = scheduler.createSchedule();
            Console.WriteLine(schedule);
        }
    }
}
