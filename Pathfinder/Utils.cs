using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Pathfinder {
    public static class PathfindingUtils {
        public static int GetNodeHash(int x, int y) {
            int hash = 5011;
            hash = 8957 * hash + x;
            hash = 46587 * hash + y;
            return hash;
        }

        public static bool IsWithin(int range, int threshold) {
            return false;
        }

        public static Vector2 AbsoluteDifference(this Vector2 origin, Vector2 compare) {
            return new Vector2(Math.Abs(origin.X - compare.X), Math.Abs(origin.Y - compare.Y));
        }


        // probably won't use reckless absolutes
        // fast cood fast cood
        public static int RecklessAbsolute(int value) {
            return (value + (value >> 31)) ^ (value >> 31);  // magics
        }
        
        public static float RecklessAbsolute(float value) {
            return value > 0 ? value : -value;
        }
    }
}
