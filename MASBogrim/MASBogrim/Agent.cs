using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim
{
    public class Agent : IAgent
    {
        public int AgentId { get; private set; }
        private bool _bidNewAmount = false;
        private double _lastPriceBidded = 0;
        private double _minJumpAmount = 0;
        public List<Agent> AuctionParticipants;

        public Agent(int id)
        {
            AgentId = id;
            AuctionParticipants = new List<Agent>();
        }

        public double CalculateNewPrice()
        {
            Random rnd = new Random();
            int amountToBid = rnd.Next((int)(_lastPriceBidded + _minJumpAmount), (int)(_lastPriceBidded + 2* _minJumpAmount));
            return amountToBid;
        }

        public bool ShouldBid()
        {
            Random rnd = new Random();
            int shouldBid = rnd.Next(1,6);
            if(shouldBid == 1)
            {
                _bidNewAmount = true;
            }
            else
            {
                _bidNewAmount = false;
            }
            return _bidNewAmount;
        }

        public bool ShouldEnterAuction(IProduct product, MAS mas, Auction auction)
        {
            Random rnd = new Random();
            int shouldEnter = rnd.Next(2);
            if(shouldEnter == 1)
            {
                PrintProductInfo(product, auction.Id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void PrintWinner(int id, double price, int auctionId)
        {
            if(id == AgentId)
            {
                Console.WriteLine($"Agent: {AgentId} -- Auction {auctionId} - I won !!! Price: {price}$");
            }
            else if(id ==0)
            {
                Console.WriteLine($"Agent: {AgentId} -- Auction {auctionId} - No one bid so there is no winner");
            }
            else
            {
                Console.WriteLine($"Agent: {AgentId} -- Auction {auctionId} - Winner is: Agent {id}. He bidded {price}$");
            }
        }

        public void PrintPrices(double startPrice, double minJumpPrice, int id, int auctionId)
        {
            _minJumpAmount = minJumpPrice;
            _lastPriceBidded = startPrice;
            if(id != 0)
            {
                Console.WriteLine($"Agent: {AgentId} -- Auction {auctionId} -  Current price: {_lastPriceBidded}, Minimum jump price: {_minJumpAmount}, Bidder: agent {id}");
            }
            else
            {
                Console.WriteLine($"Agent: {AgentId} -- Auction {auctionId} -  Current price: {_lastPriceBidded}, Minimum jump price: {_minJumpAmount}");
            }
        }

        public void PrintProductInfo(IProduct product, int auctionId)
        {
            Console.WriteLine($"Agent: {AgentId} -- Auction {auctionId} - {product.GetProductInformation()}"); 
        }
    }
}
