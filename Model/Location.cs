using System;

namespace Models
{
    public class Location
    {
        public Single PosX { get; set; }
        public Single PosY { get; set; }
        public Single PosZ { get; set; }
        public UInt32 RotX { get; set; }
        public UInt32 RotY { get; set; }
        public UInt32 RotZ { get; set; }

        public Location(Single positionX, Single positionY, Single positionZ,
            UInt32 rotationX, UInt32 rotationY, UInt32 rotationZ)
        {
            PosX = positionX;
            PosY = positionY;
            PosZ = positionZ;
            RotX = rotationX;
            RotY = rotationY;
            RotZ = rotationZ;

        }
    }
}