using System;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Preferences {
        Hashtable preferences;

        public Preferences() {
            preferences = new Hashtable();
        }

        public void AddPreference(String name, Object a) {
            if(!preferences.ContainsKey(name)) preferences.Add(name, a);
        }

        public void DeletePreference(String name) {
            if (preferences.ContainsKey(name)) preferences.Remove(name);
        }

        public bool Exists(String name) {
            return preferences.ContainsKey(name);
        }

        public Object GetPreference(String name) {
            return preferences[name];
        }


    }
}
