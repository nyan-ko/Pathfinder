using System;

namespace Pathfinder.Projections
{
    [Flags]
    public enum CollisionType : byte
    {
        None, Horizontal, Vertical
    }
}