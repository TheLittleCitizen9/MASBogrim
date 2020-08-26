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
            _auction = auction;
            _agents = agents;
            _agentsInAuction = new List<Agent>();
            _auctions = auctions;
            _product = product;
            _secondsUntilClosingEntrance = secondsUntilClosingEntrance;
            _secondsUntilClosingBids = secondsUntilClosingBids;
            _currentPrice = _auction.StartPrice;
            _highestBidderId = 0;
            _timers = new Timers(secondsUntilClosingEntrance, secondsUntilClosingBids, this);
        }

        public void Main()
        {
            Console.WriteLine("Auction is starting !!");
            int count = 0;
            while (!IsThereAWinner)
            {
                if (_agents.Count == 0)
                {
                    Console.WriteLine("Auction closed because no agent wante d to join");
                }
                else if (DateTime.Now >= _auction.StartTime && count < 1)
                {
                    _timers.StartAuctionTimer.Start();
                    count++;
                    //Thread.Sleep(2000);
                    SendProductInfo();
                }
            }
            SendWinner();
        }

        public void SendProductInfo()
        {
            //_timers.StartAuctionTimer.Start();
            foreach (var agent in _agents)
            {
                int agentId = agent.AgentId;
                Task<bool> task = new Task<bool>(() => agent.ShouldEnterAuction(_product, this));
                task.Start();
                if(task.Result)
                {
                    AddAgentToAuction(agentId);
                }
            }
        }

        public void PrintPrices()
        {
            foreach (var agent in _agentsInAuction)
            {
                int agentId = agent.AgentId;
                Task task = new Task(() => agent.PrintPrices(_currentPrice, _auction.MinPriceJump, _highestBidderId));
                task.Start();
            }
        }
        public void SendPrices()
        {
            lock(_locker)
            {
                _timers.StartBidsTimer.Start();
            }
            
            PrintPrices();
            
            foreach (var agent in _agents)
            {
                int agentId = agent.AgentId;
                Task<bool> task = new Task<bool>(() => agent.ShouldBid());
                task.Start();
                if (task.Result)
                {
                    UpdatePrice(agent.CalculateNewPrice(), agentId);
                }
            }
        }
        public void UpdatePrice(double newPrice, int id)
        {
            lock(_locker)
            {
                _timers.StartBidsTimer.Stop();
                _timers.TerminateAuction.Stop();
            }
            if(newPrice - _auction.MinPriceJump > _currentPrice)
            {
                lock(_locker)
                {
                    _currentPrice = newPrice;
                    _highestBidderId = id;
                }
            }
            SendPrices();
        }
        public void FinishAuction()
        {
            _timers.TerminateAuction.Start();
            Console.WriteLine("going once, going twice...");
            foreach (var agent in _agentsInAuction)
            {
                int agentId = agent.AgentId;
                Task<bool> task = new Task<bool>(() => agent.ShouldBid());
                task.Start();
                if (task.Result)
                {
                    UpdatePrice(agent.CalculateNewPrice(), agentId);
                }
            }
        }
        public void SendWinner()
        {
            Console.WriteLine($"MAS -- Auction finished");
            if (_agentsInAuction.Count == 0)
            {
                Console.WriteLine($"MAS -- There were no agents in auction - so no winner");
            }
            else
            {
                foreach (var agent in _agentsInAuction)
                {
                    int agentId = agent.AgentId;
                    Task task = new Task(() => agent.PrintWinner(_highestBidderId, _currentPrice));
                    task.Start();
                }
            }
            
        }
        public void RemoveAgentFromAuction(int id)
        {
            Agent agentToRemove = _agents.Where(a => a.AgentId == id).ToList()[0];
            _agents.Remove(agentToRemove);
        }
        public void AddAgentToAuction(int id)
        {
            _agentsInAuction.Add(_agents.Find(a => a.AgentId == id));
            Console.WriteLine($"MAS -- Agent {id} entered auction");
        }
    }
}
