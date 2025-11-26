import {useEffect, useState} from "react";

// ---------- TYPES ----------
type Transaction = {
    id: number;
    name: string;
    amount: number;
    date: string;
    confirmed: boolean;
    removing?: boolean;
};

type Player = {
    id: number;
    name: string;
    confirmed: boolean;
    removing?: boolean;
};

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

    const [transactions, setTransactions] = useState<Transaction[]>([
        {id: 482193, name: "Alice Jensen", amount: 740, date: "2025-11-25 14:23", confirmed: false},
        {id: 905127, name: "Mark Sørensen", amount: 120, date: "2025-11-26 02:41", confirmed: false},
        {id: 771520, name: "Laura Schmidt", amount: 960, date: "2025-11-24 22:09", confirmed: false},
        {id: 333874, name: "Jonas Holm", amount: 380, date: "2025-11-25 19:56", confirmed: false},
    ]);

    const [players, setPlayers] = useState<Player[]>([
        {id: 1, name: "Bjørn Ludvigsen", confirmed: false},
        {id: 2, name: "Anette Lassen", confirmed: false}
    ]);

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

    return (
        <div className="flex flex-col items-center w-full mt-10">

            {/* ---------------- COUNTDOWN BANNER ---------------- */}
            <div className="my-dock rounded-xl bg-base-300 shadow-md p-6 mb-10 w-full max-w-5xl text-center">
                <h1 className="text-3xl font-bold mb-4">
                    Countdown to Saturday, 17:00
                </h1>
                <div className="text-4xl font-mono">
                    {countdown.days}d : {countdown.hours}h : {countdown.minutes}m : {countdown.seconds}s
                </div>
            </div>

            {/* ---------------- SIDE BY SIDE PANELS ---------------- */}
            <div className="my-dock rounded-xl bg-base-300 shadow-md p-6 w-full max-w-5xl">

                {/* -------- TRANSACTIONS -------- */}
                <div className="flex flex-col md:flex-row justify-between gap-10">
                    <div className="flex-1">
                        <h2 className="mb-4 text-xl font-semibold">Transactions to Confirm</h2>

                        <table className="border-collapse border border-gray-300">
                            <thead>
                            <tr className="bg-gray-100">
                                <th className="border px-4 py-2">Name</th>
                                <th className="border px-4 py-2">Transaction</th>
                                <th className="border px-4 py-2">Amount</th>
                                <th className="border px-4 py-2">Date</th>
                                <th className="border px-4 py-2">Confirm</th>
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
                                    <td className="border px-4 py-2">{t.name}</td>
                                    <td className="border px-4 py-2">{t.id}</td>
                                    <td className="border px-4 py-2">{t.amount}</td>
                                    <td className="border px-4 py-2">{t.date}</td>
                                    <td className="border px-4 py-2 text-center">
                                        {t.confirmed ? (
                                            <span className="text-green-600 text-xl font-bold">✔</span>
                                        ) : (
                                            <button
                                                onClick={() => confirmTransaction(t.id)}
                                                className="px-3 py-1 bg-blue-500 hover:bg-blue-600 text-white rounded"
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

                    {/* -------- PLAYERS -------- */}
                    <div>
                        <h2 className="mb-4 text-xl font-semibold">Players to Confirm</h2>

                        <table className="border-collapse border border-gray-300">
                            <thead>
                            <tr className="bg-gray-100">
                                <th className="border px-4 py-2">Player Name</th>
                                <th className="border px-4 py-2">Confirm</th>
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
                                    <td className="border px-4 py-2">{p.name}</td>
                                    <td className="border px-4 py-2 text-center">
                                        {p.confirmed ? (
                                            <span className="text-green-600 text-xl font-bold">✔</span>
                                        ) : (
                                            <button
                                                onClick={() => confirmPlayer(p.id)}
                                                className="px-3 py-1 bg-green-500 hover:bg-green-600 text-white rounded"
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
                </div>
            </div>
        </div>
    );
}
