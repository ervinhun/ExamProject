import {useEffect} from "react";
import {NavLink} from "react-router-dom";
import {useAtom} from "jotai";
import {activeGamesAtom, fetchActiveGamesAtom} from "@core/atoms/game";
import {mapGameStatus} from "@core/types/game";
import lott from "@ui/assets/lott.png";

export default function GamesDashboard() {
    const [activeGames,] = useAtom(activeGamesAtom);
    const [, fetchActiveGames] = useAtom(fetchActiveGamesAtom);

    useEffect(() => {
        if (activeGames.length === 0) {
            fetchActiveGames();
        }
    }, []);

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

    const mapDayOfWeek = (dayNum: number | undefined) => {
        const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
        if (dayNum === undefined || dayNum < 0 || dayNum > 6) return "N/A";
        return days[dayNum];
    };

    const getStatusColor = (status: string) => {
        switch (status) {
            case "Active":
                return "badge-success";
            case "Pending Draw":
                return "badge-warning";
            case "Completed":
                return "badge-info";
            default:
                return "badge-ghost";
        }
    };

    const getGameTypeColor = (type: string) => {
        return type === "Lotto" ? "badge-custom-pink" : "badge-custom-light-blue";
    };

    const mapGamePicture = (type: string | undefined) => {
        switch (type) {
            case "Lotto":
                return lott;
            // Future game types can be added here
            default:
                return undefined;
        }
    }

    return (
        <div className="container mx-auto ">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary ml-3">Games</h1>
                        <p className="text-base text-base-content/70 mt-1 ml-3">Browse and play available lottery
                            games</p>
                    </div>
                </div>

                {/* Active Games Grid */}
                <div className="flex flex-wrap justify-center gap-9">
                    {activeGames.length === 0 ? (
                        <div className="col-span-full text-center py-12">
                            <svg xmlns="http://www.w3.org/2000/svg"
                                 className="h-16 w-16 mx-auto text-base-content/30 mb-4" fill="none" viewBox="0 0 24 24"
                                 stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                      d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                            </svg>
                            <p className="text-xl text-base-content/60">No active games available</p>
                            <p className="text-sm text-base-content/40 mt-2">Check back later for new games</p>
                        </div>
                    ) : (
                        activeGames.map((game) => {
                            const statusText = mapGameStatus(game.status);
                            const dateText = game.isAutoRepeatable
                                ? `${mapDayOfWeek(game.drawDayOfWeek)}, ${game.drawTimeOfDay?.substring(0, 5)}`
                                : formatDate(game.drawDate);
                            return (
                                <div key={game.id}
                                     className="card bg-base-200 border border-base-300 rounded-lg shadow-lg hover:shadow-xl transition-shadow overflow-hidden w-120">
                                    {/* Image */}
                                    <figure className="relative h-48 bg-gradient-to-br from-primary/20 to-secondary/20">
                                        <img
                                            src={mapGamePicture(game.template?.gameType)}
                                            alt={game.template?.name || "Game Image"}
                                            className="absolute inset-0 w-full h-full object-cover opacity-80"
                                        />
                                    </figure>

                                    <div className="p-6 text-center">
                                        {/* Badge */}
                                        <span
                                            className={`inline-flex items-center ${getGameTypeColor(game.template?.gameType || "")} text-xs font-medium px-2 py-1 rounded-sm gap-1 mb-3`}>
                                            <svg className="w-3 h-3" xmlns="http://www.w3.org/2000/svg" fill="none"
                                                 viewBox="0 0 24 24" stroke="currentColor">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                                      d="M13 10V3L4 14h7v7l9-11h-7z"/>
                                            </svg>
                                            {game.template?.gameType || "N/A"}
                                        </span>

                                        <h5 className="mb-2 text-2xl font-semibold tracking-tight">
                                            {game.template?.name || "Unknown Game"}
                                        </h5>

                                        {/* Description */}
                                        <p className="text-sm text-base-content/70 mb-4">
                                            {game.template?.description || "No description available"}
                                        </p>

                                        <div className="divider my-2"></div>

                                        {/* Game Details */}
                                        <div className="space-y-2 text-sm text-left">
                                            <div className="flex justify-between">
                                                <span className="text-base-content/60">Status:</span>
                                                <span className={`badge badge-sm ${getStatusColor(statusText)}`}>
                                                    {statusText}
                                                </span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="text-base-content/60">Draw Date:</span>
                                                <span className="font-medium">{dateText}</span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="text-base-content/60">Minimal Price:</span>
                                                <span className="font-medium">{game.template?.basePrice || 0} DKK</span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="text-base-content/60">Numbers Pool:</span>
                                                <span
                                                    className="font-medium">1-{game.template?.poolOfNumbers || "N/A"}</span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="text-base-content/60">Pick:</span>
                                                <span className="font-medium">
                                                    {game.template?.minNumbersPerTicket || 0}-{game.template?.maxNumbersPerTicket || 0} numbers
                                                </span>
                                            </div>
                                        </div>

                                        {/* Action Button */}
                                        <NavLink
                                            to={`play/${game.template?.gameType?.toLowerCase()}/${game.id}`}
                                            className="mt-6 inline-flex items-center text-white bg-primary hover:bg-primary-focus border-0 focus:ring-4 focus:ring-primary/30 shadow-sm font-medium rounded-lg text-sm px-5 py-2.5 transition-colors"
                                        >
                                            Play Now
                                            <svg className="w-4 h-4 ms-1.5 -me-0.5" xmlns="http://www.w3.org/2000/svg"
                                                 fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                                      d="M19 12H5m14 0-4 4m4-4-4-4"/>
                                            </svg>
                                        </NavLink>
                                    </div>
                                </div>
                            );
                        })
                    )}
                </div>

                {/* Info Section */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div className="card bg-base-200 shadow-lg">
                        <div className="card-body">
                            <h3 className="card-title text-lg">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-primary" fill="none"
                                     viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                          d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                                </svg>
                                How to Play
                            </h3>
                            <p className="text-sm text-base-content/70">
                                Select your numbers, purchase tickets, and wait for the draw. Match the winning numbers
                                to claim prizes!
                            </p>
                        </div>
                    </div>

                    <div className="card bg-base-200 shadow-lg">
                        <div className="card-body">
                            <h3 className="card-title text-lg">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-success" fill="none"
                                     viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                          d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                                </svg>
                                My Tickets
                            </h3>
                            <p className="text-sm text-base-content/70 mb-3">
                                View your purchased tickets and check results.
                            </p>
                            <NavLink to="/tickets" className="btn btn-sm btn-ghost">
                                View Tickets →
                            </NavLink>
                        </div>
                    </div>

                    <div className="card bg-base-200 shadow-lg">
                        <div className="card-body">
                            <h3 className="card-title text-lg">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-accent" fill="none"
                                     viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                          d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                                </svg>
                                Wallet
                            </h3>
                            <p className="text-sm text-base-content/70 mb-3">
                                Add funds to your account to purchase tickets.
                            </p>
                            <NavLink to="/wallet" className="btn btn-sm btn-ghost">
                                Add Funds →
                            </NavLink>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}   