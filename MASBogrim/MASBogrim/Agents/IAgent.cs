using System;

namespace MASBogrim
{
    public interface IAgent
    {
        bool ShouldEnterAuction(IProduct product, MAS mas, Auction auction);
        Tuple<int, bool> ShouldBid(Auction auction);
        Tuple<int, double> CalculateNewPrice();
        void PrintWinner(int id, double price, int auctionId);
        void PrintPrices(double startPrice, double minJumpPrice, int id, int auctionId);
        void PrintProductInfo(IProduct product, int auctionId);
        int GetAgentId();
    }
}