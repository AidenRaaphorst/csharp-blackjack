namespace Blackjack
{
    internal class Deck
    {
        private static readonly Card.Value[] _values = Card.GetAllValues();
        private static readonly Card.Symbol[] _symbols = Card.GetAllSymbols();
        private static readonly Random _random = new Random();
        private readonly List<Card> _cards = new List<Card>();

        public Deck()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    this._cards.Add(new Card(_values[j], _symbols[i]));
                }
            }
        }

        public void Shuffle(int repeatAmount = 1)
        {
            for (int i = 0; i < repeatAmount; i++)
            {
                int cardOneIndex = this._cards.Count;
                while (cardOneIndex > 1)
                {
                    cardOneIndex--;
                    int cardTwoIndex = _random.Next(cardOneIndex + 1);
                    (this._cards[cardOneIndex], this._cards[cardTwoIndex]) = (this._cards[cardTwoIndex], this._cards[cardOneIndex]);
                }
            }
        }

        public void Reset()
        {
            this._cards.Clear();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    this._cards.Add(new Card(_values[j], _symbols[i]));
                }
            }
        }

        public void RemoveCard(Card card) => this._cards.Remove(card);

        public Card GetCard()
        {
            Card card = this._cards[0];
            RemoveCard(card);
            return card;
        }

        public bool IsEmpty()
        {
            return _cards.Count == 0;
        }
    }
}
