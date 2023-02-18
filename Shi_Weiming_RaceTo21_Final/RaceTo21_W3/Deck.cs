using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method

namespace RaceTo21
{
    public class Deck
    {
        public List<Card> cards = new List<Card>();//

        /* dectionary map card id to image filename*/
        public Dictionary<String, String> dictIDToFilename = new Dictionary<string, string>();

        //build the deck
        public Deck()
        {
            Console.WriteLine("*********** Building deck...");
            string[] suits = { "Spades", "Hearts", "Clubs", "Diamonds" };

            for (int cardVal = 1; cardVal <= 13; cardVal++)
            {
                foreach (string cardSuit in suits)
                {
                    string cardName;
                    string cardLongName;
                    string imageFileName;
                    //Set the name for each card
                    switch (cardVal)
                    {
                        case 1:
                            cardName = "A";
                            cardLongName = "Ace";
                            imageFileName = "A";
                            break;
                        case 11:
                            cardName = "J";
                            cardLongName = "Jack";
                            imageFileName = "J";
                            break;
                        case 12:
                            cardName = "Q";
                            cardLongName = "Queen";
                            imageFileName = "Q";
                            break;
                        case 13:
                            cardName = "K";
                            cardLongName = "King";
                            imageFileName = "K";
                            break;
                        default:
                            cardName = cardVal.ToString();
                            cardLongName = cardName;
                            imageFileName = cardVal.ToString();
                            if(imageFileName.Length < 2){
                                imageFileName = "0" + imageFileName;
                            }
                            break;
                    }
                    cards.Add(new Card(cardName + cardSuit.First<char>(), cardLongName + " of " + cardSuit));

                    imageFileName = "card_assets/" + "card_" + cardSuit.ToLower() + "_" + imageFileName + ".png";
                    //Console.WriteLine(imageFileName);
                    dictIDToFilename[cardName + cardSuit.First<char>()] = imageFileName;
                }
            }

            /*
            //this part is used to test someone got 21 points
            string cardSuit2 = "Spader";

            cards.Add(new Card("A" + cardSuit2.First<char>(), "Ace" + " of " + cardSuit2));
            cards.Add(new Card("A" + cardSuit2.First<char>(), "Ace" + " of " + cardSuit2));
            cards.Add(new Card("10" + cardSuit2.First<char>(), "Ten" + " of " + cardSuit2));

            cards.Add(new Card("A" + cardSuit2.First<char>(), "Ace" + " of " + cardSuit2));
            cards.Add(new Card("J" + cardSuit2.First<char>(), "Jack" + " of " + cardSuit2));
            cards.Add(new Card("A" + cardSuit2.First<char>(), "Ace" + " of " + cardSuit2));
            */
        }

        public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random();

            // one-line method that uses Linq:
            // cards = cards.OrderBy(a => rng.Next()).ToList();

            // multi-line method that uses Array notation on a list!
            // (this should be easier to understand)
            for (int i=0; i<cards.Count; i++)
            {
                Card tmp = cards[i];
                int swapindex = rng.Next(cards.Count);
                cards[i] = cards[swapindex];
                cards[swapindex] = tmp;
            }
        }

        /* Maybe we can make a variation on this that's more useful,
         * but at the moment it's just really to confirm that our 
         * shuffling method(s) worked! And normally we want our card 
         * table to do all of the displaying, don't we?!
         */

        public void ShowAllCards()
        {
            //for (int i=0; i<cards.Count; i++)
            //{
            //    Console.Write(i+":"+cards[i].displayName); // a list property can look like an Array!
            //    if (i < cards.Count -1)
            //    {
            //        Console.Write(" ");
            //    } else
            //    {
            //        Console.WriteLine("");
            //    }
            //}
        }

        public Card DealTopCard()//
        {
            Card card = cards[cards.Count - 1];//
            cards.RemoveAt(cards.Count - 1);
            // Console.WriteLine("I'm giving you " + card);
            return card;
        }
    }
}

