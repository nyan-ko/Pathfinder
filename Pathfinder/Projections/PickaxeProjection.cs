namespace Pathfinder.Projections
{
    public struct PickaxeProjection
    {
        public byte MiningSpeed;
        public byte PickaxePower;

        public PickaxeProjection(byte speed, byte power)
        {
            MiningSpeed = speed;
            PickaxePower = power;
        }
    }
}