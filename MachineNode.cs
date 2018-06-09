using System;
using System.Collections.Generic;

namespace Scheduler {
    class MachineNode {

        private int creditsScheduled;
        private int majorCreditsScheduled;
        private List<Machine> machines;
        private int year;
        private int quarter;
        private Preferences preferences;
        private int classesScheduled;

        //------------------------------------------------------------------------------
        // 
        // creates machine node from year and quarter
        // 
        //------------------------------------------------------------------------------
        public MachineNode(int year, int quarter) {
            this.year = year;
            this.quarter = quarter;
            machines = new List<Machine>();
            creditsScheduled = 0;
            majorCreditsScheduled = 0;
            classesScheduled = 0;
            preferences = new Preferences();
        }

        //------------------------------------------------------------------------------
        // 
        // creates machine node from scratch
        // 
        //------------------------------------------------------------------------------
        public MachineNode(List<Machine> m, int year, int quarter, Preferences p) {
            this.year = year;
            this.quarter = quarter;
            machines = m;
            creditsScheduled = 0;
            majorCreditsScheduled = 0;
            preferences = p;
        }

        //------------------------------------------------------------------------------
        // 
        // returns year
        // 
        //------------------------------------------------------------------------------
        public int GetYear() { return year; }

        //------------------------------------------------------------------------------
        // 
        // returns wuarter
        // 
        //------------------------------------------------------------------------------
        public int GetQuarter() { return quarter; }

        //------------------------------------------------------------------------------
        // 
        // returns ALL machined 
        // 
        //------------------------------------------------------------------------------
        public List<Machine> GetMachines() { return machines; }

        //------------------------------------------------------------------------------
        // 
        // returns credits currently scheduled on machine node. not implemented because
        // we dont have credits in machine. we only check for classes scheduled
        //------------------------------------------------------------------------------
        public int GetCreditsScheduled() {
            return creditsScheduled;
        }

        //------------------------------------------------------------------------------
        // 
        // temporary until getcreditsscheduled is implemented
        // 
        //------------------------------------------------------------------------------
        public int GetClassesScheduled() {
            return classesScheduled;
        }

        //------------------------------------------------------------------------------
        // 
        // temporary until getcreditsscheduled is implemented
        // 
        //------------------------------------------------------------------------------
        public void AddClassesScheduled(int k) {
            classesScheduled += k;
        }

        //------------------------------------------------------------------------------
        // 
        // will work with getcreditsscheduled for preferences
        // 
        //------------------------------------------------------------------------------
        public int GetMajorCreditsScheduled() {
            return majorCreditsScheduled;
        }

        //------------------------------------------------------------------------------
        // 
        // adds a new machine to node
        // 
        //------------------------------------------------------------------------------
        public void AddMachine(int year, int quarter, List<DayTime> dateTime, List<Job> jobs) {
            Machine m = new Machine(year, quarter, dateTime, jobs);
            machines.Add(m);
        }

        //------------------------------------------------------------------------------
        // 
        // adds a new machine to node
        // 
        //------------------------------------------------------------------------------
        public void AddMachine(Machine m) {
            machines.Add(m);
        }

        //------------------------------------------------------------------------------
        // 
        // removes machine from node
        // 
        //------------------------------------------------------------------------------
        public void RemoveMachine(Machine x) {
            if (machines.Contains(x)) machines.Remove(x);
        }

        //------------------------------------------------------------------------------
        // 
        // not actually used because the scheduler is in charge of this; I will keep it
        // here in case it is possibly better to delegate it to this class
        //------------------------------------------------------------------------------
        public void ScheduleMachine(Machine x, Job j) {
            if (machines.Contains(x)) {
                int num = machines.IndexOf(x);
                Machine m = machines[num];
                if (!m.CheckInUse()) {
                    m.SetInUse(true);
                    m.SetCurrentJobProcessing(j);
                } else {
                    Console.WriteLine("Cannot schedule -- machine is busy");
                }
            }
        }

        //------------------------------------------------------------------------------
        // 
        // not used but here for extendability
        // 
        //------------------------------------------------------------------------------
        public void UnscheduleMachine(Machine x) {
            if(machines.Contains(x)) {
                int num = machines.IndexOf(x);
                Machine m = machines[num];
                m.SetInUse(false);
                m.SetCurrentJobProcessing(null);
            }
        }

        //------------------------------------------------------------------------------
        // 
        // returns all machines for final plan used by scheduler
        // 
        //------------------------------------------------------------------------------
        public List<Machine> GetAllScheduledMachines() {
            List<Machine> scheduledMachines = new List<Machine>();
            for(int i = 0; i < machines.Count; i++ ) {
                Machine m = machines[i];
                if (m.CheckInUse()) scheduledMachines.Add(m);
            }
            return scheduledMachines;
        }
    }
}
