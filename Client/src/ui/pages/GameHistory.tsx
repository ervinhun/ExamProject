import {useMemo, useState} from "react";
import {useAtom} from "jotai";
import {authAtom} from "@core/atoms/auth.ts";
import EnterWinningNumbers from "@ui/pages/Admin/Games/EnterWinningNumbers.tsx";

interface WeekResult {
    id: number;
    date: string;
    income?: number;
    winners?: number;
    winAmount?: number;
    numbers: number[];
}

// Helper: get last Saturday
function getLastSaturday(offsetWeeks: number): Date {
    const today = new Date();
    const date = new Date(today);

    // Move to last Saturday
    const day = today.getDay();
    const diff = (day === 6 ? 0 : day + 1); // days after Saturday
    date.setDate(today.getDate() - diff - offsetWeeks * 7);

    return date;
}

function generateWinningNumbers(): number[] {
    const nums = new Set<number>();
    while (nums.size < 3) nums.add(Math.floor(Math.random() * 16) + 1);
    return [...nums];
}

function generateWinningAmount(): number {
    const winChance = 0.2;

    if (Math.random() < winChance) {
        const min = 100;
        const max = 700;
        return Math.floor(Math.random() * (max - min + 1) + min);
    }
    return 0;
}

function generateIncome() {
    const ticketPrice = [20, 40, 80, 160];
    let total = 0;
    for (const price of ticketPrice) {
        const count = Math.floor(Math.random() * 20);
        total += price * count;
    }
    return total;
}

export default function GameHistory() {
    const [auth] = useAtom(authAtom);
    const [page, setPage] = useState(1);

    function isPlayer() {
        return auth.roles.includes(0);
    }

    function isAdmin() {
        return auth.roles.includes(2);
    }

    // Generate 35 weeks of data only once
    const data = useMemo<WeekResult[]>(() => {
        const arr: WeekResult[] = [];
        for (let i = 0; i < 35; i++) {
            const date = getLastSaturday(i);

            if (isPlayer())
                arr.push({
                    id: i + 1,
                    date: date.toISOString().split("T")[0],
                    winAmount: generateWinningAmount(),
                    numbers: generateWinningNumbers()
                });

            if (isAdmin())
                arr.push({
                    id: i + 1,
                    date: date.toISOString().split("T")[0],
                    income: generateIncome(),
                    winners: Math.floor(Math.random() * 200),
                    numbers: generateWinningNumbers()
                });
        }
        return arr;
    }, [auth.roles]);

    const pageSize = 10;
    const pageCount = Math.ceil(data.length / pageSize);
    const start = (page - 1) * pageSize;
    const pageItems = data.slice(start, start + pageSize);


    return (
        <div className="flex flex-col items-center w-full mt-10">
            {isAdmin() && (
                <div className="my-dock rounded-xl bg-base-300 shadow-md p-6 mb-10 w-full max-w-5xl text-center">
                    <EnterWinningNumbers/>
                </div>
            )};

            <div className="my-dock rounded-xl bg-base-300 shadow-md p-6 w-full max-w-5xl mx-auto mt-10">

                <h1 className="text-3xl font-bold mb-6 text-center">
                    Game History
                </h1>

                <h2 className="text-xl font-semibold mb-4">
                    Last {pageItems.length} Results
                </h2>

                <div className="flex flex-col gap-4">
                    {pageItems.map((week) => (
                        <div key={week.id}
                             className="p-4 rounded-lg bg-base-200 shadow flex items-center justify-between">

                            {/* Winning Numbers */}
                            <div className="flex items-center gap-3">
                                {week.numbers.map((n, idx) => (
                                    <span
                                        key={idx}
                                        className="w-10 h-10 flex items-center justify-center rounded-full bg-primary text-white font-bold text-lg"
                                    >
                                    {n}
                                </span>
                                ))}
                            </div>
                            <div>
                                {isAdmin() && <div>Online income: {week.income}kr</div>}
                            </div>
                            {/* Winners + Date */}
                            <div className="text-right">
                                <div className="font-semibold">
                                    {isAdmin() && <div>Winners: {week.winners}</div>}
                                    {isPlayer() && <div>Won: {week.winAmount}kr</div>}
                                </div>
                                <div className="text-sm opacity-70">
                                    Closed: {week.date}
                                </div>
                            </div>

                        </div>
                    ))}
                </div>

                {/* Pagination */}
                <div className="flex justify-center gap-4 mt-6">
                    <button
                        onClick={() => setPage((p) => Math.max(1, p - 1))}
                        disabled={page === 1}
                        className="btn btn-secondary"
                    >
                        Prev
                    </button>

                    <span className="text-lg font-semibold">
                    Page {page} / {pageCount}
                </span>

                    <button
                        onClick={() => setPage((p) => Math.min(pageCount, p + 1))}
                        disabled={page === pageCount}
                        className="btn btn-secondary"
                    >
                        Next
                    </button>
                </div>
            </div>
        </div>
    );
}
