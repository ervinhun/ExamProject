import { useEffect, useState } from "react";
import { useAtom, useSetAtom } from "jotai";
import { allGamesAtom, fetchAllGamesAtom } from "@core/atoms/game";
import { formatDateTime } from "@utils/dateUtils";
import { ticketApi } from "@core/api/controllers/ticket";
import type { MyTicketDto } from "@core/types/ticket";
import type { GameInstanceDto } from "@core/types/game";
import { addNotificationAtom } from "@core/atoms/error";

export default function GamesHistory() {
    const [allGames] = useAtom(allGamesAtom);
    const [, fetchAllGames] = useAtom(fetchAllGamesAtom);
    const addNotification = useSetAtom(addNotificationAtom);

    // Modal state
    const [selectedGame, setSelectedGame] = useState<GameInstanceDto | null>(null);
    const [winningTickets, setWinningTickets] = useState<MyTicketDto[]>([]);
    const [isLoadingTickets, setIsLoadingTickets] = useState(false);

    useEffect(() => {
        fetchAllGames();
    }, [fetchAllGames]);

    // Filter only completed games (status === 1) and sort by date (newest first)
    const completedGames = allGames
        .filter(game => game.status === 1)
        .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

    const handleViewWinners = async (game: GameInstanceDto) => {
        if (!game.id) {
            addNotification({
                type: "error",
                message: "Game ID is missing"
            });
            return;
        }
        
        setSelectedGame(game);
        setIsLoadingTickets(true);
        try {
            const tickets = await ticketApi.getAllWinningTicketsForGameId(game.id);
            setWinningTickets(tickets);
        } catch (error) {
            const errorMessage = error instanceof Error ? error.message : String(error);
            addNotification({
                type: "error",
                message: errorMessage || "Failed to load winning tickets"
            });
        } finally {
            setIsLoadingTickets(false);
        }
    };

    const handleCloseModal = () => {
        setSelectedGame(null);
        setWinningTickets([]);
    };

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
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {completedGames.length === 0 ? (
                                        <tr>
                                            <td colSpan={10} className="text-center py-8 text-base-content/60">
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
                                                    <td>
                                                        <button
                                                            className="btn btn-sm btn-info"
                                                            onClick={() => handleViewWinners(game)}
                                                        >
                                                            <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                                                            </svg>
                                                            Winners
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

                {/* Winning Tickets Modal */}
                {selectedGame && (
                    <div className="modal modal-open">
                        <div className="modal-box max-w-4xl">
                            <div className="flex items-center justify-between mb-6">
                                <div>
                                    <h3 className="text-2xl font-bold flex items-center gap-2">
                                        <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-success" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                                        </svg>
                                        Winning Tickets
                                    </h3>
                                    <p className="text-base-content/70 mt-1">
                                        {selectedGame.template?.name} - Week {selectedGame.week}, {selectedGame.year}
                                    </p>
                                </div>
                                <button
                                    className="btn btn-sm btn-circle btn-ghost"
                                    onClick={handleCloseModal}
                                >
                                    ✕
                                </button>
                            </div>

                            {/* Winning Numbers Display */}
                            {selectedGame.winningNumbers && selectedGame.winningNumbers.length > 0 && (
                                <div className="alert alert-info mb-4">
                                    <div className="flex items-center gap-2">
                                        <span className="font-semibold">Winning Numbers:</span>
                                        <div className="flex flex-wrap gap-2">
                                            {selectedGame.winningNumbers.map((num, idx) => (
                                                <div
                                                    key={idx}
                                                    className="w-8 h-8 flex items-center justify-center rounded-full bg-success text-white font-semibold shadow text-sm"
                                                >
                                                    {num}
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                </div>
                            )}

                            {isLoadingTickets ? (
                                <div className="flex justify-center items-center py-12">
                                    <span className="loading loading-spinner loading-lg text-primary"></span>
                                </div>
                            ) : winningTickets.length === 0 ? (
                                <div className="text-center py-12 text-base-content/60">
                                    <svg xmlns="http://www.w3.org/2000/svg" className="h-16 w-16 mx-auto mb-4 opacity-50" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                    <p className="text-lg">No winning tickets for this game</p>
                                </div>
                            ) : (
                                <div className="overflow-x-auto">
                                    <table className="table table-zebra">
                                        <thead>
                                            <tr>
                                                <th>Player Name</th>
                                                <th>Email</th>
                                                <th>Ticket Numbers</th>
                                                <th>Purchased At</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {winningTickets.map((ticket) => (
                                                <tr key={ticket.id}>
                                                    <td className="font-semibold">
                                                        {ticket.player?.firstName} {ticket.player?.lastName}
                                                    </td>
                                                    <td>{ticket.player?.email}</td>
                                                    <td>
                                                        <div className="flex flex-wrap gap-2">
                                                            {ticket.pickedNumbers.map((num, idx) => (
                                                                <div
                                                                    key={idx}
                                                                    className={`w-8 h-8 flex items-center justify-center rounded-full font-semibold shadow text-sm ${
                                                                        selectedGame.winningNumbers?.includes(num)
                                                                            ? 'bg-success text-white'
                                                                            : 'bg-base-300 text-base-content'
                                                                    }`}
                                                                >
                                                                    {num}
                                                                </div>
                                                            ))}
                                                        </div>
                                                    </td>
                                                    <td>{formatDateTime(ticket.boughtAt)}</td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            )}

                            <div className="modal-action">
                                <button className="btn btn-primary" onClick={handleCloseModal}>
                                    Close
                                </button>
                            </div>
                        </div>
                        <div className="modal-backdrop" onClick={handleCloseModal}></div>
                    </div>
                )}
            </div>
        </div>
    );
}
