using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneApp1
{
    public class GameItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public GameItemType ItemType { get; set; }
        public string ProductId { get; set; }
        public bool Purchased { get; set; }

    }

    public enum GameItemType
    {
        Map,
        Special
    }
}
