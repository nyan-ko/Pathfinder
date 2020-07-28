﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Pathfinder {
    public static class Utils {
        public static bool IsWithin(int range, int threshold) {
            return false;
        }

        public static Vector2 AbsoluteDifference(this Vector2 origin, Vector2 compare) {
            return new Vector2(Math.Abs(origin.X - compare.X), Math.Abs(origin.Y - compare.Y));
        }
    }
}
