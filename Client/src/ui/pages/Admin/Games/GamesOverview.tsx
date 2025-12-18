import { NavLink } from "react-router-dom";
import { useAtom } from "jotai";
import { activeGamesAtom, fetchActiveGamesAtom, gameTemplatesAtom, fetchGameTemplatesAtom } from "@core/atoms/game";
import { formatDateTime, mapDayOfWeek } from "@utils/dateUtils";
import { getStatusColor, getGameStatus, getWeekBadgeClass } from "@utils/gameUtils";
import { useEffect, useState } from "react";
import type { GameInstanceDto } from "@core/types/game";

export const GamesOverview: React.FC = () => {
    const [activeGames] = useAtom(activeGamesAtom);
    const [, fetchActiveGames] = useAtom(fetchActiveGamesAtom);
    const [templates] = useAtom(gameTemplatesAtom);
    const [, fetchGameTemplates] = useAtom(fetchGameTemplatesAtom);

    const [selectedGame, setSelectedGame] = useState<GameInstanceDto | null>(null);

    useEffect(() => {
        fetchActiveGames();
        if(templates.length === 0) {
            fetchGameTemplates();
        }
    }, []);

    const handleViewGame = (game: GameInstanceDto) => {
        setSelectedGame(game);
    };

    const handleCloseModal = () => {
        setSelectedGame(null);
    };

    const stats = {
        activeGames: activeGames.length,
        totalPlayers: 0, // Placeholder - needs real participant data
        totalRevenue: activeGames.reduce((sum, game) => sum + (game.template?.basePrice || 0) * 10, 0), // Placeholder calculation
        pendingDraws: activeGames.filter(game => getGameStatus(game) === "Pending Draw" && !game.isDrawn).length
    };

    // Get template usage stats
    const templateStats = templates.map(template => ({
        id: template.id,
        name: template.name,
        gamesCreated: activeGames.filter(game => game.template?.id === template.id).length
    })).sort((a, b) => b.gamesCreated - a.gamesCreated).slice(0, 3);

    return (
        <div className="container mx-auto">
            <div className="space-y-8">
            {/* Header */}
            <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                <div className="flex-1">
                    <h1 className="text-4xl font-bold text-primary">Games Overview</h1>
                    <p className="text-base text-base-content/70 mt-1">Manage and monitor all lottery games</p>
                </div>
                <NavLink to="/admin/games/start" className="btn bg-amber-900 hover:bg-amber-800 text-white border-0">
                    + Start New Game
                </NavLink>
            </div>

            {/* Stats Cards */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                <div className="stats shadow bg-base-200">
                    <div className="stat">
                        <div className="stat-figure text-error">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="inline-block w-8 h-8 stroke-current">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                            </svg>
                        </div>
                        <div className="stat-title">Active Games</div>
                        <div className="stat-value text-base-content">{stats.activeGames}</div>
                    </div>
                </div>

                <div className="stats shadow bg-base-200">
                    <div className="stat">
                        <div className="stat-figure text-info">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="inline-block w-8 h-8 stroke-current">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                            </svg>
                        </div>
                        <div className="stat-title">Total Players</div>
                        <div className="stat-value text-base-content">{stats.totalPlayers}</div>
                    </div>
                </div>

                <div className="stats shadow bg-base-200">
                    <div className="stat">
                        <div className="stat-figure text-warning">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="inline-block w-8 h-8 stroke-current">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <div className="stat-title">Pending Draws</div>
                        <div className="stat-value text-base-content">{stats.pendingDraws}</div>
                    </div>
                </div>
            </div>

            {/* Active Games Table */}
            <div className="card bg-base-200 shadow-lg">
                <div className="card-body">
                    <div className="flex justify-between items-center mb-4">
                        <h2 className="card-title">Active Games</h2>
                        <NavLink to="/admin/games/active" className="btn btn-sm btn-ghost">
                            View All →
                        </NavLink>
                    </div>
                    
                    <div className="overflow-x-auto">
                        <table className="table table-zebra">
                            <thead>
                                <tr>
                                    <th>Game Name</th>
                                    <th>Year</th>
                                    <th>Week</th>
                                    <th>Repeatable</th>
                                    <th>Status</th>
                                    <th>Draw Date</th>
                                    <th>Participants</th>
                                    <th>Tickets Sold</th>
                                    <th>Prize Pool</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {activeGames.length === 0 ? (
                                    <tr>
                                        <td colSpan={10} className="text-center py-8 text-base-content/60">
                                            No active games at the moment
                                        </td>
                                    </tr>
                                ) : (
                                    activeGames
                                        .sort((a, b) => {
                                            // Priority: Pending Draw (status 2) games first
                                            const statusA = getGameStatus(a);
                                            const statusB = getGameStatus(b);
                                            
                                            if (statusA === "Pending Draw" && statusB !== "Pending Draw") return -1;
                                            if (statusA !== "Pending Draw" && statusB === "Pending Draw") return 1;
                                            
                                            // Secondary sort by week (newest first)
                                            return b.week - a.week;
                                        })
                                        .slice(0, 5)
                                        .map((game) => {
                                        const statusText = getGameStatus(game);
                                        const drawDateText = game.isAutoRepeatable
                                            ? `${mapDayOfWeek(game.drawDayOfWeek)}, ${game.drawTimeOfDay?.substring(0, 5) || 'N/A'}`
                                            : formatDateTime(game.drawDate);
                                        
                                        return (
                                            <tr key={game.id}>
                                                <td className="font-semibold">{game.template?.name || "Unknown"}</td>
                                                <td>{game.year}</td>
                                                <td>Week {game.week}</td>
                                                <td>
                                                    {game.isAutoRepeatable ? (
                                                        <span className="text-success">✓ Yes</span>
                                                    ) : (
                                                        <span className="text-base-content/50">✗ No</span>
                                                    )}
                                                </td>
                                                <td>
                                                    <span className={`badge ${getStatusColor(statusText)}`}>
                                                        {statusText}
                                                    </span>
                                                </td>
                                                <td>{drawDateText}</td>
                                                <td>{game.participants || 0}</td>
                                                <td>{game.ticketsSold || 0}</td>
                                                <td className="font-mono">{game.prizePool || 0} DKK</td>
                                                <td>
                                                    <div className="flex gap-2">
                                                        <button 
                                                            className="btn btn-xs btn-info"
                                                            onClick={() => handleViewGame(game)}
                                                        >
                                                            View
                                                        </button>
                                                        {statusText === "Pending Draw" && !game.isDrawn && (
                                                            <NavLink 
                                                                to={`/admin/games/draw/${game.id}`}
                                                                className="btn btn-xs btn-warning"
                                                            >
                                                                Draw
                                                            </NavLink>
                                                        )}
                                                    </div>
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

            {/* Game Details Modal */}
            {selectedGame && (
                <div className="modal modal-open">
                    <div className="modal-box max-w-3xl">
                        <div className="flex items-center justify-between mb-6">
                            <h3 className="text-2xl font-bold flex items-center gap-2">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-primary" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                                </svg>
                                Game Details
                            </h3>
                            <button
                                className="btn btn-sm btn-circle btn-ghost"
                                onClick={handleCloseModal}
                            >
                                ✕
                            </button>
                        </div>

                        <div className="space-y-6">
                            {/* Game Info Section */}
                            <div className="bg-base-300 p-4 rounded-lg">
                                <h4 className="font-bold text-lg mb-3 text-primary">Game Information</h4>
                                <div className="grid grid-cols-2 gap-4">
                                    <div>
                                        <p className="text-sm text-base-content/70">Game Name</p>
                                        <p className="font-semibold text-lg">{selectedGame.template?.name || "Unknown"}</p>
                                    </div>
                                    <div>
                                        <p className="text-sm text-base-content/70">Status</p>
                                        <span className={`badge ${getStatusColor(getGameStatus(selectedGame))}`}>
                                            {getGameStatus(selectedGame)}
                                        </span>
                                    </div>
                                    <div>
                                        <p className="text-sm text-base-content/70">Year</p>
                                        <p className="font-semibold">{selectedGame.year}</p>
                                    </div>
                                    <div>
                                        <p className="text-sm text-base-content/70">Week</p>
                                        <p className="font-semibold">Week {selectedGame.week}</p>
                                    </div>
                                    <div>
                                        <p className="text-sm text-base-content/70">Type</p>
                                        <p className="font-semibold">
                                            {selectedGame.isAutoRepeatable ? (
                                                <span className="text-success">✓ Repeatable</span>
                                            ) : (
                                                <span>One-time</span>
                                            )}
                                        </p>
                                    </div>
                                    <div>
                                        <p className="text-sm text-base-content/70">Draw Schedule</p>
                                        <p className="font-semibold">
                                            {selectedGame.isAutoRepeatable && selectedGame.drawDayOfWeek !== undefined && selectedGame.drawTimeOfDay
                                                ? `${mapDayOfWeek(selectedGame.drawDayOfWeek)}, ${selectedGame.drawTimeOfDay.substring(0, 5)}`
                                                : selectedGame.drawDate ? formatDateTime(selectedGame.drawDate) : 'N/A'}
                                        </p>
                                    </div>
                                </div>
                            </div>

                            {/* Statistics Section */}
                            <div className="bg-base-300 p-4 rounded-lg">
                                <h4 className="font-bold text-lg mb-3 text-primary">Statistics</h4>
                                <div className="grid grid-cols-2 gap-4">
                                    <div>
                                        <p className="text-sm text-base-content/70">Participants</p>
                                        <p className="font-semibold text-lg">{selectedGame.participants || 0}</p>
                                    </div>
                                    <div>
                                        <p className="text-sm text-base-content/70">Tickets Sold</p>
                                        <p className="font-semibold text-lg">{selectedGame.ticketsSold || 0}</p>
                                    </div>
                                    <div>
                                        <p className="text-sm text-base-content/70">Tickets Won</p>
                                        <p className="font-semibold text-lg">{selectedGame.ticketsWon || 0}</p>
                                    </div>
                                    <div>
                                        <p className="text-sm text-base-content/70">Prize Pool</p>
                                        <p className="font-semibold text-lg font-mono">{(selectedGame.prizePool || 0).toLocaleString()} DKK</p>
                                    </div>
                                </div>
                            </div>

                            {/* Template Details Section */}
                            {selectedGame.template && (
                                <div className="bg-base-300 p-4 rounded-lg">
                                    <h4 className="font-bold text-lg mb-3 text-primary">Template Details</h4>
                                    <div className="grid grid-cols-2 gap-4">
                                        <div>
                                            <p className="text-sm text-base-content/70">Game Type</p>
                                            <p className="font-semibold">{selectedGame.template.gameType}</p>
                                        </div>
                                        <div>
                                            <p className="text-sm text-base-content/70">Base Price</p>
                                            <p className="font-semibold font-mono">{selectedGame.template.basePrice} DKK</p>
                                        </div>
                                        <div>
                                            <p className="text-sm text-base-content/70">Pool of Numbers</p>
                                            <p className="font-semibold">1 - {selectedGame.template.poolOfNumbers}</p>
                                        </div>
                                        <div>
                                            <p className="text-sm text-base-content/70">Winning Numbers</p>
                                            <p className="font-semibold">{selectedGame.template.maxWinningNumbers} numbers</p>
                                        </div>
                                        <div>
                                            <p className="text-sm text-base-content/70">Numbers Per Ticket</p>
                                            <p className="font-semibold">
                                                {selectedGame.template.minNumbersPerTicket} - {selectedGame.template.maxNumbersPerTicket}
                                            </p>
                                        </div>
                                        <div className="col-span-2">
                                            <p className="text-sm text-base-content/70">Description</p>
                                            <p className="font-semibold">{selectedGame.template.description}</p>
                                        </div>
                                    </div>
                                </div>
                            )}

                            {/* Winning Numbers Section */}
                            {selectedGame.winningNumbers && selectedGame.winningNumbers.length > 0 && (
                                <div className="bg-base-300 p-4 rounded-lg">
                                    <h4 className="font-bold text-lg mb-3 text-primary">Winning Numbers</h4>
                                    <div className="flex flex-wrap gap-3">
                                        {selectedGame.winningNumbers.map((num, idx) => (
                                            <div
                                                key={idx}
                                                className="w-12 h-12 flex items-center justify-center rounded-full bg-success text-white font-bold shadow-lg text-lg"
                                            >
                                                {num}
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            )}
                        </div>

                        <div className="modal-action">
                            {getGameStatus(selectedGame) === "Pending Draw" && !selectedGame.isDrawn && (
                                <NavLink
                                    to={`/admin/games/draw/${selectedGame.id}`}
                                    className="btn btn-warning"
                                >
                                    <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M7 20l4-16m2 16l4-16M6 9h14M4 15h14" />
                                    </svg>
                                    Draw Numbers
                                </NavLink>
                            )}
                            <button className="btn btn-primary" onClick={handleCloseModal}>
                                Close
                            </button>
                        </div>
                    </div>
                    <div className="modal-backdrop" onClick={handleCloseModal}></div>
                </div>
            )}

            {/* Quick Actions & Templates */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Quick Actions */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title mb-4">Quick Actions</h2>
                        <div className="space-y-3">
                            <NavLink 
                                to="/admin/games/start" 
                                className="btn btn-block bg-amber-900 hover:bg-amber-800 text-white border-0 justify-start text-base"
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4v16m8-8H4" />
                                </svg>
                                Start New Game
                            </NavLink>
                            
                            <NavLink 
                                to="/admin/games/history" 
                                className="btn btn-block btn-accent justify-start text-base"
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                                View History
                            </NavLink>
                        </div>
                    </div>
                </div>

                {/* Popular Templates */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title mb-4">Popular Templates</h2>
                        <div className="space-y-3">
                            {templateStats.length === 0 ? (
                                <p className="text-center py-4 text-base-content/60">No templates available</p>
                            ) : (
                                templateStats.map((template) => (
                                    <div key={template.id} className="flex justify-between items-center p-3 bg-base-300 rounded-lg hover:bg-base-100 transition-colors">
                                        <div>
                                            <p className="font-semibold text-base">{template.name}</p>
                                            <p className="text-sm text-base-content/60">
                                                {template.gamesCreated} active {template.gamesCreated === 1 ? 'game' : 'games'}
                                            </p>
                                        </div>
                                        <NavLink to="/admin/games/start" className="btn btn-sm btn-ghost">
                                            Use →
                                        </NavLink>
                                    </div>
                                ))
                            )}
                        </div>
                        <NavLink to="/admin/games/templates" className="btn btn-sm btn-ghost mt-2 text-base">
                            View All Templates
                        </NavLink>
                    </div>
                </div>
            </div>
            </div>
        </div>
    );
};
