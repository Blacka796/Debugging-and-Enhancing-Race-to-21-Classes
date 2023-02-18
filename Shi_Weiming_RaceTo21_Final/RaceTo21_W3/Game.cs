using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public Task nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true
        private int agreedUponRounds = int.MaxValue; //set how many rounds they want to play
        int rounds = 0; //set an int value as round

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = Task.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */
        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            if (nextTask == Task.GetNumberOfPlayers)
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = Task.GetNames;
            }
            else if (nextTask == Task.GetNames)
            {
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                    int agreedRounds = cardTable.GetPlayerAgreedUponRounds(count);
                    if (agreedRounds < agreedUponRounds)
                    {
                        agreedUponRounds = agreedRounds;
                    }//ask player for how many rounds, and if player enter different rounds, it will set the smallest number as round
                }
                nextTask = Task.IntroducePlayers;
            }
            else if (nextTask == Task.IntroducePlayers)
            {
                cardTable.ShowPlayers(players);
                nextTask = Task.PlayerTurn;
            }
            else if (nextTask == Task.PlayerTurn)
            {
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                    if (cardTable.OfferACard(player))
                    {
                        Card card = deck.DealTopCard();//
                        player.cards.Add(card);
                        player.score = ScoreHand(player);
                        if (player.score > 21)//check if player score bigger than 21, it will bust
                        {
                            player.status = PlayerStatus.bust;
                        }
                        else if (player.score == 21)//check if player score equal to 21, will win
                        {
                            player.status = PlayerStatus.win;
                        }
                    }
                    else//else player choose to stay will not win or loose
                    {
                        player.status = PlayerStatus.stay;
                    }
                }
                cardTable.ShowHand(player);
                nextTask = Task.CheckForEnd;
            }
            else if (nextTask == Task.CheckForEnd)
            {
                //if no active players, someone got 21 or all but one bust
                //there should be a winner
                if (!CheckActivePlayers() || check21Player() || checkOnlyOneBurstPlayer())
                {
                    //get winner
                    Player winner = DoFinalScoring();
                    bool nobodyTakeCards = true; // true if no player take cards
                    foreach (var player in players)
                    {
                        if (player.score > 0)
                        {
                            nobodyTakeCards = false;
                        }

                        if (player == winner) //if player is winner, add points to the winner
                        {
                            //add points to winner
                            winner.points += winner.score;
                        }
                        else //if player bust, decrease the point
                        {
                            if (player.status == PlayerStatus.bust)
                            {
                                player.points -= player.score - 21;
                            }
                        }
                    }

                    // if no player take cards, show message
                    if (nobodyTakeCards)
                    {
                        if (players.Count == 1) //if there is only 1 player left, show the winner
                        {
                            cardTable.AnnounceWinner(players[0]);
                        }
                        else //if no player take cards, show this message
                        {
                            Console.WriteLine("No player take cards!");
                        }
                    }
                    else //else show the winner
                    {
                        cardTable.AnnounceWinner(winner);
                    }

                    rounds++; //count this as 1 round after game
                    if (rounds >= agreedUponRounds)
                    {
                        //rounds reach the maximum number
                        Console.WriteLine("Reach agreed-upon number of rounds!");
                        nextTask = Task.GameOver;
                    }
                    else
                    {

                        //ask players if they want to play again, and save them to nextRoundPlayers
                        List<Player> nextRoundPlayers = new List<Player>();
                        foreach (var player in players)
                        {
                            Console.Write(player.name + ", continue to play?(Y/N): ");
                            string option = Console.ReadLine().ToUpper();
                            if (option.Equals("Y"))
                            {
                                nextRoundPlayers.Add(player);
                            }
                        }

                        //if at least one player want to play, shuffle players and create a new deck.
                        if (nextRoundPlayers.Count > 0)
                        {
                            deck = new Deck();
                            deck.Shuffle();
                            Shuffle(nextRoundPlayers);

                            if (nextRoundPlayers.Count == 1)
                            {
                                //only one player in next round
                                players = nextRoundPlayers;
                                currentPlayer = 0;
                                players[0].status = PlayerStatus.win;
                                nextTask = Task.CheckForEnd;
                            }
                            else if (winner == null)
                            {
                                // no winner in last round, no need to move winner to last
                                players = nextRoundPlayers;
                                currentPlayer = 0;
                                nextTask = Task.PlayerTurn;
                            }
                            else
                            {
                                //move winner to last position
                                nextRoundPlayers.Remove(winner);
                                nextRoundPlayers.Add(winner);
                                players = nextRoundPlayers;
                                currentPlayer = 0;
                                nextTask = Task.PlayerTurn;
                            }
                        }
                        else
                        {
                            nextTask = Task.GameOver;
                        }
                    }
                }
                else
                {
                    //to next player
                    currentPlayer++;
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player...
                    }
                    nextTask = Task.PlayerTurn;
                }
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Task.GameOver;
            }
        }

        /* shuffle the list of players*/
        public void Shuffle(List<Player> players)
        {
            Random rng = new Random();

            for (int i = 0; i < players.Count; i++)
            {
                Player tmp = players[i];
                int swapindex = rng.Next(players.Count);
                players[i] = players[swapindex];
                players[swapindex] = tmp;
            }
            for (int i = 0; i < players.Count; i++)
            {
                players[i].reset();
            }

        }

        /* return true if someone got 21*/
        public bool check21Player()
        {
            foreach (var player in players)
            {
                if (player.score == 21)
                {
                    player.status = PlayerStatus.win;
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        /* return true if all but one player bust*/
        public bool checkOnlyOneBurstPlayer()
        {
            Player onlyNonBurstPlayer = null; // to save the only one non-bursted player
            int numBursts = 0; //to count how many players burst
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.bust) //count if any players are bust
                {
                    numBursts++;
                }
                else
                {
                    onlyNonBurstPlayer = player;
                }
            }

            //if only one player doesn't burst, set it's status as win
            if (numBursts == players.Count - 1 && onlyNonBurstPlayer != null)
            {
                onlyNonBurstPlayer.status = PlayerStatus.win;
                return true;
            }
            return false;
        }

        /*count the score for each player */
        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active) //this part is for cheat mod and test each function
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else // set the score for each card
            {
                foreach (Card card in player.cards)//
                {
                    string cardName = card.id;
                    string faceValue = cardName.Remove(cardName.Length - 1);//
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            }
            return score;
        }

        /*return true if no active players */
        public bool CheckActivePlayers()
        {
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        /*count the final score and transer value to win lose checking function*/
        public Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                // if busted don't bother checking!
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
            return null; // everyone must have busted because nobody won!
        }
    }
}
