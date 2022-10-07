using System;

namespace Capstone
{
    public class SlotItemOutOfStockException : Exception
    {
        public SlotItemOutOfStockException(SlotItem slotItem) : base($"{slotItem.ProductName} at location {slotItem.Location} is out of stock.") {}
    }
}

