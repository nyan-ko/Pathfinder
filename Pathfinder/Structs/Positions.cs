using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Structs {
    public struct TilePosition {
        public int X;
        public int Y;

        public TilePosition(int x, int y) {
            X = x;
            Y = y;
        }

        public TilePosition(PixelPosition pixelPosition) {
            X = pixelPosition.X / 16;
            Y = pixelPosition.Y / 16;
        }
    }

    public struct PixelPosition {
        public int X;
        public int Y;
        
        public PixelPosition(int x, int y) {
            X = x;
            Y = y;
        }

        public PixelPosition(TilePosition tilePosition) {
            X = tilePosition.X * 16;
            Y = tilePosition.Y * 16;
        }

        public float Distance(PixelPosition compare) {
            var x = X - compare.X;
            var y = Y - compare.Y;
            return x * x + y * y;
        }
    }
}
