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
            scheduler.MakeStartingPoint("nothing yet");
            scheduler.InitDegreePlan(16, 2); //get this from UI later
            ArrayList schedule = new ArrayList();
            Console.WriteLine("Scheduled following courses:");
            schedule = scheduler.CreateSchedule();
            Console.WriteLine(schedule);
            Console.WriteLine("Unable to schedule following courses:");
            Console.WriteLine(scheduler.GetUnscheduledCourses());
            Console.ReadLine();
        }
    }
}
