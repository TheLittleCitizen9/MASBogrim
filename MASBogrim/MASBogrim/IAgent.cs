using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim
{
    public interface IAgent
    {
        void ShouldBid();
        void CalculateNewPrice();
    }
}
