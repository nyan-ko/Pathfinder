using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Moves {
    public struct ActionCost {
        public float TotalCost;
        public float TurnAroundCost;
        public float MovementTowardsGoalCost;
        public bool Jumping;

        public static ActionCost CreateActionCost(float turnAroundCost, float movementCost, bool needJump) {
            if (movementCost != -1) {
                return new ActionCost(turnAroundCost, movementCost, needJump);
            }
            else {
                return ImpossibleCost;
            }
        }

        private ActionCost(float turnAroundCost, float movementCost, bool needJump) {
            TurnAroundCost = turnAroundCost;
            MovementTowardsGoalCost = movementCost;
            Jumping = needJump;
            TotalCost = turnAroundCost + movementCost;
        }

        public static ActionCost ImpossibleCost = new ActionCost() { TotalCost = float.MaxValue };

        public static ActionCost StartingNodeCost = new ActionCost() { TotalCost = 0 };

        public static bool operator < (ActionCost cost, ActionCost compare) {
            return cost.TotalCost < compare.TotalCost;
        }

        public static bool operator > (ActionCost cost, ActionCost compare) {
            return cost.TotalCost > compare.TotalCost;
        }

        public static bool operator != (ActionCost cost, ActionCost compare) {
            return cost.TotalCost != compare.TotalCost;
        }

        public static bool operator == (ActionCost cost, ActionCost compare) {
            return cost.TotalCost == compare.TotalCost;
        }

        public override string ToString() {
            return TotalCost.ToString();
        }
    }
}
