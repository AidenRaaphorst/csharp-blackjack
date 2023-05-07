namespace Blackjack
{
    internal class Testing
    {

        public static bool test(int playerPoints, int dealerPoints, int playerCardCount, int dealerCardCount, bool expected, int i = -1)
        {
            bool result = playerPoints <= 21
                            && playerPoints == dealerPoints
                            && (playerCardCount == dealerCardCount)
                            ;

            if (i >= 0)
            {
                Console.Write($"{i}: ");
            }

            if (result == expected)
            {
                Console.WriteLine("PASSED!");
            }
            else
            {
                Console.WriteLine("FAILED!");
                Console.WriteLine("Expected: " + expected);
                Console.WriteLine("  Result: " + result);
            }

            return result;
        }

        public static void TestDealerComparison()
        {
            List<(List<Card>, List<Card>, (bool, bool, bool))> tests = new List<(List<Card>, List<Card>, (bool, bool, bool))>
            {
                (Card.MockCards(21, 2), Card.MockCards(21, 2), (false, false, true)),
                (Card.MockCards(21, 2), Card.MockCards(21, 3), (true, false, false)),
                (Card.MockCards(21, 3), Card.MockCards(21, 2), (false, true, false)),

                (Card.MockCards(21, 4), Card.MockCards(21, 4), (false, false, true)),
                (Card.MockCards(21, 4), Card.MockCards(21, 5), (false, false, true)),
                (Card.MockCards(21, 5), Card.MockCards(21, 4), (false, false, true)),

                (Card.MockCards(18, 2), Card.MockCards(18, 2), (false, false, true)),
                (Card.MockCards(18, 2), Card.MockCards(18, 3), (false, false, true)),
                (Card.MockCards(18, 3), Card.MockCards(18, 2), (false, false, true)),

                (Card.MockCards(18, 4), Card.MockCards(18, 4), (false, false, true)),
                (Card.MockCards(18, 4), Card.MockCards(18, 5), (false, false, true)),
                (Card.MockCards(18, 5), Card.MockCards(18, 4), (false, false, true)),

                (Card.MockCards(18, 4), Card.MockCards(21, 5), (false, true, false)),
                (Card.MockCards(22, 4), Card.MockCards(21, 5), (false, true, false)),

                (Card.MockCards(22, 4), Card.MockCards(22, 5), (false, true, false)),
                (Card.MockCards(22, 4), Card.MockCards(23, 5), (false, true, false)),
                (Card.MockCards(23, 4), Card.MockCards(22, 5), (false, true, false)),

                (Card.MockCards(21, 4), Card.MockCards(22, 5), (true, false, false)),
                (Card.MockCards(22, 4), Card.MockCards(21, 5), (false, true, false)),

                (Card.MockCards(18, 4), Card.MockCards(17, 5), (true, false, false)),
                (Card.MockCards(12, 4), Card.MockCards(17, 5), (false, true, false))
            };

            for (int i = 0; i < tests.Count; i++)
            {
                (List<Card> playerCards, List<Card> dealerCards, (bool, bool, bool) expected) = tests[i];
                Player.TestDealerComparison(playerCards, dealerCards, expected, i);
            }
        }
    }
}
