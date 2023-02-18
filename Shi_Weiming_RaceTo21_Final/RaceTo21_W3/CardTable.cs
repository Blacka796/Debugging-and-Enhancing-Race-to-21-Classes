using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class CardTable
    {
        public List<Card> cards = new List<Card>();

        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
        }

        /*get how many round does player want to play
         * Called by game object
         * game object provide play rounds
         * return rounds to the game object
         */
        public int GetPlayerAgreedUponRounds(int playerNum)
        {
            while(true)//as how many rounds does player want to play
            {
                Console.Write("What is the agreed upon rounds of player# " + playerNum + "? ");
                try{
                    string response = Console.ReadLine();
                    int rounds = int.Parse(response);
                    if(rounds >= 1){ //check if player enter the positive number
                        return rounds;
                    }
                }catch(Exception e){

                }
            }
        }

        /*ask player if they want a card or no
         * called by game object
         * return if player want more card or no
         */
        public bool OfferACard(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want a card? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else //check if player enter the correct character
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        /*show what card does each player have
         * get the card from the player hand
         */
        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                Console.Write(player.name + " has: ");
                int index = 0;
                foreach (Card card in player.cards)//
                {
                    String name = card.displayName;
                    String[] tokens = name.Split(' ');
                    String value = tokens[0];
                    switch(value){
                        case "1":
                        value = "Ace";
                        break;
                       case "2":
                        value = "Two";
                        break;
                       case "3":
                        value = "Three";
                        break;
                        case "4":
                        value = "Four";
                        break;
                       case "5":
                        value = "Five";
                        break;
                       case "6":
                        value = "Six";
                        break;
                       case "7":
                        value = "Seven";
                        break;
                       case "8":
                        value = "Eight";
                        break;
                        case "9":
                        value = "Nine";
                        break;
                       case "10":
                        value = "Ten";
                        break;
                       case "11":
                        value = "Jack";
                        break;
                       case "12":
                        value = "Queen";
                        break;
                       case "13":
                        value = "King";
                        break;
                    }
                    Console.Write(value + " " + tokens[1] + " " + tokens[2]);
                    if(index < player.cards.Count-1){
                        Console.Write(", ");
                    }
                    index++;
                }
                
                Console.Write("=" + player.score + "/21 "); //show the player's current score
                if (player.status != PlayerStatus.active)
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        /*get what card does player have and show them
         * called by game object
         * return value to the game object
         */
        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }

        /*get the winner from win lose check
         * called by win lose checking function
         * return value to the checking function
         */
        public void AnnounceWinner(Player player)
        {
            if (player != null)
            {
                Console.WriteLine(player.name + " wins!");
            }
            else
            {
                Console.WriteLine("Everyone busted!");
            }
            Console.Write("Press <Enter> to continue... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}