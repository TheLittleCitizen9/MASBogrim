using System;
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
        private Auction _auction;
        public event Action<IProduct> GetProductInfo;
        public event Action GetBids;
        public event Action<int, double> GetWinner;

        public MAS(Auction auction, List<Agent> agents, List<Auction> auctions, 
            IProduct product, int secondsUntilClosingEntrance, int secondsUntilClosingBids)
        {
            _auction = auction;
            _agents = agents;
            _auctions = auctions;
            _product = product;
            _secondsUntilClosingEntrance = secondsUntilClosingEntrance;
            _secondsUntilClosingBids = secondsUntilClosingBids;
        }

        public void StartAuction()
        {
            if(DateTime.Now == _auction.StartTime)
            {
                SendProductInfo();
            }
            Thread.Sleep(_secondsUntilClosingEntrance);
            if(_agents.Count == 0)
            {
                Console.WriteLine("Auction closed because no agent wanted to join");
            }
        }
        public void SendProductInfo()
        {
            GetProductInfo?.Invoke(_product);
        }
        public void SendPrices()
        {

        }
        public void UpdtePrice(double newPrice)
        {

        }
        public void FinishAuction()
        {

        }
        public void SendWinner()
        {

        }
        public void CloseRegistration()
        {

        }
        public void RemoveAgentFromAuction(int id)
        {

        }
        public void AddAgentToAuction(int id)
        {

        }
    }
}
