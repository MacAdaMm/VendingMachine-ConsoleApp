using System;
using System.Collections.Generic;
using System.IO;
namespace Capstone
{
    class VendingMachine
    {
        private static string logFilePath = Path.Combine(Path.GetTempPath(), "VendingMachine");
        private static string salesReportPath = Path.Join(logFilePath, "SalesReports");

        // store products by slot location.
        private static Dictionary<string, SlotItem> productLookup = new Dictionary<string, SlotItem>();

        // handles all transactions and their logging.
        private static TransactionHandler transactionHandler = new TransactionHandler(logFilePath);

        // controls the lifetime of our application.
        private static bool IsRunning = true;

        // Controls what menu will be written to the console each loop. defaults to MainMenu.
        private static Action CurrentMenu = MainMenu;

        static void Main(string[] args)
        {
            // Parse out all products in vendingMachine.csv and create an instance of each product stored by the Slot Location (Ex. "A1") in the productLookup Dictionary.
            PopulateProductLookup();

            // run the program until the user chooses to exit.
            while (IsRunning)
            {
                CurrentMenu.Invoke();
            }
        }

        /* Initialization */
        // Prompts the user for a input file to stock the vending machine.
        // Parse out all products in vendingMachine.csv and create an instance of each product stored by the Slot Location (Ex. "A1") in the productLookup Dictionary.
        private static void PopulateProductLookup()
        {
            string inputFilePath = "";
            while (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Please enter the vending machine stock input file path.");
                inputFilePath = Console.ReadLine();
            }

            // The vending machine reads its inventory from an input file when the vending machine starts.
            try
            {
                // Parse out all products in vendingMachine.csv and create an instance of each product stored by the Slot Location (Ex. "A1") in the ItemLocation Dictionary.
                using (StreamReader sr = new StreamReader(inputFilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] lineArray = line.Split("|");

                        // Add our product to the itemLocation using its Slot Location as the key for easy lookup when the user want to select that item for purchase.
                        // Every product is initially stocked to the maximum amount.
                        productLookup[lineArray[0]] = new SlotItem(lineArray[0], lineArray[1], decimal.Parse(lineArray[2]), lineArray[3]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /* Menus */
        private static void MainMenu()
        {
            Console.WriteLine("(1) Display Vending Machine Items");
            Console.WriteLine("(2) Purchase");
            Console.WriteLine("(3) Exit");

            int userChoice = PromptUserForMenuItem(numberOfChoices: 4);
            
            if (userChoice == 1)
            {
                DisplayVendingMachineItems();
            }
            else if (userChoice == 2)
            {
                // goto Purchase menu.
                CurrentMenu = PurchaseMenu;
            }
            else if (userChoice == 3)
            {
                ExitProgram();
            }
            else if (userChoice == 4)
            {
                GenerateSalesReport();
            }
        }
        private static void PurchaseMenu()
        {
            Console.WriteLine($"Current Money Provided: {transactionHandler.Balance:C2}");
            Console.WriteLine();
            Console.WriteLine("(1) Feed Money");
            Console.WriteLine("(2) Select Product");
            Console.WriteLine("(3) Finish Transaction");

            int userChoice = PromptUserForMenuItem(numberOfChoices: 3);

            if (userChoice == 1)
            {
                FeedMoney();
            }
            else if (userChoice == 2)
            {
                // Because we dont want this menu to loop like the others we dont set CurrentMenu = SelectItemMenu
                SelectProductMenu(); 
                // After the machine dispenses the product return the customer to the Purchase menu
            }
            else if (userChoice == 3)
            {
                /*
                 The machine returns the customer's money using nickels, dimes, and quarters (using the smallest amount of coins possible). <- DONE
                 The machine's current balance updates to $0 remaining. <- DONE
                 After completing their purchase, the user returns to the "Main" menu to continue using the vending machine. <- TODO
                 */
                Console.WriteLine(transactionHandler.DispenseChange());
                CurrentMenu = MainMenu;
            }
        }
        private static void SelectProductMenu()
        {
            /*
                    Show the list of products available and allow the customer to enter a code to select an item.
                        If the product code doesn't exist, the vending machine informs the customer and returns them to the Purchase menu.
                        If a product is currently sold out, the vending machine informs the customer and returns them to the Purchase menu.
                        If a customer selects a valid product, it's dispensed to the customer.
                        Dispensing an item prints the item name, cost, and the money remaining. Dispensing also returns its purchaseSaying.
            */

            DisplayVendingMachineItems();

            string userInput = Console.ReadLine().ToUpper();
            if (!productLookup.ContainsKey(userInput))
            {
                Console.WriteLine("product code doesn't exist.\n");
            }
            else if (!productLookup[userInput].IsInStock)
            {
                Console.WriteLine("product is currently sold out.\n");
            }
            else if(productLookup[userInput].Price > transactionHandler.Balance)
            {
                Console.WriteLine("Balance to low to make transaction.\n");
            }
            else
            {
                Console.WriteLine("Dispensing...\n");
                Console.WriteLine(transactionHandler.DispenseProduct(productLookup[userInput]));
            }
        }

        /* Menu Actions */
        private static int PromptUserForMenuItem(int numberOfChoices)
        {
            int userChoiceAsInt;
            while (!int.TryParse(Console.ReadLine(), out userChoiceAsInt) && userChoiceAsInt > 0 && userChoiceAsInt <= numberOfChoices)
            {
                Console.WriteLine("Please enter a valid menu item number.");
            }
            return userChoiceAsInt;
        }
        private static void DisplayVendingMachineItems()
        {
            foreach (var item in productLookup)
            {
                string stockString = item.Value.IsInStock ? item.Value.Stock.ToString() : "Out of Stock";
                Console.WriteLine($"{item.Key} {item.Value.ProductName} {stockString} {item.Value.Price:C2}");
            }
            Console.WriteLine();
        }
        private static void FeedMoney()
        {
            Console.WriteLine("Please insert bills only!\n");
            Console.Write("Amount: ");
            decimal userMoneyAsDecimal;
            while (!decimal.TryParse(Console.ReadLine(), out userMoneyAsDecimal))
            {
                Console.WriteLine("Please enter a valid amount.\n");
            }
            transactionHandler.FeedMoney(userMoneyAsDecimal);

        }
        private static void GenerateSalesReport()
        {
            if (!Directory.Exists(salesReportPath))
            {
                Directory.CreateDirectory(salesReportPath);
            }

            try
            {
                string dateFormated = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(':', '_');
                using (StreamWriter sw = new StreamWriter(Path.Combine(salesReportPath, $"{dateFormated}_SalesReport.txt")))
                {
                    decimal totalSalesValue = 0;
                    foreach(SlotItem item in productLookup.Values)
                    {
                        int numberOfSales = SlotItem.MaxStock - item.Stock;
                        totalSalesValue += numberOfSales * item.Price;
                        sw.WriteLine($"{item.ProductName}|{numberOfSales}");
                    }
                    sw.WriteLine($"\n**TOTAL SALES** {totalSalesValue:C2}");
                }
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        private static void ExitProgram()
        {
            IsRunning = false;
        }
    }
}
