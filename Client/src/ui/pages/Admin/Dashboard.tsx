import {useEffect, useState} from "react";
import {NavLink} from "react-router-dom";
import {useAtom} from "jotai";
import {approveTransactionAtom, fetchPendingTransactionsAtom, pendingTransactionsAtom} from "@core/atoms/transaction";
import {mapTransactionStatus, mapTransactionType} from "@core/types/transaction";
import {AppliedUser} from "@core/types/users.ts";
import {userApi} from "@core/api/controllers/user.ts";
import getAge from "@utils/getAge.ts";
import {activeGamesAtom, fetchActiveGamesAtom} from "@core/atoms/game";
import {getNextGame, getNextDrawTime, calculateCountdown} from "@utils/countdownUtils";

// Countdown hook
function useCountdown(targetDate: Date | null) {
    const [countdown, setCountdown] = useState(() => calculateCountdown(targetDate));

    useEffect(() => {
        if (!targetDate) {
            setCountdown({ days: 0, hours: 0, minutes: 0, seconds: 0 });
            return;
        }

        const interval = setInterval(() => {
            setCountdown(calculateCountdown(targetDate));
        }, 1000);

        return () => clearInterval(interval);
    }, [targetDate]);

    return countdown;
}

// ---------- COMPONENT ----------
export default function Dashboard() {
    // const countdown = useCountdown();
    const [pendingTransactions] = useAtom(pendingTransactionsAtom);
    const [, fetchPendingTransactions] = useAtom(fetchPendingTransactionsAtom);
    const [, approveTransaction] = useAtom(approveTransactionAtom);
    const [appliedPlayers, setAppliedPlayers] = useState<AppliedUser[]>([]);
    const [activeGames] = useAtom(activeGamesAtom);
    const [, fetchActiveGames] = useAtom(fetchActiveGamesAtom);
    
    // Get the next game and its countdown
    const nextGame = getNextGame(activeGames);
    const nextDrawTime = nextGame ? getNextDrawTime(nextGame) : null;
    const countdown = useCountdown(nextDrawTime);

    const stats = {
        activeGames: 3,
        totalPlayers: 127,
        totalRevenue: 4580.50,
        pendingTransactions: pendingTransactions.length,
        pendingPlayers: 2,
        recentActivity: 23
    };

    useEffect(() => {
        fetchPendingTransactions().catch((err) => {
            console.error("Error fetching pending transactions:", err);
        });
        fetchActiveGames().catch((err) => {
            console.error("Error fetching active games:", err);
        });
    }, []);

    useEffect(() => {
        userApi.getAllAppliedUsers().then((data) => {
            setAppliedPlayers(data);
        });
    }, []);

    for (const p of appliedPlayers) {
        p.age = getAge(p.player.dob);
    }

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

    const confirmPlayer = async (userId: string, isApproved: boolean, isActive: boolean) => {
        const result = await userApi.confirmAppliedUsers(userId, isApproved, isActive);
        if (result) {
            setAppliedPlayers(prev =>
                prev.map(item =>
                    item.player.id === userId
                        ? {
                            ...item,
                            status: isApproved ? "Confirmed" : "Rejected",
                            player: {
                                ...item.player,
                                activated: isActive
                            }
                        }
                        : item
                )
            )
            setTimeout(() => {
                setAppliedPlayers(prev => prev.filter(p => p.player.id !== userId));
            }, 2600)
        }
    }

    return (
        <div className="container mx-auto">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary">Dashboard</h1>
                        <p className="text-base text-base-content/70 mt-1">Overview of your lottery system</p>
                    </div>
                </div>

                {/* Stats Cards */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 ">
                    <div className="stats shadow bg-base-200">
                        <div className="stat">
                            <div className="stat-figure text-info">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                                     className="inline-block w-8 h-8 stroke-current">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                          d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z"/>
                                </svg>
                            </div>
                            <div className="stat-title">Pending Players</div>
                            <div className="stat-value text-base-content">{appliedPlayers.length}</div>
                        </div>
                    </div>
                    <div className="stats shadow bg-base-200">
                        <div className="stat">
                            <div className="stat-figure text-warning">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                                     className="inline-block w-8 h-8 stroke-current">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                          d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                                </svg>
                            </div>
                            <div className="stat-title">Pending Transactions</div>
                            <div className="stat-value text-base-content">{stats.pendingTransactions}</div>
                        </div>
                    </div>

                    <div className="stats shadow bg-primary text-primary-content">
                        <div className="stat">
                            <div className="stat-figure text-primary-content">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                                     className="inline-block w-8 h-8 stroke-current">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                          d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                                </svg>
                            </div>
                            <div className="stat-title text-primary-content opacity-80">
                                Next Draw
                                {nextGame && <span className="text-xs ml-2">({nextGame.template?.name})</span>}
                            </div>
                            <div className="stat-value text-primary-content text-2xl">
                                {nextGame ? (
                                    <>
                                        {countdown.days > 0 && <>{countdown.days}d </>}
                                        {String(countdown.hours).padStart(2, '0')}:
                                        {String(countdown.minutes).padStart(2, '0')}:
                                        {String(countdown.seconds).padStart(2, '0')}
                                    </>
                                ) : (
                                    <span className="text-lg">No active games</span>
                                )}
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
                                    { // Showing only the first 3 applications
                                        appliedPlayers.slice(0, 3).map(a => (
                                                <tr key={a.id}>
                                                    <td className="font-semibold">
                                                        {a.player.firstName} {a.player.lastName}
                                                    </td>

                                                    <td>{a.player.email}</td>

                                                    <td>
                                                        {a.status.trim() === "Confirmed" ? (
                                                            <span className="text-success text-xl font-bold cursor-default">✔</span>
                                                        ) : a.age < 18 ? (
                                                            <button
                                                                onClick={() => confirmPlayer(a.player.id!, false, false)}
                                                                className="btn btn-xs btn-error"
                                                            >
                                                                Decline
                                                            </button>
                                                        ) : (
                                                            <button
                                                                onClick={() => confirmPlayer(a.player.id!, true, false)}
                                                                className="btn btn-xs btn-success"
                                                            >
                                                                Approve
                                                            </button>
                                                        )}
                                                    </td>
                                                </tr>
                                            ))}
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
                                                            <span
                                                                className="badge badge-success badge-sm">Approved</span>
                                                        ) : (
                                                            <button
                                                                onClick={() => approveTransaction(t.id)}
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
