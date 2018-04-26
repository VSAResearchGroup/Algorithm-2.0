using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class DegreePlan {
        //list of lists, depending on the degree plan. 
        //Right now, list[0] is only the courses 
        //required to get into the major
        private ArrayList plan; 
        
        public DegreePlan(ArrayList major) {
            plan = new ArrayList();
            //insert values into list[0]
            plan.Add(major);
        }

        public ArrayList GetList(int index) {
            ArrayList myList = (ArrayList)plan[index];
            return myList;
        }

    }
}
