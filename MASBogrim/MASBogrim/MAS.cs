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
        private List<Agent> _agents;
        private List<Agent> _agentsInAuction;
        private List<Auction> _auctions;
        private IProduct _product;
        private int _secondsUntilClosingEntrance;
        private int _secondsUntilClosingBids;
        private double _currentPrice;
        private Auction _auction;
        private int _highestBidderId;
        public bool IsThereAWinner = false;
        private object _locker = new object();
        private Timers _timers;

        public MAS(Auction auction, List<Agent> agents, List<Auction> auctions, 
            IProduct product, int secondsUntilClosingEntrance, int secondsUntilClosingBids)
        {
            //_auction = auction;
            //_agents = agents;
            //_agentsInAuction = new List<Agent>();
            _auctions = auctions;
            _product = product;
            _secondsUntilClosingEntrance = secondsUntilClosingEntrance;
            _secondsUntilClosingBids = secondsUntilClosingBids;
            //_currentPrice = _auction.StartPrice;
            //_highestBidderId = 0;
        }

        public void Manager()
        {
            Console.WriteLine("! I Am The MAS !");
            foreach (var auction in _auctions)
            {
                Task task = new Task(() => Main(auction));
                task.Start();
            }
        }

        public void Main(Auction auction)
        {
            _timers = new Timers(_secondsUntilClosingEntrance, _secondsUntilClosingBids, this, auction);
            Console.WriteLine($"Auction {auction.Id} is starting !!");
            int count = 0;
            while (!IsThereAWinner)
            {
                if (auction.Agents.Count == 0)
                {
                    Console.WriteLine($"Auction {auction.Id} closed because no agent wante d to join");
                }
                else if (DateTime.Now >= auction.StartTime && count < 1)
                {
                    _timers.StartAuctionTimer.Start();
                    count++;
                    SendProductInfo(auction);
                }
            }
            SendWinner(auction);
        }

        public void SendProductInfo(Auction auction)
        {
            foreach (var agent in auction.Agents)
            {
                int agentId = agent.AgentId;
                Task<bool> task = new Task<bool>(() => agent.ShouldEnterAuction(_product, this));
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
                Task task = new Task(() => agent.PrintPrices(auction.CurrentPrice, auction.MinPriceJump, _highestBidderId));
                task.Start();
            }
        }
        public void SendPrices(Auction auction)
        {
            lock(_locker)
            {
                _timers.StartBidsTimer.Start();
            }
            
            PrintPrices(auction);
            
            foreach (var agent in auction.BiddingAgents)
            {
                int agentId = agent.AgentId;
                Task<bool> task = new Task<bool>(() => agent.ShouldBid());
                task.Start();
                if (task.Result)
                {
                    UpdatePrice(agent.CalculateNewPrice(), agentId, auction);
                }
            }
        }
        public void UpdatePrice(double newPrice, int id, Auction auction)
        {
            lock(_locker)
            {
                _timers.StartBidsTimer.Stop();
                _timers.TerminateAuction.Stop();
            }
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
            _timers.TerminateAuction.Start();
            Console.WriteLine("going once, going twice...");
            foreach (var agent in auction.BiddingAgents)
            {
                int agentId = agent.AgentId;
                Task<bool> task = new Task<bool>(() => agent.ShouldBid());
                task.Start();
                if (task.Result)
                {
                    UpdatePrice(agent.CalculateNewPrice(), agentId, auction);
                }
            }
        }
        public void SendWinner(Auction auction)
        {
            Console.WriteLine($"MAS -- Auction {auction.Id} finished");
            if (auction.BiddingAgents.Count == 0)
            {
                Console.WriteLine($"MAS -- There were no agents in auction {auction.Id} - so no winner");
            }
            else
            {
                foreach (var agent in auction.BiddingAgents)
                {
                    int agentId = agent.AgentId;
                    Task task = new Task(() => agent.PrintWinner(auction.HighestBidder, auction.CurrentPrice));
                    task.Start();
                }
            }
            
        }
        public void RemoveAgentFromAuction(int id, Auction auction)
        {
            Agent agentToRemove = auction.Agents.Where(a => a.AgentId == id).ToList()[0];
            auction.Agents.Remove(agentToRemove);
        }
        public void AddAgentToAuction(int id, Auction auction)
        {
            auction.BiddingAgents.Add(auction.Agents.Find(a => a.AgentId == id));
            Console.WriteLine($"MAS -- Agent {id} entered auction {auction.Id}");
        }
    }
}
