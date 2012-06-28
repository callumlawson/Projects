using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace HauntedHouse.Utilities
{
    public class ScreenDebuger
    {
        //ArrayList variableStrings;
      
        int numDebugStrings = 4;

   
        Dictionary<string, string> debugStringsDict;
        ArrayList debugStrings;

        public ScreenDebuger()
        {
            debugStringsDict = new Dictionary<string, string>();
            debugStrings = new ArrayList();
            //variableStrings = new ArrayList();
            //valueStrings = new ArrayList();
        }

        public void writeTempDebug(string variableName, string value)
        {
            debugStrings.Clear();

            if (debugStringsDict.ContainsKey(variableName))
            {
                debugStringsDict[variableName] = value;
            }   
            else
            {
                debugStringsDict.Add(variableName, value);
            }

            foreach(KeyValuePair<string,string> pair in debugStringsDict)
            {
                debugStrings.Add(pair.Key + ": " + pair.Value);
            }
        }

        public int NumDebugStrings
        {
            get { return this.numDebugStrings; }
            set { this.numDebugStrings = value; }
        }

        public ArrayList DebugStrings
        {
            get { return this.debugStrings; }
            //set { this.debugStrings = value; }
        }
    }
}
