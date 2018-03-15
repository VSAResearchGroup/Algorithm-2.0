using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Scheduler {
        private Network network;
        private ArrayList machineNodes;
        private ArrayList allCourses;
        private DegreePlan myPlan;
        private ArrayList finalPlan;
        private Preferences prefs;
        private ArrayList completedPrior;

        public ArrayList createSchedule() {
            /*
             * 
        •	For each major course
            o	Depth first search traversal down to the lowest node
            o	If job is marked scheduled, skip the branch (this prevents from visiting repeat branches. Because if a job is marked visited, that means all its prerequisites have been scheduled)
            o	Schedule it by finding the earliest available machine to take it (machine that is unscheduled currently)
            o	Mark machine as in use
            o	Mark job as visited
            o	Traverse back up, scheduling them until you come back to the major course
            o	Schedule the major course
            o	Mark machine as in use
        •	If a certain amount of machines for a particular quarter is taken up (your preferred maximum amount of credits), find a machine in a later quarter
            o	Make sure to preserve the order
        •	To change preferences (such as time of the day), pick different machines which specialize in that quarter, just make sure you are preserving the order of the sequence
        •	In the end, traverse through the list of machines. The ones that are scheduled are your schedule
        •	Done 
             *
             */
            return finalPlan;
        }

        private void initJobsMachines(String filename) {

        }

        private void createNetwork() {

        }

        private void makeStartingPoint() {

        }

        private void initPlan(String jsn) {

        }

        private void priorCompletedCourses(String filename) {

        }

        private ArrayList getBusyMachines() {
            ArrayList busy = new ArrayList();
            for (int i = 0; i < machineNodes.Capacity; i++) {
                MachineNode mn = (MachineNode)machineNodes[i];
                busy.Add(mn.getAllScheduledMachines());
            }
            return busy;
        }

    }
}
