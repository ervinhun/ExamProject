import { playerApi } from "@core/api/controllers/player"
import { authAtom } from "@core/atoms/auth";
import { mapTransactionStatus, mapTransactionType } from "@core/types/transaction";
import { useAtom, useSetAtom } from "jotai";
import { useEffect, useState } from "react";
import { getWalletForPlayerIdAtom, requestDepositAtom, walletAtom } from "@core/atoms/wallet";
import { addNotificationAtom } from "@core/atoms/error";

export default function Wallet() {
    const [wallet,] = useAtom(walletAtom);
    const [,getWalletForPlayerId] = useAtom(getWalletForPlayerIdAtom);
    const [,requestDeposit] = useAtom(requestDepositAtom);
    const [authUser,] = useAtom(authAtom);
    const addNotification = useSetAtom(addNotificationAtom);

    const [depositAmount, setDepositAmount] = useState("");
    const [withdrawAmount, setWithdrawAmount] = useState("");
    const [mobilePayTransactionNumber, setMobilePayTransactionNumber] = useState("");

    useEffect(() => {
        if(authUser?.id && !wallet){
            getWalletForPlayerId(authUser?.id!);
        }
    }, []);

    const handleDeposit = async (e: React.FormEvent) => {
        e.preventDefault();
        await requestDeposit({
            playerId: authUser?.id!,
            walletId: wallet?.id!,
            amount: parseFloat(depositAmount),
            mobilePayTransactionNumber: mobilePayTransactionNumber
        }).then(() => {
            setDepositAmount("");
            setMobilePayTransactionNumber("");
            addNotification({type: "success", message: "Deposit request submitted successfully"});
        }).catch((err) => {
            console.error("Deposit failed:", err);
            addNotification({type: "error", message: `Deposit failed: ${err.message}`});
        });
    };

    const handleWithdraw = (e: React.FormEvent) => {
        e.preventDefault();
        // TODO: Implement withdraw logic
        console.log("Withdraw:", withdrawAmount);
        setWithdrawAmount("");
    };

    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat('da-DK', { 
            style: 'currency', 
            currency: 'DKK' 
        }).format(amount);
    };

    const formatDate = (dateStr: string) => {
        const date = new Date(dateStr);
        return date.toLocaleDateString("da-DK", { 
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit", 
            minute: "2-digit",
            hour12: false
        });
    };

    const getStatusColor = (status: number | string): string => {
        const statusText = typeof status === "number" ? mapTransactionStatus(status) : status;
        switch (statusText) {
            case "Approved": return "badge-success";
            case "Pending": return "badge-warning";
            case "Rejected": return "badge-error";
            case "Canceled": return "badge-ghost";
            default: return "badge-ghost";
        }
    };

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
                        <h1 className="text-4xl font-bold text-primary ml-3">My Wallet</h1>
                        <p className="text-base text-base-content/70 mt-1 ml-3">Manage your balance and transactions</p>
                    </div>
                </div>

                {/* Balance Card */}
                <div className="card bg-gradient-to-br from-primary to-secondary shadow-xl mt-7 mb-11">
                    <div className="card-body">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-primary-content/70 font-medium">Current Balance</p>
                                <h2 className="text-5xl font-bold text-primary-content mt-2">
                                    {wallet ? formatCurrency(wallet.balance) : "Loading..."}
                                </h2>
                            </div>
                            <div className="text-primary-content/20">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-24 w-24" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                                </svg>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Quick Actions */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-11">
                    {/* Deposit Card */}
                    <div className="card bg-base-200 shadow-lg">
                        <div className="card-body">
                            <h2 className="card-title text-2xl mb-4">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-success" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4v16m8-8H4" />
                                </svg>
                                Deposit Funds
                            </h2>
                            <form onSubmit={handleDeposit} className="space-y-4">
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">MobilePay Transaction number</span>
                                    </label>
                                    <input 
                                        type="text"
                                        placeholder="Enter transaction number"
                                        className="input input-bordered w-full"
                                        value={`#${mobilePayTransactionNumber}`}
                                        onChange={(e) => {
                                            const value = e.target.value.replace('#', '');
                                            if(value.length <= 11 && /^\d*$/.test(value)){
                                                setMobilePayTransactionNumber(value);
                                            }
                                        }}
                                        required
                                    />
                                </div>
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Amount (DKK)</span>
                                    </label>
                                    <input 
                                        type="number"
                                        placeholder="Enter amount"
                                        className="input input-bordered w-full"
                                        value={depositAmount}
                                        onChange={(e) => setDepositAmount(e.target.value)}
                                        min="0"
                                        step="1"
                                        required
                                    />
                                </div>
                                <div className="flex gap-2">
                                    <button type="button" onClick={() => setDepositAmount("100")} className="btn btn-sm btn-ghost">+100</button>
                                    <button type="button" onClick={() => setDepositAmount("500")} className="btn btn-sm btn-ghost">+500</button>
                                    <button type="button" onClick={() => setDepositAmount("1000")} className="btn btn-sm btn-ghost">+1000</button>
                                </div>
                                <button type="submit" className="btn btn-success w-full">Deposit</button>
                            </form>
                        </div>
                    </div>

                    {/* Withdraw Card */}
                    <div className="card bg-base-200 shadow-lg">
                        <div className="card-body">
                            <h2 className="card-title text-2xl mb-4">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-warning" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M20 12H4" />
                                </svg>
                                Withdraw Funds
                            </h2>
                            <form onSubmit={handleWithdraw} className="space-y-4">
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Amount (DKK)</span>
                                    </label>
                                    <input 
                                        type="number"
                                        placeholder="Enter amount"
                                        className="input input-bordered w-full"
                                        value={withdrawAmount}
                                        onChange={(e) => setWithdrawAmount(e.target.value)}
                                        min="0"
                                        max={wallet?.balance || 0}
                                        step="0.01"
                                        required
                                    />
                                </div>
                                <div className="text-sm text-base-content/60">
                                    Available: {wallet ? formatCurrency(wallet.balance) : "0 kr."}
                                </div>
                                <button type="submit" className="btn btn-warning w-full">Withdraw</button>
                            </form>
                        </div>
                    </div>
                </div>

                {/* Transaction History */}
                <div className="card bg-base-200 shadow-lg mt-11">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Transaction History</h2>
                        
                        <div className="overflow-x-auto">
                            <table className="table table-zebra">
                                <thead>
                                    <tr>
                                        <th>Date</th>
                                        <th>Type</th>
                                        <th>MobilePay #</th>
                                        <th>Amount</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {wallet?.transactions.length === 0 ? (
                                        <tr>
                                            <td colSpan={5} className="text-center py-8 text-base-content/60">
                                                No transactions yet
                                            </td>
                                        </tr>
                                    ) : (
                                        wallet?.transactions.map((transaction) => {
                                            const typeText = mapTransactionType(transaction.type);
                                            const statusText = mapTransactionStatus(transaction.status);
                                            return (
                                                <tr key={transaction.id}>
                                                    <td>{formatDate(transaction.createdAt)}</td>
                                                    <td>
                                                        <span className="flex items-center gap-2">
                                                            {getTypeIcon(transaction.type)}
                                                            {typeText}
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
                                                    <td className={`font-mono font-semibold ${
                                                        typeText === "Deposit" || typeText === "RewardPayout" || typeText === "Refund" 
                                                            ? 'text-success' 
                                                            : 'text-error'
                                                    }`}>
                                                        {(typeText === "Deposit" || typeText === "RewardPayout" || typeText === "Refund") ? '+' : '-'}
                                                        {formatCurrency(Math.abs(transaction.amount))}
                                                    </td>
                                                    <td>
                                                        <span className={`badge badge-sm ${getStatusColor(transaction.status)}`}>
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
                    </div>
                </div>
            </div>
        </div>
    )
}
