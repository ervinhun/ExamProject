import { useEffect } from "react";
import { useAtom } from "jotai";
import { allGamesAtom, fetchAllGamesAtom } from "@core/atoms/game";
import { formatDateTime } from "@utils/dateUtils";

export default function GamesHistory() {
    const [allGames] = useAtom(allGamesAtom);
    const [, fetchAllGames] = useAtom(fetchAllGamesAtom);

    useEffect(() => {
        fetchAllGames();
    }, [fetchAllGames]);

    // Filter only completed games (status === 1) and sort by date (newest first)
    const completedGames = allGames
        .filter(game => game.status === 1)
        .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

    return (
        <div className="container mx-auto">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary ml-3">Games History</h1>
                        <p className="text-base text-base-content/70 mt-1 ml-3">View all completed lottery games</p>
                    </div>
                </div>

                {/* Summary Stats */}
                {completedGames.length > 0 && (
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <div className="stats shadow bg-base-200">
                            <div className="stat">
                                <div className="stat-title">Total Completed Games</div>
                                <div className="stat-value text-primary">{completedGames.length}</div>
                            </div>
                        </div>

                        <div className="stats shadow bg-base-200">
                            <div className="stat">
                                <div className="stat-title">Total Tickets Sold</div>
                                <div className="stat-value text-secondary">
                                    {completedGames.reduce((sum, game) => sum + (game.ticketsSold || 0), 0)}
                                </div>
                            </div>
                        </div>

                        <div className="stats shadow bg-base-200">
                            <div className="stat">
                                <div className="stat-title">Total Prize Pool</div>
                                <div className="stat-value text-accent">
                                    {completedGames.reduce((sum, game) => sum + (game.prizePool || 0), 0).toLocaleString()} DKK
                                </div>
                            </div>
                        </div>
                    </div>
                )}

                {/* Games History Table */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Completed Games</h2>
                        
                        <div className="overflow-x-auto">
                            <table className="table table-zebra">
                                <thead>
                                    <tr>
                                        <th>Game Name</th>
                                        <th>Year</th>
                                        <th>Week</th>
                                        <th>Draw Date</th>
                                        <th>Winning Numbers</th>
                                        <th>Participants</th>
                                        <th>Tickets Sold</th>
                                        <th>Tickets Won</th>
                                        <th>Prize Pool</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {completedGames.length === 0 ? (
                                        <tr>
                                            <td colSpan={9} className="text-center py-8 text-base-content/60">
                                                No completed games yet
                                            </td>
                                        </tr>
                                    ) : (
                                        completedGames.map((game) => {
                                            const drawDateText = game.isAutoRepeatable && game.drawDayOfWeek !== undefined && game.drawTimeOfDay
                                                ? `${["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"][game.drawDayOfWeek]}, ${game.drawTimeOfDay.substring(0, 5)}`
                                                : game.drawDate ? formatDateTime(game.drawDate) : 'N/A';
                                            
                                            return (
                                                <tr key={game.id}>
                                                    <td className="font-semibold">{game.template?.name || "Unknown"}</td>
                                                    <td>{game.year}</td>
                                                    <td>Week {game.week}</td>
                                                    <td>{drawDateText}</td>
                                                    <td>
                                                        <div className="flex flex-wrap gap-2">
                                                            {game.winningNumbers && game.winningNumbers.length > 0 ? (
                                                                game.winningNumbers.map((num, idx) => (
                                                                    <div
                                                                        key={idx}
                                                                        className="w-8 h-8 flex items-center justify-center rounded-full bg-success text-white font-semibold shadow text-sm"
                                                                    >
                                                                        {num}
                                                                    </div>
                                                                ))
                                                            ) : (
                                                                <span className="text-base-content/60">No numbers drawn</span>
                                                            )}
                                                        </div>
                                                    </td>
                                                    <td>{game.participants || 0}</td>
                                                    <td>{game.ticketsSold || 0}</td>
                                                    <td>{game.ticketsWon || 0}</td>
                                                    <td className="font-mono">{game.prizePool || 0} DKK</td>
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
