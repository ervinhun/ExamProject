import { useEffect, useState } from "react";
import { useAtom } from "jotai";
import { allTransactionsAtom, fetchAllTransactionsAtom } from "@core/atoms/transaction";
import { mapTransactionStatus, mapTransactionType } from "@core/types/transaction";
import { formatDateTime } from "@utils/dateUtils";
import { getStatusColor } from "@utils/gameUtils";
import { formatCurrency } from "@utils/priceUtils";

export default function AllTransactions() {
    const [allTransactions] = useAtom(allTransactionsAtom);
    const [, fetchAllTransactions] = useAtom(fetchAllTransactionsAtom);
    
    const [searchTerm, setSearchTerm] = useState("");
    const [statusFilter, setStatusFilter] = useState<string>("all");
    const [typeFilter, setTypeFilter] = useState<string>("all");
    const [sortBy, setSortBy] = useState<"date" | "amount">("date");
    const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc");

    useEffect(() => {
        fetchAllTransactions();
    }, [fetchAllTransactions]);

    // Filter transactions
    const filteredTransactions = allTransactions.filter(transaction => {
        const statusText = mapTransactionStatus(transaction.status);
        const typeText = mapTransactionType(transaction.type);
        
        const matchesSearch = !searchTerm || 
            transaction.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
            transaction.mobilePayTransactionNumber?.toLowerCase().includes(searchTerm.toLowerCase());
        
        const matchesStatus = statusFilter === "all" || statusText === statusFilter;
        const matchesType = typeFilter === "all" || typeText === typeFilter;
        
        return matchesSearch && matchesStatus && matchesType;
    });

    // Sort transactions
    const sortedTransactions = [...filteredTransactions].sort((a, b) => {
        if (sortBy === "date") {
            const dateA = new Date(a.createdAt).getTime();
            const dateB = new Date(b.createdAt).getTime();
            return sortOrder === "asc" ? dateA - dateB : dateB - dateA;
        } else {
            return sortOrder === "asc" ? a.amount - b.amount : b.amount - a.amount;
        }
    });

    const getTypeIcon = (type: number | string) => {
        const typeText = typeof type === "number" ? mapTransactionType(type) : type;
        const isPositive = typeText === "Deposit" || typeText === "RewardPayout" || typeText === "Refund";
        
        if (isPositive) {
            return (
                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 text-success" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4v16m0 0l-4-4m4 4l4-4" />
                </svg>
            );
        }
        return (
            <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 text-error" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 20V4m0 0l4 4m-4-4l-4 4" />
            </svg>
        );
    };

    return (
        <div className="container mx-auto">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary ml-3">All Transactions</h1>
                        <p className="text-base text-base-content/70 mt-1 ml-3">Complete transaction history and analytics</p>
                    </div>
                </div>

                {/* Filters and Search */}
                <div className="card bg-gradient-to-br from-base-200 to-base-300 shadow-xl border border-base-300">
                    <div className="card-body">
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                            {/* Search */}
                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-semibold">Search</span>
                                </label>
                                <div className="relative">
                                    <input 
                                        type="text" 
                                        placeholder="Player name or MobilePay #..." 
                                        className="input input-bordered w-full pr-10"
                                        value={searchTerm}
                                        onChange={(e) => setSearchTerm(e.target.value)}
                                    />
                                    {searchTerm && (
                                        <button 
                                            className="absolute right-2 top-1/2 -translate-y-1/2 btn btn-ghost btn-xs"
                                            onClick={() => setSearchTerm("")}
                                        >
                                            ✕
                                        </button>
                                    )}
                                </div>
                            </div>

                            {/* Status Filter */}
                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-semibold">Status</span>
                                </label>
                                <select 
                                    className="select select-bordered w-full"
                                    value={statusFilter}
                                    onChange={(e) => setStatusFilter(e.target.value)}
                                >
                                    <option value="all">All Statuses</option>
                                    <option value="Pending">Pending</option>
                                    <option value="Approved">Approved</option>
                                    <option value="Rejected">Rejected</option>
                                    <option value="Canceled">Canceled</option>
                                </select>
                            </div>

                            {/* Type Filter */}
                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-semibold">Type</span>
                                </label>
                                <select 
                                    className="select select-bordered w-full"
                                    value={typeFilter}
                                    onChange={(e) => setTypeFilter(e.target.value)}
                                >
                                    <option value="all">All Types</option>
                                    <option value="Deposit">Deposit</option>
                                    <option value="Withdrawal">Withdrawal</option>
                                    <option value="TicketPurchase">Ticket Purchase</option>
                                    <option value="RewardPayout">Reward Payout</option>
                                    <option value="Refund">Refund</option>
                                </select>
                            </div>

                            {/* Sort */}
                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-semibold">Sort By</span>
                                </label>
                                <div className="flex gap-2">
                                    <select 
                                        className="select select-bordered flex-1"
                                        value={sortBy}
                                        onChange={(e) => setSortBy(e.target.value as "date" | "amount")}
                                    >
                                        <option value="date">Date</option>
                                        <option value="amount">Amount</option>
                                    </select>
                                    <button 
                                        className="btn btn-square"
                                        onClick={() => setSortOrder(sortOrder === "asc" ? "desc" : "asc")}
                                    >
                                        {sortOrder === "asc" ? "↑" : "↓"}
                                    </button>
                                </div>
                            </div>
                        </div>

                        {/* Filter Results Info */}
                        {(searchTerm || statusFilter !== "all" || typeFilter !== "all") && (
                            <div className="mt-4 flex items-center gap-2">
                                <span className="badge badge-primary">{sortedTransactions.length}</span>
                                <span className="text-sm text-base-content/70">
                                    transaction(s) found
                                </span>
                                <button 
                                    className="btn btn-ghost btn-xs ml-auto"
                                    onClick={() => {
                                        setSearchTerm("");
                                        setStatusFilter("all");
                                        setTypeFilter("all");
                                    }}
                                >
                                    Clear Filters
                                </button>
                            </div>
                        )}
                    </div>
                </div>

                {/* Transactions Table */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Transaction History</h2>
                        
                        <div className="overflow-x-auto">
                            <table className="table table-zebra">
                                <thead>
                                    <tr>
                                        <th>Date</th>
                                        <th>Name</th>
                                        <th>Type</th>
                                        <th>MobilePay #</th>
                                        <th>Amount</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {sortedTransactions.length === 0 ? (
                                        <tr>
                                            <td colSpan={6} className="text-center py-8 text-base-content/60">
                                                {searchTerm || statusFilter !== "all" || typeFilter !== "all"
                                                    ? "No transactions match your filters"
                                                    : "No transactions available"
                                                }
                                            </td>
                                        </tr>
                                    ) : (
                                        sortedTransactions.map((transaction) => {
                                            const typeText = mapTransactionType(transaction.type);
                                            const statusText = mapTransactionStatus(transaction.status);
                                            const isPositive = typeText === "Deposit" || typeText === "RewardPayout" || typeText === "Refund";
                                            
                                            return (
                                                <tr key={transaction.id}>
                                                    <td className="text-sm">{formatDateTime(transaction.createdAt)}</td>
                                                    <td className="font-semibold">{transaction.name}</td>
                                                    <td>
                                                        <span className="flex items-center gap-2">
                                                            {getTypeIcon(transaction.type)}
                                                            <span className="badge badge-info badge-sm">
                                                                {typeText}
                                                            </span>
                                                        </span>
                                                    </td>
                                                    <td>
                                                        {transaction.mobilePayTransactionNumber ? (
                                                            <span className="font-mono text-sm">
                                                                #{transaction.mobilePayTransactionNumber}
                                                            </span>
                                                        ) : (
                                                            <span className="text-base-content/40 text-sm">N/A</span>
                                                        )}
                                                    </td>
                                                    <td className={`font-mono font-semibold ${isPositive ? 'text-success' : 'text-error'}`}>
                                                        {isPositive ? '+' : '-'}{formatCurrency(Math.abs(transaction.amount))}
                                                    </td>
                                                    <td>
                                                        <span className={`badge badge-sm ${getStatusColor(statusText)}`}>
                                                            {statusText}
                                                        </span>
                                                    </td>
                                                </tr>
                                            );
                                        })
                                    )}
                                </tbody>
                            </table>
                        </div>

                        {/* Pagination info */}
                        {sortedTransactions.length > 0 && (
                            <div className="mt-4 text-sm text-base-content/60 text-center">
                                Showing {sortedTransactions.length} of {allTransactions.length} total transactions
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}