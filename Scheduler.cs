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

        private const int QUARTERS = 4;

        public Scheduler() {
            machineNodes = new ArrayList();
            finalPlan = new ArrayList();
            completedPrior = new ArrayList();
            OpenSQLConnection();
            InitMachineNodes();
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

        private void InitMachineNodes() {
            for(int i = 0; i < QUARTERS; i++) {
                MachineNode m = new MachineNode(0, i);
                machineNodes.Add(m);
            }
        }

        /*  
         BIG DATABASE MACHINES ASSUMPTION: each course is entered continuosly. For example,
         course 1 has DayTimes of A, B and C. Course 2 had DayTimes of D, E, and F.
         OK: ABCDEF, NOT OK: ABDCEF or any other combo

            this is ok as long as the query makes it that way, you cannot assume that in the database it will 
            be entered that way. MAKE SURE YOU DO SOMETHING LIKE GROUP BY AND TEST IT!!!
             */


        //create a query that will pull all the different machines
        //which means getting every single time slot
        //distinct year, quarter, time, and set of DayTimes
        private void InitMachines() {
            string query = "select top 20 CourseID, StartTimeID, EndTimeID, DayID, QuarterID, SectionID from CourseTime order by CourseID ASC;";
            DataTable dt = ExecuteQuery(query);
            Machine dummyMachine = new Machine();
            DayTime dummyDayTime = new DayTime();
            int dt_size = dt.Rows.Count - 1;
            DataRow dr = dt.Rows[dt_size];
            int currentCourse = (int)dr.ItemArray[0];
            int currentSection = (int)dr.ItemArray[5];
            int course = 0;
            int start = 0;
            int end = 0;
            int day = 0;
            int quarter = 0;
            int section = 0;

            //this is another way to loop through the table
            //this is better because you can use continue
            //transfer the data from the for loop
            while (dt_size >= 0) {
                dr = dt.Rows[dt_size];
                course = (int)dr.ItemArray[0];
                section = (int)dr.ItemArray[5];

                //same course but different section is a different machine
                //different course is a different machine
                if ((currentCourse == course && currentSection != section) || (currentCourse != course)) {
                    dummyMachine = new Machine();
                    currentCourse = (int)dr.ItemArray[0];
                }
                start = (int)dr.ItemArray[1];
                end = (int)dr.ItemArray[2];
                day = (int)dr.ItemArray[3];
                quarter = (int)dr.ItemArray[4];

                dummyDayTime.SetDayTime(day, start, end);
                dummyMachine.AddDayTime(dummyDayTime);
                dummyMachine.SetQuarter(quarter);
                dummyMachine.AddJob(new Job(course));

                //we add a new machine when we peek to the next row and see
                //(different course) OR (same course, different section)
                if ((int)dt.Rows[dt_size - 1].ItemArray[0] != currentCourse ||
                    ((int)dt.Rows[dt_size - 1].ItemArray[0] == currentCourse &&
                    (int)dt.Rows[dt_size - 1].ItemArray[5] != currentSection)) {
                   
                    for (int i = 0; i < machineNodes.Count; i++) {
                        MachineNode mn = (MachineNode)machineNodes[i];
                        ArrayList machines = mn.GetMachines();
                        if (machines.Count > 0) {
                            for (int j = 0; j < machines.Count; j++) {
                                Machine m = (Machine)machines[j];
                                if (m == dummyMachine) { //found the machine, just add job
                                    m.AddJob(new Job(course));
                                } else { //machine does not exist, add it in
                                    machines.Add(dummyMachine);
                                }
                            }
                        } else if(dummyMachine.GetYear() == mn.GetYear() && dummyMachine.GetQuarter() == mn.GetQuarter()) {
                            machines.Add(dummyMachine);
                        }
                    }
                }
                dt_size--;
            }
            //print machines for testing
            for (int i = 0; i < machineNodes.Count; i++) {
                ArrayList machines = (ArrayList)machineNodes[i];
                for (int j = 0; j < machines.Count; j++) {
                    Machine m = (Machine)machines[j];
                    m.Print();
                }
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
            •	Time interval for when a person is available to go to 
                        class. For example, they are available 8AM-1PM.
                        LOOK AT TABLE TimeSlot
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
