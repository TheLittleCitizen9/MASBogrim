using System;
using System.Collections.Generic;

namespace MASBogrim
{
    public class Auction
    {
        public IProduct Product { get; private set; }
        public double StartPrice { get; private set; }
        public double MinPriceJump { get; private set; }
        public DateTime StartTime { get; private set; }
        public int Id { get; private set; }
        public double CurrentPrice { get; set; }

        public List<IAgent> Agents { get; set; }

        public List<IAgent> BiddingAgents { get; set; }

        public int HighestBidder { get; set; }
        public bool IsThereAWinner { get; set; }
        public Timers _timers;
        public int SecondsUntilClosingEntrance { get; set; }
        public int SecondsUntilClosingBids { get; set; }

        public Auction(IProduct product, double startPrice, 
            double minPriceJump, DateTime startTime, int id, int secondsUntilClosingEntrance, int secondsUntilClosingBids)
        {
            Product = product;
            StartPrice = startPrice;
            MinPriceJump = minPriceJump;
            StartTime = startTime;
            Id = id;
            CurrentPrice = StartPrice;
            Agents = new List<IAgent>();
            BiddingAgents = new List<IAgent>();
            IsThereAWinner = false;
            SecondsUntilClosingEntrance = secondsUntilClosingEntrance;
            SecondsUntilClosingBids = secondsUntilClosingBids;
        }
    }
}
