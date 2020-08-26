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
        private List<Auction> _auctions;
        private IProduct _product;
        private int _secondsUntilClosingEntrance;
        private int _secondsUntilClosingBids;
        private double _currentPrice;
        private Auction _auction;
        private int _highestBidderId;
        private bool _isThereAWinner = false;
        private object _locker = new object();
        private System.Timers.Timer _startAuctionTimer;
        private System.Timers.Timer _startBidsTimer;
        private System.Timers.Timer _terminateAuction;

        public MAS(Auction auction, List<Agent> agents, List<Auction> auctions, 
            IProduct product, int secondsUntilClosingEntrance, int secondsUntilClosingBids)
        {
            _auction = auction;
            _agents = agents;
            _auctions = auctions;
            _product = product;
            _secondsUntilClosingEntrance = secondsUntilClosingEntrance;
            _secondsUntilClosingBids = secondsUntilClosingBids;
            _currentPrice = _auction.StartPrice;
            _highestBidderId = 0;
            _startAuctionTimer = new System.Timers.Timer();
            _startBidsTimer = new System.Timers.Timer();
            _terminateAuction = new System.Timers.Timer();
            _startAuctionTimer.Elapsed += new ElapsedEventHandler(OnTimedFinishEntranceEvent);
            _startAuctionTimer.Interval = _secondsUntilClosingEntrance * 1000;
            _startAuctionTimer.Enabled = false;
            _startBidsTimer.Elapsed += new ElapsedEventHandler(OnTimedFinishBiddingEvent);
            _startBidsTimer.Interval = _secondsUntilClosingBids * 1000;
            _startBidsTimer.Enabled = false;
            _terminateAuction.Elapsed += new ElapsedEventHandler(OnTimedTerminateAuctionEvent);
            _terminateAuction.Interval = _secondsUntilClosingBids * 1000;
            _terminateAuction.Enabled = false;
        }

        public void StartAuction()
        {
            Console.WriteLine("Auction is starting !!");
            Main();
        }

        public void Main()
        {
            int count = 0;
            DateTime timeToEnd = DateTime.Now.AddSeconds(_secondsUntilClosingEntrance);
            while (!_isThereAWinner)
            {
                if (_agents.Count == 0)
                {
                    Console.WriteLine("Auction closed because no agent wante d to join");
                }
                else if (DateTime.Now >= _auction.StartTime && count < 1)
                {
                    _startAuctionTimer.Start();
                    count++;
                    Thread.Sleep(2000);
                    StartRun();
                    
                }

            }
            SendWinner();
        }

        public void StartRun()
        {
            SendProductInfo();
        }

        
        public void SendProductInfo()
        {
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
            foreach (var agent in _agents)
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
                _startBidsTimer.Start();
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
                _startBidsTimer.Stop();
                _terminateAuction.Stop();
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
            _terminateAuction.Start();
            Console.WriteLine("going once, going twice...");
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
        public void SendWinner()
        {
            Console.WriteLine($"MAS -- Auction finished");
            foreach (var agent in _agents)
            {
                int agentId = agent.AgentId;
                Task task = new Task(() => agent.PrintWinner(_highestBidderId, _currentPrice));
                task.Start();
            }
        }
        public void RemoveAgentFromAuction(int id)
        {
            Agent agentToRemove = _agents.Where(a => a.AgentId == id).ToList()[0];
            _agents.Remove(agentToRemove);
        }
        public void AddAgentToAuction(int id)
        {
            Console.WriteLine($"MAS -- Agent {id} entered auction");
        }

        private void OnTimedFinishEntranceEvent(object source, ElapsedEventArgs e)
        {
            SendPrices();
        }

        private void OnTimedFinishBiddingEvent(object source, ElapsedEventArgs e)
        {
            
            FinishAuction();
        }

        private void OnTimedTerminateAuctionEvent(object source, ElapsedEventArgs e)
        {
            _isThereAWinner = true;
        }
    }
}
