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
            InitMachineNodes();
            InitMachines();
            InitYearTwo(); //temporary fix for the second year
        }

        public ArrayList CreateSchedule() {
            ArrayList majorCourses = myPlan.GetList(0);
            for (int i = 0; i < majorCourses.Count; i++) { 
                Job job = (Job)majorCourses[i];
                ScheduleCourse(job);
            }
            finalPlan = GetBusyMachines();
            return finalPlan;
        }
        //**********************   LEFT OFF HERE ****************************************
        //TO DO NEXT ON THIS METHOD: make the top comments a reality; this will be a
        //recursive function
        private void ScheduleCourse(Job job) {
            //if j does not have prerequisites
                //schedule j
            //else
                //schedule j's prerequisites by geting shortest group and whatnot
            int num = job.GetID();
            List<CourseNode> groups = network.FindShortPath(num);//find prerequisite group
            int shortest = GetShortestGroup(groups);//so now we have the shortest list
            List<CourseNode> group = groups[shortest].prereqs;
            ArrayList jobsToBeScheduled = new ArrayList();
            for (int j = 0; j < group.Count; j++) {
                Job myJob = new Job(groups[j].courseID);
                jobsToBeScheduled.Add(myJob);
            }//now we have a list full of jobs to be scheduled

            for (int k = 0; k < jobsToBeScheduled.Count; k++) { //schedule them all here
                ScheduleCourse((Job)jobsToBeScheduled[k]);
            }//now they are scheduled
        }

        private int GetShortestGroup(List<CourseNode> groups) {
            int shortest = 0;
            for (int j = 1; j < groups.Count; j++) { //find the shortest group.
                if (groups[j].prereqs.Count < shortest) {
                    shortest = j;
                }
            }//so now we have the shortest list
            return shortest;
        }

         private void InitYearTwo() {
            //init more machine nodes for the next year
            for (int i = 1; i <= QUARTERS; i++) {
                MachineNode m = new MachineNode(1, i);
                machineNodes.Add(m);
            }
            //transfer all the same classes to the set of machine nodes
            for (int i = 4; i < 8; i++) {
                MachineNode oldMn = (MachineNode)machineNodes[i - 4];
                MachineNode newMn = (MachineNode)machineNodes[i];
                for (int j = 0; j < oldMn.GetMachines().Count; j++) {
                    Machine oldMachine = (Machine)oldMn.GetMachines()[j];
                    Machine newMachine = new Machine(oldMachine);
                    newMachine.SetYear(1);
                    newMn.AddMachine(newMachine);
                }
            }
        }

        private void InitMachineNodes() {
            for (int i = 1; i <= QUARTERS; i++) {
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
            string query = "select CourseID, StartTimeID, EndTimeID, DayID, QuarterID, SectionID from CourseTime order by CourseID ASC;";
            DataTable dt = ExecuteQuery(query);
            Machine dummyMachine = new Machine();
            DayTime dummyDayTime = new DayTime();
            int dt_size = dt.Rows.Count - 1;
            DataRow dr = dt.Rows[dt_size];
            int currentCourse = (int)dr.ItemArray[0];
            int currentQuarter = (int)dr.ItemArray[4];
            int currentSection = (int)dr.ItemArray[5];
            int course = 0;
            int start = 0;
            int end = 0;
            int day = 0;
            int quarter = 0;
            int section = 0;

            while (dt_size >= 0) {
                dr = dt.Rows[dt_size];
                //check for null values
                if (dr.ItemArray[0] == DBNull.Value || dr.ItemArray[1] == DBNull.Value ||
                    dr.ItemArray[2] == DBNull.Value || dr.ItemArray[3] == DBNull.Value ||
                    dr.ItemArray[4] == DBNull.Value || dr.ItemArray[5] == DBNull.Value) {
                    dt_size--;
                    continue;
                }


                course = (int)dr.ItemArray[0];
                section = (int)dr.ItemArray[5];
                quarter = (int)dr.ItemArray[4];
                //going to have to do the same with year probably

                //same course but different section is a different machine
                //different course is a different machine
                if ((currentCourse == course && (currentSection != section || currentQuarter != quarter)) || (currentCourse != course)) {
                    dummyMachine = new Machine();
                    currentCourse = (int)dr.ItemArray[0];
                    currentSection = (int)dr.ItemArray[5];
                    currentQuarter = (int)dr.ItemArray[4];
                }
                start = (int)dr.ItemArray[1];
                end = (int)dr.ItemArray[2];
                day = (int)dr.ItemArray[3];
                dummyDayTime = new DayTime();
                dummyDayTime.SetDayTime(day, start, end);
                dummyMachine.AddDayTime(dummyDayTime);
                dummyMachine.SetQuarter(quarter);

                //we add a new machine when we peek to the next row and see
                //(different course) OR (same course and (different section OR dif qtr))
                if (dt_size == 0 || ((int)dt.Rows[dt_size - 1].ItemArray[0] != currentCourse ||
                    ((int)dt.Rows[dt_size - 1].ItemArray[0] == currentCourse && ((int)dt.Rows[dt_size - 1].ItemArray[5] != currentSection) || (int)dt.Rows[dt_size - 1].ItemArray[4] != currentQuarter)
                    )
                    ) {
                    dummyMachine.AddJob(new Job(course));
                    for (int i = 0; i < machineNodes.Count; i++) {
                        MachineNode mn = (MachineNode)machineNodes[i];
                        ArrayList machines = mn.GetMachines();
                        if (machines.Count > 0) {
                            for (int j = 0; j < machines.Count; j++) {
                                Machine m = (Machine)machines[j];
                                if (m == dummyMachine) { //found the machine, just add job
                                    m.AddJob(new Job(course));
                                } else if (dummyMachine.GetYear().Equals(mn.GetYear()) && dummyMachine.GetQuarter().Equals(mn.GetQuarter())) { //machine does not exist, add it in
                                    machines.Add(dummyMachine);
                                    break;
                                }
                            }
                        } else if (dummyMachine.GetYear().Equals(mn.GetYear()) && dummyMachine.GetQuarter().Equals(mn.GetQuarter())) {
                            machines.Add(dummyMachine);
                            break;
                        } else {
                            Console.WriteLine("Dummy Machine Year: " + dummyMachine.GetYear());
                            Console.WriteLine("Dummy Machine Quarter: " + dummyMachine.GetQuarter());
                            Console.WriteLine("mn Year: " + mn.GetYear());
                            Console.WriteLine("mn Quarter: " + mn.GetQuarter());
                            Console.WriteLine('\n');
                        }
                    }
                }
                dt_size--;
            }
            //print machines for testing
            for (int i = 0; i < machineNodes.Count; i++) {
                MachineNode mn = (MachineNode)machineNodes[i];
                ArrayList machines = mn.GetMachines();
                Console.WriteLine("Quarter: " + mn.GetQuarter());
                for (int j = 0; j < machines.Count; j++) {
                    Machine m = (Machine)machines[j];
                    m.Print();
                }
            }
        }

        public void MakeStartingPoint(string s) {

        }

        private DataTable ExecuteQuery(string query) {
            OpenSQLConnection();
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
                Job job = new Job((int)row.ItemArray[0]);
                courseNums.Add(job);
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
