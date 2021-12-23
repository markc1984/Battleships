using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    public enum CurrentPlayer { Player, Computer }
    public class Player
    {
        #region Class constructors
        public Player()
        {
       
        }
        #endregion

        #region Class variables
        private List<Ship> shipinventory = new();
        private List<Coordinates> attemptedcoordinates = new();
        private bool shipseliminated;

        #endregion

        #region Accessor variables for entrants
        public string? EntrantName { get; set; } = "";

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

        #region Class methods
        // CheckForDuplicateTries checks existing entered coordinates against currently entered coordinates and returns true if there is a match
        public bool CheckForDuplicateTries(Coordinates coords)
        {
            for (int i = 0; i < AttemptedCoordinates.Count; i++)
            {
                if (coords.X == AttemptedCoordinates[i].X && coords.Y == AttemptedCoordinates[i].Y)
                {
                    return true;
                }
            }
            return false;
        }

        // CheckIfPlayerIsEliminated checks if all ships have been eliminated and returns true if it is found stated player loses all ships
        public bool CheckIfPlayerIsEliminated(Player player)
        {
            // initialise a counter
            int shipseliminated = 0;

            // for each ship in the player inventory
            for (int i = 0; i < player.ShipInventory.Count; i++)
            {
                // if that ship has been eliminated, increase ship eliminated counter
                if (player.ShipInventory[i].IsShipEliminated)
                {
                    shipseliminated++;
                }
            }
            // if the counter equals the total number of ships in the inventory, this signifies the player has lost the game

            if (shipseliminated == player.ShipInventory.Count)
            {
                // flag the player as eliminated, this should break out of the while loop in the main method
                player.AllShipsEliminated = true;
                return true;
            }
            return false;
        }

        // RegisterHitCoordinates records a valid, unique shot for a player
        public Coordinates? RegisterHitCoordinates(Coordinates coords, Player player)
        {
            attemptedcoordinates.Add(coords);

            for (int i = 0; i < player.ShipInventory.Count; i++)
            {
                if (player.ShipInventory[i].IsShipEliminated)
                {
                    continue;
                }
                for (int j = 0; j < player.ShipInventory[i].CoOrds.Count; j++)
                {
                    if (coords.X == player.ShipInventory[i].CoOrds[j].X && coords.Y == player.ShipInventory[i].CoOrds[j].Y)
                    {
                        player.ShipInventory[i].CoOrds[j].WasCoordHit = true;
                        // update the last added element in the attempted coordinates list
                        attemptedcoordinates[attemptedcoordinates.Count - 1] = player.ShipInventory[i].CoOrds[j];
                        return player.ShipInventory[i].CoOrds[j];
                    }
                }
            }
            return null;
        }


        #endregion
    }
}