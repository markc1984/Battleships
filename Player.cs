using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    // the player class stores all information about the player, the list of ships in the inventory and a list of attempted coordinates, irrespective of whether those coordinates belonged to a ship or not
    public class Player
    {
        #region Class constructors
        public Player()
        {
            // when a player class is instantiated, automatically populate new ships to the grid
            for (int i = 0; i < 2; i++)
            {
                shipinventory.Add(new Ship((int)ShipType.Destroyer));
            }
            shipinventory.Add(new Ship((int)ShipType.Battleship));
        }
        #endregion

        #region Class variables
        private string entrantName = "";
        private List<Ship> shipinventory = new List<Ship>();
        private List<Coordinates> attemptedcoordinates = new List<Coordinates>();
        private bool shipseliminated;

        #endregion

        #region Accessor variables for entrants
        public string EntrantName
        {
            get { return entrantName; }
            set { entrantName = value; }
        }

        public List<Ship> ShipInventory
        {
            get { return shipinventory; }
            set { shipinventory = value; }
        }

        public bool AllShipsEliminated
        {
            get { return shipseliminated; }
            set { shipseliminated = value; }
        }

        public List<Coordinates> AttemptedCoordinates
        {
            get { return attemptedcoordinates; }
            set { attemptedcoordinates = value; }
        }
        #endregion
    }
}