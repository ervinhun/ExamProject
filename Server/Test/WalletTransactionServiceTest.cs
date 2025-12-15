using Api.Dto.Transaction;
using Api.Services.Game;
using DataAccess;
using DataAccess.Enums;
using Test.Util;
using Utils.Exceptions;

namespace Test;

public class WalletTransactionServiceTest(
    MyDbContext ctx,
    ISeeder seeder,
    IWalletTransactionsService walletTransactionsService)
{
    private Guid ExistingUserWithWallet = new Guid("1");
    private Guid ExistinUserWithWalletWalletID = new Guid("WalletId");
    private double ValidWalletBalance = 100;
    private Guid ValidTransactionId = new Guid("ValidTransactionId");
    private Guid ExistingUserWithoutWallet = new Guid("2");
    private string AlreadyExistingMobilePayeTransactionNumber = "1234";
    private String ValideMobilePayTransactionNumber = "1234567890";
    private DateTime dateTimeToUse = new DateTime(2025, 12, 14, 9, 07, 33);


    [Fact]
    public async Task GetWalletForPlayerIdReturnsWalletDto()
    {
        var walletDto = await walletTransactionsService.GetWalletForPlayerId(new Guid("1"));
        Assert.NotNull(walletDto);
        Assert.NotEmpty(walletDto.Transactions);
        Assert.Equal(ValidWalletBalance, walletDto.Balance);
        Assert.Equal(ExistingUserWithWallet, walletDto.PlayerId);
    }

    [Fact]
    public async Task GetWalletForPlayerIdThrowsExceptionWhenPlayerHasNoWallet()
    {
        await Assert.ThrowsAsync<ServiceException>(() =>
            walletTransactionsService.GetWalletForPlayerId(ExistingUserWithoutWallet));
    }

    [Fact]
    public async Task
        GetPendingTransactionsReturnsListOfTransactions() // For this method there is no sad path, as if there are no pending transactions, it will return empty list
    {
        var transactions = await walletTransactionsService.GetPendingTransactions();
        Assert.NotEmpty(transactions);
    }

    [Fact]
    public async Task RegisterTransactionSuccess()
    {
        var BalanceBeforeConfirmingTransaction =
            await walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        Assert.Equal(ValidWalletBalance, BalanceBeforeConfirmingTransaction.Balance);

        await walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, new TransactionDto
        {
            Id = ValidTransactionId,
            UserId = ExistingUserWithWallet,
            Name = "Top up",
            WalletId = ExistinUserWithWalletWalletID,
            MobilePayTransactionNumber = "123456789",
            Amount = 200,
            TransactionHistory = new List<TransactionHistoryDto>(ArraySegment<TransactionHistoryDto>.Empty),
            Status = TransactionStatus.Requested,
            Type = TransactionType.Deposit,
            CreatedAt = dateTimeToUse,
            UpdatedAt = dateTimeToUse
        });

        var BalanceAfterConfirmingTransaction =
            await walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        Assert.Equal(BalanceBeforeConfirmingTransaction.Balance, BalanceAfterConfirmingTransaction.Balance);
        Assert.Equal(BalanceBeforeConfirmingTransaction.UpdatedAt, BalanceAfterConfirmingTransaction.UpdatedAt);
    }

    [Fact]
    public async Task RegisterTransactionThrowsErrorWhenAttributesAreMissing()
    {

        await Assert.ThrowsAsync<ServiceException>(() =>
            walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, new TransactionDto()));

        var transactionDtonew = new TransactionDto
        {
            Id = ValidTransactionId,
            UserId = ExistingUserWithWallet,
            Name = "Top up",
            WalletId = ExistinUserWithWalletWalletID,
            MobilePayTransactionNumber = null,
            Amount = 50,
            TransactionHistory = new List<TransactionHistoryDto>(ArraySegment<TransactionHistoryDto>.Empty),
            Status = TransactionStatus.Requested,
            Type = TransactionType.Deposit,
            CreatedAt = dateTimeToUse,
            UpdatedAt = dateTimeToUse
        };

        await Assert.ThrowsAsync<ServiceException>(() =>
            walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, transactionDtonew));

        transactionDtonew.MobilePayTransactionNumber = AlreadyExistingMobilePayeTransactionNumber;

        await Assert.ThrowsAsync<ServiceException>(() =>
            walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, transactionDtonew));

        transactionDtonew.MobilePayTransactionNumber = ValideMobilePayTransactionNumber;
        transactionDtonew.Amount = 0;
        transactionDtonew.Type = TransactionType.TicketPurchase;

        await walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, transactionDtonew);
    }

    [Fact]
    public async Task ApproveTransactionSuccess()
    {
        var BalanceBeforeConfirmingTransaction =
            await walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        await walletTransactionsService.ApproveTransaction(ExistingUserWithWallet, ValidTransactionId);
        var BalanceAfterConfirmingTransaction =
            await walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);

        Assert.NotEqual(BalanceBeforeConfirmingTransaction.Balance, BalanceAfterConfirmingTransaction.Balance);
        Assert.NotEqual(BalanceBeforeConfirmingTransaction.UpdatedAt, BalanceAfterConfirmingTransaction.UpdatedAt);
        Assert.Equal(BalanceAfterConfirmingTransaction.Balance, BalanceBeforeConfirmingTransaction.Balance + 200);
        Assert.Equal(TransactionStatus.Approved,
            BalanceAfterConfirmingTransaction.Transactions.Single(t => t.Id == ValidTransactionId).Status);
    }

    [Fact]
    public async Task ApproveTransactionThrowsErrorWhenTransactionDoesNotExist()
    {
        await Assert.ThrowsAsync<ServiceException>(() =>
            walletTransactionsService.ApproveTransaction(ExistingUserWithWallet, Guid.NewGuid()));
    }

    [Fact]
    public async Task ApproveTransactionThrowsErrorWhenTransactionIsAlreadyApproved()
    {
        await walletTransactionsService.ApproveTransaction(ExistingUserWithWallet, ValidTransactionId);
        await Assert.ThrowsAsync<ServiceException>(() =>
            walletTransactionsService.ApproveTransaction(ExistingUserWithWallet, ValidTransactionId));
    }

    [Fact]
    public async Task RejectTransactionTestSuccess()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task RejectTransactionTestThrowsErrorWhenTransactionDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ApproveTransactionWithdrowalSuccess()
    {
        var BalanceBeforeConfirmingTransaction = await walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        
        var transactionDtonew = new TransactionDto
        {
            Id = ValidTransactionId,
            UserId = ExistingUserWithWallet,
            Name = "Top up",
            WalletId = ExistinUserWithWalletWalletID,
            MobilePayTransactionNumber = null,
            Amount = 50,
            TransactionHistory = new List<TransactionHistoryDto>(ArraySegment<TransactionHistoryDto>.Empty),
            Status = TransactionStatus.Requested,
            Type = TransactionType.Withdrawal,
            CreatedAt = dateTimeToUse,
            UpdatedAt = dateTimeToUse
        };
        
        await walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, transactionDtonew);
        await walletTransactionsService.ApproveTransaction(ExistingUserWithWallet, ValidTransactionId);
        
        var BalanceAfterConfirmingTransaction = await walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        
        Assert.NotEqual(BalanceBeforeConfirmingTransaction.Balance, BalanceAfterConfirmingTransaction.Balance);
        Assert.NotEqual(BalanceBeforeConfirmingTransaction.UpdatedAt, BalanceAfterConfirmingTransaction.UpdatedAt);
        Assert.Equal(BalanceAfterConfirmingTransaction.Balance, BalanceBeforeConfirmingTransaction.Balance - 50);
        Assert.Equal(TransactionStatus.Approved,
            BalanceAfterConfirmingTransaction.Transactions.Single(t => t.Id == ValidTransactionId).Status);
    }
}