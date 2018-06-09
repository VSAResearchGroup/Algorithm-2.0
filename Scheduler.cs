using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Scheduler {
    class Scheduler {
        SqlConnection myConnection;      //Declare the SQL connection to

        private CourseNetwork network;   //use Cashman network
        private List<MachineNode> machineNodes; //representing each quarter
        private DegreePlan myPlan;       //pull from DB
        private List<Machine> finalPlan; //output schedule
        private Preferences preferences; //currently hard coded, take from UI later
        private List<Job> completedPrior;//starting point, not implemented currently
        private List<Job> unableToSchedule;//list of courses that didn't fit into the schedule

        private const int QUARTERS = 4;


        //------------------------------------------------------------------------------
        // 
        // default constructor
        // 
        //------------------------------------------------------------------------------
        public Scheduler() {
            machineNodes = new List<MachineNode>();
            finalPlan = new List<Machine>();
            completedPrior = new List<Job>();
            unableToSchedule = new List<Job>();
            InitMachineNodes();
            InitMachines();
            InitYearTwo(); //temporary fix for the second year
            InitNetwork();
        }

        //------------------------------------------------------------------------------
        // 
        // initiates Andrue Cashman's network
        // 
        //------------------------------------------------------------------------------
        private void InitNetwork() {
            string rawcourses = File.ReadAllText("../../AllCourses.json");
            string rawpreqs = File.ReadAllText("../../PrereqNetwork.json");
            network = new CourseNetwork(rawcourses, rawpreqs);
            network.BuildNetwork();
        }

        //------------------------------------------------------------------------------
        // 
        // creates schedule by looping through all the major courses
        // 
        //------------------------------------------------------------------------------
        public List<Machine> CreateSchedule() {
            List<Job> majorCourses = myPlan.GetList(0);
            for (int i = 0; i < majorCourses.Count; i++) {
                Job job = majorCourses[i];
                ScheduleCourse(job);
            }
            finalPlan = GetBusyMachines();
            return finalPlan;
        }

        
        //------------------------------------------------------------------------------
        // similar to depth first search algorithm. Does the action of searching through
        // network and scheduling prerequisites before scheduling the class
        // 
        //------------------------------------------------------------------------------
        private void ScheduleCourse(Job job) {
            if (IsScheduled(job)) {
                return;
            }
            int num = job.GetID();
            List<CourseNode> groups = network.FindShortPath(num);//find prerequisite group
            if (PrereqsExist(groups) && !job.GetPrerequisitesScheduled()) { //if j does not have prerequisites (OR its prerequisites have been scheduled) schedule j  
                //schedule j's prerequisites by geting shortest group and whatnot
                int shortest = GetShortestGroup(groups);//so now we have the shortest list
                List<CourseNode> group = groups[shortest].prereqs;
                List<Job> jobsToBeScheduled = new List<Job>();
                for (int j = 0; j < group.Count; j++) {
                    Job myJob = new Job(group[j].prerequisiteID);
                    jobsToBeScheduled.Add(myJob);
                }//now we have a list full of jobs to be scheduled

                for (int k = 0; k < jobsToBeScheduled.Count; k++) { //schedule them all here
                    ScheduleCourse(jobsToBeScheduled[k]);
                }//now they are scheduled
                job.SetPrerequisitesScheduled(true);

            }
            PutCourseOnMachine(job, groups);
            if (!job.GetScheduled()) { //figure out what to do if it wasn't able to be scheduled, make this a function later
                unableToSchedule.Add(job);
            }
        }

        //------------------------------------------------------------------------------
        // checks if a course is already scheduled. Because courses are returned as 
        // numbers from Cashman network and not Job type, we can't check for the 
        // instance, we have to find it
        //------------------------------------------------------------------------------
        private bool IsScheduled(Job j) {
            for (int i = 0; i < finalPlan.Count; i++) {
                Machine m = finalPlan[i];
                if (m.GetCurrentJobProcessing().GetID() == j.GetID()) {
                    return true;
                }
            }
            return false;
        }

        //------------------------------------------------------------------------------
        // checks if prerequisite exists; this function can be eliminated, I just didn't
        // quite understand why cashman network had so many lists of lists.
        // 
        //------------------------------------------------------------------------------
        private bool PrereqsExist(List<CourseNode> groups) {
            for (int i = 0; i < groups.Count; i++) {
                if (groups[i].prereqs != null) {
                    return true;
                }
            }
            return false;
        }

        //------------------------------------------------------------------------------
        // does the actual action of putting a course on a machine; this will be the hub
        // for implementing preferences, not all are implemented at the moment; also,
        // right now unscheduled courses are simply going into a list but if you were to 
        // do something else, this function is where it would originate
        //------------------------------------------------------------------------------
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
                if (mostRecentPrereqQuarter == -1 || mostRecentPrereqYear == -1) { //has prerequisites but they weren't able to be scheduled
                    unableToSchedule.Add(j);
                    return;
                }

                //schedule 1 or more quarters after, mind the year
                //schedule on nearest available machine
                //start i at whatever quarter you calculate, not simply zero

                start = (mostRecentPrereqYear * 4 + mostRecentPrereqQuarter - 1) + 1;

            }
            for (int i = start; i < machineNodes.Count; i++) {
                MachineNode mn = machineNodes[i];
                //if machine node exeeds preferences continue to next node
                if (mn.GetClassesScheduled() > 3) {
                    continue;
                }
                List<Machine> machines = mn.GetMachines();
                for (int k = 0; k < machines.Count; k++) {
                    Machine m = machines[k];
                    if (m.CanDoJob(j) && !m.CheckInUse()) { //if not in use and it can do the job
                        if (Overlap(j, m, mn)) { //can't schedule it if the times overlap even if machine found
                            continue;
                        }
                        m.SetCurrentJobProcessing(j);
                        m.SetInUse(true);
                        j.SetScheduled(true);
                        j.SetQuarterScheduled(m.GetQuarter());
                        j.SetYearScheduled(m.GetYear());
                        mn.AddClassesScheduled(1);
                        finalPlan.Add(m);
                        return;
                    }
                }
            }
        }

        //------------------------------------------------------------------------------
        // 
        // check to see if a job overlaps with another job's times in a single 
        // MachineNode; can't be in 2 places at once you know?
        //------------------------------------------------------------------------------
        private bool Overlap(Job j, Machine goal, MachineNode mn) {
            bool flag = false;
            //need list of all the start and end times from goal
            List<DayTime> dt = goal.GetDateTime();
            List<Machine> myMachines = mn.GetAllScheduledMachines();
            for (int i = 0; i < myMachines.Count; i++) {
                Machine m = myMachines[i];
                List<DayTime> tempDT = m.GetDateTime();
                if(dt.Count==tempDT.Count) {
                    for (int k = 0; k < dt.Count; k++) {
                        if ((dt[k].GetStartTime() >= tempDT[k].GetStartTime() && dt[k].GetStartTime() <= tempDT[k].GetEndTime()) ||
                        (dt[k].GetEndTime() >= tempDT[k].GetStartTime() && dt[k].GetEndTime() <= tempDT[k].GetEndTime())) {
                            return true;
                        }
                    }
                } else {
                    int min = Math.Min(dt.Count, tempDT.Count);
                    if(dt.Count == min) {
                        flag = compareDays(dt, tempDT);
                    } else {
                        flag = compareDays(tempDT, dt);
                    }
                    if(flag) {
                        return flag;
                    }
                }
            }
            return flag;
        }

        //------------------------------------------------------------------------------
        // helper function for overlap. it's pretty tricky to compare the lists when
        // they are not the same length because of nulls and out of ranges. Can be 
        // eliminated with a more clever algorithm that works of any size lists
        //------------------------------------------------------------------------------
        private bool compareDays(List<DayTime> smaller, List<DayTime> larger) {
            for (int k = 0; k < smaller.Count; k++) {// go through all days in smaller
                int smallDay = smaller[k].GetDay(); //get day from smaller
                    int largeDayIndex = -1;
                for (int j =0; j < larger.Count; j++) { //find that day in larger
                    if(larger[j].GetDay() == smallDay) {
                        largeDayIndex = j;
                        break;
                    }
                }
                //compare that day
                if ((smaller[k].GetStartTime() >= larger[largeDayIndex].GetStartTime() && smaller[k].GetStartTime() <= larger[largeDayIndex].GetEndTime()) ||
                (smaller[k].GetEndTime() >= larger[largeDayIndex].GetStartTime() && smaller[k].GetEndTime() <= larger[largeDayIndex].GetEndTime())) {
                    return true;
                }
            }
            return false;
        }

        //------------------------------------------------------------------------------
        // find by retrieving job and looking at when it was scheduled
        // only called if the job actually has prerequisites
        // 
        //------------------------------------------------------------------------------
        private int[] GetMostRecentPrereq(List<CourseNode> groups) {
            int mostRecentPrereqYear = -1;
            int mostRecentPrereqQuarter = -1;
            for (int i = 1; i < groups.Count; i++) {
                for (int j = 0; j < finalPlan.Count; j++) {
                    Machine m = finalPlan[j];
                    if (m.GetCurrentJobProcessing() is null || groups[i].prereqs[0] is null) continue;

                    if (m.GetCurrentJobProcessing().GetID() == groups[i].prereqs[0].prerequisiteID) { //found the course
                        if (m.GetCurrentJobProcessing().GetYearScheduled() > mostRecentPrereqYear ||
                            (m.GetCurrentJobProcessing().GetYearScheduled() == mostRecentPrereqYear &&
                            m.GetCurrentJobProcessing().GetQuarterScheduled() > mostRecentPrereqQuarter)) { //now check if it is more recent
                            mostRecentPrereqYear = m.GetCurrentJobProcessing().GetYearScheduled();
                            mostRecentPrereqQuarter = m.GetCurrentJobProcessing().GetQuarterScheduled();
                        }
                    }
                }
            }
            return new int[] { mostRecentPrereqYear, mostRecentPrereqQuarter };
        }

        //------------------------------------------------------------------------------
        // if for course A you have to take [B, F, K] OR [J, Z], we pick the latter
        // option because we don't want to take a lot of classes; in the long run,
        // this is not always the fastest option so this can be optimized
        //------------------------------------------------------------------------------
        private int GetShortestGroup(List<CourseNode> groups) {
            int shortest = int.MaxValue;
            for (int j = 1; j < groups.Count; j++) { //find the shortest group that is not null
                if (groups[j].prereqs.Count < shortest && groups[j].prereqs != null) {
                    shortest = j;
                }
            }//so now we have the shortest list
            return shortest;
        }

        //------------------------------------------------------------------------------
        // temporary until we have more data, better data
        // 
        // 
        //------------------------------------------------------------------------------
        private void InitYearTwo() {
            //init more machine nodes for the next year
            for (int i = 1; i <= QUARTERS; i++) {
                MachineNode m = new MachineNode(1, i);
                machineNodes.Add(m);
            }
            //transfer all the same classes to the set of machine nodes
            for (int i = 4; i < 8; i++) {
                MachineNode oldMn = machineNodes[i - 4];
                MachineNode newMn = machineNodes[i];
                for (int j = 0; j < oldMn.GetMachines().Count; j++) {
                    Machine oldMachine = oldMn.GetMachines()[j];
                    Machine newMachine = new Machine(oldMachine);
                    newMachine.SetYear(1);
                    newMn.AddMachine(newMachine);
                }
            }
        }

        //------------------------------------------------------------------------------
        // initializes machineNodes
        // 
        // 
        //------------------------------------------------------------------------------
        private void InitMachineNodes() {
            for (int i = 1; i <= QUARTERS; i++) {
                MachineNode m = new MachineNode(0, i);
                machineNodes.Add(m);
            }
        }

        //------------------------------------------------------------------------------
        // create a query that will pull all the different machines
        // which means getting every single time slot
        // distinct year, quarter, time, and set of DayTimes
        //------------------------------------------------------------------------------
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
                    ((int)dt.Rows[dt_size - 1].ItemArray[0] == currentCourse && ((int)dt.Rows[dt_size - 1].ItemArray[5] != currentSection) || (int)dt.Rows[dt_size - 1].ItemArray[4] != currentQuarter))) {
                    dummyMachine.AddJob(new Job(course));
                    for (int i = 0; i < machineNodes.Count; i++) {
                        MachineNode mn = machineNodes[i];
                        List<Machine> machines = mn.GetMachines();
                        if (machines.Count > 0) {
                            for (int j = 0; j < machines.Count; j++) {
                                Machine m = machines[j];
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
            //print machines for testing; unnecessary
            for (int i = 0; i < machineNodes.Count; i++) {
                MachineNode mn = machineNodes[i];
                List<Machine> machines = mn.GetMachines();
                Console.WriteLine("Quarter: " + mn.GetQuarter());
                for (int j = 0; j < machines.Count; j++) {
                    Machine m = machines[j];
                    m.Print();
                }
            }
        }

        //------------------------------------------------------------------------------
        // to be implemented when we can take in courses from the UI that the user has
        // taken. we simply skip taking that course in "putcourseonmachine" or
        // something
        //------------------------------------------------------------------------------
        public void MakeStartingPoint(string s) {

        }

        //------------------------------------------------------------------------------
        // concise code to execute queries
        // 
        // 
        //------------------------------------------------------------------------------
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

        //------------------------------------------------------------------------------
        // retrieves the degree plan we seek. input hard coded from the driver but in 
        // the future it should be taken from the UI. 
        // query admissionrequiredcourses
        //------------------------------------------------------------------------------
        public void InitDegreePlan(int majorID, int schoolID) {
            string query = "select CourseID from AdmissionRequiredCourses where MajorID ="
                            + majorID + " and SchoolID = " + schoolID + " order by CourseID ASC ";
            DataTable dt = ExecuteQuery(query);
            List<Job> courseNums = new List<Job>();
            foreach (DataRow row in dt.Rows) {
                Job job = new Job((int)row.ItemArray[0]);
                courseNums.Add(job);
            }
            myPlan = new DegreePlan(courseNums);
        }

        //------------------------------------------------------------------------------
        // sql connection
        // 
        // 
        //------------------------------------------------------------------------------
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
        //------------------------------------------------------------------------------
        // hard coded now, take from UI later
        //•	Being scheduled during the summer(NOT IMPLEMENTED)
        //•	Maximum number of core courses per quarter(NOT IMPLEMENTED)
        //•	How many quarters you’d like to spread the plan over(MAX of 16)
        //                                                         (NOT IMPLEMENTED)
        //•	Time interval for when a person is available to go to
        //               class. For example, they are available 8AM-1PM.
        //              LOOK AT TABLE TimeSlot(NOT IMPLEMENTED)
        //•	Credits they would like to take per quarter.   (IMPLEMENTED)
        //•	starting quarter of plan. [1,2,3,4]  (NOT IMPLEMENTED)
        // 
        //------------------------------------------------------------------------------
        private void CreatePreferences() {
            preferences = new Preferences();
            preferences.AddPreference("SUMMER", false);
            preferences.AddPreference("CORE_PER_QUARTER", 10);
            preferences.AddPreference("MAX_QUARTERS", 16);
            preferences.AddPreference("TIME_INTERVAL", new DayTime(1, 70, 130)); //do a whole bunch?
            preferences.AddPreference("CREDITS_PER_QUARTER", 15);
            preferences.AddPreference("STARTING_QUARTER", 2);
        }

        //------------------------------------------------------------------------------
        // PASSES busy machines to driver as final plan. in the future, it will be
        // serialized and passed to UI
        //------------------------------------------------------------------------------
        private List<Machine> GetBusyMachines() {
            List<Machine> busy = new List<Machine>();
            for (int i = 0; i < machineNodes.Capacity; i++) {
                List<Machine> machines = machineNodes[i].GetAllScheduledMachines();
                for (int j = 0; j < machines.Count; j++) {
                    busy.Add(machines[j]);
                }
            }
            return busy;
        }

        //------------------------------------------------------------------------------
        // PASSES unscheduled machines to driver as final plan. in the future, it will 
        // be serialized and passed to UI
        //------------------------------------------------------------------------------
        public List<Job> GetUnscheduledCourses() {
            return unableToSchedule;
        }

    }
}
