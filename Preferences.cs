using System;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Preferences {
        Hashtable preferences;

        //------------------------------------------------------------------------------
        // 
        // default constructor
        // 
        //------------------------------------------------------------------------------
        public Preferences() {
            preferences = new Hashtable();
        }

        //------------------------------------------------------------------------------
        // 
        // extendable to add more preferences
        // 
        //------------------------------------------------------------------------------
        public void AddPreference(String name, Object a) {
            if(!preferences.ContainsKey(name)) preferences.Add(name, a);
        }

        //------------------------------------------------------------------------------
        // 
        // removes preferences
        // 
        //------------------------------------------------------------------------------
        public void DeletePreference(String name) {
            if (preferences.ContainsKey(name)) preferences.Remove(name);
        }

        //------------------------------------------------------------------------------
        // 
        // checks if a preference exists; not used at the moment
        // 
        //------------------------------------------------------------------------------
        public bool Exists(String name) {
            return preferences.ContainsKey(name);
        }

        //------------------------------------------------------------------------------
        // 
        // returns a preference
        // 
        //------------------------------------------------------------------------------
        public Object GetPreference(String name) {
            return preferences[name];
        }


    }
}
