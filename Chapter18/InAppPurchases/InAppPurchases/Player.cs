using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneApp1
{
    public class Player
    {
        public Player()
        {
            CreateDummyPlayer();
        }
        public void CreateDummyPlayer()
        {
            Gold = 43;
            Items = new List<GameItem>()
                {
                    new GameItem()
                        {
                            Id= 1,
                            Name = "Bonus Map 1",
                            ProductId = "MapUnlock1",
                            ItemType = GameItemType.Map,
                            Purchased = false
                        },
                    new GameItem()
                        {
                            Id= 2,
                            Name = "Bonus Map 2",
                            ProductId = "MapUnlock2",
                            ItemType = GameItemType.Map,
                            Purchased = false
                        },
                    new GameItem()
                        {
                            Id= 3,
                            Name = "Special Item 1",
                            ProductId = "SpecialItem1",
                            ItemType = GameItemType.Special,
                            Purchased = false
                        },
                    new GameItem()
                        {
                            Id= 4,
                            Name = "Special Item 2",
                            ProductId = "SpecialItem2",
                            ItemType = GameItemType.Special,
                            Purchased = false
                        },
                };
        }
        public int Gold { get; set; }
        public List<GameItem> Items { get; set; }
    }
}
