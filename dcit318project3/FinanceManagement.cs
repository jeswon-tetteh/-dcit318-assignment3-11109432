using System;
using System.Collections.Generic;

record Transaction(int Id, DateTime Date, decimal Amount, string Category);

interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing bank transfer of {transaction.Amount} for {transaction.Category}");
    }
}

class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing mobile money transfer of {transaction.Amount} for {transaction.Category}");
    }
}

class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing crypto wallet transfer of {transaction.Amount} for {transaction.Category}");
    }
}

class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Updated balance: {Balance}");
        }
    }
}

class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        SavingsAccount account = new SavingsAccount("123456789", 1000m);

        Transaction transaction1 = new Transaction(1, DateTime.Now, 200m, "Groceries");
        Transaction transaction2 = new Transaction(2, DateTime.Now, 150m, "Utilities");
        Transaction transaction3 = new Transaction(3, DateTime.Now, 300m, "Entertainment");

        ITransactionProcessor mobileProcessor = new MobileMoneyProcessor();
        ITransactionProcessor bankProcessor = new BankTransferProcessor();
        ITransactionProcessor cryptoProcessor = new CryptoWalletProcessor();

        mobileProcessor.Process(transaction1);
        account.ApplyTransaction(transaction1);
        _transactions.Add(transaction1);

        bankProcessor.Process(transaction2);
        account.ApplyTransaction(transaction2);
        _transactions.Add(transaction2);

        cryptoProcessor.Process(transaction3);
        account.ApplyTransaction(transaction3);
        _transactions.Add(transaction3);
    }
}

class Program
{
    static void Main()
    {
        FinanceApp app = new FinanceApp();
        app.Run();
    }
}