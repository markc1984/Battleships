using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    // the coordinates class stores X and Y values as integers directly relatable to the battleship game, as well as if the coordinate was hit, who called that particular shot (because each player can call the same shot once) and the ship those 
    // coordinates belong to
    public class Coordinates
    {
        private int x;
        private int y;
        private bool coordhit;

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public bool WasCoordHit
        {
            get { return coordhit; }
            set { coordhit = value; }
        }

        public Ship? Ship { get; set; }

    }


}

