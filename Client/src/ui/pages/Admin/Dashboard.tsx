import {useEffect} from "react";
import { NavLink } from "react-router-dom";
import { useAtom } from "jotai";
import { approveTransactionAtom, fetchPendingTransactionsAtom, pendingTransactionsAtom } from "@core/atoms/transaction";
import { mapTransactionStatus, mapTransactionType } from "@core/types/transaction";


// ---------- UTILS ----------
function getNextSaturdayAt17() {
    const now = new Date();
    const target = new Date();

    const day = now.getDay(); // 0=Sun, 6=Sat
    const daysToSaturday = (6 - day + 7) % 7;

    target.setDate(now.getDate() + daysToSaturday);
    target.setHours(17, 0, 0, 0);

    // If it's already Saturday 17:00 or later → next week
    if (now > target) {
        target.setDate(target.getDate() + 7);
    }

    return target;
}

// Main countdown hook
// function useCountdown() {
//     const [remaining, setRemaining] = useState(() => {
//         return getNextSaturdayAt17().getTime() - Date.now();
//     });

//     useEffect(() => {
//         const interval = setInterval(() => {
//             setRemaining(getNextSaturdayAt17().getTime() - Date.now());
//         }, 1000);

//         return () => clearInterval(interval);
//     }, []);

//     const totalSeconds = Math.max(0, Math.floor(remaining / 1000));

//     const days = Math.floor(totalSeconds / (3600 * 24));
//     const hours = Math.floor((totalSeconds % (3600 * 24)) / 3600);
//     const minutes = Math.floor((totalSeconds % 3600) / 60);
//     const seconds = totalSeconds % 60;

//     return {days, hours, minutes, seconds};
// }

// ---------- COMPONENT ----------
export default function Dashboard() {
    // const countdown = useCountdown();
    const [pendingTransactions] = useAtom(pendingTransactionsAtom);
    const [,fetchPendingTransactions] = useAtom(fetchPendingTransactionsAtom);
    const [,approveTransaction] = useAtom(approveTransactionAtom);

    const stats = {
        activeGames: 3,
        totalPlayers: 127,
        totalRevenue: 4580.50,
        pendingTransactions: pendingTransactions.length,
        pendingPlayers: 2,
        recentActivity: 23
    };

    useEffect(() => {
        if(pendingTransactions.length === 0){
            fetchPendingTransactions().catch((err) => {
                console.error("Error fetching pending transactions:", err);
            }); 
        }
    }, []);

    const confirmTransaction = async (id: number) => {
        // TODO: Implement actual transaction confirmation API call
        await approveTransaction(id).then(() => {
            console.log("Transaction confirmed:", id);
        }).catch((err) => {
            console.error("Error confirming transaction:", err);
        }).finally(() => {
            fetchPendingTransactions().catch((err) => {
                console.error("Error fetching pending transactions:", err);
            }); 
        });
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

    const getStatusColor = (status: string) => {
        switch (status) {
            case "Active": return "badge-success";
            case "Pending Draw": return "badge-warning";
            case "Pending": return "badge-warning";
            case "Completed": return "badge-info";
            default: return "badge-ghost";
        }
    };

    return (
        <div className="container mx-auto px-4 py-6 my-7">
            <div className="space-y-8">
            {/* Header */}
            <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">

                <div>
                    <h1 className="text-4xl font-bold text-primary">Dashboard</h1>
                    <p className="text-base text-base-content/70 mt-1">Overview of your lottery system</p>
                </div>
            </div>

            {/* Stats Cards */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 ">
                <div className="stats shadow bg-base-200">
                    <div className="stat">
                        <div className="stat-figure text-info">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="inline-block w-8 h-8 stroke-current">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                            </svg>
                        </div>
                        <div className="stat-title">Pending Players</div>
                        <div className="stat-value text-base-content">{stats.pendingPlayers}</div>
                    </div>
                </div>
                <div className="stats shadow bg-base-200">
                    <div className="stat">
                        <div className="stat-figure text-warning">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="inline-block w-8 h-8 stroke-current">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <div className="stat-title">Pending Transactions</div>
                        <div className="stat-value text-base-content">{stats.pendingTransactions}</div>
                    </div>
                </div>

                <div className="stats shadow bg-primary text-primary-content">
                    <div className="stat">
                        <div className="stat-figure text-primary-content">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="inline-block w-8 h-8 stroke-current">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <div className="stat-title text-primary-content opacity-80">Next Draw</div>
                        <div className="stat-value text-primary-content">
                            {/* {countdown.days}d {countdown.hours}h {countdown.minutes}m */}
                        </div>
                    </div>
                </div>
            </div>

            {/* Pending Confirmations */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Players to Confirm */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title mb-4">Pending Player Applications</h2>
                        <div className="overflow-x-auto">
                            <table className="table table-zebra">
                                <thead>
                                    <tr className="text-base">
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {/* {players.map(p => (
                                        <tr
                                            key={p.id}
                                            className={`transition-opacity duration-500 ${
                                                p.removing ? "opacity-0" : "opacity-100"
                                            }`}
                                        >
                                            <td className="font-semibold">{p.name}</td>
                                            <td>{p.email}</td>
                                            <td>
                                                {p.confirmed ? (
                                                    <span className="text-success text-xl font-bold">✔</span>
                                                ) : (
                                                    <button
                                                        onClick={() => confirmPlayer(p.id)}
                                                        className="btn btn-xs btn-success"
                                                    >
                                                        Approve
                                                    </button>
                                                )}
                                            </td>
                                        </tr>
                                    ))} */}
                                </tbody>
                            </table>
                        </div>
                        <NavLink to="/admin/players/applications" className="btn btn-sm btn-ghost mt-2">
                            View All Applications →
                        </NavLink>
                    </div>
                </div>
                {/* Transactions to Confirm */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title mb-4">Pending Transactions</h2>
                        <div className="overflow-x-auto">
                            <table className="table table-zebra">
                                <thead>
                                    <tr className="text-base">
                                        <th>Name</th>
                                        <th>Type</th>
                                        <th>Amount</th>
                                        <th>Date</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {pendingTransactions.length === 0 ? (
                                        <tr>
                                            <td colSpan={5} className="text-center py-8 text-base-content/60">
                                                No pending transactions
                                            </td>
                                        </tr>
                                    ) : (
                                        pendingTransactions.map(t => {
                                            const statusText = mapTransactionStatus(t.status);
                                            const typeText = mapTransactionType(t.type);
                                            const getStatusBadge = () => {
                                                switch (statusText) {
                                                    case "Approved": return "badge-success";
                                                    case "Pending": return "badge-warning";
                                                    case "Rejected": return "badge-error";
                                                    case "Canceled": return "badge-ghost";
                                                    default: return "badge-ghost";
                                                }
                                            };
                                            
                                            return (
                                                <tr key={t.id}>
                                                    <td className="font-semibold">{t.name}</td>
                                                    <td>
                                                        <span className="badge badge-sm">{typeText}</span>
                                                    </td>
                                                    <td className="font-mono">{t.amount.toFixed(2)} DKK</td>
                                                    <td className="text-sm">{formatDate(t.createdAt)}</td>
                                                    <td>
                                                        {statusText === "Approved" ? (
                                                            <span className="badge badge-success badge-sm">Approved</span>
                                                        ) : (
                                                            <button
                                                                onClick={() => confirmTransaction(t.id)}
                                                                className="btn btn-xs btn-primary"
                                                            >
                                                                Confirm
                                                            </button>
                                                        )}
                                                    </td>
                                                </tr>
                                            );
                                        })
                                    )}
                                </tbody>
                            </table>
                        </div>
                        <NavLink to="/admin/transactions/pending" className="btn btn-sm btn-ghost mt-2">
                            View All Transactions →
                        </NavLink>
                    </div>
                </div>
            </div>

            {/* Recent Players Info */}
            <div className="card bg-base-200 shadow-lg">
                <div className="card-body">
                    <div className="flex justify-between items-center mb-4">
                        <h2 className="card-title">Recent Players</h2>
                        <NavLink to="/admin/players" className="btn btn-sm btn-ghost">
                            View All Players →
                        </NavLink>
                    </div>
                    
                    <div className="overflow-x-auto">
                        <table className="table table-zebra">
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Email</th>
                                    <th>Join Date</th>
                                    <th>Balance</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {/* {recentPlayers.map((player) => (
                                    <tr key={player.id}>
                                        <td className="font-semibold">{player.name}</td>
                                        <td>{player.email}</td>
                                        <td>{player.joinDate}</td>
                                        <td className="font-mono">${player.balance}</td>
                                        <td>
                                            <div className="flex gap-2">
                                                <button className="btn btn-xs btn-info">View</button>
                                                <button className="btn btn-xs btn-ghost">Edit</button>
                                            </div>
                                        </td>
                                    </tr>
                                ))} */}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            </div>
        </div>
    );
}
