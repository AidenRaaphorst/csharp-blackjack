namespace Blackjack
{
    internal class Dealer : Player
    {
        private readonly Deck _deck;

        public Dealer() : base(name: "Bank", isDealer: true)
        {
            _deck = new Deck();
            _deck.Shuffle(3);
        }

        public Card GiveCard()
        {
            return this._deck.GetCard();
        }

        public void GetNewDeck()
        {
            _deck.Reset();
            _deck.Shuffle(3);
        }

        public string GetCardsAndPoints(bool color = false)
        {
            int c = this.Cards.Count;
            string s = string.Join(", ", this.Cards) + (c == 1 ? $", ?" : "") + $" ({GetPoints()} punten)";

            if (GetPoints() > 21)
            {
                if (color)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                s = $"{this.Name} is dood!: " + s;
            }
            else
            {
                s = $"{this.Name}: " + s;
            }

            return s;
        }
    }
}
