using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;

namespace Pathfinder.Input {
    public class PathfinderTriggersSet : TriggersSet {
        private List<Trigger> inputs;
        private List<Trigger> activeInputs = new List<Trigger>();
        private List<string> oldInputs = new List<string>();

        private long frameCount = 0;

        public void SetList(List<Trigger> triggers) {
            inputs = triggers;
            inputs.OrderBy(x => x.DelayFromStart);
        }

        public void Update() {
            if (oldInputs.Count != 0) {
                foreach (var input in oldInputs) {
                    KeyStatus[input] = false;
                }
                oldInputs.Clear();
            }

            int i = 0;
            while (inputs[i].DelayFromStart <= frameCount) {
                activeInputs.Add(inputs[i]);
                inputs.RemoveAt(i);
                i++;
            }

            if (activeInputs.Count != 0) {
                for (int k = 0; k < activeInputs.Count; k++) {
                    Trigger trigger = activeInputs[k];

                    if (trigger.Duration <= 0) {
                        activeInputs.RemoveAt(k);
                        continue;
                    }

                    KeyStatus[trigger.Input] = true;
                    oldInputs.Add(trigger.Input);
                    trigger.Duration--;
                }
            }

            frameCount++;
        }
    }
}
