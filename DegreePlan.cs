﻿using System.Collections.Generic;

namespace Scheduler {
    class DegreePlan {
        //list of lists, depending on the degree plan. 
        //Right now, list[0] is only the courses 
        //required to get into the major
        private List<List<Job>> plan;

        //------------------------------------------------------------------------------
        // 
        // creates a degree plan based on list passed. not sure where to find this 
        // in db or if we even have this data
        //------------------------------------------------------------------------------
        public DegreePlan(List<Job> major) {
            plan = new List<List<Job>>();
            //insert values into list[0]
            plan.Add(major);
        }

        //------------------------------------------------------------------------------
        // 
        // retrieves the list at the index. currently only using first list but later
        // we can make a reccomendation system that uses the other lists
        //------------------------------------------------------------------------------
        public List<Job> GetList(int index) {
            return plan[index];
        }

    }
}
