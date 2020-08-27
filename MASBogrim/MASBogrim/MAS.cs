using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MASBogrim
{
    public class MAS
    {
        private List<Auction> _auctions;
        private IProduct _product;
        private int _secondsUntilClosingEntrance;
        private int _secondsUntilClosingBids;
        private object _locker = new object();

        public MAS(List<Auction> auctions, IProduct product, int secondsUntilClosingEntrance, int secondsUntilClosingBids)
        {
            _auctions = auctions;
            _product = product;
            _secondsUntilClosingEntrance = secondsUntilClosingEntrance;
            _secondsUntilClosingBids = secondsUntilClosingBids;
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
            auction._timers = new Timers(_secondsUntilClosingEntrance, _secondsUntilClosingBids, this, auction);
            Console.WriteLine($"Auction {auction.Id} is starting !!");
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
                    SendProductInfo(auction);
                }
            }
            Thread.Sleep(2000);
            SendWinner(auction);
        }
        public void SendProductInfo(Auction auction)
        {
            foreach (var agent in auction.Agents)
            {
                int agentId = agent.AgentId;
                var biddingAgent = agent;
                Task<bool> task = new Task<bool>(() => biddingAgent.ShouldEnterAuction(_product, this, auction));
                task.Start();
                if(task.Result)
                {
                    AddAgentToAuction(agentId, auction);
                }
            }
        }
        public void PrintPrices(Auction auction)
        {
            foreach (var agent in auction.BiddingAgents)
            {
                int agentId = agent.AgentId;
                var biddingAgent = agent;
                Task task = new Task(() => biddingAgent.PrintPrices(auction.CurrentPrice, auction.MinPriceJump, auction.HighestBidder, auction.Id));
                task.Start();
            }
        }
        public void SendPrices(Auction auction)
        {
            auction._timers.StartBidsTimer.Start();
            
            PrintPrices(auction);
            
            foreach (var agent in auction.BiddingAgents)
            {
                int agentId = agent.AgentId;
                var biddingAgent = agent;
                Task<bool> task = new Task<bool>(() => biddingAgent.ShouldBid());
                task.Start();
                if (task.Result)
                {
                    UpdatePrice(biddingAgent.CalculateNewPrice(), agentId, auction);
                }
            }
        }
        public void UpdatePrice(double newPrice, int id, Auction auction)
        {
            auction._timers.StartBidsTimer.Stop();
            auction._timers.TerminateAuction.Stop();
            if(newPrice - auction.MinPriceJump > auction.CurrentPrice)
            {
                lock(_locker)
                {
                    auction.CurrentPrice = newPrice;
                    auction.HighestBidder = id;
                }
            }
            SendPrices(auction);
        }
        public void FinishAuction(Auction auction)
        {
            auction._timers.TerminateAuction.Start();
            
            Console.WriteLine("going once, going twice...");
            foreach (var agent in auction.BiddingAgents)
            {
                int agentId = agent.AgentId;
                var biddingAgent = agent;
                Task<bool> task = new Task<bool>(() => biddingAgent.ShouldBid());
                task.Start();
                if (task.Result)
                {
                    UpdatePrice(biddingAgent.CalculateNewPrice(), agentId, auction);
                }
            }
        }
        public void SendWinner(Auction auction)
        {
            auction._timers.TerminateAuction.Stop();
            auction._timers.StartBidsTimer.Stop();
            auction._timers.StartAuctionTimer.Stop();
            Console.WriteLine($"MAS -- Auction {auction.Id} finished");
            if (auction.BiddingAgents.Count == 0)
            {
                Console.WriteLine($"MAS -- There were no agents in auction {auction.Id} - so no winner");
            }
            else
            {
                foreach (var agent in auction.BiddingAgents)
                {
                    var biddingAgent = agent;
                    Task task = new Task(() => biddingAgent.PrintWinner(auction.HighestBidder, auction.CurrentPrice, auction.Id));
                    task.Start();
                }
            }
            
        }
        public void AddAgentToAuction(int id, Auction auction)
        {
            auction.BiddingAgents.Add(auction.Agents.Find(a => a.AgentId == id));
            Console.WriteLine($"MAS -- Agent {id} entered auction {auction.Id}");
        }
    }
}
