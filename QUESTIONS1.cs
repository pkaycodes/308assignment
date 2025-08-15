using System;
using System.Collections.Generic;

public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Bank transfer: Amount {transaction.Amount} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Mobile money: Amount {transaction.Amount} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Crypto wallet: Amount {transaction.Amount} for {transaction.Category}");
    }
}

public class Account
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

public sealed class SavingsAccount : Account
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

public class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        SavingsAccount account = new SavingsAccount("123456", 1000m);

        Transaction t1 = new Transaction(1, DateTime.Now, 200m, "Groceries");
        Transaction t2 = new Transaction(2, DateTime.Now, 100m, "Utilities");
        Transaction t3 = new Transaction(3, DateTime.Now, 50m, "Entertainment");

        ITransactionProcessor processor1 = new MobileMoneyProcessor();
        processor1.Process(t1);
        account.ApplyTransaction(t1);
        _transactions.Add(t1);

        ITransactionProcessor processor2 = new BankTransferProcessor();
        processor2.Process(t2);
        account.ApplyTransaction(t2);
        _transactions.Add(t2);

        ITransactionProcessor processor3 = new CryptoWalletProcessor();
        processor3.Process(t3);
        account.ApplyTransaction(t3);
        _transactions.Add(t3);
    }
}

class Program
{
    static void Main(string[] args)
    {
        FinanceApp app = new FinanceApp();
        app.Run();
    }
}
