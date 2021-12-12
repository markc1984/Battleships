using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    public enum Orientation { Horizontal, Vertical };
    public enum ShipType { Battleship, Destroyer };

    public class Ship
    {
        public Ship()
        {

        }

        public Ship(int shiptype)
        {
            Random r = new Random();
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

        private int numofspaces;
        private List<Coordinates> coordinates = new List<Coordinates>();
        private int orientatation;
        private bool shipeliminated;
        private bool alreadyadded;
        private int typeofship = 0;

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

        public bool AlreadyAdded
        {
            get { return alreadyadded; }
            set { alreadyadded = value; }
        }

        public int TypeOfShip
        {
            get { return typeofship; }
            set { typeofship = value; }
        }

    }
}
