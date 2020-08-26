using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim
{
    public interface IAgent
    {
        bool ShouldBid();
        double CalculateNewPrice();
    }
}
