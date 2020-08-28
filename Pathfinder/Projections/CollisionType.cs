using System;

namespace Pathfinder.Projections
{
    [Flags]
    public enum CollisionType : byte
    {
        None = 0,
        Horizontal = 2,
        Vertical = 4
    }
}