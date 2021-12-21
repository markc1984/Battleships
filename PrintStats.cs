using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    public static class PrintStats
    {
        public static void PrintGrid(Player currentplayer, Player opposition)
        {
            // only run if human currentplayer is current currentplayer to stop unncessary code execution

            char c = 'A';
            int rowa = 1;
            int rowb = 1;
            List<string> legend = new List<string> { "Legend", "* = Unhit Coordinate", "o = Unhit Ship Coordinate", "x = Computer miss", "x = Player miss", "X = Direct Hit" };

            Console.Clear();
            bool match = false;
            Console.WriteLine("------------");
            Console.WriteLine(currentplayer.EntrantName);
            Console.WriteLine("------------");

            // 2D loop iteration i and j equals axis coords
            for (int i = 0; i <= 10; i++)
            {
                for (int j = 0; j <= 30; j++)
                {
                    // if the position is the second grid, produce a tab keypress and reset character back to A
                    if (j == 11 || j == 21)
                    {
                        Console.Write("\t");
                        c = 'A';
                    }

                    // if we are at the origin of either grid, we don't want any character to show as its not a valid grid coordinate
                    if (j == 0 && i == 0 || j == 11 && i == 0)
                    {
                        Console.Write("  ");
                    }

                    // if we are at ONLY the beginning of the grid at X and within the bounds of Y, write letter coordinates out for each grid
                    if (i == 0 && j >= 0 && j <= 9 || i == 0 && j >= 11 && j <= 20)
                    {
                        Console.Write(c + "  ");
                        c++;
                    }

                    // only run this code if we are beyond row or column 0 (aka not at the origin or grid labels)
                    if (i != 0 || j != 0)
                    {
                        for (int k = 0; k < currentplayer.ShipInventory.Count; k++)
                        {
                            for (int l = 0; l < currentplayer.ShipInventory[k].CoOrds.Count; l++)
                            {
                                if (currentplayer.ShipInventory[k].CoOrds[l].X == i && (currentplayer.ShipInventory[k].CoOrds[l].Y) == j && !currentplayer.ShipInventory[k].CoOrds[l].wasCoordHit)
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write("o  ");
                                    match = true;
                                    Console.ResetColor();
                                }

                                else if (currentplayer.ShipInventory[k].CoOrds[l].X == i && (currentplayer.ShipInventory[k].CoOrds[l].Y) == j && currentplayer.ShipInventory[k].CoOrds[l].wasCoordHit)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("X  ");
                                    match = true;
                                    Console.ResetColor();
                                }
                            }
                        }
                    }

                    // run this code block if there are previously no matching coordinates above
                    if (!match)
                    {
                        if (j == 0 && i > 0 && i < 11)
                        {
                            if (i == 10)
                            {
                                Console.Write(rowa);
                            }
                            else
                            {
                                Console.Write(rowa + " ");
                            }
                            rowa++;
                        }
                        else if (j == 11 && i > 0 && i < 11)
                        {
                            if (i == 10)
                            {
                                Console.Write(rowb);

                            }
                            else
                            {
                                Console.Write(rowb + " ");
                            }
                            rowb++;
                        }

                        for (int k = 0; k < currentplayer.AttemptedCoordinates.Count; k++)
                        {
                            // offset coordinates 10 places on Y axis because we want them to show in the secondary grid
                            if (currentplayer.AttemptedCoordinates[k].X == i && (currentplayer.AttemptedCoordinates[k].Y + 10) == j && !currentplayer.AttemptedCoordinates[k].wasCoordHit)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("x  ");
                                match = true;
                                Console.ResetColor();
                            }

                            // mark red X on left hand side currentplayer grid if your ship was hit by the computer
                            else if (currentplayer.AttemptedCoordinates[k].X == i && (currentplayer.AttemptedCoordinates[k].Y + 10) == j && currentplayer.AttemptedCoordinates[k].wasCoordHit)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("X  ");
                                match = true;
                                Console.ResetColor();
                            }

                            // show computer coordinates fired at currentplayer that didn't hit a ship on the left hand side grid (currentplayer ship view)
                            if (opposition.AttemptedCoordinates[k].X == i && (opposition.AttemptedCoordinates[k].Y) == j && !opposition.AttemptedCoordinates[k].wasCoordHit)
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write("x  ");
                                match = true;
                                Console.ResetColor();
                            }

                        }

                        if (!match)
                        {
                            if (i > 0 && j > 0 && j <= 20)
                            {
                                Console.Write("*  ");
                            }
                        }

                        // draw legend
                        if (j == 21 && i < legend.Count)
                        {
                            switch (i)
                            {
                                case 2:
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    break;
                                case 3:
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    break;
                                case 4:
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    break;
                                case 5:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                default: Console.ForegroundColor = ConsoleColor.White; break;
                            }

                            Console.Write(legend[i]);
                            Console.ResetColor();
                        }
                        match = false;
                    }
                    // once we hit the edge of the boundary start a new line for the next row
                    if (j == 30)
                    {
                        Console.WriteLine();
                    }
                    match = false;
                }
            }

        }

    }
}
