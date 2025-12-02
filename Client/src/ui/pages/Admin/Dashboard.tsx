import {useEffect, useState} from "react";
import { NavLink } from "react-router-dom";

// ---------- TYPES ----------
type Transaction = {
    id: number;
    name: string;
    amount: number;
    type: string;
    date: string;
    confirmed: boolean;
    removing?: boolean;
};

type Player = {
    id: number;
    name: string;
    email: string;
    balance: number;
    status: string;
    confirmed: boolean;
    removing?: boolean;
};

// type Game = {
//     id: string;
//     name: string;
//     status: string;
//     drawDate: string;
//     participants: number;
//     ticketsSold: number;
// };

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
function useCountdown() {
    const [remaining, setRemaining] = useState(() => {
        return getNextSaturdayAt17().getTime() - Date.now();
    });

    useEffect(() => {
        const interval = setInterval(() => {
            setRemaining(getNextSaturdayAt17().getTime() - Date.now());
        }, 1000);

        return () => clearInterval(interval);
    }, []);

    const totalSeconds = Math.max(0, Math.floor(remaining / 1000));

    const days = Math.floor(totalSeconds / (3600 * 24));
    const hours = Math.floor((totalSeconds % (3600 * 24)) / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;

    return {days, hours, minutes, seconds};
}

// ---------- COMPONENT ----------
export default function Dashboard() {
    const countdown = useCountdown();

    const stats = {
        activeGames: 3,
        totalPlayers: 127,
        totalRevenue: 4580.50,
        pendingTransactions: 4,
        pendingPlayers: 2,
        recentActivity: 23
    };

    const [transactions, setTransactions] = useState<Transaction[]>([
        {id: 482193, name: "Alice Jensen", amount: 740, date: "2025-11-25 14:23", confirmed: false,type:"deposit"},
        {id: 905127, name: "Mark Sørensen", amount: 120, date: "2025-11-26 02:41", confirmed: false,type:"deposit"},
        {id: 771520, name: "Laura Schmidt", amount: 960, date: "2025-11-24 22:09", confirmed: false,type:"Deposit"},
        {id: 333874, name: "Jonas Holm", amount: 380, date: "2025-11-25 19:56", confirmed: false, type:"Deposit" },
    ]);

    const [players, setPlayers] = useState<Player[]>([
        {id: 1, name: "Bjørn Ludvigsen", email: "bjorn@example.com", balance: 250, status: "Pending", confirmed: false},
        {id: 2, name: "Anette Lassen", email: "anette@example.com", balance: 0, status: "Pending", confirmed: false}
    ]);


    const recentPlayers = [
        {id: 101, name: "Emma Nielsen", email: "emma@example.com", joinDate: "2025-11-26", balance: 150},
        {id: 102, name: "Oliver Hansen", email: "oliver@example.com", joinDate: "2025-11-25", balance: 300},
        {id: 103, name: "Sofia Andersen", email: "sofia@example.com", joinDate: "2025-11-24", balance: 75},
        {id: 104, name: "Lucas Petersen", email: "lucas@example.com", joinDate: "2025-11-23", balance: 200}
    ];

    const confirmTransaction = (id: number) => {
        setTransactions(prev =>
            prev.map(tran => (tran.id === id ? {...tran, confirmed: true} : tran))
        );
        setTimeout(() => {
            setTransactions(prev =>
                prev.map(tran => (tran.id === id ? {...tran, removing: true} : tran))
            );
        }, 2000);
        setTimeout(() => {
            setTransactions(prev => prev.filter(t => t.id !== id));
        }, 2600);
    };

    const confirmPlayer = (id: number) => {
        setPlayers(prev =>
            prev.map(p => (p.id === id ? {...p, confirmed: true} : p))
        );
        setTimeout(() => {
            setPlayers(prev =>
                prev.map(p => (p.id === id ? {...p, removing: true} : p))
            );
        }, 2000);
        setTimeout(() => {
            setPlayers(prev => prev.filter(p => p.id !== id));
        }, 2600);
    };

    // const formatDate = (dateStr: string) => {
    //     const date = new Date(dateStr);
    //     return date.toLocaleDateString("en-US", { 
    //         month: "short", 
    //         day: "numeric", 
    //         hour: "2-digit", 
    //         minute: "2-digit" 
    //     });
    // };

    // const getStatusColor = (status: string) => {
    //     switch (status) {
    //         case "Active": return "badge-success";
    //         case "Pending Draw": return "badge-warning";
    //         case "Pending": return "badge-warning";
    //         case "Completed": return "badge-info";
    //         default: return "badge-ghost";
    //     }
    // };

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
                            {countdown.days}d {countdown.hours}h {countdown.minutes}m
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
                                    {players.map(p => (
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
                                    {transactions.map(t => (
                                        <tr
                                            key={t.id}
                                            className={`transition-opacity duration-500 ${
                                                t.removing ? "opacity-0" : "opacity-100"
                                            }`}
                                        >
                                            <td className="font-semibold">{t.name}</td>
                                            <td className="font-mono">{t.type}</td>
                                            <td className="font-mono">${t.amount}</td>
                                            <td className="text-sm">{t.date}</td>
                                            <td>
                                                {t.confirmed ? (
                                                    <span className="text-success text-xl font-bold">✔</span>
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
                                    ))}
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
                                {recentPlayers.map((player) => (
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
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            </div>
        </div>
    );
}
