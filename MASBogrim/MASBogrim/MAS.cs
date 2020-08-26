﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
        public event Action<IProduct, MAS> GetProductInfo;
        public event Action GetBids;
        public event Action<int, double> GetWinner;
        public event Action<double, double> PrintPrices;

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
                GetProductInfo += agent.ShouldEnterAuction;
            }
            DateTime timeToEnd = DateTime.Now.AddSeconds(_secondsUntilClosingEntrance);
            while(DateTime.Now < timeToEnd)
            {
                if (DateTime.Now >= _auction.StartTime)
                {
                    SendProductInfo();
                }
                //Thread.Sleep(_secondsUntilClosingEntrance);
                if (_agents.Count == 0)
                {
                    Console.WriteLine("Auction closed because no agent wanted to join");
                }
            }
            FinishAuction();
            SendWinner();
            
        }
        public void SendProductInfo()
        {
            GetProductInfo?.Invoke(_product, this);
        }
        public void SendPrices()
        {
            PrintPrices?.Invoke(_currentPrice, _auction.MinPriceJump);
            GetBids?.Invoke();
        }
        public void UpdtePrice(double newPrice, int id)
        {
            if(newPrice - _auction.MinPriceJump >= _currentPrice)
            {
                _currentPrice = newPrice;
                _highestBidderId = id;
            }
            SendPrices();
        }
        public void FinishAuction()
        {
            GetBids?.Invoke();
        }
        public void SendWinner()
        {
            GetWinner?.Invoke(_highestBidderId, _currentPrice);
        }
        public void CloseRegistration()
        {

        }
        public void RemoveAgentFromAuction(int id)
        {
            foreach (var agent in _agents)
            {
                if(agent.AgentId == id)
                {
                    _agents.Remove(agent);
                }
            }
        }
        public void AddAgentToAuction(int id)
        {
            foreach (var agent in _agents)
            {
                Console.WriteLine($"MAS -- Agent {id} entered auction");
                if (agent.AgentId == id)
                {
                    GetBids += agent.ShouldBid;
                    PrintPrices += agent.PrintPrices;
                    GetWinner += agent.PrintWinner;
                }
            }
            SendPrices();
        }
    }
}
