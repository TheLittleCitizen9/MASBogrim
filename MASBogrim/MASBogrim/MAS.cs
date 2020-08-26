﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private object _locker = new object();
        //public event Action<IProduct, MAS> GetProductInfo;
        //public event Action GetBids;
        //public event Action<int, double> GetWinner;
        public event Action<double, double, int> GetPrices;

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
        }

        public void StartAuction()
        {
            Console.WriteLine("Auction is starting !!");
            foreach (var agent in _agents)
            {
                //GetProductInfo += agent.ShouldEnterAuction;
            }
            Main();
            //foreach (var agent in _agents)
            //{
            //    Thread thread = new Thread(() => X());
            //    thread.Start();
            //}
            
        }

        public void Main()
        {
            DateTime timeToEnd = DateTime.Now.AddSeconds(_secondsUntilClosingEntrance);
            while (DateTime.Now <= timeToEnd)
            {
                if (DateTime.Now == _auction.StartTime)
                {
                    SendProductInfo();
                }
                if (_agents.Count == 0)
                {
                    Console.WriteLine("Auction closed because no agent wanted to join");
                }
            }
            FinishAuction();
            Thread.Sleep(TimeSpan.FromSeconds(_secondsUntilClosingBids));
            SendWinner();
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

            //var delegates = GetProductInfo.GetInvocationList();
            //Parallel.ForEach(delegates, d => d.DynamicInvoke(_product, this));
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
            //var delegates = PrintPrices.GetInvocationList();
            //Parallel.ForEach(delegates, d => d.DynamicInvoke(_currentPrice, _auction.MinPriceJump, _highestBidderId));

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
        public void CloseRegistration()
        {

        }
        public void RemoveAgentFromAuction(int id)
        {
            Agent agentToRemove = _agents.Where(a => a.AgentId == id).ToList()[0];
            _agents.Remove(agentToRemove);
        }
        public void AddAgentToAuction(int id)
        {
            //Agent agent = _agents.Find(a => a.AgentId == id);
            //GetBids += agent.ShouldBid;
            //GetPrices += agent.PrintPrices;
            //GetWinner += agent.PrintWinner;
            Console.WriteLine($"MAS -- Agent {id} entered auction");
            SendPrices();
        }
    }
}
