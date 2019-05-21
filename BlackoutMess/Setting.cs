using System;
using System.Linq;

namespace BlackoutMess
{
    public class Setting
    {
        public string Name;

        public string[] States;

        public string State { get; private set; }

        public Setting(string name, string[] states, string state)
        {
            Name = name;
            States = states;
            if (!SetState(state))
            {
                throw new ArgumentException();
            }
        }

        public Setting(string name, string[] states)
        {
            Name = name;
            States = states;
        }

        // Returns true when operation successful
        public bool SetState(string state)
        {
            if (States.Any(s => s == state))
            {
                State = state;
                return true;
            }

            return false;
        }
    }
}