using System.Collections.Generic;
using UnityEngine;

/* BASE STATE STYLE /*
[System.Serializable]
public class Base : States
{
    public Base()
    {
        states = new List<State>()
        {
            new State("Test1"),
            new State("Test2"),
            new State("Test3")
        };
    }
}
*/

namespace Game.Utilities
{
    /// <summary>
    ///It manages all the simple state schema of a script, just create 
    ///an instance of it where you want to create states and call its 
    ///voids for actions.
    /// </summary>
    public class States
    {
        /// <summary>
        /// State structure
        /// </summary>
        public class State
        {
            [HideInInspector]
            public string name;
            public bool value;

            public State(string _name, bool _value = false)
            {
                name = _name;
                value = _value;
            }
        }

        /// <summary>
        /// Current list of states
        /// </summary>
        protected List<State> states;

        /// <summary>
        /// Sets the current state value.
        /// </summary>
        /// <param name="_stateName">State name to be modified.</param>
        /// <param name="_value">Value that the state.</param>
        public void SetState(string _stateName, bool _value)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].name.Equals(_stateName))
                {
                    states[i].value = _value;
                }
            }
        }

        /// <summary>
        /// Returns the current state value.
        /// </summary>
        /// <param name="_stateName">State name to be modified.</param>
        /// <returns></returns>
        public bool GetState(string _stateName)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].name.Equals(_stateName))
                {
                    return states[i].value;
                }
            }
            return false;
        }
    }
}