// filepath: /Users/kamil/easv/fullstack/ExamProject/Client/src/ui/pages/Admin/Games/ActiveGames.tsx
import { useEffect, useState } from "react";
import { useAtom, useSetAtom } from "jotai";
import { NavLink } from "react-router-dom";
import { activeGamesAtom, fetchActiveGamesAtom } from "@core/atoms/game";
import { formatDateTime, mapDayOfWeek } from "@utils/dateUtils";
import { getStatusColor, getGameStatus } from "@utils/gameUtils";
import type { GameInstanceDto } from "@core/types/game";
import { addNotificationAtom } from "@core/atoms/error";

export default function ActiveGames() {
    const [activeGames] = useAtom(activeGamesAtom);
    const [, fetchActiveGames] = useAtom(fetchActiveGamesAtom);

    // Filter and sort states
    const [filterStatus, setFilterStatus] = useState<string>("all");
    const [filterTemplate, setFilterTemplate] = useState<string>("all");
    const [sortBy, setSortBy] = useState<string>("week-desc");

    useEffect(() => {
        fetchActiveGames();
    }, [fetchActiveGames]);

    // Get unique templates for filter
    const uniqueTemplates = Array.from(
        new Set(activeGames.map(game => game.template?.name).filter(Boolean))
    );

    // Filter and sort games
    const filteredGames = activeGames
        .filter(game => {
            if (filterStatus !== "all") {
                const status = getGameStatus(game);
                if (filterStatus === "Active" && status !== "Active") return false;
                if (filterStatus === "Pending Draw" && status !== "Pending Draw") return false;
            }
            if (filterTemplate !== "all" && game.template?.name !== filterTemplate) return false;
            return true;
        })
        .sort((a, b) => {
            switch (sortBy) {
                case "week-desc":
                    return b.week - a.week;
                case "week-asc":
                    return a.week - b.week;
                case "name-asc":
                    return (a.template?.name || "").localeCompare(b.template?.name || "");
                case "name-desc":
                    return (b.template?.name || "").localeCompare(a.template?.name || "");
                case "tickets-desc":
                    return b.ticketsSold - a.ticketsSold;
                case "tickets-asc":
                    return a.ticketsSold - b.ticketsSold;
                default:
                    return 0;
            }
        });

    // Calculate stats
    const stats = {
        totalActive: activeGames.filter(game => getGameStatus(game) === "Active").length,
        totalPendingDraw: activeGames.filter(game => getGameStatus(game) === "Pending Draw").length,
        totalTicketsSold: activeGames.reduce((sum, game) => sum + (game.ticketsSold || 0), 0),
        totalPrizePool: activeGames.reduce((sum, game) => sum + (game.prizePool || 0), 0),
    };

    const getDrawDateDisplay = (game: GameInstanceDto): string => {
        if (game.isAutoRepeatable && game.drawDayOfWeek !== undefined && game.drawTimeOfDay) {
            return `${mapDayOfWeek(game.drawDayOfWeek)}, ${game.drawTimeOfDay.substring(0, 5)}`;
        } else if (game.drawDate) {
            return formatDateTime(new Date(game.drawDate).toISOString());
        }
        return "N/A";
    };

    return (
        <div className="container mx-auto">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary">Active Games</h1>
                        <p className="text-base text-base-content/70 mt-1">View and manage all active lottery games</p>
                    </div>
                    <NavLink to="/admin/games/start" className="btn bg-amber-900 hover:bg-amber-800 text-white border-0">
                        <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4v16m8-8H4" />
                        </svg>
                        Start New Game
                    </NavLink>
                </div>

                {/* Summary Stats */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                    <div className="stats shadow bg-base-200">
                        <div className="stat">
                            <div className="stat-figure text-success">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                            </div>
                            <div className="stat-title">Active Games</div>
                            <div className="stat-value text-success">{stats.totalActive}</div>
                            <div className="stat-desc">Currently accepting tickets</div>
                        </div>
                    </div>

                    <div className="stats shadow bg-base-200">
                        <div className="stat">
                            <div className="stat-figure text-warning">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                            </div>
                            <div className="stat-title">Pending Draw</div>
                            <div className="stat-value text-warning">{stats.totalPendingDraw}</div>
                            <div className="stat-desc">Awaiting number draw</div>
                        </div>
                    </div>

                    <div className="stats shadow bg-base-200">
                        <div className="stat">
                            <div className="stat-figure text-secondary">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 110 4v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 110-4V7a2 2 0 00-2-2H5z" />
                                </svg>
                            </div>
                            <div className="stat-title">Total Tickets Sold</div>
                            <div className="stat-value text-secondary">{stats.totalTicketsSold}</div>
                            <div className="stat-desc">Across all active games</div>
                        </div>
                    </div>

                    <div className="stats shadow bg-base-200">
                        <div className="stat">
                            <div className="stat-figure text-accent">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                            </div>
                            <div className="stat-title">Total Prize Pool</div>
                            <div className="stat-value text-accent">{stats.totalPrizePool.toLocaleString()}</div>
                            <div className="stat-desc">DKK across all games</div>
                        </div>
                    </div>
                </div>

                {/* Filters and Sorting */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <div className="flex flex-wrap gap-4 items-center">
                            {/* Status Filter */}
                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-semibold">Filter by Status</span>
                                </label>
                                <select
                                    className="select select-bordered select-sm"
                                    value={filterStatus}
                                    onChange={(e) => setFilterStatus(e.target.value)}
                                >
                                    <option value="all">All Statuses</option>
                                    <option value="Active">Active</option>
                                    <option value="Pending Draw">Pending Draw</option>
                                </select>
                            </div>

                            {/* Template Filter */}
                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-semibold">Filter by Game</span>
                                </label>
                                <select
                                    className="select select-bordered select-sm"
                                    value={filterTemplate}
                                    onChange={(e) => setFilterTemplate(e.target.value)}
                                >
                                    <option value="all">All Games</option>
                                    {uniqueTemplates.map((template) => (
                                        <option key={template} value={template}>
                                            {template}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            {/* Sort By */}
                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-semibold">Sort By</span>
                                </label>
                                <select
                                    className="select select-bordered select-sm"
                                    value={sortBy}
                                    onChange={(e) => setSortBy(e.target.value)}
                                >
                                    <option value="week-desc">Week (Newest First)</option>
                                    <option value="week-asc">Week (Oldest First)</option>
                                    <option value="name-asc">Name (A-Z)</option>
                                    <option value="name-desc">Name (Z-A)</option>
                                    <option value="tickets-desc">Tickets (Most First)</option>
                                    <option value="tickets-asc">Tickets (Least First)</option>
                                </select>
                            </div>

                            {/* Results Count */}
                            <div className="ml-auto">
                                <div className="badge badge-lg badge-primary">
                                    {filteredGames.length} games
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Active Games Table */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4 flex items-center gap-2">
                            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                            </svg>
                            Active Games List
                        </h2>

                        <div className="overflow-x-auto">
                            <table className="table table-zebra">
                                <thead>
                                    <tr>
                                        <th>Game Name</th>
                                        <th>Year</th>
                                        <th>Week</th>
                                        <th>Draw Schedule</th>
                                        <th>Status</th>
                                        <th>Type</th>
                                        <th>Tickets Sold</th>
                                        <th>Prize Pool</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {filteredGames.length === 0 ? (
                                        <tr>
                                            <td colSpan={9} className="text-center py-8 text-base-content/60">
                                                <svg xmlns="http://www.w3.org/2000/svg" className="h-16 w-16 mx-auto mb-4 opacity-50" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                                                </svg>
                                                No active games found
                                            </td>
                                        </tr>
                                    ) : (
                                        filteredGames.map((game) => {
                                            const status = getGameStatus(game);
                                            const drawDateText = getDrawDateDisplay(game);

                                            return (
                                                <tr key={game.id}>
                                                    <td className="font-semibold">{game.template?.name || "Unknown"}</td>
                                                    <td>{game.year}</td>
                                                    <td>Week {game.week}</td>
                                                    <td>{drawDateText}</td>
                                                    <td>
                                                        <span className={`badge ${getStatusColor(status)}`}>
                                                            {status}
                                                        </span>
                                                    </td>
                                                    <td>
                                                        {game.isAutoRepeatable ? (
                                                            <span className="badge badge-info badge-sm">Repeatable</span>
                                                        ) : (
                                                            <span className="badge badge-ghost badge-sm">One-time</span>
                                                        )}
                                                    </td>
                                                    <td>
                                                        <div className="flex items-center gap-2">
                                                            <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 110 4v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 110-4V7a2 2 0 00-2-2H5z" />
                                                            </svg>
                                                            {game.ticketsSold || 0}
                                                        </div>
                                                    </td>
                                                    <td className="font-mono">{(game.prizePool || 0).toLocaleString()} DKK</td>
                                                    <td>
                                                        <div className="flex gap-2">
                                                            {status === "Pending Draw" && !game.isDrawn && (
                                                                <NavLink
                                                                    to={`/admin/games/draw/${game.id}`}
                                                                    className="btn btn-sm btn-warning"
                                                                >
                                                                    <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M7 20l4-16m2 16l4-16M6 9h14M4 15h14" />
                                                                    </svg>
                                                                    Draw
                                                                </NavLink>
                                                            )}
                                                            <NavLink
                                                                to={`/admin/games/overview`}
                                                                className="btn btn-sm btn-info"
                                                            >
                                                                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                                                                </svg>
                                                                View
                                                            </NavLink>
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
            </div>
        </div>
    );
}
