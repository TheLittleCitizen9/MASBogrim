using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim
{
    public class Agent : IAgent
    {
        public int AgentId { get; private set; }
        private bool _enterAuction = false;
        private bool _bidNewAmount = false;
        private Agent _highestBidder = null;
        private double _lastPriceBidded = 0;
        private double _minJumpAmount = 0;
        public List<Agent> AuctionParticipants;
        public event Action<double> GetNewPrice;
        public event Action<int> ExitAuction;
        public event Action<int> EnterAuction;

        public Agent(int id)
        {
            AgentId = id;
            AuctionParticipants = new List<Agent>();
        }

        public void CalculateNewPrice()
        {
            Random rnd = new Random();
            int amountToBid = rnd.Next((int)_lastPriceBidded, (int)(_lastPriceBidded + _minJumpAmount + 1));
            SendNewPrice(amountToBid);
        }

        public void ShouldBid()
        {
            Random rnd = new Random();
            int shouldBid = rnd.Next(2);
            if(shouldBid == 1)
            {
                _bidNewAmount = true;
                CalculateNewPrice();
            }
        }

        public void ShouldEnterAuction()
        {

        }

        public void PrintWinner(int id, double price)
        {
            if(id == AgentId)
            {
                Console.WriteLine($"Agent: {AgentId} -- I won !!!");
            }
            else
            {
                Console.WriteLine($"Agent: {AgentId} -- Winner is: Agent {id}. He bidded {price}$");
            }
        }

        public void AddMASToEvents(MAS mas)
        {

        }

        public void SendNewPrice(int amount)
        {
            GetNewPrice?.Invoke(amount);
        }

        public void PrintPrices(double startPrice, double minJumpPrice)
        {
            _minJumpAmount = minJumpPrice;
            _lastPriceBidded = startPrice;
            Console.WriteLine($"Agent: {AgentId} -- Current price: {_lastPriceBidded}, Minimum jump price: {_minJumpAmount}");
        }

        public void PrintProductInfo(IProduct product)
        {
            product.PrintProductInformation();
        }
    }
}
