using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim
{
    public class Auction
    {
        public string ProductName { get; private set; }
        public string ProductInfo { get; private set; }
        public double StartPrice { get; private set; }
        public double MinPriceJump { get; private set; }
        public DateTime StartTime { get; private set; }
        public int Id { get; private set; }
        public double CurrentPrice { get; set; }

        public List<Agent> Agents { get; set; }

        public List<Agent> BiddingAgents { get; set; }

        public int HighestBidder { get; set; }

        public Auction(string productName, string info, double startPrice,
            double minPriceJump, DateTime startTime, int id)
        {
            ProductName = productName;
            ProductInfo = info;
            StartPrice = startPrice;
            MinPriceJump = minPriceJump;
            StartTime = startTime;
            Id = id;
            CurrentPrice = StartPrice;
            Agents = new List<Agent>();
            BiddingAgents = new List<Agent>();
        }
    }
}
