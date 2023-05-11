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
            GetPlayerBets();

            if (_players.Count == 0)
            {
                Clear();
                Console.WriteLine("Er zijn geen spelers, druk op een toets om te stoppen");
                Console.ReadKey();
                return;
            }

            // Prepare the game
            _dealer.GetNewDeck();
            _dealer.Hit(_dealer.GiveCard());
            foreach (Player player in _players)
            {
                player.Hit(_dealer.GiveCard());
                player.Hit(_dealer.GiveCard());
            }

            // Ask all the players if they want to hit or stand
            foreach (Player player in _players)
            {
                // Insurance & Even money
                if (_dealer.Cards[0].CValue == Card.Value.Ace)
                {
                    Clear();
                    PrintStats(player);
                    Console.WriteLine($"\n{player.Name} is aan de beurt: " + player.GetCardsAndPoints(noName: true));
                    Console.WriteLine("\nDe dealer heeft een Aas als eerste kaart.");
                    Console.Write("Wil je verzekeren? (y/n) ");

                    if (WaitForKey(ConsoleKey.Y, ConsoleKey.N) == ConsoleKey.Y)
                    {
                        player.Insured = true;
                    }

                    if(player.HasBlackjack)
                    {
                        Clear();
                        PrintStats(player);
                        Console.WriteLine($"\n{player.Name} is aan de beurt: " + player.GetCardsAndPoints(noName: true));
                        Console.WriteLine("\nJij hebt Blackjack, en de dealer heeft een Aas als eerste kaart.");
                        Console.Write("Wil je voor 'even money' gaan? (y/n) ");

                        if (WaitForKey(ConsoleKey.Y, ConsoleKey.N) == ConsoleKey.Y)
                        {
                            player.EvenMoney = true;
                        }
                    }
                }

                bool done = false;
                while (player.GetPoints() < 21 && !done)
                {
                    Clear();
                    done = AskPlayersHitOrStand(player);
                }
            }

            while (_dealer.GetPoints() < 17)
            {
                _dealer.Hit(_dealer.GiveCard());
            }

            // End of the round
            PrintResults();
            foreach (Player player in _players)
            {
                player.DetermineWinnings(_dealer);
            }

            PrintLeaderboard();
            AskPlayAgain();
        }

        private void GetPlayerBets()
        {
            // Needed so the loop doesn't throw an InvalidOperationException when removing a player
            List<Player> removedPlayers = new List<Player>();

            foreach (Player player in _players)
            {
                if (player.LeaveBet)
                {
                    continue;
                }

                if (player.Money < 1)
                {
                    Clear();
                    Console.WriteLine($"{player.Name}, je hebt niet genoeg geld, je bent uit het spel gezet.");
                    Console.WriteLine("\nDruk op een toets om door te gaan");
                    Console.ReadKey();

                    removedPlayers.Add(player);
                    continue;
                }

                while (true)
                {
                    Clear();
                    Console.Write($"{player.Name}, hoeveel wil je inzetten (€{1:F2} - €{player.Money:F2})? €");

                    string input = Console.ReadLine().Replace(".", ",");

                    if (!double.TryParse(input, out double amount))
                    {
                        continue;
                    }

                    if (amount < 1 || amount > player.Money)
                    {
                        continue;
                    }

                    player.BetMoney(amount);
                    break;
                }
            }

            foreach (Player player in removedPlayers)
            {
                _players.Remove(player);
            }
        }

        private bool AskPlayersHitOrStand(Player player)
        {
            PrintStats(player);
            ConsoleKey choice;

            Console.WriteLine($"\n{player.Name} is aan de beurt: " + player.GetCardsAndPoints(noName: true));
            if (player.Cards.Count == 2)
            {
                Console.Write("Hitten, passen of double down? (h/p/d): ");
                choice = WaitForKey(ConsoleKey.H, ConsoleKey.P, ConsoleKey.D);
            }
            else
            {
                Console.Write("Hitten of passen? (h/p): ");
                choice = WaitForKey(ConsoleKey.H, ConsoleKey.P);
            }

            if (choice == ConsoleKey.H)
            {
                player.Hit(_dealer.GiveCard());
                return false;
            }
            else if (choice == ConsoleKey.D)
            {
                player.BetMoney(player.BetAmount * 2);
                player.Hit(_dealer.GiveCard());
                return true;
            }

            return true;
        }

        private void AskPlayAgain()
        {
            ConsoleKey key;
            bool stop = false;

            Console.WriteLine("\n\nWil iedereen nog een ronde spelen? ");
            Console.WriteLine("Als iemand wil stoppen, druk dan op 'a'.");
            Console.Write("\nWil je nog een ronde spelen? (y/n/a): ");

            key = WaitForKey(ConsoleKey.Y, ConsoleKey.N, ConsoleKey.A);

            if (key == ConsoleKey.N)
            {
                stop = true;
            }
            else if (key == ConsoleKey.A)
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
            }

            if (!stop)
            {
                _dealer.ResetRound();
                foreach (Player player in _players)
                {
                    player.ResetRound();
                }
                Start();
            }
        }

        private static ConsoleKey WaitForKey(params ConsoleKey[] allowedKeys)
        {
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (allowedKeys.Contains(key))
                {
                    return key;
                }
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

            foreach (Player player in _players)
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
            int[] columnWidths = { _leaderboard.Count.ToString().Length, 0, 0, 0 };
            string columnSeparator = " | ";
            string headerRow = "";

            // Sort array by rounds won and calculate column widths
            if (_leaderboard.Count == 1)
            {
                CalculateColumnWidth(_leaderboard[0]);
            }
            else
            {
                _leaderboard.Sort((p1, p2) =>
                {
                    CalculateColumnWidth(p1);
                    CalculateColumnWidth(p2);

                    return p2.WonRounds.CompareTo(p1.WonRounds);
                });
            }

            // Sets the maximum needed column widths taking the headers into account
            for (int i = 0; i < columnHeaders.Length; i++)
            {
                columnWidths[i] = Math.Max(columnWidths[i], columnHeaders[i].Length);
            }

            // Build header row string
            headerRow = columnSeparator + columnHeaders[0].PadLeft(columnWidths[0]) + columnSeparator +
                        columnHeaders[1].PadRight(columnWidths[1]) + columnSeparator +
                        columnHeaders[2].PadLeft(columnWidths[2]) + columnSeparator +
                        columnHeaders[3].PadLeft(columnWidths[3]) + columnSeparator;

            // Print scoreboard text, header row and row separator
            Console.WriteLine("\n" + " Scorebord ".PadLeft((headerRow.Length + 11) / 2, '=').PadRight(headerRow.Length, '=') + "\n");
            Console.WriteLine(headerRow);
            Console.WriteLine("".PadLeft(headerRow.Length, '-'));

            for (int i = 0; i < _leaderboard.Count; i++)
            {
                Player player = _leaderboard[i];
                string placeString = $"{i + 1}e".PadLeft(columnWidths[0]) + columnSeparator;
                string nameString = $"{player.Name}".PadRight(columnWidths[1]) + columnSeparator;
                string moneyString;
                if(player.Money < 0)
                {
                    moneyString = $"-€{Math.Abs(player.Money):F2}".PadLeft(columnWidths[2]) + columnSeparator;
                }
                else
                {
                    moneyString = $"€{player.Money:F2}".PadLeft(columnWidths[2]) + columnSeparator;
                }
                string wonRoundsString = $"{player.WonRounds}".PadLeft(columnWidths[3]) + columnSeparator;

                Console.WriteLine(columnSeparator + placeString + nameString + moneyString + wonRoundsString);
            }

            // Helper method to calculate the width needed per column
            void CalculateColumnWidth(Player player)
            {
                columnWidths[1] = Math.Max(columnWidths[1], player.Name.Length);
                columnWidths[2] = Math.Max(columnWidths[2], $"{player.Money:F2}".Length + 1);
                columnWidths[3] = Math.Max(columnWidths[3], player.WonRounds.ToString().Length);
            }
        }
    }
}
