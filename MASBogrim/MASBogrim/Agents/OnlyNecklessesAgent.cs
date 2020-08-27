using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim.Agents
{
    public class OnlyNecklessesAgent : IAgent
    {
        private int _agentId;
        private bool _bidNewAmount = false;
        private double _currentPrice = 0;
        private double _minJumpAmount = 0;
        private double _lastPriceBidded = 0;
        public List<Agent> AuctionParticipants;

        public OnlyNecklessesAgent(int id)
        {
            _agentId = id;
            AuctionParticipants = new List<Agent>();
        }

        public Tuple<int, double> CalculateNewPrice()
        {
            double amountToBid = _currentPrice + _minJumpAmount;
            _lastPriceBidded = amountToBid;
            return new Tuple<int, double>(_agentId, amountToBid);
        }

        public void PrintPrices(double startPrice, double minJumpPrice, int id, int auctionId)
        {
            _minJumpAmount = minJumpPrice;
            _currentPrice = startPrice;
            if (id != 0)
            {
                Console.WriteLine($"Agent: {_agentId} -- Auction {auctionId} -  Current price: {_currentPrice}, Minimum jump price: {_minJumpAmount}, Bidder: agent {id}");
            }
            else
            {
                Console.WriteLine($"Agent: {_agentId} -- Auction {auctionId} -  Current price: {_currentPrice}, Minimum jump price: {_minJumpAmount}");
            }
        }

        public void PrintProductInfo(IProduct product, int auctionId)
        {
            Console.WriteLine($"Agent: {_agentId} -- Auction {auctionId} - {product.GetProductInformation()}");
        }

        public void PrintWinner(int id, double price, int auctionId)
        {
            if (id == _agentId)
            {
                Console.WriteLine($"Agent: {_agentId} -- Auction {auctionId} - I won !!! Price: {price}$");
            }
            else if (id == 0)
            {
                Console.WriteLine($"Agent: {_agentId} -- Auction {auctionId} - No one bid so there is no winner");
            }
            else
            {
                Console.WriteLine($"Agent: {_agentId} -- Auction {auctionId} - Winner is: Agent {id}. He bidded {price}$");
            }
        }

        public Tuple<int, bool> ShouldBid(Auction auction)
        {
            Random rnd = new Random();
            int shouldBid = rnd.Next(1, 6);
            if (shouldBid == 1)
            {
                if (_currentPrice == _lastPriceBidded)
                {
                    _bidNewAmount = false;
                }
                else
                {
                    _bidNewAmount = true;
                }
            }
            else
            {
                _bidNewAmount = false;
            }
            return new Tuple<int, bool>(_agentId, _bidNewAmount);
        }

        public bool ShouldEnterAuction(IProduct product, MAS mas, Auction auction)
        {
            if(product.GetName()!= "Neckless")
            {
                return false;
            }
            return true;
        }

        public int GetAgentId()
        {
            return _agentId;
        }
    }
}
