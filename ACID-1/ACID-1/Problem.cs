using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ACID_1
{
    /// <summary>
    /// Shows how PCC and OCC works and times it.
    /// NOTE: You must change the values for "theIsolationLevel" and 
    /// in- or outcomment "#define USE_OCC" in Db.cs
    /// </summary>

    public class Problem
    {
        private System.Data.IsolationLevel theIsolationLevel=System.Data.IsolationLevel.ReadUncommitted;
        private int customers = 2;

        /// <summary>
        /// Transaction A
        /// A customer coming into a shop buying X items.
        /// At the register the customer randomly decides not to buy
        /// the items anyway and leaves emptyhanded.
        /// </summary>
        public void TransactionA(string customerName, int loops)
        {
            Db myDb = new Db(false);
            Random rnd = new Random();
            int totalBought = 0;

            for (int i = 0; i < loops; i++)
            {
                Console.WriteLine(customerName + " going to shop #" + i);
                while (true)
                {
                    // Begin the composit transaction
                    SqlTransaction TA = myDb.BeginTransaction(theIsolationLevel);
                    // The customer decides on how many items to buy
                    int toBuy = rnd.Next(1, 10);
                    // Tap-dance while waiting for the cleark to fill the shelf
                    while (myDb.Read() < toBuy) {
                        Console.WriteLine(customerName + " waiting for refill");
                        Thread.Sleep(10);
                    }
                    int onShelf = myDb.Read();
                    // The customer contemplates how many items to buy.... Hmmmmmmmm
                    int pondering = rnd.Next(1, 200);
                    Thread.Sleep(pondering);
                    // And picks them from the shelf
                    if (! myDb.Write(onShelf - toBuy) )
                    {
                        // We are the victim of a deadlock
                        Console.WriteLine(customerName + " DL " + i);
                        myDb.RollBack(TA);
                        continue;
                    }
                    // Then the customer makes his way to the register and stand in line
                    int timeWastedToTheRegister = rnd.Next(1, 200);
                    Thread.Sleep(timeWastedToTheRegister);
                    // At the teller there is now a 20% chance the customer bails on the purchase
                    int dice = rnd.Next(1, 5);
                    if (dice == 1)
                    {
                        // Bailing - No Sale
                        myDb.RollBack(TA);
                    }
                    else
                    {
                        // Customer buys the item(s)
                        myDb.Commit(TA);
                        totalBought += toBuy;
                        Console.WriteLine(customerName + "bought " + toBuy);
                    }
                    break;
                }
            }
            myDb.Close();
            // Signal TransactionB to end
            customers--;
            Console.WriteLine(customerName + ": Total Bought = " + totalBought);
        }

        /// <summary>
        /// Transaction B
        /// A store clerk refilling the shelf as fast as he can
        /// </summary>
        public void TransactionB()
        {
            Db myDb = new Db(false);
            int totalRefilled = 0;
            while (customers > 0)
            {
                // Begin the composit transaction
                SqlTransaction TB = myDb.BeginTransaction(theIsolationLevel);
                int onShelf = myDb.Read();
                if (onShelf == 50)
                {
                    myDb.Commit(TB);
                    continue;
                }
                // we fill the shelf
                 if (! myDb.Write(50) )
                {
                    // We are the victim of a deadlock
                    myDb.RollBack(TB);
                    continue;
                }
                totalRefilled += (50 - onShelf);
                Console.WriteLine("Shelf added " + (50 - onShelf));
                myDb.Commit(TB);
            }
            Console.WriteLine("Total Refilled = " + totalRefilled);
            Console.WriteLine("Total on shelf = " + myDb.Read());
            myDb.Close();
        }

        /// <summary>
        /// The "Consumer" is a store customer who wants to buy a certain number of items.
        /// But sometimes he/she drops the purchase, maybe becase it's to expensive.
        /// Who knows?
        /// </summary>
        public Problem()
        {
            var watch = new System.Diagnostics.Stopwatch();
            Db initDB = new Db(true);
            initDB.Close();
            // Ready TransactionA, customer 1
            Thread Customer1 = new Thread(() => TransactionA("Customer1", 10));
            // Ready TransactionA, customer 2
            Thread Customer2 = new Thread(() => TransactionA("Customer2", 10));
            // Ready TransactionB
            ThreadStart TransactionBRef = new ThreadStart(TransactionB);
            Console.WriteLine("In Main: Creating the TransactionB thread");
            Thread TransactionBThread = new Thread(TransactionBRef);
            // Let the fun begin
            watch.Start();
            Customer1.Start();
            Customer2.Start();
            TransactionBThread.Start();
            // Now wait for the threads to end
            Customer1.Join();
            Customer2.Join();
            TransactionBThread.Join();
            watch.Stop();
            Console.WriteLine($"Done in {watch.ElapsedMilliseconds} ms");
        }
    }
}
