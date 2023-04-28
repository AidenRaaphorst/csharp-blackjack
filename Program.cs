namespace Blackjack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Player> players = new List<Player>();
            Blackjack game;

            Blackjack.PrintTitle();
            Console.WriteLine("Namen van de spelers (bijvoorbeeld: Mark, John, Amy).");
            Console.WriteLine("Laat leeg om de standaard namen te gebruiken.");
            Console.Write("\nNamen van de spelers: ");
            string names = Console.ReadLine();
            
            if(names == "")
            {
                int amountOfPlayers;
                Console.Write("\nHoeveel spelers zijn er? ");

                while(!int.TryParse(Console.ReadLine(), out amountOfPlayers) || amountOfPlayers < 1)
                {
                    Console.Write("Geen getal of te weinig, hoeveel spelers zijn er? ");
                }

                for (int i = 0; i < amountOfPlayers; i++)
                {
                    players.Add(new Player(money: 10));
                }
            }
            else
            {
                foreach (string name in names.Split(", "))
                {
                    players.Add(new Player(name: name, money: 10));
                }
            }

            game = new Blackjack(players);
            game.Start();
        }
    }
}