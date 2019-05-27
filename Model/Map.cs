using System.Collections.Generic;

namespace Models
{
    public class Map
    {
        public List<Monster> Monsters { get; set; }
        public List<Object> Objects { get; set; }
        public List<Event> Events { get; set; }
        public Floor FloorCode { get; set; }
        public byte FloorNumber { get; set; }

        public Map()
        {
            Monsters = new List<Monster>();
            Objects = new List<Object>();
            Events = new List<Event>();
        }
    }
}