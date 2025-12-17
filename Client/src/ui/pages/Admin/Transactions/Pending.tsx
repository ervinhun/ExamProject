import { useEffect, useState } from "react";
import { useAtom, useSetAtom } from "jotai";
import { pendingTransactionsAtom, fetchPendingTransactionsAtom, approveTransactionAtom } from "@core/atoms/transaction";
import { addNotificationAtom } from "@core/atoms/error";
import { formatDate } from "@utils/dateUtils";
import { mapTransactionStatus, mapTransactionType } from "@core/types/transaction";

export default function Pending() {
    const [pendingTransactions] = useAtom(pendingTransactionsAtom);
    const [, fetchPendingTransactions] = useAtom(fetchPendingTransactionsAtom);
    const [, approveTransaction] = useAtom(approveTransactionAtom);
    const addNotification = useSetAtom(addNotificationAtom);
    
    const [searchTerm, setSearchTerm] = useState("");
    const [isApproving, setIsApproving] = useState<string | null>(null);

    useEffect(() => {
        fetchPendingTransactions();
    }, [fetchPendingTransactions]);

    // Filter transactions by MobilePay number
    const filteredTransactions = pendingTransactions.filter(transaction => {
        if (!searchTerm) return true;
        return transaction.mobilePayTransactionNumber?.toLowerCase().includes(searchTerm.toLowerCase());
    });

    const handleApprove = async (transactionId: string) => {
        setIsApproving(transactionId);
        try {
            await approveTransaction(transactionId);
            addNotification({
                type: "success",
                message: "Transaction approved successfully"
            });
            // Refresh the list
            await fetchPendingTransactions();
        } catch (error: any) {
            addNotification({
                type: "error",
                message: error?.message || "Failed to approve transaction"
            });
        } finally {
            setIsApproving(null);
        }
    };

    const totalAmount = filteredTransactions.reduce((sum, t) => sum + t.amount, 0);

    return (
        <div className="container mx-auto">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary ml-3">Pending Transactions</h1>
                        <p className="text-base text-base-content/70 mt-1 ml-3">Review and approve pending deposit requests</p>
                    </div>
                </div>

                {/* Search Bar */}
                <div className="card bg-gradient-to-br from-base-200 to-base-300 shadow-xl border border-base-300">
                    <div className="card-body">
                        <div className="flex flex-col sm:flex-row items-start sm:items-center gap-3">
                            <div className="flex items-center gap-2">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 text-primary" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                                </svg>
                                <span className="text-sm font-medium text-primary whitespace-nowrap">Search Transaction</span>
                            </div>
                            <div className="flex-1 w-full sm:w-auto relative">
                                <input 
                                    type="text" 
                                    placeholder="Enter MobilePay transaction number..." 
                                    className="input input-bordered w-full bg-base-100 focus:input-primary transition-all duration-200 pr-20"
                                    value={`${searchTerm}`}
                                    onChange={(e) => {
                                        const value = e.target.value.replace('#', '');
                                        if(value.length <= 11 && /^\d*$/.test(value)){
                                            setSearchTerm(value);
                                        }
                                    }}
                                />
                                {searchTerm && (
                                    <button 
                                        className="absolute right-2 top-1/2 -translate-y-1/2 btn btn-ghost btn-xs text-error hover:bg-error/20"
                                        onClick={() => setSearchTerm("")}
                                    >
                                        <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                                        </svg>
                                        Clear
                                    </button>
                                )}
                            </div>
                        </div>
                        {searchTerm && (
                            <div className="mt-3 px-2">
                                <div className="flex items-center gap-2 text-sm">
                                    <span className="badge badge-primary badge-sm">{filteredTransactions.length}</span>
                                    <span className="text-base-content/70">
                                        {filteredTransactions.length === 1 ? 'transaction' : 'transactions'} found matching <span className="font-semibold text-primary">"#{searchTerm}"</span>
                                    </span>
                                </div>
                            </div>
                        )}
                    </div>
                </div>

                {/* Transactions Table */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Pending Transactions</h2>
                        
                        <div className="overflow-x-auto">
                            <table className="table table-zebra">
                                <thead>
                                    <tr>
                                        <th>Player Name</th>
                                        <th>Type</th>
                                        <th>Amount</th>
                                        <th>MobilePay #</th>
                                        <th>Date</th>
                                        <th>Status</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {filteredTransactions.length === 0 ? (
                                        <tr>
                                            <td colSpan={7} className="text-center py-8 text-base-content/60">
                                                {searchTerm 
                                                    ? `No transactions found matching "#${searchTerm}"`
                                                    : "No pending transactions"
                                                }
                                            </td>
                                        </tr>
                                    ) : (
                                        filteredTransactions.map((transaction) => {
                                            const statusText = mapTransactionStatus(transaction.status);
                                            const typeText = mapTransactionType(transaction.type);
                                            const isProcessing = isApproving === transaction.id;

                                            return (
                                                <tr key={transaction.id}>
                                                    <td className="font-semibold">{transaction.name}</td>
                                                    <td>
                                                        <span className="badge badge-info badge-sm">
                                                            {typeText}
                                                        </span>
                                                    </td>
                                                    <td className="font-mono font-bold text-success">
                                                        +{transaction.amount.toFixed(2)} DKK
                                                    </td>
                                                    <td className="font-mono text-sm">
                                                        {transaction.mobilePayTransactionNumber || "N/A"}
                                                    </td>
                                                    <td className="text-sm">{formatDate(transaction.createdAt)}</td>
                                                    <td>
                                                        <span className="badge badge-warning badge-sm">
                                                            {statusText}
                                                        </span>
                                                    </td>
                                                    <td>
                                                        <button
                                                            onClick={() => handleApprove(transaction.id)}
                                                            className="btn btn-success btn-xs"
                                                            disabled={isProcessing}
                                                        >
                                                            {isProcessing ? (
                                                                <>
                                                                    <span className="loading loading-spinner loading-xs"></span>
                                                                    Processing...
                                                                </>
                                                            ) : (
                                                                <>
                                                                    <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 13l4 4L19 7" />
                                                                    </svg>
                                                                    Approve
                                                                </>
                                                            )}
                                                        </button>
                                                    </td>
                                                </tr>
                                            );
                                        })
                                    )}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}