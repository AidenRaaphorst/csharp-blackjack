namespace Blackjack
{
    internal class Player
    {
        private static int playerNumber = 0;
        public string Name { get; protected set; }
        public List<Card> Cards { get; }
        public bool IsDealer { get; }
        public int PlayedRounds { get; private set; }
        public bool HasWonRound { get; private set; }
        public bool HasLostRound { get; private set; }
        public bool HasBlackJack { get; private set; }
        public bool HasTwentyOne { get; private set; }
        public int WonRounds { get; private set; }
        public int LostRounds { get; private set; }
        public double Money { get; private set; }
        public double BetAmount { get; private set; }
        public double BetMultiplier { get; private set; }
        public bool LeaveBet { get; private set; }
        public bool Insured { get; set; }
        public bool EvenMoney { get; set; }

        public Player(int money = 10, string name = "", bool isDealer = false)
        {
            if (!isDealer)
            {
                playerNumber++;
            }

            this.Name = name != "" ? name : "Speler " + playerNumber;
            this.Cards = new List<Card>();
            this.PlayedRounds = 0;
            this.IsDealer = isDealer;
            this.HasWonRound = false;
            this.HasLostRound = false;
            this.HasBlackJack = false;
            this.HasTwentyOne = false;
            this.WonRounds = 0;
            this.LostRounds = 0;
            this.Money = money;
            this.BetAmount = 0.0;
            this.BetMultiplier = 1.0;
            this.LeaveBet = false;
            this.Insured = false;
            this.EvenMoney = false;
        }

        public void Hit(Card card)
        {
            this.Cards.Add(card);
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            int cardCount = this.Cards.Count;
            int points = GetPoints();

            if (points > 21)
            {
                this.HasWonRound = false;
                this.HasLostRound = true;
                this.HasBlackJack = false;
                this.HasTwentyOne = false;
            }
            else if (points == 21)
            {
                this.HasWonRound = true;
                this.HasLostRound = false;
                this.HasBlackJack = cardCount == 2;
                this.HasTwentyOne = cardCount > 2;
            }
            else
            {
                this.HasWonRound = false;
                this.HasLostRound = false;
                this.HasBlackJack = false;
                this.HasTwentyOne = false;
            }
        }

        public int GetPoints()
        {
            int totalPoints = 0;
            foreach (Card card in this.Cards)
            {
                totalPoints += card.GetIntValue(totalPoints);
            }

            return totalPoints;
        }

        public void ResetRound()
        {
            this.Cards.Clear();
            this.HasLostRound = false;
            this.HasWonRound = false;
            this.HasBlackJack = false;
            this.HasTwentyOne = false;
            this.BetMultiplier = 1.0;
            this.Insured = false;

            if (!this.LeaveBet)
            {
                this.BetAmount = 0.0;
            }
        }

        public string GetCardsAndPoints(bool noName = false, bool color = false, bool roundEnd = false)
        {
            int cardCount = this.Cards.Count;
            int points = this.GetPoints();
            string s = string.Join(", ", this.Cards) + $" ({GetPoints()} punten)";

            if (noName)
            {
                return s;
            }

            if (this.HasWonRound)
            {
                if (color)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                string reason;
                if (!roundEnd)
                {
                    reason = points == 21 && cardCount == 2 ? "BlackJack!" : "21 punten!";
                }
                else
                {
                    reason = points == 21 && cardCount == 2 ? "BlackJack!" : "gewonnen van de bank!";
                }
                s = $"{this.Name} heeft {reason}: {s}";
            }
            else if (this.HasLostRound)
            {
                if (color)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }

                string reason = points > 21 ? "is dood" : "heeft verloren";
                s = $"{this.Name} {reason}!: " + s;
            }
            else if (roundEnd)
            {
                if (color)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }

                s = $"{this.Name} heeft gelijk gespeeld: " + s;
            }
            else
            {
                s = $"{this.Name}: " + s;
            }

            return s;
        }

        public void ComparePointsToDealer(Dealer dealer, bool gameEnded = false)
        {
            int playerPoints = GetPoints();
            int dealerPoints = dealer.GetPoints();
            int playerCardCount = this.Cards.Count;
            int dealerCardCount = dealer.Cards.Count;

            this.BetMultiplier = playerPoints == 21 && playerCardCount == 2 && !this.EvenMoney ? 1.5 : 1.0;

            if (playerPoints > 21)
            {
                this.HasWonRound = false;
                this.HasLostRound = true;
                this.LeaveBet = false;
            }
            else if (playerPoints <= 21 && playerPoints == dealerPoints)
            {
                if (playerPoints < 21 || playerCardCount == dealerCardCount || (playerCardCount > 2 && dealerCardCount > 2))
                {
                    this.HasWonRound = false;
                    this.HasLostRound = false;
                    this.LeaveBet = true;
                }
                else
                {
                    if (playerCardCount < dealerCardCount)
                    {
                        this.HasWonRound = true;
                        this.HasLostRound = false;
                        this.LeaveBet = false;
                    }
                    else
                    {
                        this.HasWonRound = false;
                        this.HasLostRound = true;
                        this.LeaveBet = false;
                    }
                }
            }
            else if (playerPoints <= 21 && playerPoints > dealerPoints)
            {
                this.HasWonRound = true;
                this.HasLostRound = false;
                this.LeaveBet = false;
            }
            else if (playerPoints <= 21 && dealerPoints > 21)
            {
                this.HasWonRound = true;
                this.HasLostRound = false;
                this.LeaveBet = false;
            }
            else
            {
                this.HasWonRound = false;
                this.HasLostRound = true;
                this.LeaveBet = false;
            }

            if (gameEnded)
            {
                this.PlayedRounds++;
                if (this.HasWonRound)
                {
                    this.WonRounds++;
                }
                if (this.HasLostRound)
                {
                    this.LostRounds++;
                }
            }
        }

        public void BetMoney(double amount)
        {
            this.BetAmount = amount;
        }

        public void DetermineWinnings(Dealer dealer)
        {
            if (this.Insured)
            {
                double insuranceAmount = this.BetAmount / 2;

                if(dealer.HasBlackJack)
                {
                    this.Money += insuranceAmount * 2;
                }
                else
                {
                    this.Money -= insuranceAmount;
                }
            }

            if (this.LeaveBet)
            {
                return;
            }
            else if (this.HasWonRound)
            {
                this.Money += this.BetAmount * this.BetMultiplier;
            }
            else if (this.HasLostRound)
            {
                this.Money -= this.BetAmount;
            }

            this.LeaveBet = false;

        }

        public override string ToString()
        {
            return this.Name;
        }


        public static bool TestDealerComparison(List<Card> playerCards, List<Card> dealerCards, (bool wonRound, bool lostRound, bool leaveBet) expected, int index = -1)
        {
            Player player = new Player();
            Dealer dealer = new Dealer();
            bool passed;

            player.Cards.AddRange(playerCards);
            dealer.Cards.AddRange(dealerCards);
            player.ComparePointsToDealer(dealer);

            if (index >= 0)
            {
                Console.Write($"Test {index}: ");
            }
            if (expected == (player.HasWonRound, player.HasLostRound, player.LeaveBet))
            {
                passed = true;
                Console.WriteLine("Passed!");
            }
            else
            {
                passed = false;
                Console.WriteLine("Failed!");
                Console.WriteLine($"Expected (wonRound, lostRound): {expected}");
                Console.WriteLine($"  Result (wonRound, lostRound): {(player.HasWonRound, player.HasLostRound, player.LeaveBet)}\n");
            }

            return passed;
        }
    }
}
