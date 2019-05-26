using System.Collections.Generic;

namespace Models
{
    public class Map
    {
        public List<IEntity> Entities { get; set; }
        public List<Event> Events { get; set; }
        public Floor FloorCode { get; set; }
        public byte FloorNumber { get; set; }
    }
}