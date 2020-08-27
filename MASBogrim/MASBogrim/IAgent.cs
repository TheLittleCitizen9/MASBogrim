using System;

namespace MASBogrim
{
    public interface IAgent
    {
        Tuple<int, bool> ShouldBid();
        Tuple<int, double> CalculateNewPrice();
    }
}