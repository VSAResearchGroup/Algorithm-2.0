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
        private Preferences preferences;

        public MachineNode(int year, int quarter) {
            this.year = year;
            this.quarter = quarter;
            machines = new ArrayList();
            creditsScheduled = 0;
            majorCreditsScheduled = 0;
            preferences = new Preferences();
        }

        public MachineNode(ArrayList m, int year, int quarter, Preferences p) {
            this.year = year;
            this.quarter = quarter;
            machines = m;
            creditsScheduled = 0;
            majorCreditsScheduled = 0;
            preferences = p;
        }

        public int GetYear() { return year; }

        public int GetQuarter() { return quarter; }

        public ArrayList GetMachines() { return machines; }

        public int GetCreditsScheduled() {
            return creditsScheduled;
        }

        public int GetMajorCreditsScheduled() {
            return majorCreditsScheduled;
        }

        public void AddMachine(int year, int quarter, ArrayList dateTime, ArrayList jobs) {
            Machine m = new Machine(year, quarter, dateTime, jobs);
            machines.Add(m);
        }

        public void RemoveMachine(Machine x) {
            if (machines.Contains(x)) machines.Remove(x);
        }

        //does not yet take preferences into account
        public void ScheduleMachine(Machine x, Job j) {
            if (machines.Contains(x)) {
                int num = machines.IndexOf(x);
                Machine m = (Machine)machines[num];
                if (!m.CheckInUse()) {
                    m.SetInUse(true);
                    m.SetCurrentJobProcessing(j);
                } else {
                    Console.WriteLine("Cannot schedule -- machine is busy");
                }
            }
        }

        public void UnscheduleMachine(Machine x) {
            if(machines.Contains(x)) {
                int num = machines.IndexOf(x);
                Machine m = (Machine)machines[num];
                m.SetInUse(false);
                m.SetCurrentJobProcessing(null);
            }
        }

        public ArrayList GetAllScheduledMachines() {
            ArrayList scheduledMachines = new ArrayList();
            for(int i = 0; i < machines.Capacity; i++ ) {
                Machine m = (Machine)machines[i];
                if (m.CheckInUse()) scheduledMachines.Add(m);
            }
            return scheduledMachines;
        }
    }
}
