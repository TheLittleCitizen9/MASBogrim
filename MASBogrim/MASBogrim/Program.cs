using MASBogrim.Agents;
using MASBogrim.Buildings;
using MASBogrim.Products;
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
            Agent agent3 = new Agent(3);
            OnlyNecklessesAgent onlyNecklessesAgent = new OnlyNecklessesAgent(6);

            List<Room> rooms = new List<Room>() { new Room(RoomType.Bedroom, 20) };
            Building building1 = new Building("Hilton", true, true, true, true, 10, 4, 8, rooms);
            Building building2 = new Building("Cramim", true, true, true, true, 10, 4, 8, rooms);
            Building building3 = new Building("Bereshit", true, true, true, true, 10, 4, 8, rooms);

            Neckless neckless = new Neckless("Neckless", 24, JewleryMaterial.WhiteGold);

            Auction auction = new Auction(neckless, 40000, 1000, DateTime.Now.AddSeconds(10), 8, 4, 3);
            Auction auction2 = new Auction(building2, 10000, 1000, DateTime.Now.AddSeconds(10), 2, 4, 3);
            Auction auction3 = new Auction(building3, 10000, 1000, DateTime.Now.AddSeconds(10), 10, 4, 3);

            auction.Agents = new List<IAgent>() { agent1, agent2, onlyNecklessesAgent };
            auction2.Agents = new List<IAgent>() { agent1, agent2, onlyNecklessesAgent };
            auction3.Agents = new List<IAgent>() { agent1, agent2, agent3, onlyNecklessesAgent };

            MAS mas = new MAS(new List<Auction>() { auction, auction2, auction3});

            mas.Manager();
        }
    }
}
