using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace MASBogrim
{
    public class Timers
    {
        public Timer StartAuctionTimer;
        public Timer StartBidsTimer;
        public Timer TerminateAuction;
        private MAS _mas;

        public Timers(int secondsUntilClosingEntrance, int secondsUntilClosingBids, MAS mas)
        {
            _mas = mas;
            StartAuctionTimer = new Timer();
            StartBidsTimer = new Timer();
            TerminateAuction = new Timer();
            StartAuctionTimer.Elapsed += new ElapsedEventHandler(OnTimedFinishEntranceEvent);
            StartAuctionTimer.Interval = secondsUntilClosingEntrance * 1000;
            StartAuctionTimer.Enabled = false;
            StartBidsTimer.Elapsed += new ElapsedEventHandler(OnTimedFinishBiddingEvent);
            StartBidsTimer.Interval = secondsUntilClosingBids * 1000;
            StartBidsTimer.Enabled = false;
            TerminateAuction.Elapsed += new ElapsedEventHandler(OnTimedTerminateAuctionEvent);
            TerminateAuction.Interval = secondsUntilClosingBids * 1000;
            TerminateAuction.Enabled = false;
        }

        private void OnTimedFinishEntranceEvent(object source, ElapsedEventArgs e)
        {
            _mas.SendPrices();
        }

        private void OnTimedFinishBiddingEvent(object source, ElapsedEventArgs e)
        {

            _mas.FinishAuction();
        }

        private void OnTimedTerminateAuctionEvent(object source, ElapsedEventArgs e)
        {
            _mas.IsThereAWinner = true;
        }
    }
}
