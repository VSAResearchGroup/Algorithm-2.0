using System;
using System.Collections;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Scheduler {
    class Scheduler {
        SqlConnection myConnection;      //Declare the SQL connection to

        private CourseNetwork network;   //use Cashman network
        private ArrayList machineNodes;
        private DegreePlan myPlan;       //pull from DB
        private ArrayList finalPlan;     //output schedule
        private Preferences preferences;
        private ArrayList completedPrior;//starting point
        private ArrayList unableToSchedule;

        private const int QUARTERS = 4;

        public Scheduler() {
            machineNodes = new ArrayList();
            finalPlan = new ArrayList();
            completedPrior = new ArrayList();
            unableToSchedule = new ArrayList();
            InitMachineNodes();
            InitMachines();
            InitYearTwo(); //temporary fix for the second year
            InitNetwork();
        }

        private void InitNetwork() {
            string rawcourses = File.ReadAllText("../../AllCourses.json");
            string rawpreqs = File.ReadAllText("../../PrereqNetwork.json");
            network = new CourseNetwork(rawcourses, rawpreqs);
            network.BuildNetwork();
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

        //make it similar to depth first search?
        private void ScheduleCourse(Job job) {
            int num = job.GetID();
            List<CourseNode> groups = network.FindShortPath(num);//find prerequisite group
            if (!PrereqsExist(groups) || job.GetPrerequisitesScheduled()) { //if j does not have prerequisites (OR its prerequisites have been scheduled) schedule j  
                PutCourseOnMachine(job, groups);
                return;
            } else {//schedule j's prerequisites by geting shortest group and whatnot
                if (job.GetScheduled()) {
                    return;
                }
                int shortest = GetShortestGroup(groups);//so now we have the shortest list
                List<CourseNode> group = groups[shortest].prereqs;
                ArrayList jobsToBeScheduled = new ArrayList();
                for (int j = 0; j < group.Count; j++) {
                    Job myJob = new Job(group[j].prerequisiteID);
                    jobsToBeScheduled.Add(myJob);
                }//now we have a list full of jobs to be scheduled

                for (int k = 0; k < jobsToBeScheduled.Count; k++) { //schedule them all here
                    ScheduleCourse((Job)jobsToBeScheduled[k]);
                }//now they are scheduled
                job.SetPrerequisitesScheduled(true);
            }
            return;
        }


        private bool PrereqsExist(List<CourseNode> groups) {
            for(int i = 0; i < groups.Count; i++) {
                if(groups[i].prereqs != null) {
                    return true;
                }
            }
            return false;
        }
        //preferences not implemented yet but this is where they would happen

        //HERE YOU WILL ALSO DETERMINE WHAT TO DO IF IT CANT BE SCHEDULED
        private void PutCourseOnMachine(Job j, List<CourseNode> groups) {
            //get most recent prereq
            int mostRecentPrereqYear = 0;
            int mostRecentPrereqQuarter = 1;
            int start = 0;
            //if no prereqs then schedule at any time
            if (PrereqsExist(groups)) { //this is if there are prereqs
                int[] yq = GetMostRecentPrereq(groups);
                mostRecentPrereqYear = yq[0];
                mostRecentPrereqQuarter = yq[1];

                //schedule 1 or more quarters after, mind the year
                //schedule on nearest available machine
                //start i at whatever quarter you calculate, not simply zero

                start = (mostRecentPrereqYear * 4 + mostRecentPrereqQuarter - 1) + 1;

            }
            for (int i = start; i < machineNodes.Count; i++) {
                MachineNode mn = (MachineNode)machineNodes[i];
                ArrayList machines = mn.GetMachines();
                for (int k = 0; k < machines.Count; k++) {
                    Machine m = (Machine)machines[k];
                    if (m.CanDoJob(j) && !m.CheckInUse()) { //if not in use and it can do the job
                        m.SetCurrentJobProcessing(j);
                        m.SetInUse(true);
                        j.SetScheduled(true);
                        finalPlan.Add(m);
                        break;
                    }
                }
            }
            if (!j.GetScheduled()) { //figure out what to do if it wasn't able to be scheduled
                unableToSchedule.Add(j);  
            }
        }

        //find by retrieving job and looking at when it was scheduled
        //only called if the job actually has prerequisites
        //this algorithm should be optimized for speed. right now it is just doing a linear search
        // n times where n is the number of prerequisites
        private int[] GetMostRecentPrereq(List<CourseNode> groups) {
            int[] yq = new int[2];
            int mostRecentPrereqYear = -1;
            int mostRecentPrereqQuarter = -1;
            for (int i = 0; i < groups.Count; i++) {
                for (int k = 0; k < machineNodes.Count; k++) {
                    MachineNode mn = (MachineNode)machineNodes[k];
                    ArrayList machines = mn.GetMachines();
                    for (int j = 0; j < machines.Count; j++) {
                        Machine m = (Machine)machines[j];
                        if(m.GetCurrentJobProcessing().GetID() == groups[i].courseID) { //found the course
                            if(m.GetCurrentJobProcessing().GetYearScheduled() > mostRecentPrereqYear ||
                                (m.GetCurrentJobProcessing().GetYearScheduled() == mostRecentPrereqYear &&
                                m.GetCurrentJobProcessing().GetQuarterScheduled() > mostRecentPrereqQuarter) ) { //now check if it is more recent
                                mostRecentPrereqYear = m.GetCurrentJobProcessing().GetYearScheduled();
                                mostRecentPrereqQuarter = m.GetCurrentJobProcessing().GetQuarterScheduled();
                            }
                        }
                    }
                }
            }
            yq[0] = mostRecentPrereqYear;
            yq[1] = mostRecentPrereqQuarter;
            return yq;
        }

        private int GetShortestGroup(List<CourseNode> groups) {
            int shortest = int.MaxValue;
            for (int j = 1; j < groups.Count; j++) { //find the shortest group that is not null
                if (groups[j].prereqs.Count < shortest && groups[j].prereqs != null) {
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
            •	Being scheduled during the summer   (WILL IMPLEMENT THIS QUARTER)
            •	Maximum number of core courses per quarter   (WILL NOT IMPLEMENT THIS QUARTER)
            •	How many quarters you’d like to spread the plan over (MAX of 16)   (WILL NOT IMPLEMENT THIS QUARTER)
            •	Time interval for when a person is available to go to 
                        class. For example, they are available 8AM-1PM.
                        LOOK AT TABLE TimeSlot   (WILL NOT IMPLEMENT THIS QUARTER)
            •	Credits they would like to take per quarter.   (WILL IMPLEMENT THIS QUARTER)
            •	starting quarter of plan. [1,2,3,4]  (WILL IMPLEMENT THIS QUARTER)

        */

        //hard coded now, take from UI later
        private void CreatePreferences() {
            preferences = new Preferences();
            preferences.AddPreference("SUMMER", false);
            preferences.AddPreference("CORE_PER_QUARTER", 10);
            preferences.AddPreference("MAX_QUARTERS", 16);
            preferences.AddPreference("TIME_INTERVAL", new DayTime(1, 70, 130)); //do a whole bunch?
            preferences.AddPreference("CREDITS_PER_QUARTER", 15);
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

        public ArrayList GetUnscheduledCourses() {
            return unableToSchedule;
        }

    }
}
