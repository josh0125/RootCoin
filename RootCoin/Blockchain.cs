﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace RootCoin
{
    class Blockchain
    {
        public List<Block> Chain { get; set; }

        public int Difficulty { get; set; }

        public List<Transaction> pendingTransactions { get; set; }
        public decimal MiningReward { get; set; }
        public Blockchain(int difficulty, decimal miningReward)
        {
            this.Chain = new List<Block>();
            this.Chain.Add(CreateGenisisBlock());
            this.Difficulty = difficulty;
            this.MiningReward = miningReward;
            this.pendingTransactions = new List<Transaction>();
        }

        public Block CreateGenisisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyyMMddHHmmssffff"), new List<Transaction>());
        }

        public Block GetLastestBlock()
        {
            return this.Chain.Last();
        }

        public void AddBlock(Block newBlock)
        {
            newBlock.PreviousHash = this.GetLastestBlock().Hash;
            newBlock.Hash = newBlock.CalculateHash();
            this.Chain.Add(newBlock);
        }

        public void addPendingTransaction(Transaction transaction)
        {
            if (transaction.FromAddress is null || transaction.ToAddress is null)
            {
                throw new Exception("Transactions must include a to and from address");
            }

            if (transaction.Amount > this.GetBalanceofWallet(transaction.FromAddress))
            {
                throw new Exception("There msut be sufficient funds in your wallet");
            }

            if (transaction.IsValid() == false)
            {
                throw new Exception("Cannot add an invalid transaction to a block");
            }

            this.pendingTransactions.Add(transaction);
        }

        public decimal GetBalanceofWallet(PublicKey address)
        {
            decimal balance = 0;

            string addressDER = BitConverter.ToString(address.toDer()).Replace("-", "");

            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Tranactions)
                {
                    if (!(transaction.FromAddress is null))
                    {
                        string fromDER = BitConverter.ToString(transaction.FromAddress.toDer()).Replace("-", "");

                        if (fromDER == addressDER)
                        {
                            balance -= transaction.Amount;
                        }
                    }
                    
                    string toDER = BitConverter.ToString(transaction.ToAddress.toDer()).Replace("-", "");

                    if(toDER == addressDER)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }

        public void MinePendingTransactions(PublicKey miningRewardWallet)
        {
            Transaction rewardTx = new Transaction(null, miningRewardWallet, MiningReward);
            this.pendingTransactions.Add(rewardTx);

            Block newBlock = new Block(GetLastestBlock().Index + 1, DateTime.Now.ToString("yyyyMMddHHmmssffff"), this.pendingTransactions, GetLastestBlock().Hash);
            newBlock.Mine(this.Difficulty);

            Console.WriteLine("Block Successfully Mined");
            this.Chain.Add(newBlock);
            this.pendingTransactions = new List<Transaction>();
        }
        public bool IsChainValid()
        {
            for (int i = 1; i < this.Chain.Count; i++)
            {
                Block currentBlock = this.Chain[i];
                Block previousBlock = this.Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }


            }

            return true;
        }
    }
}