using System.Collections.Generic;

namespace Scheduler {
    class DegreePlan {
        //list of lists, depending on the degree plan. 
        //Right now, list[0] is only the courses 
        //required to get into the major
        private List<List<Job>> plan; 
        
        public DegreePlan(List<Job> major) {
            plan = new List<List<Job>>();
            //insert values into list[0]
            plan.Add(major);
        }

        public List<Job> GetList(int index) {
            return plan[index];
        }

    }
}
