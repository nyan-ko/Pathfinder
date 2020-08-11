using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Input {
    public struct Trigger {
        public string Input;
        public int Duration;
        public int DelayFromStart;

        public Trigger(string input, int duration, int delay) {
            Input = input;
            Duration = duration;
            DelayFromStart = delay;
        }
    }
}
