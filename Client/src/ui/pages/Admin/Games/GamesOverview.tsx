import { NavLink } from "react-router-dom";
import { useAtom } from "jotai";
import { activeGamesAtom, fetchActiveGamesAtom, gameTemplatesAtom, fetchGameTemplatesAtom } from "@core/atoms/game";
import { mapGameStatus } from "@core/types/game";
import { useEffect } from "react";

export const GamesOverview: React.FC = () => {
    const [activeGames] = useAtom(activeGamesAtom);
    const [, fetchActiveGames] = useAtom(fetchActiveGamesAtom);
    const [templates] = useAtom(gameTemplatesAtom);
    const [, fetchGameTemplates] = useAtom(fetchGameTemplatesAtom);

    useEffect(() => {
        if(activeGames.length === 0) {
            fetchActiveGames();
        }
        if(templates.length === 0) {
            fetchGameTemplates();
        }
    }, []);

    const stats = {
        activeGames: activeGames.length,
        totalPlayers: 0, // Placeholder - needs real participant data
        totalRevenue: activeGames.reduce((sum, game) => sum + (game.template?.basePrice || 0) * 10, 0), // Placeholder calculation
        pendingDraws: activeGames.filter(game => mapGameStatus(game.status) === "Pending Draw" && !game.isDrawn).length
    };

    // Get template usage stats
    const templateStats = templates.map(template => ({
        id: template.id,
        name: template.name,
        gamesCreated: activeGames.filter(game => game.template?.id === template.id).length
    })).sort((a, b) => b.gamesCreated - a.gamesCreated).slice(0, 3);

    const formatDate = (dateStr: string | Date | undefined) => {
        if (!dateStr) return "N/A";
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
            case "Completed": return "badge-info";
            default: return "badge-ghost";
        }
    };

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
                        <div className="stat-figure text-accent">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="inline-block w-8 h-8 stroke-current">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <div className="stat-title">Total Revenue</div>
                        <div className="stat-value text-base-content">{stats.totalRevenue.toLocaleString()} DKK</div>
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
                                        <td colSpan={7} className="text-center py-8 text-base-content/60">
                                            No active games at the moment
                                        </td>
                                    </tr>
                                ) : (
                                    activeGames.slice(0, 5).map((game) => {
                                        const statusText = mapGameStatus(game.status);
                                        return (
                                            <tr key={game.id}>
                                                <td className="font-semibold">{game.template?.name || "Unknown"}</td>
                                                <td>
                                                    <span className={`badge ${getStatusColor(statusText)}`}>
                                                        {statusText}
                                                    </span>
                                                </td>
                                                <td>{formatDate(game.drawDate)}</td>
                                                <td>-</td>
                                                <td>-</td>
                                                <td className="font-mono">{game.template?.basePrice || 0} DKK</td>
                                                <td>
                                                    <div className="flex gap-2">
                                                        <button className="btn btn-xs btn-info">View</button>
                                                        {!game.isDrawn && (
                                                            <button className="btn btn-xs btn-warning">Draw</button>
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
                                to="/admin/games/templates" 
                                className="btn btn-block btn-secondary justify-start text-base"
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                </svg>
                                Manage Templates
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
