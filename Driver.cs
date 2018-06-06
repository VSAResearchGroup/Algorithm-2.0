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
            scheduler.InitDegreePlan(22, 6); //get this from UI later
            ArrayList schedule = new ArrayList();
            Console.WriteLine("Scheduled following courses:");
            schedule = scheduler.CreateSchedule();

            /*print all busy machines*/
            for (int i = 0; i < schedule.Count; i++) {
                ArrayList machines = (ArrayList)schedule[i];
                for (int j = 0; j < machines.Count; j++) {
                    Machine m = (Machine)machines[j];
                    m.PrintBusyMachine();
                }
            }

            /*print what couldn't be scheduled*/
            Console.WriteLine("Unable to schedule following courses:");
            ArrayList unScheduled = new ArrayList();
            unScheduled = scheduler.GetUnscheduledCourses();
            for (int i = 0; i < unScheduled.Count; i++) {
                Job j = (Job)unScheduled[i];
                Console.WriteLine(j.GetID());
            }
            Console.ReadLine();
        }
    }
}
