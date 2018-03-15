using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class MachineNode {

        private int creditsScheduled;
        private int majorCreditsScheduled;
        private ArrayList machines;
        private int year;
        private int quarter;

        public MachineNode(ArrayList m, int year, int quarter) {
            this.year = year;
            this.quarter = quarter;
            machines = m;
            creditsScheduled = 0;
            majorCreditsScheduled = 0;
        }

        public int getCredits() {
            return creditsScheduled;
        }

        public int getMajorCreditsScheduled() {
            return majorCreditsScheduled;
        }

        public void addMachine(int year, int quarter, ArrayList dateTime, ArrayList jobs) {
            Machine m = new Machine(year, quarter, dateTime, jobs);
            machines.Add(m);
        }

        public void removeMachine(Machine x) {
            if (machines.Contains(x)) machines.Remove(x);
        }

        public void scheduleMachine(Machine x, Job j) {
            if (machines.Contains(x)) {
                int num = machines.IndexOf(x);
                Machine m = (Machine)machines[num];
                if (!m.checkInUse()) {
                    m.setInUse(true);
                    m.setCurrentJobProcessing(j);
                } else {
                    Console.WriteLine("Cannot schedule because machine is busy");
                }
            }
        }

        public void unscheduleMachine(Machine x) {
            if(machines.Contains(x)) {
                int num = machines.IndexOf(x);
                Machine m = (Machine)machines[num];
                m.setInUse(false);
                m.setCurrentJobProcessing(null);
            }
        }

        public ArrayList getAllScheduledMachines() {
            ArrayList scheduledMachines = new ArrayList();
            for(int i = 0; i < machines.Capacity; i++ ) {
                Machine m = (Machine)machines[i];
                if (m.checkInUse()) scheduledMachines.Add(m);
            }
            return scheduledMachines;
        }
    }
}
