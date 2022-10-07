using Capstone;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneTests
{
    [TestClass]
    public class SlotItemTests
    {
        [DataTestMethod]
        [DataRow("Gum", "Chew Chew, Yum!")]
        [DataRow("guM", "Chew Chew, Yum!")]
        public void DispenseProductTest(string type, string expectedResult)
        {
            SlotItem slotItem = new SlotItem("", "", 0, type);
            for (int i = 0; i < 4; i++)
            {
                slotItem.Dispense();
            }

            Assert.AreEqual(expectedResult, slotItem.Dispense());
            Assert.ThrowsException<SlotItemOutOfStockException>(() => slotItem.Dispense());
        }
    }
}
