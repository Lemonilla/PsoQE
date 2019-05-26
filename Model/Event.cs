using System;
using System.Collections.Generic;

namespace Models
{
    public class Event
    {
        public int EventNumber { get; set; }
        public int Delay { get; set; }
        public int SectionId { get; set; }
        public int WaveId { get; set; }
        public List<IEventCommand> OnCompletion { get; set; } 
    }

    public abstract class IEventCommand
    {
        public EventCommandCode Code;
        public List<object> param;
    }

    public class UnlockDoorEvent : IEventCommand
    {
        UnlockDoorEvent(UInt16 id)
        {
            Code = EventCommandCode.UnlockDoor;
            param = new List<object>() { id };
        }
    }
    public class LockDoorEvent : IEventCommand
    {
        LockDoorEvent(UInt16 id)
        {
            Code = EventCommandCode.LockDoor;
            param = new List<object>() { id };
        }
    }
    public class CallEvent : IEventCommand
    {
        CallEvent(UInt16 id)
        {
            Code = EventCommandCode.CallEvent;
            param = new List<object>() { id };
        }
    }
    public class UnhideEvent : IEventCommand
    {
        UnhideEvent(UInt16 sectionId, UInt16 appear_flag)
        {
            Code = EventCommandCode.CallEvent;
            param = new List<object>() { sectionId, appear_flag };
        }
    }
    
    public enum EventCommandCode
    {
        UnlockDoor = 0x0a,
        LockDoor = 0x0b,
        CallEvent = 0x0c,
        UnhideObjects = 0x08
    }
}