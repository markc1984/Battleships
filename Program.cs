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
            StartGame();

            while (true)
            {
                Coordinates? inputcoordinates;
                if (currentplayer == 0)
                {
                    // print
                    Console.Write(players[currentplayer].EntrantName + ", please enter your co-ordinate: ");
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
                    // if the shot has already been taken, reset the loop and start over
                    if (TakeAShot(inputcoordinates))
                    {
                        continue;
                    }
                    // if shot hasn't been taken, move on to next player
                    else
                    {
                        // decrement current player to human, increment opposing player to CPU
                        currentplayer--;
                        opposition++;
                    }
                }

                // here we check each iteration of the loop for progress of each player after each turn, if the player is eliminated we reinitialise the game
                if (CheckIfPlayerIsEliminated(players[currentplayer]))
                {
                    Console.WriteLine("Congratulations " + players[opposition].EntrantName + " you won the game");
                    Console.WriteLine("Press enter to start again...");
                    Console.ReadLine();
                    Console.Clear();
                    StartGame();
                }
            }
        }

        // CheckForDuplicateTries checks existing entered coordinates against currently entered coordinates and returns true if there is a match
        private static bool CheckForDuplicateTries(Coordinates coords)
        {
            for (int i = 0; i < players[currentplayer].AttemptedCoordinates.Count; i++)
            {
                if (coords.X == players[currentplayer].AttemptedCoordinates[i].X && coords.Y == players[currentplayer].AttemptedCoordinates[i].Y)
                {
                    return true;
                }
            }
            return false;
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
        private static void StartGame()
        {
            players = new List<Player>();
            // Add players to game
            for (int i = 0; i < 2; i++)
            {
                players.Add(new Player());
            }

            // set the current player back to the human player if it wasn't set at that before
            currentplayer = 0;
            opposition = 1;

            // Populate game with ships in random locations

            for (int i = 0; i < 2; i++)
            {
                AddRandomShipsToGame(players[i]);
            }

            Console.Write("Please enter your name: ");

            players[0].EntrantName = Console.ReadLine();
            players[1].EntrantName = "Computer";


            // for testing - output ship coordinates to console
            Console.WriteLine("----------");
            Console.WriteLine(players[0].EntrantName);
            Console.WriteLine("----------");
            for (int i = 0; i < players[0].ShipInventory.Count; i++)
            {
                int ori = 0;
                ori = players[0].ShipInventory[i].ShipOrientation;
                Console.WriteLine(GetShipName(players[0].ShipInventory[i]));

                if (ori == 0)
                {
                    Console.WriteLine("Horizontal");
                }
                else
                {
                    Console.WriteLine("Vertical");
                }
                for (int j = 0; j < players[0].ShipInventory[i].CoOrds.Count; j++)
                {
                    KeyValuePair<char, int> pair = CoordinatesLookupReverse(players[0].ShipInventory[i].CoOrds[j]);

                    Console.WriteLine(pair.Key + pair.Value.ToString());
                }
                Console.WriteLine();
            }

            Console.WriteLine("----------");
            Console.WriteLine(players[1].EntrantName);
            Console.WriteLine("----------");
            for (int i = 0; i < players[1].ShipInventory.Count; i++)
            {
                int ori = 0;
                ori = players[1].ShipInventory[i].ShipOrientation;

                Console.WriteLine(GetShipName(players[1].ShipInventory[i]));

                if (ori == 0)
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
            // display a key of what the different colours signify
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Red = Hit/Ship Destroyed");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Blue = Miss/Ship Active");
            Console.ResetColor();
            Console.WriteLine();

            for (int i = 0; i < players.Count; i++)
            {
                Console.WriteLine("----------");
                Console.WriteLine(players[i].EntrantName);
                Console.WriteLine("----------");

                if (players[i].AttemptedCoordinates.Count > 0)
                {
                    Console.Write("Attempted Coordinates: ");
                }

                for (int k = 0; k < players[i].AttemptedCoordinates.Count; k++)
                {
                    KeyValuePair<char, int> keyValuePair = CoordinatesLookupReverse(players[i].AttemptedCoordinates[k]);
                    // mark attempted coordinate as red if it was a hit
                    if (players[i].AttemptedCoordinates[k].wasCoordHit)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(keyValuePair.Key + keyValuePair.Value.ToString() + " - Hit - " + "(" + GetShipName(players[i].AttemptedCoordinates[k].Ship) + ") ");
                    }
                    // mark attempted coordinate as blue if it was a miss
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(keyValuePair.Key + keyValuePair.Value.ToString() + " ");
                    }
                    Console.ResetColor();
                }
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
            Console.WriteLine();
        }

        // The TakeAShot method initiates a shot from either player, it returns a boolean true or false to the initial calling method as to whether or not there is a duplicate coordinates entry by either player
        private static bool TakeAShot(Coordinates coords)
        {
            KeyValuePair<char, int> keyValuePair = CoordinatesLookupReverse(coords);
            // if the coordinates aren't duplicated by the current player
            if (!CheckForDuplicateTries(coords))
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
            // if the coordinates are a duplicate entry
            else
            {
                Console.WriteLine(keyValuePair.Key + keyValuePair.Value.ToString() + " has already been fired upon by you");
                return true;
            }
            // call the PrintStatistics method to display the current status of the game
            //PrintStatistics();
            PrintGrid();
            return false;
        }

        // RegisterHitCoordinates records a valid, unique shot relative to the player
        private static Coordinates? RegisterHitCoordinates(Coordinates coords, Player player)
        {
            players[currentplayer].AttemptedCoordinates.Add(coords);

            // traverse player ship coordinates
            for (int i = 0; i < player.ShipInventory.Count; i++)
            {
                // if ship has already been eliminated, we do not need to check again, move to next ship
                if (player.ShipInventory[i].IsShipEliminated)
                {
                    continue;
                }
                // for each coordinate of a ship
                for (int j = 0; j < player.ShipInventory[i].CoOrds.Count; j++)
                {
                    // if the inputted coordinates match a ship occupying coordinate
                    if (coords.X == player.ShipInventory[i].CoOrds[j].X && coords.Y == player.ShipInventory[i].CoOrds[j].Y)
                    {
                        player.ShipInventory[i].CoOrds[j].wasCoordHit = true;
                        player.ShipInventory[i].CoOrds[j].shotCalledBy = player;
                        // update the last added element in the attempted coordinates list to signify that we have a hit coordinate and who the shot was called by
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
            // initialise a hit counter
            int hits = 0;

            // for each coordinate that belongs to the ship
            for (int i = 0; i < ship.CoOrds.Count; i++)
            {
                // if the coordinate of ship was hit
                if (ship.CoOrds[i].wasCoordHit)
                {
                    // increment the hit counter
                    hits++;
                }
            }
            // if the number of spaces the ship occupies matches the number of hits this ship has received
            if (hits == ship.NumOfSpaces)
            {
                // flag the ship as eliminated and return true that the ship was eliminated
                ship.IsShipEliminated = true;
                return true;
            }
            // return false if ship is still intact
            return false;
        }

        // CheckIfPlayerIsEliminated checks if all ships have been eliminated and returns true if it is found stated player loses all ships
        private static bool CheckIfPlayerIsEliminated(Player player)
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

        // method that validates the entered X and Y literal coordinates, converts them to an integer and returns a coordinate object
        private static Coordinates? CoordinatesLookupTable(string str)
        {
            char column;

            Char.TryParse(str.Substring(0, 1), out column);
            // if the value in the lookup table exists, it means the entry is valid
            if (map.TryGetValue(column, out int y))
            {
                // if the numeric part of the string successfully parses
                if (Int32.TryParse(str.Substring(1), out int x))
                {
                    // if the numeric value is equal or lower than the permitted boundary size
                    if (y <= 10)
                    {
                        // retrieve the numeric equivalent of the lettered coordinate (column)

                        // create a coordinate object and return it to calling method
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
            // there was no such entry in the table, therefore return null
            else
            {
                return null;
            }
            return null;
        }

        private static void AddRandomShipsToGame(Player player)
        {
            // instantiate a random object
            Random r = new Random();
            for (int i = 0; i < player.ShipInventory.Count; i++)
            {
                // create a random set of numeric X and Y coordinates between 1 and 11
                int x = r.Next(1, 11);
                int y = r.Next(1, 11);

                // Send x any y values, along with the associated ship to be placed and the player the ship belongs to
                GenerateCoordinateRanges(x, y, player.ShipInventory[i], player);
            }
        }
        private static void GenerateCoordinateRanges(int x, int y, Ship ship, Player player)
        {   // Create blank list of coordinates to be tested
            List<Coordinates> coordinates = new List<Coordinates>();
            // the end coordinate that the ship will reach on the x axis using provided random coordinates
            int xupperbound = x + ship.NumOfSpaces;
            // the end coordinate that the ship will reach on the y axis using provided random coordinates
            int yupperbound = y + ship.NumOfSpaces;
            // origin x and y coordinates
            int xvalue = x;
            int yvalue = y;

            // if the shpi has been randomly placed horizontally across the axis, the x axis will remain fixed relative to Y
            if (ship.ShipOrientation == (int)Orientation.Horizontal)
            {
                // these are the potential upper boundary limitations for a given ship placed horizontally
                // if the chosen random coordinates PLUS the width of the ship go beyond 10 i.e beyond the bounds of the grid
                if (yupperbound > 10)
                {
                    // offset the ship position start in the opposite direction to remain within the boundary
                    yvalue = yvalue - ship.NumOfSpaces;

                }
                // if the chosen random coordinates PLUS the width of the ship does not exceed 10
                for (int i = 0; i < ship.NumOfSpaces; i++)
                {
                    // increment number of coordinate spaces from origin                    
                    coordinates.Add(new Coordinates { X = x, Y = yvalue++ });
                }
            }
            // if the random ship orientation is vertical
            else
            {
                if (xupperbound > 10)
                {
                    xvalue = xvalue - ship.NumOfSpaces;

                }

                for (int i = 0; i < ship.NumOfSpaces; i++)
                {
                    coordinates.Add(new Coordinates { X = xvalue++, Y = y });
                }

            }

            // if the ship hasn't already been added to the grid
            if (!ship.AlreadyAdded)
            {
                // if there is a collision detected, generate new random coordinates and recurse till non conflicting coordinates are found
                if (CheckForShipCollision(coordinates, player))
                {
                    Random r = new Random();
                    int newx = r.Next(1, 11);
                    int newy = r.Next(1, 11);
                    GenerateCoordinateRanges(newx, newy, ship, player);
                }
                else
                {
                    // If no collisions were found then we can safely assign these ship coordinates to the current ship
                    ship.CoOrds = coordinates;

                    // tag the ship to these coordinates so we can keep track of what ship the coordinates belong to
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
            // we now traverse the calculated coordinates and check other ships to see if they are already placed on the board
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

        public static void PrintGrid()
        {
            char c = 'A';
            int counter = 1;

            Console.Clear();
            bool match = false;
            Console.WriteLine("------------");
            Console.WriteLine(players[currentplayer].EntrantName);
            Console.WriteLine("------------");

            for (int i = 0; i <= 10; i++)
            {
                for (int j = 0; j <= 10; j++)
                {
                    if (j == 0 && i == 0)
                    {
                        Console.Write("  ");

                    }
                    if (j >= 0 && i == 0 && j <= 9)
                    {
                        Console.Write(c + "  ");
                        c++;
                    }

                    for (int k = 0; k < players[currentplayer].ShipInventory.Count; k++)
                    {

                        for (int l = 0; l < players[currentplayer].ShipInventory[k].CoOrds.Count; l++)
                        {


                            if (players[currentplayer].ShipInventory[k].CoOrds[l].X == i && players[currentplayer].ShipInventory[k].CoOrds[l].Y == j)
                            {
                                if (players[currentplayer].ShipInventory[k].CoOrds[l].wasCoordHit)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("X  ");
                                    match = true;
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write("o  ");
                                    match = true;
                                    Console.ResetColor();
                                }
                            }
                        }
                    }

                    if (!match)
                    {
                        for (int k = 0; k < players[currentplayer].AttemptedCoordinates.Count; k++)
                        {
                            if (players[currentplayer].AttemptedCoordinates[k].X == i && players[currentplayer].AttemptedCoordinates[k].Y == j)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("x  ");
                                match = true;
                                Console.ResetColor();
                            }
                        }

                        if (j == 0 && i > 0 && i < 10)
                        {
                            Console.Write(counter + " ");
                            counter++;
                        
                        }

                        if (j == 0 && i == 10)
                        {
                            Console.Write(counter);
                        }

                        if (!match)
                        {
                            if (i > 0 && j > 0)
                            {
                                Console.Write("*  ");
                            }
                        }
                        match = false;
                    }


                    if (j == 10)
                    {
                        Console.WriteLine();
                    }

                    match = false;
                }
            }
        }
    }
}



