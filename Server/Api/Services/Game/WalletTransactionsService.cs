using Api.Dto.test;
using Api.Dto.Transaction;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using DataAccess.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Utils;
using Utils.Exceptions;
using TransactionStatus = DataAccess.Enums.TransactionStatus;

namespace Api.Services.Game;

public class WalletTransactionsService(MyDbContext ctx) : IWalletTransactionsService
{
    public async Task<WalletDto> GetWalletForPlayerId(Guid id)
    {
        try
        {
            var wallet = await ctx.Wallets.Include(w=>w.Transactions.OrderByDescending(t=>t.CreatedAt).Skip(0).Take(15)).FirstOrDefaultAsync(w=> w.PlayerId == id);
            if (wallet == null) throw new ServiceException("Wallet not found");
            
            var transactionsDtos = new List<TransactionDto>();
            foreach (var walletTransaction in wallet.Transactions)
            {
                transactionsDtos.Add(new TransactionDto
                {
                    Id = walletTransaction.Id,
                    UserId = walletTransaction.UserId,
                    Amount = walletTransaction.Amount,
                    Status = walletTransaction.Status,
                    MobilePayTransactionNumber = walletTransaction.MobilePayTransactionNumber,
                    Type = walletTransaction.Type,
                    CreatedAt = walletTransaction.CreatedAt,
                    UpdatedAt = walletTransaction.UpdatedAt
                });
            }

            return new WalletDto
            {
                Id = wallet.Id,
                PlayerId = wallet.PlayerId,
                AccountNumber = "123-321-123",
                Balance = wallet.Balance,
                Transactions = transactionsDtos,
                UpdatedAt = wallet.UpdatedAt

            };
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public async Task<List<TransactionDto>> GetPendingTransactions()
    {
        var transactions = await ctx.Transactions
            .Where(t=>t.Status == TransactionStatus.Requested)
            .ToListAsync();
        var transactionsDtos = new List<TransactionDto>();
        foreach (var transaction in transactions)
        {
            transactionsDtos.Add(new TransactionDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                Name = transaction.Name,
                WalletId = transaction.WalletId,
                MobilePayTransactionNumber = transaction.MobilePayTransactionNumber,
                Amount = transaction.Amount,
                Status = transaction.Status,
                Type = transaction.Type,
                CreatedAt = DateTimeHelper.ToCopenhagen(transaction.CreatedAt),
                UpdatedAt = DateTimeHelper.ToCopenhagen(transaction.UpdatedAt)
            });
        }

        return transactionsDtos;
    }

    public async Task RegisterTransaction(Guid actionUser, TransactionDto transactionDto)
    {
        var user = await ctx.Users.SingleOrDefaultAsync(p => p.Id == actionUser);
        if(user == null) throw new ServiceException("User not found.");
        if(!user.Activated) throw new  ServiceException("User must be activated.");
        var wallet = await ctx.Wallets.SingleOrDefaultAsync(w => w.PlayerId == actionUser);
        if(wallet == null) throw new ServiceException("Wallet not found.");
        if (wallet.PlayerId != actionUser) throw new ServiceException("Something went wrong, please try again.");
        
        if (transactionDto.Type == TransactionType.Deposit)
        {
            if (transactionDto.MobilePayTransactionNumber == null)
            {
                throw new ServiceException("Transaction number is required for deposit.");
            }
            if (await ctx.Transactions.AnyAsync(t =>
                    t.MobilePayTransactionNumber == transactionDto.MobilePayTransactionNumber))
            {
                throw new ServiceException("There is already registered transaction with this MobilePay transaction number");
            }
        }

        if (transactionDto.Type == TransactionType.TicketPurchase)
        {
            if (transactionDto.Amount > wallet.Balance)
            {
                throw new ServiceException("Not enough funds, please make a deposit.");
            }

            await RemoveAmountFromWallet(transactionDto.Id, transactionDto.WalletId, transactionDto.Amount);
        }
        var transaction = new Transaction
        {
            UserId = transactionDto.UserId,
            Name = transactionDto.Name,
            WalletId = transactionDto.WalletId,
            MobilePayTransactionNumber = transactionDto.MobilePayTransactionNumber,
            Status = transactionDto.Status,
            Type = transactionDto.Type,
            Amount = transactionDto.Amount,
            CreatedAt = DateTime.UtcNow,
        };
        var transactionHistory = new TransactionHistory
        {
            TransactionId = transaction.Id,
            Transaction = transaction,
            ActionUser = actionUser,
            Status = transactionDto.Status,
            Type = transactionDto.Type,
        };
        ctx.Transactions.Add(transaction);
        ctx.TransactionHistories.Add(transactionHistory);
        await ctx.SaveChangesAsync();
    }

    public async Task ApproveTransaction(Guid actionUser, Guid transactionId)
    {
        try
        {
            var transaction = await ctx.Transactions.Where(t => t.Status == TransactionStatus.Requested)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
            if (transaction == null) throw new ServiceException("Transaction not found");
            var transactionHistory = new TransactionHistory
            {
                Transaction = transaction,
                TransactionId = transactionId,
                ActionUser = actionUser,
            };
            switch (transaction.Type)
            {
                case TransactionType.Deposit:
                    await SendAmountToWallet(transactionId, transaction.WalletId, transaction.Amount);
                    transactionHistory.Type = TransactionType.Deposit;
                    break;
                case TransactionType.Withdrawal:
                    await RemoveAmountFromWallet(actionUser, transaction.WalletId, transaction.Amount);
                    transactionHistory.Type = TransactionType.Withdrawal;
                    break;
            }
            transactionHistory.Status = TransactionStatus.Approved;
            transaction.Status = TransactionStatus.Approved;
            transaction.UpdatedAt = DateTime.UtcNow;
            
            ctx.TransactionHistories.Add(transactionHistory);
            await ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public Task RejectTransaction(Guid actionUser, Guid transactionId)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateTransactionById(Guid id, UpdateTransactionDto transactionDto)
    {
        var transaction = await ctx.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction == null) throw new ServiceException("Transaction not found");
        
    }

    private async Task SendAmountToWallet(Guid transactionId, Guid walletId, double amount)
    {
        try
        {
            var wallet = await ctx.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);
            if (wallet == null) throw new ServiceException("Wallet not found");
            var transactionHistory = new TransactionHistory
            {
                TransactionId = transactionId,
                ActionUser = Guid.Empty,
                Status = TransactionStatus.Approved,
                Type = TransactionType.SystemAdjustment
            };
            wallet.Balance += amount;
            ctx.TransactionHistories.Add(transactionHistory);
            await ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
        
    }

    private async Task RemoveAmountFromWallet(Guid transactionId, Guid walletId, double amount)
    {
        try
        {
            var wallet = await ctx.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);
            if (wallet == null) throw new ServiceException("Wallet not found");
            var transactionHistory = new TransactionHistory
            {
                TransactionId = transactionId,
                ActionUser = Guid.Empty,
                Status = TransactionStatus.Approved,
                Type = TransactionType.SystemAdjustment
            };
            wallet.Balance -= amount;
            ctx.TransactionHistories.Add(transactionHistory);
            await ctx.SaveChangesAsync();
        }
        catch(Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }
}