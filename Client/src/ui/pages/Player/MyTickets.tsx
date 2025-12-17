import {useEffect} from "react";
import {useAtom} from "jotai";
import {allGamesAtom, fetchAllGamesAtom} from "@core/atoms/game.ts";
import {fetchSubscribedTicketsForPlayerAtom, fetchTicketsForPlayerAtom, mySubscribedTicketsAtom, myTicketsAtom} from "@core/atoms/tickets.ts";
import {formatCurrency} from "@utils/priceUtils.ts";
import {formatDate} from "@utils/dateUtils.ts";
import {getTicketStatus} from "@utils/gameUtils.ts";

export default function MyTickets() {

    const [,fetchTicketsForPlayer] = useAtom(fetchTicketsForPlayerAtom);
    const [,fetchSubscribedTicketsForPlayer] = useAtom(fetchSubscribedTicketsForPlayerAtom);
    const [,fetchAllGames] = useAtom(fetchAllGamesAtom);
    const [gameInstance] = useAtom(allGamesAtom)
    const [myTickets] = useAtom(myTicketsAtom)
    const [mySubscribedTickets] = useAtom(mySubscribedTicketsAtom)

    useEffect(() => {
        fetchTicketsForPlayer();
        fetchSubscribedTicketsForPlayer();
        fetchAllGames();
    }, [fetchTicketsForPlayer, fetchAllGames]);

    // Sort tickets by purchase date (latest first)
    const sortedTickets = [...myTickets].sort((a, b) => {
        return new Date(b.boughtAt).getTime() - new Date(a.boughtAt).getTime();
    });


    return (
        <div className="container mx-auto">
            <div className="space-y-8">
                {/* Title */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary ml-3">My Tickets</h1>
                        <p className="text-base text-base-content/70 mt-1 ml-3">View all your purchased tickets</p>
                    </div>
                </div>

                {/* Active Subscriptions */}
                {mySubscribedTickets.length > 0 && (
                    <div className="bg-green-50 p-6 rounded-xl shadow-md">
                        <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
                            <span>🔄 Active Subscriptions</span>
                        </h2>
                        <div className="overflow-x-auto">
                            <table className="table table-zebra w-full">
                                <thead>
                                    <tr>
                                        <th>Game</th>
                                        <th>Numbers</th>
                                        <th>Price per Draw</th>
                                        <th>Status</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                {mySubscribedTickets.map((sub) => (
                                    <tr key={sub.gameTemplateId}>
                                        <td>
                                            <div>
                                                <div className="font-semibold">
                                                    {sub.gameTemplate?.name ?? "Unknown Game"}
                                                </div>
                                                <div className="text-sm text-gray-500">
                                                    Auto-renewing subscription
                                                </div>
                                            </div>
                                        </td>
                                        <td>
                                            <div className="flex flex-wrap gap-2">
                                                {sub.pickedNumbers.map(n => (
                                                    <div
                                                        key={`${sub.gameTemplateId}-${n}`}
                                                        className="w-9 h-9 flex items-center justify-center rounded-full bg-green-600 text-white font-semibold shadow-md"
                                                    >
                                                        {n}
                                                    </div>
                                                ))}
                                            </div>
                                        </td>
                                        <td className="font-semibold text-green-600">
                                            {formatCurrency(sub.price)}
                                        </td>
                                        <td>
                                            <span className={`badge ${sub.isExpired ? 'badge-error' : 'badge-success'} badge-lg`}>
                                                {sub.isExpired ? 'Expired' : 'Active'}
                                            </span>
                                        </td>
                                        <td>
                                            <button 
                                                className="btn btn-sm btn-error"
                                                onClick={() => {
                                                    // TODO: Add cancel subscription functionality
                                                    alert('Cancel subscription feature coming soon!');
                                                }}
                                            >
                                                Cancel
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )}

                {/* Active Tickets */}
                <div className="bg-amber-50 p-6 rounded-xl shadow-md">
                    <h2 className="text-2xl font-semibold mb-4">My Tickets</h2>

                    {sortedTickets.length === 0 ? (
                        <div className="text-center py-8">
                            <p className="text-gray-500 mb-4">You don't have any tickets yet.</p>
                            <a href="/games" className="btn btn-primary">
                                Buy Your First Ticket
                            </a>
                        </div>
                    ) : (
                        <div className="overflow-x-auto">
                            <table className="table table-zebra w-full">
                                <thead>
                                    <tr>
                                        <th>Game</th>
                                        <th>Numbers</th>
                                        <th>Price</th>
                                        <th>Purchase Date</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>

                                <tbody>
                                {sortedTickets.map((t) => {
                                    const game = gameInstance.find(
                                        gi => gi.id === t.gameInstanceId
                                    );

                                    // Get ticket status
                                    const gameStatus = typeof game?.status === 'number' ? game.status : 0;
                                    const isWinning = t.isWinning ?? false;
                                    const isDrawn = game?.isDrawn ?? false;
                                    
                                    const { badge: statusBadge, text: statusText } = getTicketStatus(
                                        gameStatus,
                                        isDrawn,
                                        isWinning
                                    );

                                    return (
                                        <tr key={t.id}>
                                            <td>
                                                <div>
                                                    <div className="font-semibold">
                                                        {game?.template?.name ?? "Unknown Game"}
                                                    </div>
                                                    <div className="text-sm text-gray-500">
                                                        Week {game?.week ?? "?"}
                                                    </div>
                                                </div>
                                            </td>
                                            <td>
                                                <div className="flex flex-wrap gap-2">
                                                    {t.pickedNumbers.map(n => (
                                                        <div
                                                            key={`${t.id}-${n}`}
                                                            className="w-9 h-9 flex items-center justify-center rounded-full bg-primary text-white font-semibold shadow-md"
                                                        >
                                                            {n}
                                                        </div>
                                                    ))}
                                                </div>
                                            </td>
                                            <td className="font-semibold text-primary">
                                                {formatCurrency(t.fullPrice)}
                                            </td>
                                            <td className="text-sm text-gray-600">
                                                {formatDate(t.boughtAt)}
                                            </td>
                                            <td>
                                                <span className={`badge ${statusBadge} badge-lg`}>
                                                    {statusText}
                                                </span>
                                            </td>
                                        </tr>
                                    );
                                })}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>

            </div>
        </div>
    )
}
