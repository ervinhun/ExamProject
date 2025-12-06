using Api.Dto.test;
using Api.Dto.Transaction;
using DataAccess.Enums;

namespace Api.Services.Game;

public interface IWalletTransactionsService
{
    Task<WalletDto> GetWalletForPlayerId(Guid playerId);
    Task<List<TransactionDto>> GetPendingTransactions();
    Task RegisterTransaction(Guid actionUser, TransactionDto transactionDto);
    Task ApproveTransaction(Guid actionUser, Guid transactionId);
    Task RejectTransaction(Guid actionUser, Guid transactionId);
    Task UpdateTransactionById(Guid id, UpdateTransactionDto transactionDto);
}