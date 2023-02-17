using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace RootCoin
{
    class Program
    {
        static void Main(string[] args)
        {

            PrivateKey key1 = new PrivateKey();
            PublicKey wallet1 = key1.publicKey();

            PrivateKey key2 = new PrivateKey();
            PublicKey wallet2 = key2.publicKey();

            Blockchain rootcoin = new Blockchain(2, 100);

            Console.WriteLine("Start the Miner");
            rootcoin.MinePendingTransactions(wallet1);
            Console.WriteLine("\nBalance of Wallet1 is $" + rootcoin.GetBalanceofWallet(wallet1).ToString());

            Transaction tx1 = new Transaction(wallet1, wallet2, 10);
            tx1.SignTransaction(key1);
            rootcoin.addPendingTransaction(tx1);
            Console.WriteLine("Start the Miner");
            rootcoin.MinePendingTransactions(wallet2);
            Console.WriteLine("\nBalance of Wallet1 is $" + rootcoin.GetBalanceofWallet(wallet1).ToString());
            Console.WriteLine("\nBalance of Wallet2 is $" + rootcoin.GetBalanceofWallet(wallet2).ToString());





            string blockJSON = JsonConvert.SerializeObject(rootcoin, Formatting.Indented);
            Console.WriteLine(blockJSON);

            if (rootcoin.IsChainValid())
            {  
                Console.WriteLine("BlockChain is valid!"); 
            }
            else
            {
                Console.WriteLine("BlockChain is invalid!");

            }
        }
    }
}
