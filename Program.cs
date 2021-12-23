// See https://aka.ms/new-console-template for more informationn
namespace Battleships
{
    public class Program
    {
        private static List<Player> players = new();
        private static Dictionary<char, int> map = new();
        private static Dictionary<int, char> mapreverse = new();
        private static int currentplayer = 0;
        private static int opposition = 1;
        private static bool cheatmode = false;
        private static ConsoleKey key;



        static void Main(string[] args)
        {
            CreateLookupTables();
            InitialiseGame();

            while (true)
            {
                Coordinates? inputcoordinates;
                if (currentplayer == (int)CurrentPlayer.Player)
                {
                    Console.Clear();

                    PrintGrid(players[currentplayer], players[opposition]);

                    if (cheatmode)
                    {
                        PrintGrid(players[opposition], players[currentplayer]);
                    }

                    // print
                    Console.Write(players[currentplayer].EntrantName + ", please enter your grid co-ordinates (e.g A3, B5): ");
                    // Use lookup table to convert literal coordinates in to a numeric coordinate object
                    string? consoleinput;

                    consoleinput = Console.ReadLine();

                    if (!String.IsNullOrWhiteSpace(consoleinput))
                    {
                        if (consoleinput.ToLower() == "cheat")
                        {
                            if (!cheatmode)
                            {
                                cheatmode = true;
                            }
                            else if (cheatmode)
                            {
                                cheatmode = false;
                            }
                            continue;
                        }
                        inputcoordinates = CoordinatesLookupTable(consoleinput);
                        // if returned coordinates are null, it means the entry entered went beyond the 10 x 10 grid
                        if (inputcoordinates == null)
                        {
                            Console.WriteLine("Coordinates out of bounds, please enter to try again.....");
                            Console.ReadKey();
                            continue;
                        }

                        else
                        {
                            Console.WriteLine();
                            // don't swap to next player, reiterate the loop until non duplicate coordinates are found
                            if (TakeAShot(inputcoordinates))
                            {
                                continue;
                            }
                            else
                            {
                                currentplayer++;
                                opposition--;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid entry, press enter to try again.....");
                        Console.ReadKey();
                        continue;
                    }
                }
                // if the current player is the computer
                else
                {
                    //randomly choose coordinates
                    Random random = new();
                    inputcoordinates = new Coordinates { X = random.Next(1, 11), Y = random.Next(1, 11) };
                    // reinterate loop if the shot has already been taken
                    if (TakeAShot(inputcoordinates))
                    {
                        continue;
                    }
                    else
                    {
                        currentplayer--;
                        opposition++;
                    }
                }

                // check if the player is eliminated, if so reinitialise the game
                if (players[currentplayer].CheckIfPlayerIsEliminated(players[currentplayer]))
                {
                    Console.WriteLine("Congratulations " + players[opposition].EntrantName + " you won the game");
                    Console.WriteLine("Press enter to start again...");
                    Console.ReadKey();
                    Console.Clear();
                    InitialiseGame();
                }
            }
        }

        public static string PrintDashes(int numOfDashes)
        {
            string dashes = "";

            for (int i = 0; i < numOfDashes; i++)
            {
                dashes += "-";
            }
            return dashes;
        }

        public static void PrintGrid(Player player, Player opposition)
        {
            char c = 'A';
            int rowa = 1;
            int rowb = 1;
            List<string> legend = new() {PrintDashes(6), "Legend", PrintDashes(6), "* = Unhit Coordinate", "o = Unhit Player Ship Coordinate", "x = Computer miss", "x = Player miss", "X = Direct Hit" };
            bool match = false;

            Console.WriteLine(PrintDashes(player.EntrantName.Length));
            Console.WriteLine(player.EntrantName);
            Console.WriteLine(PrintDashes(player.EntrantName.Length));

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

                    // if at the origin of either grid, don't display characters as it's not a valid grid coordinate
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

                    // if we are beyond row or column 0
                    if (i != 0 || j != 0)
                    {
                        for (int k = 0; k < player.ShipInventory.Count; k++)
                        {
                            for (int l = 0; l < player.ShipInventory[k].CoOrds.Count; l++)
                            {
                                if (player.ShipInventory[k].CoOrds[l].X == i && (player.ShipInventory[k].CoOrds[l].Y) == j && !player.ShipInventory[k].CoOrds[l].WasCoordHit)
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write("o  ");
                                    match = true;
                                    Console.ResetColor();
                                }

                                else if (player.ShipInventory[k].CoOrds[l].X == i && (player.ShipInventory[k].CoOrds[l].Y) == j && player.ShipInventory[k].CoOrds[l].WasCoordHit)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("X  ");
                                    match = true;
                                    Console.ResetColor();
                                }
                            }
                        }
                    }

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

                        for (int k = 0; k < player.AttemptedCoordinates.Count; k++)
                        {
                            // offset attempted coordinates 10 places on Y axis because we want them to show in the secondary grid
                            if (player.AttemptedCoordinates[k].X == i && (player.AttemptedCoordinates[k].Y + 10) == j && !player.AttemptedCoordinates[k].WasCoordHit)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("x  ");
                                match = true;
                                Console.ResetColor();
                            }

                            // mark red X on left hand side of human player grid if ship coordinate was hit by the computer
                            else if (player.AttemptedCoordinates[k].X == i && (player.AttemptedCoordinates[k].Y + 10) == j && player.AttemptedCoordinates[k].WasCoordHit)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("X  ");
                                match = true;
                                Console.ResetColor();
                            }

                            // show computer coordinates fired at human player that didn't hit a ship on the left hand side grid
                            if (opposition.AttemptedCoordinates[k].X == i && (opposition.AttemptedCoordinates[k].Y) == j && !opposition.AttemptedCoordinates[k].WasCoordHit)
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


                        // legend
                        if (player.PlayerID == (int)CurrentPlayer.Player)
                        {
                            if (j == 21 && i < legend.Count)
                            {
                                if (i == 3)
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                }
                                else if (i == 4)
                                {
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                }
                                else if (i == 5)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                }
                                else if (i == 6)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                }

                                Console.Write(legend[i]);
                                Console.ResetColor();
                            }
                        }
                    }
                    // once edge of the boundary is reached, start a new line for next row
                    if (j == 30)
                    {
                        Console.WriteLine();
                    }
                    match = false;
                }
            }
            Console.WriteLine();
        }

        // CreateLookupTables creates grid lookup table to translate inputted literal coordinate to integer equivalent
        public static void CreateLookupTables()
        {
            int numeric = 1;
            for (char c = 'A'; c <= 'J'; c++)
            {
                map.Add(c, numeric);
                numeric++;
            }

            numeric = 1;
            // Create inverse grid lookup table
            for (char c = 'A'; c <= 'J'; c++)
            {
                mapreverse.Add(numeric, c);
                numeric++;
            }
        }

   
        private static void InitialiseGame()
        {
            cheatmode = false;

            players = new List<Player>();

            for (int i = 0; i < 2; i++)
            {
                players.Add(new Player());
            }

            currentplayer = 0;
            opposition = 1;
            players[0].PlayerID = (int)CurrentPlayer.Player;
            players[1].PlayerID = (int)CurrentPlayer.Computer;

            for (int i = 0; i < 2; i++)
            {
                List<Ship> shiplist = new();

                for (int j = 0; j < 2; j++)
                {
                    shiplist.Add(new Ship((int)ShipType.Destroyer));
                }
                shiplist.Add(new Ship((int)ShipType.Battleship));

                AddRandomShipsToGame(players[i], shiplist);
            }

            Console.WriteLine("Welcome to Battleships! Prepare your navy for war with the computer!");
            Console.WriteLine();
            Console.WriteLine("Hint! You can see where the computer ships are located by typing 'cheat' at the coordinates entry! Typing this again disables the option");
            Console.WriteLine();
            Console.Write("Please enter your name: ");

            players[0].EntrantName = Console.ReadLine();
            players[1].EntrantName = "Computer";

            Console.Clear();
        }

        // The TakeAShot method initiates a shot from either player, it returns a boolean true or false to the initial calling method as to whether or not there is a duplicate coordinates entry by either player
        public static bool TakeAShot(Coordinates coords)
        {
            KeyValuePair<char, int> keyValuePair = CoordinatesLookupReverse(coords);

            if (!players[currentplayer].CheckForDuplicateTries(coords))
            {
                // if a ship is returned, this means there is a match
                Coordinates? updatedCoordinates = players[currentplayer].RegisterHitCoordinates(coords, players[opposition]);

                if (updatedCoordinates == null)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(players[currentplayer].EntrantName + " fires at " + players[opposition].EntrantName + "'s cordinates at " + keyValuePair.Key + keyValuePair.Value.ToString() + " and misses...");
                    Console.ResetColor();
                    Console.Write("Press enter to continue...");
                    Console.ReadLine();
                }
                else
                {
                    Ship? shipToCheck = updatedCoordinates.Ship;
                    if (shipToCheck != null)
                    {
                        if (shipToCheck.CheckIfShipIsEliminated(shipToCheck))
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(players[currentplayer].EntrantName + " eliminates " + players[opposition].EntrantName + "'s " + GetShipName(updatedCoordinates.Ship) + "...");
                            Console.ResetColor();
                            Console.Write("Press enter to continue...");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(players[currentplayer].EntrantName + " fires at " + players[opposition].EntrantName + "'s cordinates at " + keyValuePair.Key + keyValuePair.Value.ToString() + " and HITS their " + GetShipName(updatedCoordinates.Ship) + "...");
                            Console.ResetColor();
                            Console.Write("Press enter to continue...");
                            Console.ReadLine();
                        }
                    }
                }
            }
            else
            {
                if (currentplayer == (int)CurrentPlayer.Player)
                {
                    Console.WriteLine(keyValuePair.Key + keyValuePair.Value.ToString() + " has already been fired upon by you. Press enter to try again....");
                    Console.ReadKey();
                }
                return true;
            }
            return false;
        }

         // method that coverts integer coordinates back to literal using a reverse lookup table
        public static KeyValuePair<char, int> CoordinatesLookupReverse(Coordinates coords)
        {
            KeyValuePair<char, int> store;
            // look up directionary mappings, if a corresponding key is present then output the paired value to that key
            mapreverse.TryGetValue(coords.Y, out char output);
            store = new KeyValuePair<char, int>(output, coords.X);
            return store;
        }

        // method that validates and converts string entry to decimal coordinates
        public static Coordinates? CoordinatesLookupTable(string str)
        {
            if (Char.TryParse(str.Substring(0, 1).ToUpper(), out char column))
            {
                if (map.TryGetValue(column, out int y))
                {
                    if (Int32.TryParse(str.AsSpan(1), out int x))
                    {
                        if (x >= 1 && x <= 10)
                        {
                            Coordinates coords = new()
                            {
                                X = x,
                                Y = y
                            };
                            return coords;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            else
            {
                return null;
            }
            return null;
        }

        public static void AddRandomShipsToGame(Player player, List<Ship> shipsToAdd)
        {
            player.ShipInventory = shipsToAdd;

            Random r = new();
            for (int i = 0; i < player.ShipInventory.Count; i++)
            {
                // create a random set of numeric X and Y coordinates between 1 and 11
                int x = r.Next(1, 11);
                int y = r.Next(1, 11);

                // Send x any y values and the associated ship to be placed along player the ship belongs to
                GenerateCoordinateRanges(x, y, player.ShipInventory[i], player);
            }
        }
        public static void GenerateCoordinateRanges(int x, int y, Ship ship, Player player)
        {   List<Coordinates> coordinates = new();
            // the end coordinates that the ship will reach
            int xupperboundary = x + ship.NumOfSpaces;
            int yupperboundary = y + ship.NumOfSpaces;
            // origin x and y coordinates
            int originxvalue = x;
            int originyvalue = y;

            // if the ship has been randomly placed horizontally across the axis, the x axis will remain fixed relative to Y
            if (ship.ShipOrientation == (int)ShipOrientation.Horizontal)
            {
                // if the chosen random coordinates PLUS the width of the ship go beyond 10 i.e beyond the bounds of the grid
                if (yupperboundary > 10)
                {
                    // offset the ship position start in the opposite direction to remain within the boundary
                    originyvalue -= ship.NumOfSpaces;
                }
                for (int i = 0; i < ship.NumOfSpaces; i++)
                {
                    // increment number of coordinate spaces from origin                    
                    coordinates.Add(new Coordinates { X = x, Y = originyvalue++ });
                }
            }
            else if (ship.ShipOrientation == (int)ShipOrientation.Vertical)
            {
                if (xupperboundary > 10)
                {
                    originxvalue -= ship.NumOfSpaces;
                }

                for (int i = 0; i < ship.NumOfSpaces; i++)
                {
                    coordinates.Add(new Coordinates { X = originxvalue++, Y = y });
                }

            }

            if (!ship.AlreadyAddedToGame)
            {
                // if a collision is detected, recurse with new random coordinates
                if (CheckForShipCollision(coordinates, player))
                {
                    Random r = new();
                    int newx = r.Next(1, 11);
                    int newy = r.Next(1, 11);
                    GenerateCoordinateRanges(newx, newy, ship, player);
                }
                else
                {
                    // No collisions were found, safely ship coordinates to the current ship
                    ship.CoOrds = coordinates;

                    // assign ship as the owner of these coordinates
                    for (int i = 0; i < coordinates.Count; i++)
                    {
                        coordinates[i].Ship = ship;
                    }

                    ship.AlreadyAddedToGame = true;
                }
            }
        }
        public static bool CheckForShipCollision(List<Coordinates> coords, Player player)
        {         
            for (int i = 0; i < player.ShipInventory.Count; i++)
            {
                for (int j = 0; j < player.ShipInventory[i].CoOrds.Count; j++)
                {
                    //if the provided X and Y coordinates match
                    for (int k = 0; k < coords.Count; k++)
                    {
                        if (player.ShipInventory[i].CoOrds[j].X == coords[k].X && player.ShipInventory[i].CoOrds[j].Y == coords[k].Y)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // method that returns the string name of the type of ship
        public static string GetShipName(Ship? ship)
        {
            if (ship != null)
            {
                if (ship.TypeOfShip == (int)ShipType.Battleship)
                {
                    return "Battleship";
                }
                if (ship.TypeOfShip == (int)ShipType.Destroyer)
                {
                    return "Destroyer";
                }
            }
            return "";
        }

    }
}



