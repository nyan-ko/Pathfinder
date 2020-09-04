using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;

namespace Pathfinder.Input {
    public class PathfinderTriggersSet : TriggersSet {
        private List<Trigger> inputs = new List<Trigger>();
        private List<Trigger> activeInputs = new List<Trigger>();
        private List<string> oldInputs = new List<string>();
        private bool ordered = false;

        private long frameCount = 0;

        public PathfinderTriggersSet() {
            SetupKeys();
        }

        public void AddTrigger(byte input, float duration, float delay) => AddTrigger(new Trigger(input, duration, delay));

        public void AddTrigger(Trigger trigger) {
            //if (inputs.Count != 0) {
            //    int lastIndex = inputs.Count - 1;
            //    var lastTrigger = inputs[lastIndex];

            //    if (lastTrigger.Input == trigger.Input) {
            //        lastTrigger.Duration += trigger.DelayFromStart + trigger.Duration;
            //        inputs.RemoveAt(lastIndex);
            //        inputs.Add(lastTrigger);
            //        return;
            //    }
            //}

            inputs.Add(trigger);
        }

        public void SortInputList() {
            if (ordered)
                return;

            lock (inputs) {
                var inputList = from input in inputs
                                orderby input.DelayFromStart
                                select input;
                inputs = inputList.ToList();
                ordered = true;
            }
        }

        private void UpdateInternalInputs() {
            if (oldInputs.Count != 0) {
                foreach (var input in oldInputs) {
                    KeyStatus[input] = false;
                }
                oldInputs.Clear();
            }

            while (inputs.Count > 0 && inputs[0].DelayFromStart <= frameCount) {
                activeInputs.Add(inputs[0]);
                inputs.RemoveAt(0);
            }


            if (activeInputs.Count != 0) {
                for (int k = 0; k < activeInputs.Count; k++) {
                    Trigger trigger = activeInputs[k];
                    var inputs = trigger.Input.Split(Trigger.INPUT_SEPARATOR);

                    for (int m = 0; m < inputs.Length; m++) {
                        string actualInput = inputs[m];
                        KeyStatus[actualInput] = true;
                        oldInputs.Add(actualInput);
                    }

                    if (trigger.Duration - (frameCount - trigger.DelayFromStart) <= 0) {
                        activeInputs.RemoveAt(k);
                        continue;
                    }
                }
            }

            frameCount++;
        }

        public void Update() {
            if (inputs.Count > 0) {
                if (!ordered) {
                    SortInputList();
                }

                UpdateInternalInputs();
            }
        }

        public bool DoneInputting => inputs?.Count == 0;
    }
}
