using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    public class Coordinates
    {
        private int x;
        private int y;
        private bool coordhit;
        private Player? player;
        private Ship? ship;

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

        public bool wasCoordHit
        {
            get { return coordhit; }
            set { coordhit = value; }
        }

        public Player shotCalledBy
        {
            get { return player; }
            set { player = value; }
        }

        public Ship Ship
        {
            get { return ship; }
            set { ship = value; }
        }
    }
}

