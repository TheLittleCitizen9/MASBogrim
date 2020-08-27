using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MASBogrim
{
    public class MASActions
    {
        public event Func<Tuple<int, bool>> EnterBidding;
        public event Func<Tuple<int, double>> GetNewPrice;
        
        public void SendProductInfo(Auction auction, MAS mas)
        {
            foreach (var agent in auction.Agents)
            {
                int agentId = agent.AgentId;
                var biddingAgent = agent;
                Task<bool> task = new Task<bool>(() => biddingAgent.ShouldEnterAuction(auction.Product, mas, auction));
                task.Start();
                if (task.Result)
                {
                    AddAgentToAuction(agentId, auction);
                }
            }
        }
        public void PrintPrices(Auction auction)
        {
            foreach (var agent in auction.BiddingAgents)
            {
                int agentId = agent.AgentId;
                var biddingAgent = agent;
                Task task = new Task(() => biddingAgent.PrintPrices(auction.CurrentPrice, auction.MinPriceJump, auction.HighestBidder, auction.Id));
                task.Start();
            }
        }
        public void SendPrices(Auction auction)
        {
            auction._timers.StartBidsTimer.Start();
            PrintPrices(auction);
            AgentsThatWantToBid(auction);
            InvokeGetPrices(auction);
        }
        public void UpdatePrice(List<Tuple<int, double>> results, Auction auction)
        {
            auction._timers.StartBidsTimer.Stop();
            auction._timers.TerminateAuction.Stop();
            foreach (var result in results)
            {
                if (result.Item2 > auction.CurrentPrice)
                {
                    auction.CurrentPrice = result.Item2;
                    auction.HighestBidder = result.Item1;
                }
            }
            SendPrices(auction);
        }
        public void FinishAuction(Auction auction)
        {
            auction._timers.TerminateAuction.Start();
            Console.WriteLine("going once, going twice...");
            AgentsThatWantToBid(auction);
            InvokeGetPrices(auction);
        }
        public void SendWinner(Auction auction)
        {
            auction._timers.TerminateAuction.Stop();
            auction._timers.StartBidsTimer.Stop();
            auction._timers.StartAuctionTimer.Stop();
            Console.WriteLine($"MAS -- Auction {auction.Id} finished");
            if (auction.BiddingAgents.Count == 0)
            {
                Console.WriteLine($"MAS -- There were no agents in auction {auction.Id} - so no winner");
            }
            else
            {
                foreach (var agent in auction.BiddingAgents)
                {
                    var biddingAgent = agent;
                    Task task = new Task(() => biddingAgent.PrintWinner(auction.HighestBidder, auction.CurrentPrice, auction.Id));
                    task.Start();
                }
            }
        }
        public void AddAgentToAuction(int id, Auction auction)
        {
            var agent = auction.Agents.Find(a => a.AgentId == id);
            auction.BiddingAgents.Add(agent);
            EnterBidding += agent.ShouldBid;
            Console.WriteLine($"MAS -- Agent {id} entered auction {auction.Id}");
        }
        private void InvokeGetPrices(Auction auction)
        {
            List<Task> tasks = new List<Task>();
            List<Tuple<int, double>> allResults = new List<Tuple<int, double>>();
            var delegates = GetNewPrice?.GetInvocationList();
            if (delegates != null)
            {
                foreach (var item in delegates)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        var outputMsg = item;
                        var result = (Tuple<int, double>)outputMsg?.DynamicInvoke();
                        allResults.Add(result);
                    }));
                }
                Task.WaitAll(tasks.ToArray());
                CleanGetPrices(auction);
                UpdatePrice(allResults, auction);
            }
        }
        private void AgentsThatWantToBid(Auction auction)
        {
            List<Task> tasks = new List<Task>();
            var delegates = EnterBidding?.GetInvocationList();
            foreach (var item in delegates)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var outputMsg = item;
                    var result = (Tuple<int, bool>)outputMsg?.DynamicInvoke();
                    if (result.Item2)
                    {
                        if (auction.BiddingAgents.Count > 0)
                        {
                            var agent = auction.BiddingAgents.Find(a => a.AgentId == result.Item1);
                            if (agent != null)
                            {
                                GetNewPrice += agent.CalculateNewPrice;
                            }
                        }
                    }
                }));
                Task.WaitAll(tasks.ToArray());
            }
        }
        private void CleanGetPrices(Auction auction)
        {
            foreach (var agent in auction.BiddingAgents)
            {
                GetNewPrice -= agent.CalculateNewPrice;
            }
        }
    }
}
