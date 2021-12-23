using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    public enum ShipOrientation { Horizontal, Vertical };
    public enum ShipType { Battleship, Destroyer };

    // the ship class contains information about each ship, including the type of ship, the coordinates that belong to the ship, as well as containing information such as if the ship was destroyed, its orientation on the grid, the number of spaces it occupies and
    // whetheror not that ship has already been added to the game
    public class Ship
    {
        #region class constructors
        public Ship()
        {

        }
        // randomly orientate the ship position when it is instantiated
        public Ship(int shiptype)
        {
            Random r = new();
            orientatation = r.Next(0, 2);

            if (shiptype == (int)ShipType.Battleship)
            {
                typeofship = (int)ShipType.Battleship;
                numofspaces = 5;
            }
            else if (shiptype == (int)ShipType.Destroyer)
            {
                typeofship = (int)ShipType.Destroyer;
                numofspaces = 4;
            }
        }
        #endregion

        #region private class variables
        private int numofspaces;
        private List<Coordinates> coordinates = new();
        private int orientatation;
        private bool shipeliminated;
        private bool alreadyadded;
        private int typeofship = 0;
        #endregion

        #region class accessors

        public int NumOfSpaces
        {
            get { return numofspaces; }
            set { numofspaces = value; }
        }

        public int ShipOrientation
        {
            get { return orientatation; }
            set { orientatation = value; }
        }

        public bool IsShipEliminated
        {
            get { return shipeliminated; }
            set { shipeliminated = value; }
        }

        public List<Coordinates> CoOrds
        {
            get { return coordinates; }
            set { coordinates = value; }
        }

        public bool AlreadyAddedToGame
        {
            get { return alreadyadded; }
            set { alreadyadded = value; }
        }

        public int TypeOfShip
        {
            get { return typeofship; }
            set { typeofship = value; }
        }
        #endregion

        // a method that checks to see if all coordinates belonging to ship have been hit
        public bool CheckIfShipIsEliminated(Ship? ship)
        {
            if (ship != null)
            {
                int hits = 0;

                for (int i = 0; i < CoOrds.Count; i++)
                {
                    if (CoOrds[i].WasCoordHit)
                    {
                        hits++;
                    }
                }
                if (hits == NumOfSpaces)
                {
                    ship.IsShipEliminated = true;
                    return true;
                }
            }
            return false;
        }
    }


}
