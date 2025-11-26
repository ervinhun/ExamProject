import {useMemo, useState} from "react";
import {PlayerResult, calculateWinnersShare, generateWeeks, Week} from "@utils/WinnerCalculationHelper.ts";


export default function WinningBoardsAdmin() {
    const [selectedWeekId, setSelectedWeekId] = useState<string>("");
    const [offlineIncome, setOfflineIncome] = useState<number>(0);
    const [offlineWinnersInput, setOfflineWinnersInput] = useState<string>("0");
    const weeks = useMemo(() => generateWeeks(), []);

    const selectedWeek = useMemo<Week | undefined>(
        () => weeks.find(w => w.id === selectedWeekId),
        [weeks, selectedWeekId]
    );

    const offlineWinners = useMemo(() => {
        const n = Number(offlineWinnersInput);
        if (!Number.isFinite(n) || Number.isNaN(n)) return 0;
        return Math.max(0, Math.floor(n));
    }, [offlineWinnersInput]);

    const onlineWinners = useMemo(() => {
        if (!selectedWeek) return [] as PlayerResult[];
        const players = selectedWeek.players;
        const winnersCount = Math.max(1, Math.floor(players.length * 0.2));
        return players.slice(0, winnersCount);
    }, [selectedWeek]);

    const totalWinners = onlineWinners.length + offlineWinners;

    const winAmount = useMemo(() => {
        if (!selectedWeek) return 0;
        return calculateWinnersShare(selectedWeek.onlineIncome, offlineIncome, totalWinners);
    }, [selectedWeek, offlineIncome, totalWinners]);


    return (
        <div className="p-6 max-w-4xl mx-auto">
            {/* Dropdown */}
            <div className="flex items-center gap-4 mb-6">
                <select
                    value={selectedWeekId}
                    onChange={(e) => {
                        setSelectedWeekId(e.target.value);
                        setOfflineWinnersInput("0");
                        setOfflineIncome(0);
                    }}
                    className="select select-bordered"
                >
                    <option value="">Choose week</option>

                    {weeks.map((w) => (
                        <option key={w.id} value={w.id}>
                            {w.date} (W{w.weekNumber})
                        </option>
                    ))}
                </select>

                {/* Offline income */}
                Offline income:
                <input
                    type="number"
                    id="offline-income"
                    placeholder="Offline income"
                    alt="Offline income"
                    className="input input-bordered w-40"
                    value={offlineIncome}
                    onChange={(e) => {
                        const v = Number(e.target.value);
                        setOfflineIncome(Number.isFinite(v) && !Number.isNaN(v) ? Math.max(0, v) : 0);
                    }}
                />

                {/* Offline winners */}
                Offline winners:
                <input
                    type="number"
                    id="offline-winners"
                    placeholder="Offline winners"
                    alt="Offline winners"
                    className="input input-bordered w-40"
                    value={offlineWinners}
                    onChange={(e) => setOfflineWinnersInput(e.target.value)}
                />
            </div>

            {/* Results Section */}
            {selectedWeek && (
                <div className="rounded-xl bg-base-200 p-6 shadow-md">

                    <h2 className="text-xl font-bold mb-4">
                        Week {selectedWeek.weekNumber} Summary
                    </h2>

                    <p><strong>Total online income:</strong> {selectedWeek.onlineIncome}</p>
                    <p><strong>Total players:</strong> {selectedWeek.players.length}</p>
                    <p><strong>Total winners (online+offline):</strong> {totalWinners}</p>
                    <p><strong>Winning amount per player:</strong> {winAmount}</p>

                    <table className="table mt-6">
                        <thead>
                        <tr>
                            <th>Name</th>
                            <th>Numbers Played</th>
                            <th>Winning</th>
                        </tr>
                        </thead>
                        <tbody>
                        {selectedWeek.players.map((p: PlayerResult) => (
                            <tr key={p.id}>
                                <td>{p.name}</td>
                                <td>{p.numbersPlayed}</td>
                                <td>{winAmount}kr</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}
