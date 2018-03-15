using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class DegreePlan {
        private ArrayList plan;
        private String name;
        private Deserializer converter;
        
        public DegreePlan() {
            InitDummyPlan();
            
            //make a list of dummy lists
            //insert dummy values into list[0]
        }

        public DegreePlan(String name, String json) {

        }

        public ArrayList getList(int index) {
            ArrayList myList = (ArrayList)plan[index];
            return myList;
        }

        private void InitDummyPlan() {
            ArrayList majorCourses = new ArrayList();
            Job zero = new Job(0, new ArrayList(new[] { new Job(6), new Job(7) }));
            Job one = new Job(0, new ArrayList(new[] { new Job(10), new Job(8) }));
            Job two = new Job(0, new ArrayList(new[] { new Job(1), new Job(8)}));
            Job three = new Job(0, new ArrayList(new[] { new Job(2), new Job(9) }));
            Job four = new Job(0, new ArrayList(new[] { new Job(9)}));
            Job five = new Job(0, new ArrayList(new[] { new Job(13)}));
            Job six = new Job(0, new ArrayList());
            Job seven = new Job(0, new ArrayList(new[] { new Job(10) }));
            Job eight = new Job(0, new ArrayList(new[] { new Job(11) }));
            Job nine = new Job(0, new ArrayList(new[] { new Job(12) }));
            Job ten = new Job(0, new ArrayList());
            Job eleven = new Job(0, new ArrayList());
            Job twelve = new Job(0, new ArrayList(new[] { new Job(13) }));
            Job thirteen = new Job(0, new ArrayList());
            plan.Add(majorCourses);
        }
    }
}
