using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim.Building
{
    public class Room
    {
        public RoomType RoomType { get; set; }
        public double Size { get; set; }

        public Room(RoomType roomType, double size)
        {
            RoomType = roomType;
            Size = size;
        }
    }
}
