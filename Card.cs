namespace Blackjack
{
    internal class Card
    {
        private static readonly Random _random = new Random();
        private readonly Value _value;
        private readonly Symbol _symbol;
        private readonly Color _color;
        private int _intValue;

        public Card(Value value, Symbol symbol)
        {
            this._value = value;
            this._intValue = GetIntValue();
            this._symbol = symbol;
            this._color = symbol == Symbol.Hearts || symbol == Symbol.Diamonds ? Color.Red : Color.Black;
        }

        public static Card GetRandomCard()
        {
            Array values = Enum.GetValues(typeof(Value));
            Array symbols = Enum.GetValues(typeof(Symbol));
            Value value = (Value)values.GetValue(_random.Next(values.Length));
            Symbol symbol = (Symbol)symbols.GetValue(_random.Next(symbols.Length));

            return new Card(value, symbol);
        }

        private int SetAceValue(int totalPoints) => this._intValue = totalPoints > 10 ? 1 : 11;

        public int GetIntValue(int totalPoints = 0)
        {
            return this._value switch
            {
                Value.Ace => SetAceValue(totalPoints),
                Value.Two => 2,
                Value.Three => 3,
                Value.Four => 4,
                Value.Five => 5,
                Value.Six => 6,
                Value.Seven => 7,
                Value.Eight => 8,
                Value.Nine => 9,
                Value.Ten or Value.Jack or Value.Queen or Value.King => 10,
                _ => 1,
            };
        }

        public string ValueToString()
        {
            return this._value switch
            {
                Value.Ace => "A",
                Value.Two => "2",
                Value.Three => "3",
                Value.Four => "4",
                Value.Five => "5",
                Value.Six => "6",
                Value.Seven => "7",
                Value.Eight => "8",
                Value.Nine => "9",
                Value.Ten => "10",
                Value.Jack => "J",
                Value.Queen => "Q",
                Value.King => "K",
                _ => "",
            };
        }

        public string SymbolToString()
        {
            return this._symbol switch
            {
                Symbol.Clubs => "♣",
                Symbol.Diamonds => "♦",
                Symbol.Hearts => "♥",
                Symbol.Spades => "♠",
                _ => "",
            };
        }

        public static Card GetCardByIntValue(int intValue, Symbol symbol = Symbol.Clubs)
        {
            Value value = intValue switch
            {
                11 or 1 => Value.Ace,
                2 => Value.Two,
                3 => Value.Three,
                4 => Value.Four,
                5 => Value.Five,
                6 => Value.Six,
                7 => Value.Seven,
                8 => Value.Eight,
                9 => Value.Nine,
                10 => Value.Ten,
                _ => Value.Ace,
            };
            return new Card(value, symbol);
        }

        public static List<Card> MockCards(int totalPoints, int cardCount, bool test = false)
        {
            List<Card> cards = new List<Card>();
            int pointsPerCard = totalPoints / cardCount;
            int remainder = totalPoints % cardCount;
            int intValue;
            int outputPoints = 0;
            int outputCardCount = 0;

            for(int i = 0; i < cardCount; i++)
            {
                intValue = pointsPerCard;

                if(i + 2 == cardCount && intValue + remainder > 11)
                {
                    intValue += remainder / 2;
                }
                else if(i + 1 == cardCount)
                {
                    if(intValue + remainder > 11)
                    {
                        intValue += remainder / 2;
                    }
                    else
                    {
                        intValue += remainder;
                    }
                }

                intValue = Math.Max(0, Math.Min(intValue, 11));

                if(intValue > 0)
                {
                    Card card = GetCardByIntValue(intValue);
                    outputPoints += intValue;
                    outputCardCount++;
                    cards.Add(card);
                }
            }

            if(test)
            {
                if(outputPoints == totalPoints && outputCardCount == cardCount)
                {
                    Console.WriteLine("Passed!");
                }
                else
                {
                    Console.WriteLine("Failed!");
                    Console.WriteLine("         Input points: " + totalPoints.ToString().PadLeft(totalPoints.ToString().Length));
                    Console.WriteLine("        Output points: " + outputPoints.ToString().PadLeft(totalPoints.ToString().Length));
                    Console.WriteLine("     Input card count: " + cardCount.ToString().PadLeft(totalPoints.ToString().Length));
                    Console.WriteLine("    Output card count: " + outputCardCount.ToString().PadLeft(totalPoints.ToString().Length));
                    Console.WriteLine("      Points per card: " + pointsPerCard.ToString().PadLeft(totalPoints.ToString().Length));
                    Console.WriteLine("Points/card remainder: " + remainder.ToString().PadLeft(totalPoints.ToString().Length));
                }
            }

            return cards;
        }
        
        public override string ToString()
        {
            return ValueToString() + SymbolToString();
        }

        public static Value[] GetAllValues() => (Value[]) Enum.GetValues(typeof(Value));

        public static Symbol[] GetAllSymbols() => (Symbol[])Enum.GetValues(typeof(Symbol));

        public enum Value
        {
            Ace,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King
        }

        public enum Symbol
        {
            Clubs,
            Diamonds,
            Hearts,
            Spades
        }

        public enum Color
        {
            Red,
            Black
        }
    }
}
