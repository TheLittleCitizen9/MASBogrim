using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim
{
    public class MAS
    {
        private List<Agent> _agents;
        private List<Auction> _auctions;
        private IProduct _product;
        private int _secondsUntilClosingEntrance;
        private int _secondsUntilClosingBids;
        public event Action<IProduct> GetProductInfo;
        public event Action GetBids;
        public event Action<int, double> GetWinner;

        public MAS(List<Agent> agents, List<Auction> auctions, 
            IProduct product, int secondsUntilClosingEntrance, int secondsUntilClosingBids)
        {
            _agents = agents;
            _auctions = auctions;
            _product = product;
            _secondsUntilClosingEntrance = secondsUntilClosingEntrance;
            _secondsUntilClosingBids = secondsUntilClosingBids;
        }

        public void StartAuction()
        {

        }
        public void SendProductInfo()
        {

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
