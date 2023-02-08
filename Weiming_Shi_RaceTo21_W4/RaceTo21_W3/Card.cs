using System;
namespace RaceTo21
{
    public class Card
    {
        public string id;
        public string displayName;

        public Card(string shorCardName, string longCardName)
        {
            id = shorCardName;
            displayName = longCardName;
        }
    }
}
