using System;
using System.Collections;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Scheduler {
    class Scheduler {
        SqlConnection myConnection;      //Declare the SQL connection to

        private CourseNetwork network;   //use Cashman network
        private ArrayList machineNodes;
        private DegreePlan myPlan;       //pull from DB
        private ArrayList finalPlan;     //output schedule
        private Preferences preferences;
        private ArrayList completedPrior;//starting point

        public Scheduler() {
            OpenSQLConnection();
            InitMachines();
        }

        public ArrayList CreateSchedule() {
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

        //create a query that will pull all the different machines
        //which means getting every single time slot
        //distinct year, quarter, time, and set of DayTimes
        private void InitMachines() {
            string query = "select CourseID, StartTimeID, EndTimeID, DayID, QuarterID from CourseTime;";
            DataTable dt = ExecuteQuery(query);
            Machine dummyMachine = new Machine();
            DayTime dummyDayTime = new DayTime();
            int currentCourse = 0;
            int course = (int)ItemArray[0];
            int start = 0;
            int end = 0;
            int day = 0;
            int quarter = 0;

            foreach (DataRow row in dt.Rows) {
                currentCourse = (int)row.ItemArray[0];
                if(currentCourse == course) {

                }
                course = (int)row.ItemArray[0];
                start = (int)row.ItemArray[1];
                end = (int)row.ItemArray[2];
                day = (int)row.ItemArray[3];
                quarter = (int)row.ItemArray[4];

                dummyDayTime.SetDayTime(day, start, end);
                
            }
        }

        public void MakeStartingPoint(string s) {

        }

        private DataTable ExecuteQuery(string query) {
            SqlCommand cmd = new SqlCommand(query, myConnection);
            DataTable dt = new DataTable();
            using (var con = myConnection) {
                using (var command = new SqlCommand(query)) {
                    myConnection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader()) {
                        dt.Load(dr);
                    }
                }
            }
            myConnection.Close();
            return dt;
        }

        public void InitDegreePlan(int majorID, int schoolID) {//query admissionrequiredcourses
            string query = "select CourseID from AdmissionRequiredCourses where MajorID ="
                            + majorID + " and SchoolID = " + schoolID + " ; ";
            DataTable dt = ExecuteQuery(query);
            ArrayList courseNums = new ArrayList();
            foreach (DataRow row in dt.Rows) {
                courseNums.Add(row.ItemArray[0]);
            }
            myPlan = new DegreePlan(courseNums);
        }


        private void OpenSQLConnection() {
            myConnection = new SqlConnection(@"Data Source=(localdb)\MyLocalDB;Initial Catalog=myVirtualSupportAvisor;Integrated Security=True");
        }


        /*
            •	Being scheduled during the summer
            •	Maximum number of core courses per quarter
            •	How many quarters you’d like to spread the plan over (MAX of 16)
            •	Time interval for when a person is available to go to class. For example, they are available 8AM-1PM
            •	Credits they would like to take per quarter.
            •	starting quarter of plan. [1,2,3,4]

        */
        //hard coded now, take from UI later
        private void CreatePreferences() {
            preferences = new Preferences();
            preferences.AddPreference("SUMMER", false);
            preferences.AddPreference("CORE_PER_QUARTER", 10);
            preferences.AddPreference("MAX_QUARTERS", 16);
            preferences.AddPreference("TIME_INTERVAL", new DayTime(1, 70, 130)); //do a whole bunch?
            preferences.AddPreference("CREDITS_PER_QUARTER", 20);
            preferences.AddPreference("STARTING_QUARTER", 2);
        }

        private ArrayList GetBusyMachines() {
            ArrayList busy = new ArrayList();
            for (int i = 0; i < machineNodes.Capacity; i++) {
                MachineNode mn = (MachineNode)machineNodes[i];
                busy.Add(mn.GetAllScheduledMachines());
            }
            return busy;
        }

    }
}
