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
            throw new NotImplementedException();
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

        }

        public void AddMASToEvents(MAS mas)
        {

        }

        public void SendNewPrice()
        {

        }

        public void PrintPrices(double startPrice, double minJumpPrice)
        {

        }

        public void PrintProductInfo(IProduct product)
        {

        }
    }
}
