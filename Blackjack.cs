using System.Text;

namespace Blackjack
{
    internal class Blackjack
    {
        private readonly Dealer _dealer;
        private List<Player> _players;
        private List<Player> _leaderboard = new List<Player>();

        public Blackjack(List<Player> players)
        {
            this._dealer = new Dealer();
            this._players = players;

            // Needed so the program can display the Euro symbol
            Console.OutputEncoding = Encoding.UTF8;
        }

        public void Start()
        {
            PlayersBetMoney();

            if (_players.Count == 0)
            {
                Clear();
                Console.WriteLine("Er zijn geen spelers, druk op een toets om te stoppen");
                Console.ReadKey();
                return;
            }

            _dealer.GetNewDeck();
            _dealer.Hit();

            foreach(Player player in _players)
            {
                player.Hit(_dealer.GiveCard());
                player.Hit(_dealer.GiveCard());
            }

            DealCards();
            PrintResults();

            foreach (Player player in _players)
            {
                player.GetBetOutcome();
            }

            PrintLeaderboard();
            AskAgain();
        }

        private void PlayersBetMoney() 
        {
            // Needed so the loop doesn't throw an InvalidOperationException when removing a player
            List<Player> removedPlayers = new List<Player>();

            foreach(Player player in _players)
            {
                if(player.LeaveBet)
                {
                    continue;
                }

                if(player.Money < 1)
                {
                    Clear();
                    Console.WriteLine($"{player.Name}, je hebt niet genoeg geld, je bent uit het spel gezet.");
                    Console.WriteLine("Druk op een toets om door te gaan");
                    Console.ReadKey();

                    removedPlayers.Add(player);
                    continue;
                }

                while(true)
                {
                    Clear();
                    Console.Write($"{player.Name}, hoeveel wil je inzetten (€{1:F2}-€{player.Money:F2})? €");

                    if(double.TryParse(Console.ReadLine()?.Replace(".", ","), out double amount) && amount >= 1 && amount <= player.Money)
                    {
                        player.BetMoney(amount);
                        break;
                    }
                }
            }

            foreach (Player player in removedPlayers)
            {
                _players.Remove(player);
            }
        }

        private void DealCards()
        {
            foreach (Player player in _players)
            {
                if(player.GetPoints() == 21)
                {
                    continue;
                }

                while (player.GetPoints() < 21)
                {
                    Clear();
                    PrintStats(player);

                    Console.WriteLine($"\n{player.Name} is aan de beurt: " + player.GetCardsAndPoints(noName: true));
                    Console.Write("Hitten of passen? (h/p): ");

                    ConsoleKey choice = Console.ReadKey(true).Key;
                    if (choice == ConsoleKey.H)
                    {
                        player.Hit(_dealer.GiveCard());
                    }
                    else if (choice == ConsoleKey.P)
                    {
                        break;
                    }
                }
            }

            while(_dealer.GetPoints() < 17)
            {
                _dealer.Hit();
            }
        }

        private void AskAgain()
        {
            ConsoleKey key;
            bool stop = false;

            Console.WriteLine("\n\nWil iedereen nog een ronde spelen? ");
            Console.WriteLine("Als iemand wil stoppen, druk dan op 'a'.");
            Console.Write("\nWil je nog een ronde spelen? (y/n/a): ");
            while(true)
            {
                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Y)
                {
                    break;
                }
                else if(key == ConsoleKey.N)
                {
                    stop = true;
                    break;
                }
                else if(key == ConsoleKey.A)
                {
                    while (true)
                    {
                        Clear();
                        Console.WriteLine("Wil er iemand stoppen?");
                        Console.WriteLine("Om te stoppen, typ het getal dat naast je naam staat.\n");

                        int indent = _players.Count.ToString().Length;
                        for (int i = 0; i < _players.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. ".PadLeft(indent + 2, ' ') + _players.ElementAt(i));
                        }

                        Console.Write("\nTyp getal of laat leeg: ");
                        string input = Console.ReadLine();

                        if (input == "")
                        {
                            break;
                        }

                        if (int.TryParse(input, out int index))
                        {
                            if (index > 0 && index <= _players.Count)
                            {
                                _players.RemoveAt(index - 1);
                            }
                        }
                    }

                    break;
                }
            }

            if(!stop)
            {
                _dealer.ResetRound();
                foreach (Player player in _players)
                {
                    player.ResetRound();
                }
                Start();
            }
        }

        private static void Clear()
        {
            Console.Clear();
            PrintTitle();
        }

        public static void PrintTitle()
        {
            Console.WriteLine("Laten we Blackjack spelen!\n");
        }

        private void PrintStats(Player? currentPlayer = null, bool roundEnd = false)
        {
            Console.WriteLine(_dealer.GetCardsAndPoints(color: true) + "\n");
            Console.ResetColor();

            foreach(Player player in _players)
            {
                if (player == currentPlayer)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.WriteLine(player.GetCardsAndPoints(color: true, roundEnd: roundEnd));
                Console.ResetColor();
            }
        }

        private void PrintResults()
        {
            Clear();
            _leaderboard.Clear();

            foreach (Player player in _players)
            {
                player.ComparePointsToDealer(_dealer, gameEnded: true);
                _leaderboard.Add(player);
            }
            PrintStats(roundEnd: true);
        }

        private void PrintLeaderboard()
        {
            // TODO: Make code readability better

            string[] columnHeaders = { "Plaats", "Naam", "Geld", "Gewonnen" };
            int[] columnLengths = { _leaderboard.Count.ToString().Length, 0, 0, 0 };
            string columnSeperator = " | ";
            string headerRow = "";

            // Sort array by rounds won and set maximum column length
            if(_leaderboard.Count == 1)
            {
                CalculateColumnLengths(_leaderboard[0]);
            }
            else
            {
                _leaderboard.Sort((p1, p2) => {
                    CalculateColumnLengths(p1);
                    CalculateColumnLengths(p2);

                    return p2.WonRounds.CompareTo(p1.WonRounds);
                });
            }

            // Sets the maximum needed column length taking the headers into account
            for (int i = 0; i < columnHeaders.Length; i++)
            {
                columnLengths[i] = Math.Max(columnLengths[i], columnHeaders[i].Length);
            }

            // Build header row string
            headerRow = columnSeperator + columnHeaders[0].PadLeft(columnLengths[0]) + columnSeperator +
                columnHeaders[1].PadRight(columnLengths[1]) + columnSeperator +
                columnHeaders[2].PadLeft(columnLengths[2]) + columnSeperator +
                columnHeaders[3].PadLeft(columnLengths[3]) + columnSeperator;

            // Print scoreboard text, header row and row seperator
            Console.WriteLine("\n" + " Scorebord ".PadLeft((headerRow.Length + 11) / 2, '=').PadRight(headerRow.Length, '=') + "\n");
            Console.WriteLine(headerRow);
            Console.WriteLine("".PadLeft(headerRow.Length, '-'));

            for (int i = 0; i < _leaderboard.Count; i++)
            {
                // Build player row string
                Player player = _leaderboard[i];
                string placeString = $"{i + 1}e".PadLeft(columnLengths[0]) + columnSeperator;
                string nameString = $"{player.Name}".PadRight(columnLengths[1]) + columnSeperator;
                string moneyString = $"€{player.Money:F2}".PadLeft(columnLengths[2]) + columnSeperator;
                string wonRoundsString = $"{player.WonRounds}".PadLeft(columnLengths[3]) + columnSeperator;

                Console.WriteLine(columnSeperator + placeString + nameString + moneyString + wonRoundsString);
            }

            void CalculateColumnLengths(Player player)
            {
                columnLengths[1] = Math.Max(columnLengths[1], player.Name.Length);
                columnLengths[2] = Math.Max(columnLengths[2], $"{player.Money:F2}".Length + 1);
                columnLengths[3] = Math.Max(columnLengths[3], player.WonRounds.ToString().Length);
            }
        }
    }
}
