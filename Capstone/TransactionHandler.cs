using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Capstone
{
    public class TransactionHandler
    {
        public decimal Balance { get; private set; }

        private StreamWriter logWriter;
        
        public TransactionHandler() { }
        public TransactionHandler(string logFilePath)
        {
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }

            logWriter = new StreamWriter(Path.Combine(logFilePath, "Log.txt"));
            logWriter.AutoFlush = true;
        }
        ~TransactionHandler()
        {
            logWriter?.Close();
        }

        public void FeedMoney(decimal value)
        {
            LogTransaction($"FEED MONEY {Balance:C2} {Balance + value:C2}");
            Balance += value;
        }

        public string DispenseProduct(SlotItem product)
        {
            // throws exception if out of stock,
            string result = product.Dispense();
            Balance -= product.Price;
            LogTransaction($"{product.ProductName} {product.Location} {product.Price:C2} {Balance:C2}");
            return result;
        }

        public string DispenseChange()
        {
            decimal balanceBefore = Balance;
            //Selecting "(3) Finish Transaction" allows the customer to complete the transaction and receive any remaining change.
            //The machine returns the customer's money using nickels, dimes, and quarters (using the smallest amount of coins possible).
            //The machine's current balance updates to $0 remaining.

            int quarters = (int)(Balance / 0.25M);
            Balance -= quarters * 0.25M;

            int dimes = (int)(Balance / 0.1M);
            Balance -= dimes * 0.1M;

            int nickles = (int)(Balance / 0.05M);
            Balance -= nickles * 0.05M;

            int pennies = (int)(Balance / 0.01M);
            Balance -= pennies * 0.01M;

            LogTransaction($"GIVE CHANGE {balanceBefore:C2} {Balance:C2}");
            return $"quarters: {quarters} dimes: {dimes} nickles: {nickles} pennies: {pennies}";
        }

        private void LogTransaction(string message)
        {
            logWriter?.WriteLine($"{DateTime.Now} {message}");
        }
    }
}
