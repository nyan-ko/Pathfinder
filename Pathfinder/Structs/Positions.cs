using Microsoft.Xna.Framework;

namespace Pathfinder.Structs {
    public struct TilePosition {
        public int X;
        public int Y;

        public TilePosition(int x, int y) {
            X = x;
            Y = y;
        }

        public TilePosition(PixelPosition pixelPosition) {
            X = (int)pixelPosition.X / 16;
            Y = (int)pixelPosition.Y / 16;
        }

        public TilePosition(Vector2 tilePosition) {
            X = (int)tilePosition.X;
            Y = (int)tilePosition.Y;
        }
    }

    public struct PixelPosition {
        public float X;
        public float Y;
        
        public PixelPosition(float x, float y) {
            X = x;
            Y = y;
        }

        public PixelPosition(TilePosition tilePosition) {
            X = tilePosition.X * 16;
            Y = tilePosition.Y * 16;
        }

        public PixelPosition(Vector2 pixelPosition) {
            X = (int)pixelPosition.X;
            Y = (int)pixelPosition.Y;
        }

        public float Distance(PixelPosition compare) => Distance(compare.X, compare.Y);

        public float Distance(float x, float y) {
            var _x = X - x;
            var _y = Y - y;
            return _x * _x + _y * _y;
        }

        public PixelPosition ClampToClosestTile() {
            X -= X % 16;
            Y -= Y % 16;
            return this;
        }

        public static bool operator != (PixelPosition o, PixelPosition c) {
            return o.X != c.X || o.Y != c.Y;
        }

        public static bool operator == (PixelPosition o, PixelPosition c) {
            return o.X == c.X && o.Y == c.Y;
        }

        public static PixelPosition operator + (PixelPosition o, PixelPosition c) {
            o.X += c.X;
            o.Y += c.Y;
            return o;
        }

        public static readonly PixelPosition Zero = new PixelPosition(0, 0);
    }
}
