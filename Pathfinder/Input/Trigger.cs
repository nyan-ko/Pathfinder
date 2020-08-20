using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Input {
    public struct Trigger {
        public const char INPUT_SEPARATOR = '|'; 

        public string Input;
        public float Duration;
        public float DelayFromStart;

        public Trigger(string input, float duration, float delay) {
            Input = input;
            Duration = duration;
            DelayFromStart = delay;
        }

        public Trigger(byte input, float duration, float delay) {
            if (!StringInputsByNodeInputs.TryGetValue(input, out string strInput) && input != 0) {
                throw new InvalidOperationException();
            }

            Input = strInput;
            Duration = duration;
            DelayFromStart = delay;
        }

        public static Dictionary<byte, string> StringInputsByNodeInputs = new Dictionary<byte, string> {
            { 1, "Jump" },
            { 2, "Jump|Right" },
            { 3, "Right" },
            { 4, "Down|Right" },
            { 5, "Down" },
            { 6, "Down|Left" },
            { 7, "Left" },
            { 8, "Jump|Left" }
        };
    }
}
