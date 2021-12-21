// See https://aka.ms/new-console-template for more informationn
namespace Battleships
{
    public class Program
    {
        private static List<Player> players = new List<Player>();
        private static Dictionary<char, int> map = new Dictionary<char, int>();
        private static Dictionary<int, char> mapreverse = new Dictionary<int, char>();
        private static int currentplayer = 0;
        private static int opposition = 1;


        static void Main(string[] args)
        {
            CreateLookupTables();
            InitialiseGame();

            while (true)
            {
                Console.WriteLine();
                Coordinates? inputcoordinates;
                if (currentplayer == (int)CurrentPlayer.Player)
                {
                    PrintStats.PrintGrid(players[currentplayer], players[opposition]);
                    CheatSheet();

                    // print
                    Console.Write(players[currentplayer].EntrantName + ", please enter your grid co-ordinates (e.g A3, B5): ");
                    // Use lookup table to convert literal coordinates in to a numeric coordinate object
                    string? consoleinput;

                    consoleinput = Console.ReadLine();

                    if (!String.IsNullOrWhiteSpace(consoleinput))
                    {
                        inputcoordinates = CoordinatesLookupTable(consoleinput);
                        // if returned coordinates are null, it means the entry entered went beyond the 10 x 10 grid
                        if (inputcoordinates == null)
                        {
                            Console.WriteLine("Coordinates out of bounds, please try again.");
                            continue;
                        }

                        // if the coordinates are valid, take the shot
                        else
                        {
                            Console.WriteLine();
                            // if the shot has already been taken, don't swap to next player, reiterate the loop until non duplicate coordinates are added
                            if (TakeAShot(inputcoordinates))
                            {
                                continue;
                            }
                            else
                            {
                                // increment current player to CPU, decrement opposing player to human
                                currentplayer++;
                                opposition--;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid entry, please try again.");
                        continue;
                    }
                }
                // if the current player is the computer
                else
                {
                    //randomly choose coordinates
                    Random random = new Random();
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
                    Console.ReadLine();
                    Console.Clear();
                    InitialiseGame();
                }
            }
        }

        // CreateLookupTables creates grid lookup table to translate inputted literal coordinate to integer equivalent
        private static void CreateLookupTables()
        {
            int numeric = 1;
            for (char c = 'A'; c <= 'J'; c++)
            {
                map.Add(c, numeric);
                numeric++;
            }
            // set the counter back to 1
            numeric = 1;
            // Create inverse grid lookup table to translate inputted integer coordinate to its literal equivalent
            for (char c = 'A'; c <= 'J'; c++)
            {
                mapreverse.Add(numeric, c);
                numeric++;
            }
        }

        // StartGame starts a new game by erasing and readding players
        private static void InitialiseGame()
        {
            players = new List<Player>();
            // Add players to game
            for (int i = 0; i < 2; i++)
            {
                players.Add(new Player());
            }

            currentplayer = 0;
            opposition = 1;


            for (int i = 0; i < 2; i++)
            {
                AddRandomShipsToGame(players[i]);
            }

            Console.Write("Please enter your name: ");

            players[0].EntrantName = Console.ReadLine();
            players[1].EntrantName = "Computer";

            Console.WriteLine("----------");
            Console.WriteLine(players[1].EntrantName);
            Console.WriteLine("----------");
            for (int i = 0; i < players[1].ShipInventory.Count; i++)
            {
                int orientation = (int)ShipOrientation.Horizontal;
                orientation = players[1].ShipInventory[i].ShipOrientation;

                Console.WriteLine(GetShipName(players[1].ShipInventory[i]));

                if (orientation == (int)ShipOrientation.Horizontal)
                {
                    Console.WriteLine("Horizontal");
                }
                else
                {
                    Console.WriteLine("Vertical");
                }
                for (int j = 0; j < players[1].ShipInventory[i].CoOrds.Count; j++)
                {
                    KeyValuePair<char, int> pair = CoordinatesLookupReverse(players[1].ShipInventory[i].CoOrds[j]);
                    Console.WriteLine(pair.Key + pair.Value.ToString());
                }
                Console.WriteLine();
            }
        }

        // a method that prints out the currently fired coordinates and the current state of the game
        private static void PrintStatistics()
        {
            Console.WriteLine();

            for (int i = 0; i < players.Count; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < players[i].ShipInventory.Count; j++)
                {
                    Console.Write(GetShipName(players[i].ShipInventory[j]) + " - " + "Current Status: ", ConsoleColor.White);

                    // mark destroyed ship as red
                    if (players[i].ShipInventory[j].IsShipEliminated)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Destroyed");
                        Console.ResetColor();
                    }
                    // mark active ship as blue
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("Active");
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // method that outputs the coordinate location
        private static void CheatSheet()
        {
            Console.WriteLine();
            Console.WriteLine("----------");
            Console.WriteLine(players[1].EntrantName);
            Console.WriteLine("----------");
            Console.WriteLine();
            for (int j = 0; j < players[1].ShipInventory.Count; j++)
            {
                int ori = 0;
                ori = players[1].ShipInventory[j].ShipOrientation;
                Console.WriteLine(GetShipName(players[1].ShipInventory[j]));
                Console.WriteLine("----------");


                if (ori == 0)
                {
                    Console.WriteLine("Horizontal");
                }
                else
                {
                    Console.WriteLine("Vertical");
                }
                Console.WriteLine();
                for (int k = 0; k < players[1].ShipInventory[j].CoOrds.Count; k++)
                {
                    KeyValuePair<char, int> pair = CoordinatesLookupReverse(players[1].ShipInventory[j].CoOrds[k]);

                    Console.WriteLine(pair.Key + pair.Value.ToString());
                }
                Console.WriteLine();
            }
            
        }

        // The TakeAShot method initiates a shot from either player, it returns a boolean true or false to the initial calling method as to whether or not there is a duplicate coordinates entry by either player
        private static bool TakeAShot(Coordinates coords)
        {
            KeyValuePair<char, int> keyValuePair = CoordinatesLookupReverse(coords);

            if (!players[currentplayer].CheckForDuplicateTries(coords))
            {
                // if a ship is returned, this means there is a match
                Coordinates? updatedCoordinates = RegisterHitCoordinates(coords, players[opposition]);

                if (updatedCoordinates == null)
                {
                    Console.WriteLine(players[currentplayer].EntrantName + " fires at " + players[opposition].EntrantName + "'s cordinates at " + keyValuePair.Key + keyValuePair.Value.ToString() + " and misses...");
                    Console.Write("Press enter to continue...");
                    Console.ReadLine();
                }
                else
                {
                    if (CheckIfShipIsEliminated(updatedCoordinates.Ship))
                    {
                        Console.WriteLine();
                        Console.WriteLine(players[currentplayer].EntrantName + " eliminates " + players[opposition].EntrantName + "'s " + GetShipName(updatedCoordinates.Ship) + "...");
                        Console.Write("Press enter to continue...");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine(players[currentplayer].EntrantName + " fires at " + players[opposition].EntrantName + "'s cordinates at " + keyValuePair.Key + keyValuePair.Value.ToString() + " and HITS their " + GetShipName(updatedCoordinates.Ship) + "...");
                        Console.Write("Press enter to continue...");
                        Console.ReadLine();
                    }
                }
            }
            else
            {
                Console.WriteLine(keyValuePair.Key + keyValuePair.Value.ToString() + " has already been fired upon by you");
                return true;
            }
            // call the PrintStatistics method to display the current status of the game
            //PrintStatistics();
            return false;
        }

        // RegisterHitCoordinates records a valid, unique shot for a player
        private static Coordinates? RegisterHitCoordinates(Coordinates coords, Player player)
        {
            players[currentplayer].AttemptedCoordinates.Add(coords);

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
                        player.ShipInventory[i].CoOrds[j].wasCoordHit = true;
                        player.ShipInventory[i].CoOrds[j].shotCalledBy = player;
                        // update the last added element in the attempted coordinates list
                        players[currentplayer].AttemptedCoordinates[players[currentplayer].AttemptedCoordinates.Count - 1] = player.ShipInventory[i].CoOrds[j];
                        return player.ShipInventory[i].CoOrds[j];
                    }
                }
            }
            return null;
        }

        // a method that checks to see if all coordinates belonging to ship have been hit
        private static bool CheckIfShipIsEliminated(Ship? ship)
        {
            int hits = 0;

            for (int i = 0; i < ship.CoOrds.Count; i++)
            {
                if (ship.CoOrds[i].wasCoordHit)
                {
                    hits++;
                }
            }
            if (hits == ship.NumOfSpaces)
            {
                ship.IsShipEliminated = true;
                return true;
            }
            return false;
        }

         // method that coverts integer coordinates back to literal using a reverse lookup table
        private static KeyValuePair<char, int> CoordinatesLookupReverse(Coordinates coords)
        {
            KeyValuePair<char, int> store;
            char output;
            // look up directionary mappings, if a corresponding key is present then output the paired value to that key
            mapreverse.TryGetValue(coords.Y, out output);
            store = new KeyValuePair<char, int>(output, coords.X);
            return store;
        }

        // method that validates and converts string entry to decimal coordinates
        private static Coordinates? CoordinatesLookupTable(string str)
        {
            char column;

            if (Char.TryParse(str.Substring(0, 1).ToUpper(), out column))
            {
                if (map.TryGetValue(column, out int y))
                {
                    if (Int32.TryParse(str.Substring(1), out int x))
                    {
                        if (x >= 1 && x <= 10)
                        {
                            Coordinates coords = new Coordinates();
                            coords.X = x;
                            coords.Y = y;
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

        private static void AddRandomShipsToGame(Player player)
        {
            Random r = new Random();
            for (int i = 0; i < player.ShipInventory.Count; i++)
            {
                // create a random set of numeric X and Y coordinates between 1 and 11
                int x = r.Next(1, 11);
                int y = r.Next(1, 11);

                // Send x any y values and the associated ship to be placed along player the ship belongs to
                GenerateCoordinateRanges(x, y, player.ShipInventory[i], player);
            }
        }
        private static void GenerateCoordinateRanges(int x, int y, Ship ship, Player player)
        {   List<Coordinates> coordinates = new List<Coordinates>();
            // the end coordinates that the ship will reach
            int xupperboundary = x + ship.NumOfSpaces;
            int yupperboundary = y + ship.NumOfSpaces;
            // origin x and y coordinates
            int originxvalue = x;
            int originyvalue = y;

            // if the shpi has been randomly placed horizontally across the axis, the x axis will remain fixed relative to Y
            if (ship.ShipOrientation == (int)ShipOrientation.Horizontal)
            {
                // if the chosen random coordinates PLUS the width of the ship go beyond 10 i.e beyond the bounds of the grid
                if (yupperboundary > 10)
                {
                    // offset the ship position start in the opposite direction to remain within the boundary
                    originyvalue = originyvalue - ship.NumOfSpaces;

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
                    originxvalue = originxvalue - ship.NumOfSpaces;
                }

                for (int i = 0; i < ship.NumOfSpaces; i++)
                {
                    coordinates.Add(new Coordinates { X = originxvalue++, Y = y });
                }

            }

            if (!ship.AlreadyAdded)
            {
                // if a collision is detected, recurse with new random coordinates
                if (CheckForShipCollision(coordinates, player))
                {
                    Random r = new Random();
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

                    ship.AlreadyAdded = true;
                }
            }
        }
        private static bool CheckForShipCollision(List<Coordinates> coords, Player player)
        {
            // traverse calculated coordinates and check other ships to see if they are already placed on the board
            for (int i = 0; i < player.ShipInventory.Count; i++)
            {
                for (int j = 0; j < player.ShipInventory[i].CoOrds.Count; j++)
                {
                    //if the provided X and Y coordinates match with a ship already in the data structure
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
        private static string GetShipName(Ship? ship)
        {
            if (ship.TypeOfShip == (int)ShipType.Battleship)
            {
                return "Battleship";
            }
            if (ship.TypeOfShip == (int)ShipType.Destroyer)
            {
                return "Destroyer";
            }
            return "";
        }

    }
}



