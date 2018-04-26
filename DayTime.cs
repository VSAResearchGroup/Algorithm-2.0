using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class DayTime {
        private int day;
        private int start_time;
        private int end_time;

        public DayTime() {
            day = 0;
            start_time = 0;
            end_time = 0;
        }

        public DayTime(int d, int st, int et) {
            day = d;
            start_time = st;
            end_time = et;
        }

        public int GetDay() { return day; } 
        public int GetStartTime() { return start_time; }
        public int GetEndTime() { return end_time; }

        public void SetDay(int d) { day = d; }
        public void SetStartTime(int st) { start_time = st; }
        public void SetEndTime(int et) { end_time = et; }

        public void SetDayTime(int d, int st, int et) {
            day = d;
            start_time = st;
            end_time = et;
        }

    }
}
