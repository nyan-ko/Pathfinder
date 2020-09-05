using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Pathfinder {
    public static class PathfindingUtils {
        // guaranteed no collisions, tested by iterating through all possible tile coordinates
        // numbers used can probably be smaller though
        public static long GetNodeHash(int x, int y) {
            long hash = 50113401;
            hash = 89577541 * hash + x;
            hash = 46587 * hash + y;
            return hash;
        }

        // yes this is basically Terraria.Utils.FloatIntersect() i'll probably change it
        public static bool IsEntityIntersectingWithEntity(float x, float y, float w, float h, float x2, float y2, float w2, float h2) {
            return x <= x2 + w2 &&
                x + w >= x2 &&
                y <= y2 + h2 &&
                y + h >= y2;
        } 

        public static bool IsPositionInCorrectRelativePosition(float baseX, float baseY, float compareX, float compareY, int xDirection, int yDirection) {
            var xSign = Math.Sign(compareX - baseX) * xDirection;
            var ySign = Math.Sign(compareY - baseY) * yDirection;
            return xSign >= 0 && ySign >= 0;
        }

        public static bool IsTileSolid(int x, int y) {
            Tile tile = Main.tile[x, y];
            return tile != null && Main.tileSolid[tile.type] && tile.active() && !tile.inActive();
        }

        //// probably won't use reckless absolutes
        // fast cood fast cood
        public static int RecklessAbsolute(int value) {
            return (value + (value >> 31)) ^ (value >> 31);  // magics
        }
        
        public static float RecklessAbsolute(float value) {
            return value > 0 ? value : -value;  // gets rid of 2 if checks woo so fast
        }
    }
}
