using MASBogrim.Buildings;
using System;
using System.Collections.Generic;
using System.Text;

namespace MASBogrim.Buildings
{
    public class Building : IProduct
    {
        private string _name;
        private bool _hasAirConditioning;
        private bool _hasProtectionRoom;
        private bool _hasHighwayPath;
        private bool _hasDisabledPath;
        private int _numOfBathrooms;
        private int _numOfDiningRooms;
        private int _numOfFloors;
        private List<Room> _rooms;

        public Building(string name, bool hasAirConditioning, bool hasProtectionRoom, bool hasHighwayPath, 
            bool hasDisabledPath, int numOfBathrooms, int numOfDiningRooms, int numOfFloors, List<Room> rooms)
        {
            _name = name;
            _hasAirConditioning = hasAirConditioning;
            _hasProtectionRoom = hasProtectionRoom;
            _hasHighwayPath = hasHighwayPath;
            _hasDisabledPath = hasDisabledPath;
            _numOfBathrooms = numOfBathrooms;
            _numOfDiningRooms = numOfDiningRooms;
            _numOfFloors = numOfFloors;
            _rooms = rooms;
        }

        public string GetProductInformation()
        {
            return $"Buildig name: {_name}, Number of rooms: {_rooms.Count}";
        }

        public string GetName()
        {
            return _name;
        }
    }
}
