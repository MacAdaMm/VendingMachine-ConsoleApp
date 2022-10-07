using System;
using System.Collections.Generic;
using System.Text;
using Capstone;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneTests
{
    [TestClass]
    public class MoneyTests
    {
        [DataTestMethod]
        [DataRow(1.16, "quarters: 4 dimes: 1 nickles: 1 pennies: 1")]
        [DataRow(2.49, "quarters: 9 dimes: 2 nickles: 0 pennies: 4")]
        [DataRow(0, "quarters: 0 dimes: 0 nickles: 0 pennies: 0")]
        public void DispenseChangeTest(double currentBalance, string expectedResult)
        {
            var transactionHandler = new TransactionHandler();
            transactionHandler.FeedMoney(new decimal(currentBalance)); // <- you cant use decmials in the DataRow attributes for some reason? so im just creating a new one here with a double.

            var change = transactionHandler.DispenseChange();

            Assert.AreEqual(0, transactionHandler.Balance);
            Assert.AreEqual(expectedResult, change);
        }

        [TestMethod]
        public void FeedMoneyTest()
        {
            var transactionHandler = new TransactionHandler();
            transactionHandler.FeedMoney(100M);

            Assert.AreEqual(100M, transactionHandler.Balance);

            transactionHandler.FeedMoney(10M);

            Assert.AreEqual(110M, transactionHandler.Balance);
        }

        [TestMethod]
        public void DispenseProductTest()
        {
            var transactionHandler = new TransactionHandler();
            transactionHandler.FeedMoney(1.75M);
            SlotItem mockItem = new SlotItem("A1", "Mock Item (Gum)", 1.75M, "Gum");

            transactionHandler.DispenseProduct(mockItem);

            Assert.AreEqual(0, transactionHandler.Balance);
        }
    }
}
