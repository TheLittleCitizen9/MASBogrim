using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim
{
    public class Auction
    {
        public string ProductName { get; set; }
        public string ProductInfo { get; set; }
        public double StartPrice { get; set; }
        public double minPriceJump { get; set; }
        public DateTime StartTime { get; set; }
        public int Id { get; set; }
    }
}
