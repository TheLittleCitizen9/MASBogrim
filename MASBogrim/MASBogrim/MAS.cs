using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MASBogrim
{
    public class MAS
    {
        private List<Auction> _auctions;
        private MASActions _MASActions;

        public MAS(List<Auction> auctions)
        {
            _auctions = auctions;
            _MASActions = new MASActions();
        }
        public void Manager()
        {
            Console.WriteLine("! I Am The MAS !");
            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.IsBackground = false;
                foreach (var auction in _auctions)
                {
                    Task task = new Task(() => Main(auction));
                    task.Start();
                }
            });
            Console.ReadLine();
        }
        public void Main(Auction auction)
        {
            auction._timers = new Timers(auction.SecondsUntilClosingEntrance, auction.SecondsUntilClosingBids, _MASActions, auction);
            Console.WriteLine($"Auction {auction.Id} is starting at {auction.StartTime}");
            int count = 0;
            while (!auction.IsThereAWinner)
            {
                if (auction.Agents.Count == 0)
                {
                    Console.WriteLine($"Auction {auction.Id} closed because no agent wante d to join");
                    auction.IsThereAWinner = true;
                }
                else if (DateTime.Now >= auction.StartTime && count < 1)
                {
                    auction._timers.StartAuctionTimer.Start();
                    count++;
                    _MASActions.SendProductInfo(auction, this);
                }
            }
            Thread.Sleep(2000);
            _MASActions.SendWinner(auction);
        }
    }
}
