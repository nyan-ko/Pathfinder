using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Input {
    public struct Trigger {
        public readonly string[] Input;
        public readonly float Duration;
        public readonly float DelayFromStart;

        public Trigger(string[] input, float duration, float delay) {
            Input = input;
            Duration = duration;
            DelayFromStart = delay;
        }

        public Trigger(byte input, float duration, float delay) {
            if (!StringInputsByNodeInputs.TryGetValue(input, out string[] strInput) && input != 0) {
                throw new InvalidOperationException();
            }

            Input = strInput;
            Duration = duration;
            DelayFromStart = delay;
        }

        public static Dictionary<byte, string[]> StringInputsByNodeInputs = new Dictionary<byte, string[]> {
            { 1, new string[] { "Jump" } },
            { 2, new string[] { "Jump", "Right" } },
            { 3, new string[] { "Right" } },
            { 4, new string[] { "Down", "Right" } },
            { 5, new string[] { "Down" } },
            { 6, new string[] { "Down", "Left" } },
            { 7, new string[] { "Left" } },
            { 8, new string[] { "Jump", "Left" } }
        };
    }
}
