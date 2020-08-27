using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MASBogrim
{
    public class MAS
    {
        private List<Auction> _auctions;
        private IProduct _product;
        private int _secondsUntilClosingEntrance;
        private int _secondsUntilClosingBids;
        private object _locker = new object();
        public event Func<Tuple<int, bool>> EnterBidding;
        public event Func<Tuple<int, double>> GetNewPrice;
        //public event Func<int> FinishBidding;

        public MAS(List<Auction> auctions, IProduct product, int secondsUntilClosingEntrance, int secondsUntilClosingBids)
        {
            _auctions = auctions;
            _product = product;
            _secondsUntilClosingEntrance = secondsUntilClosingEntrance;
            _secondsUntilClosingBids = secondsUntilClosingBids;
        }
        public void Manager()
        {
            Console.WriteLine("! I Am The MAS !");
            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.IsBackground = false;
                foreach (var auction in _auctions)
                {
                    Task task = new Task(() => Main(auction));
                    task.Start();
                }
            });
            Console.ReadLine();
        }
        public void Main(Auction auction)
        {
            auction._timers = new Timers(_secondsUntilClosingEntrance, _secondsUntilClosingBids, this, auction);
            Console.WriteLine($"Auction {auction.Id} is starting !!");
            int count = 0;
            while (!auction.IsThereAWinner)
            {
                if (auction.Agents.Count == 0)
                {
                    Console.WriteLine($"Auction {auction.Id} closed because no agent wante d to join");
                    auction.IsThereAWinner = true;
                }
                else if (DateTime.Now >= auction.StartTime && count < 1)
                {
                    auction._timers.StartAuctionTimer.Start();
                    count++;
                    SendProductInfo(auction);
                }
            }
            Thread.Sleep(2000);
            SendWinner(auction);
        }
        public void SendProductInfo(Auction auction)
        {
            foreach (var agent in auction.Agents)
            {
                int agentId = agent.AgentId;
                var biddingAgent = agent;
                Task<bool> task = new Task<bool>(() => biddingAgent.ShouldEnterAuction(_product, this, auction));
                task.Start();
                if(task.Result)
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

            List<Tuple<int, bool>> agentsThatWantToBid = AgentsThatWantToBid(auction);
            InvokeGetPrices(auction);

            //foreach (var agent in auction.BiddingAgents)
            //{
            //    int agentId = agent.AgentId;
            //    var biddingAgent = agent;
            //    Task<bool> task = new Task<bool>(() => biddingAgent.ShouldBid());
            //    task.Start();
            //    if (task.Result)
            //    {
            //        UpdatePrice(biddingAgent.CalculateNewPrice(), agentId, auction);
            //    }
            //}
        }
        public void UpdatePrice(List<Tuple<int, double>> results, Auction auction)
        {
            auction._timers.StartBidsTimer.Stop();
            auction._timers.TerminateAuction.Stop();
            foreach (var result in results)
            {
                if(result.Item2 > auction.CurrentPrice)
                {
                    auction.CurrentPrice = result.Item2;
                    auction.HighestBidder = result.Item1;
                }
            }
            SendPrices(auction);
        }
        //public void UpdatePrice(double newPrice, int id, Auction auction)
        //{
        //    auction._timers.StartBidsTimer.Stop();
        //    auction._timers.TerminateAuction.Stop();
        //    if(newPrice - auction.MinPriceJump > auction.CurrentPrice)
        //    {
        //        lock(_locker)
        //        {
        //            auction.CurrentPrice = newPrice;
        //            auction.HighestBidder = id;
        //        }
        //    }
        //    SendPrices(auction);
        //}
        private void InvokeGetPrices(Auction auction)
        {
            List<Task> tasks = new List<Task>();
            List<Tuple<int, double>> allResults = new List<Tuple<int, double>>();
            var delegates = GetNewPrice?.GetInvocationList();
            if(delegates != null)
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

        private List<Tuple<int, bool>> AgentsThatWantToBid(Auction auction)
        {
            List<Task> tasks = new List<Task>();
            List<Tuple<int, bool>> allResults = new List<Tuple<int, bool>>();
            var delegates = EnterBidding?.GetInvocationList();
            foreach (var item in delegates)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var outputMsg = item;
                    var result = (Tuple<int, bool>)outputMsg?.DynamicInvoke();
                    if (result.Item2)
                    {
                        if(auction.BiddingAgents.Count > 0)
                        {
                            var agent = auction.BiddingAgents.Find(a => a.AgentId == result.Item1);
                            if(agent != null)
                            {
                                GetNewPrice += agent.CalculateNewPrice;
                            }
                            
                        }
                        
                    }
                }));
                Task.WaitAll(tasks.ToArray());
            }
            return allResults;
        }

        private void CleanGetPrices(Auction auction)
        {
            var delegates = GetNewPrice?.GetInvocationList();
            foreach (var agent in auction.BiddingAgents)
            {
                GetNewPrice -= agent.CalculateNewPrice;
            }
        }

        private void CleanEnterBidding(Auction auction)
        {
            var delegates = EnterBidding.GetInvocationList();
            foreach (var agent in auction.BiddingAgents)
            {
                GetNewPrice -= agent.CalculateNewPrice;
            }
        }
        public void FinishAuction(Auction auction)
        {
            auction._timers.TerminateAuction.Start();
            
            Console.WriteLine("going once, going twice...");
            List<Tuple<int, bool>> allResults = AgentsThatWantToBid(auction);
            InvokeGetPrices(auction);

            //foreach (var agent in auction.BiddingAgents)
            //{
            //    int agentId = agent.AgentId;
            //    var biddingAgent = agent;
            //    Task<bool> task = new Task<bool>(() => biddingAgent.ShouldBid());
            //    task.Start();
            //    if (task.Result)
            //    {
            //        UpdatePrice(biddingAgent.CalculateNewPrice(), agentId, auction);
            //    }
            //}
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
            //GetNewPrice += agent.CalculateNewPrice;
            Console.WriteLine($"MAS -- Agent {id} entered auction {auction.Id}");
        }
    }
}
