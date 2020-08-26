using MASBogrim.Buildings;
using System;
using System.Collections.Generic;

namespace MASBogrim
{
    class Program
    {
        static void Main(string[] args)
        {
            Agent agent1 = new Agent(1);
            Agent agent2 = new Agent(2);

            List<Room> rooms = new List<Room>() { new Room(RoomType.Bedroom, 20) };
            Building building = new Building("dar", true, true, true, true, 10, 4, 8, rooms);

            Auction auction = new Auction("dar", "a great house", 10000, 1000, DateTime.Now.AddSeconds(10), 8);
            Auction auction2 = new Auction("dror", "a good house", 10000, 1000, DateTime.Now.AddSeconds(10), 6);

            auction.Agents = new List<Agent>() { agent1, agent2 };
            auction2.Agents = new List<Agent>() { agent1, agent2 };

            MAS mas = new MAS(auction, new List<Agent>() { agent1, agent2 }, new List<Auction>() { auction, auction2}, building, 4, 3);

            //mas.Main(auction);
            mas.Manager();
        }
    }
}
