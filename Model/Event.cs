using System;
using System.Collections.Generic;

namespace Models
{
    public class Event
    {
        public UInt32 EventNumber { get; set; }
        public UInt16 Delay { get; set; }
        public UInt16 SectionId { get; set; }
        public UInt16 WaveId { get; set; }
        public List<IEventCommand> OnCompletion { get; set; } 

        public Event()
        {
            OnCompletion = new List<IEventCommand>();
        }
    }

    public abstract class IEventCommand
    {
        public EventCommandCode Code;
        public List<object> param;
    }

    public class UnlockDoorEvent : IEventCommand
    {
        public UnlockDoorEvent(UInt16 id)
        {
            Code = EventCommandCode.UnlockDoor;
            param = new List<object>() { id };
        }
    }
    public class LockDoorEvent : IEventCommand
    {
        public LockDoorEvent(UInt16 id)
        {
            Code = EventCommandCode.LockDoor;
            param = new List<object>() { id };
        }
    }
    public class CallEvent : IEventCommand
    {
        public CallEvent(UInt32 id)
        {
            Code = EventCommandCode.CallEvent;
            param = new List<object>() { id };
        }
    }
    public class UnhideEvent : IEventCommand
    {
        public UnhideEvent(UInt16 sectionId, UInt16 appear_flag)
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