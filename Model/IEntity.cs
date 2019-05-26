using System;

namespace Models
{
    public abstract class IEntity
    {
        public UInt16 ObjectId { get; set; }
        public UInt16 FloorNumber { get; set; }
        public Location Position { get; set; }
        public UInt16 RoomNumber { get; set; }
    }

    public class Monster : IEntity
    {
        public UInt16 ChildCount { get; set; }
        public UInt16 ItemWave { get; set; }
        public UInt16 Wave1 { get; set; }
        public UInt32 Wave2 { get; set; }
        Single F1 { get; set; }
        Single F2 { get; set; }
        Single F3 { get; set; }
        Single F4 { get; set; }
        Single F5 { get; set; }
        Int32 F6 { get; set; }
        Int32 F7 { get; set; }
    }

    public class Object : IEntity
    {
        UInt16 GroupNumber { get; set; }
        Single F1 { get; set; }
        Single F2 { get; set; }
        Single F3 { get; set; }
        Int32 F4 { get; set; }
        Int32 F5 { get; set; }
        Int32 F6 { get; set; }
        Int32 F7 { get; set; }
    }
}