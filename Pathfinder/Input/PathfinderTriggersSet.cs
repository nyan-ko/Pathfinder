using System.Collections.Generic;
using System.Linq;

namespace Pathfinder.Input
{
    public class PathfinderTriggersSet : TriggersSet
    {
        private List<Trigger> inputs;
        private List<Trigger> activeInputs = new List<Trigger>();
        private List<string> oldInputs = new List<string>();

        private long frameCount = 0;

        public PathfinderTriggersSet()
        {
            SetupKeys();
        }

        public void SetList(List<Trigger> triggers)
        {
            inputs = triggers;
            inputs.OrderBy(x => x.DelayFromStart);
        }

        private void UpdateInternalInputs()
        {
            if (oldInputs.Count != 0)
            {
                foreach (var input in oldInputs)
                {
                    KeyStatus[input] = false;
                }
                oldInputs.Clear();
            }

            int i = 0;
            while (inputs[i].DelayFromStart <= frameCount)
            {
                activeInputs.Add(inputs[i]);
                inputs.RemoveAt(i);
                i++;

                if (i >= inputs.Count)
                {
                    break;
                }
            }

            if (activeInputs.Count != 0)
            {
                for (int k = 0; k < activeInputs.Count; k++)
                {
                    Trigger trigger = activeInputs[k];

                    if (trigger.Duration <= 0)
                    {
                        activeInputs.RemoveAt(k);
                        continue;
                    }
                    if (trigger.Input == "")
                    {
                        trigger.Duration--;
                        continue;
                    }

                    var inputs = trigger.Input.Split(Trigger.INPUT_SEPARATOR);

                    for (int m = 0; m < inputs.Length; m++)
                    {
                        string actualInput = inputs[m];
                        KeyStatus[actualInput] = true;
                        oldInputs.Add(actualInput);
                        trigger.Duration--;
                    }
                }
            }

            frameCount++;
        }

        public void Update()
        {
            if (inputs?.Count > 0)
            {
                UpdateInternalInputs();
            }
        }

        public bool DoneInputting => inputs?.Count == 0;
    }
}