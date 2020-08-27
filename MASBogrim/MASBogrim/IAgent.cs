using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim
{
    public interface IAgent
    {
        Tuple<int, bool> ShouldBid();
        Tuple<int, double> CalculateNewPrice();
    }
}