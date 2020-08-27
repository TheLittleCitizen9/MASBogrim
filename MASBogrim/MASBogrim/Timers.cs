using System.Timers;

namespace MASBogrim
{
    public class Timers
    {
        public Timer StartAuctionTimer;
        public Timer StartBidsTimer;
        public Timer TerminateAuction;
        private MASActions _mas;
        private Auction _auction;

        public Timers(int secondsUntilClosingEntrance, int secondsUntilClosingBids, MASActions mas, Auction auction)
        {
            _mas = mas;
            _auction = auction;
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
            _mas.SendPrices(_auction);
        }

        private void OnTimedFinishBiddingEvent(object source, ElapsedEventArgs e)
        {

            _mas.FinishAuction(_auction);
        }

        private void OnTimedTerminateAuctionEvent(object source, ElapsedEventArgs e)
        {
            _auction.IsThereAWinner = true;
        }
    }
}
