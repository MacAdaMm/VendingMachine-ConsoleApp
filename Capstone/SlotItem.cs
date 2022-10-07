using System.Collections.Generic;
using System.Text;

namespace Capstone
{
    public class SlotItem
    {
        public const int MaxStock = 5;
        public string Location { get; }
        public string ProductName { get; }
        public decimal Price { get; }
        public string Type { get; }
        public string PurchaseSaying { get; }
        public int Stock { get; set; }
        public bool IsInStock { get { return Stock > 0; } }

        public SlotItem(string location, string name, decimal price, string type)
        {
            Location = location;
            ProductName = name;
            Price = price;
            Type = type.ToLower();
            Stock = MaxStock;

            switch (Type) // <- This isn't ideal but its a solid compromise to adding 4 more classes. It could have been an abstract base class, or we could have implemented an interface to do the same thing..
            {
                case "gum":
                    PurchaseSaying = "Chew Chew, Yum!";
                    break;
                case "candy":
                    PurchaseSaying = "Munch Munch, Yum!";
                    break;
                case "beverage":
                    PurchaseSaying = "Glug Glug, Yum!";
                    break;
                case "chips":
                    PurchaseSaying = "Crunch Crunch, Yum!";
                    break;
            }
        }

        public string Dispense()
        {
            if (!IsInStock)
            {
                throw new SlotItemOutOfStockException(this);
            }
            Stock--;
            return PurchaseSaying;
        }
    }
}

