using Api.Dto.Transaction;
using Api.Services.Game;
using DataAccess;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Test.Util;
using Utils.Exceptions;

namespace Test;

[Collection("Database collection")]
public class WalletTransactionServiceTest
{
    private readonly MyDbContext _ctx;
    private readonly WalletTransactionsService _walletTransactionsService;
    private readonly DatabaseFixture _fixture;
    private readonly Seeder _seeder;

    public WalletTransactionServiceTest(DatabaseFixture fixture)
    {
        _fixture = fixture;

        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(fixture.ConnectionString)
            .Options;

        _ctx = new MyDbContext(options);

        //var seeder = new Seeder(_ctx);
        _seeder = new Seeder(_ctx);
        _seeder.Seed().GetAwaiter().GetResult();
        _walletTransactionsService = new WalletTransactionsService(_ctx);
    }

    private Guid ExistingUserWithWallet => _seeder.Player1Id;
    private Guid ExistingUserWithWalletID => _seeder.Player1WalletId;
    private readonly double ValidWalletBalance = 100;
    private Guid ValidTransactionId => _seeder.PendingDepositTransactionId;
    private Guid ExistingUserWithoutWallet => _seeder.UserWithoutWalletId;
    private readonly string AlreadyExistingMobilePayTransactionNumber = "1234";
    private readonly String ValidMobilePayTransactionNumber = "1234567890";
    private readonly DateTime dateTimeToUse = new DateTime(2025, 12, 14, 9, 07, 33, DateTimeKind.Utc);


    [Fact]
    public async Task GetWalletForPlayerIdReturnsWalletDto()
    {
        var walletDto = await _walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        Assert.NotNull(walletDto);
        Assert.NotEmpty(walletDto.Transactions);
        Assert.True(walletDto.Balance >= 40);
        Assert.Equal(ExistingUserWithWallet, walletDto.PlayerId);
    }

    [Fact]
    public async Task GetWalletForPlayerIdThrowsExceptionWhenPlayerHasNoWallet()
    {
        await Assert.ThrowsAsync<ServiceException>(() =>
            _walletTransactionsService.GetWalletForPlayerId(ExistingUserWithoutWallet));
    }

    [Fact]
    public async Task
        GetPendingTransactionsReturnsListOfTransactions() // For this method there is no sad path, as if there are no pending transactions, it will return empty list
    {
        var transactions = await _walletTransactionsService.GetPendingTransactions();
        Assert.NotEmpty(transactions);
    }

    [Fact]
    public async Task RegisterTransactionSuccess()
    {
        var BalanceBeforeConfirmingTransaction =
            await _walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        Assert.Equal(ValidWalletBalance, BalanceBeforeConfirmingTransaction.Balance);

        await _walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, new TransactionDto
        {
            Id = ValidTransactionId,
            UserId = ExistingUserWithWallet,
            Name = "Top up",
            WalletId = ExistingUserWithWalletID,
            MobilePayTransactionNumber = "123456789",
            Amount = 200,
            TransactionHistory = [],
            Status = TransactionStatus.Requested,
            Type = TransactionType.Deposit,
            CreatedAt = dateTimeToUse,
            UpdatedAt = dateTimeToUse
        });
        var BalanceAfterConfirmingTransaction =
            await _walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        Assert.Equal(BalanceBeforeConfirmingTransaction.Balance, BalanceAfterConfirmingTransaction.Balance);
        Assert.Equal(BalanceBeforeConfirmingTransaction.UpdatedAt, BalanceAfterConfirmingTransaction.UpdatedAt);
    }

    [Fact]
    public async Task RegisterTransactionThrowsErrorWhenAttributesAreMissing()
    {
        await Assert.ThrowsAsync<ServiceException>(() =>
            _walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, new TransactionDto()));

        var transactionDtonew = new TransactionDto
        {
            Id = ValidTransactionId,
            UserId = ExistingUserWithWallet,
            Name = "Top up",
            WalletId = ExistingUserWithWalletID,
            MobilePayTransactionNumber = null,
            Amount = 50,
            TransactionHistory = [],
            Status = TransactionStatus.Requested,
            Type = TransactionType.Deposit,
            CreatedAt = dateTimeToUse,
            UpdatedAt = dateTimeToUse
        };

        await Assert.ThrowsAsync<ServiceException>(() =>
            _walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, transactionDtonew));

        transactionDtonew.MobilePayTransactionNumber = AlreadyExistingMobilePayTransactionNumber;

        await Assert.ThrowsAsync<ServiceException>(() =>
            _walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, transactionDtonew));

        transactionDtonew.MobilePayTransactionNumber = ValidMobilePayTransactionNumber;
        transactionDtonew.Amount = 0;
        transactionDtonew.Type = TransactionType.TicketPurchase;

        await _walletTransactionsService.RegisterTransaction(ExistingUserWithWallet, transactionDtonew);
    }

    [Fact]
    public async Task ApproveTransactionSuccess()
    {
        await using var tx = await _ctx.Database.BeginTransactionAsync();
        

        var BalanceBeforeConfirmingTransaction =
            await _walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        await _walletTransactionsService.ApproveTransaction(ExistingUserWithWallet, ValidTransactionId);
        var BalanceAfterConfirmingTransaction =
            await _walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        Assert.NotEqual(BalanceBeforeConfirmingTransaction.Balance, BalanceAfterConfirmingTransaction.Balance);
        Assert.Equal(BalanceBeforeConfirmingTransaction.Balance + 200, BalanceAfterConfirmingTransaction.Balance);
        Assert.Equal(TransactionStatus.Approved,
            BalanceAfterConfirmingTransaction.Transactions.Single(t => t.Id == ValidTransactionId).Status);
        await tx.RollbackAsync();
    }

    [Fact]
    public async Task ApproveTransactionThrowsErrorWhenTransactionDoesNotExist()
    {
        await Assert.ThrowsAsync<ServiceException>(() =>
            _walletTransactionsService.ApproveTransaction(ExistingUserWithWallet, Guid.NewGuid()));
    }

    [Fact]
    public async Task ApproveTransactionThrowsErrorWhenTransactionIsAlreadyApproved()
    {
        await Assert.ThrowsAsync<ServiceException>(() =>
            _walletTransactionsService.ApproveTransaction(ExistingUserWithWallet, _seeder.ExistingTransactionId));
    }

    [Fact(Skip = "Not implemented yet")]
    public async Task RejectTransactionTestSuccess()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not implemented yet")]
    public async Task RejectTransactionTestThrowsErrorWhenTransactionDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ApproveTransactionWithdrowalSuccess()
    {
        var BalanceBeforeConfirmingTransaction =
            await _ctx.Wallets.AsNoTracking().Include(w => w.Transactions)
                .SingleAsync(w => w.PlayerId == ExistingUserWithWallet, cancellationToken: TestContext.Current.CancellationToken);
        //await _walletTransactionsService.ApproveTransaction(_seeder.AdminId, _seeder.TransactionIdForWithdraw);
        //TODO: Finish the test when the method is ready
        var BalanceAfterConfirmingTransaction =
            await _walletTransactionsService.GetWalletForPlayerId(ExistingUserWithWallet);
        
        
        //Assert.NotEqual(BalanceBeforeConfirmingTransaction.Balance, BalanceAfterConfirmingTransaction.Balance);
        //Assert.NotEqual(BalanceBeforeConfirmingTransaction.UpdatedAt, BalanceAfterConfirmingTransaction.UpdatedAt);
        //Assert.Equal(BalanceAfterConfirmingTransaction.Balance, BalanceBeforeConfirmingTransaction.Balance - 3);
        //Assert.Equal(TransactionStatus.Approved,
        //   BalanceAfterConfirmingTransaction.Transactions.Single(t => t.Id == ValidTransactionId).Status);
        Assert.Equal(BalanceBeforeConfirmingTransaction.Balance, BalanceAfterConfirmingTransaction.Balance);
    }
}