import {useEffect} from "react";
import {useAtom} from "jotai";
import {activeGamesAtom} from "@core/atoms/game.ts";
import {fetchTicketsForPlayerAtom, myTicketsAtom} from "@core/atoms/tickets.ts";

export default function MyTickets() {

    const [,fetchTicketsForPlayer] = useAtom(fetchTicketsForPlayerAtom);
    const [gameInstance] = useAtom(activeGamesAtom)
    const [myTickets] = useAtom(myTicketsAtom)

    useEffect(() => {
        fetchTicketsForPlayer();
    }, [fetchTicketsForPlayer]);

    // Sort tickets by purchase date (latest first)
    const sortedTickets = [...myTickets].sort((a, b) => {
        return new Date(b.boughtAt).getTime() - new Date(a.boughtAt).getTime();
    });

    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat('da-DK', { 
            style: 'currency', 
            currency: 'DKK',
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        }).format(amount);
    };

    const formatDate = (dateStr: string) => {
        const date = new Date(dateStr);
        return date.toLocaleDateString("da-DK", { 
            day: "2-digit",
            month: "2-digit",
            year: "numeric"
        });
    };


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

                {/* Active Tickets */}
                <div className="bg-amber-50 p-6 rounded-xl shadow-md">
                    <h2 className="text-2xl font-semibold mb-4">Active Tickets</h2>

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
                                    const gameInstanceColor = gameInstance.find(
                                        gi => gi.id === t.gameInstanceId
                                    );

                                    // Default values if something missing
                                    const status = gameInstanceColor?.status ?? 0;
                                    const isWinning = t.isWinning ?? false;

                                    // Status badge
                                    let statusBadge = "";
                                    let statusText = "";
                                    if (isWinning) {
                                        statusBadge = "badge-success";
                                        statusText = "🎉 Won!";
                                    } else if (status === 2) {
                                        statusBadge = "badge-error";
                                        statusText = "Lost";
                                    } else if (status === 1) {
                                        statusBadge = "badge-warning";
                                        statusText = "In Progress";
                                    } else {
                                        statusBadge = "badge-info";
                                        statusText = "Pending";
                                    }

                                    return (
                                        <tr key={t.id}>
                                            <td>
                                                <div>
                                                    <div className="font-semibold">
                                                        {gameInstanceColor?.template?.name ?? "Unknown Game"}
                                                    </div>
                                                    <div className="text-sm text-gray-500">
                                                        Week {gameInstanceColor?.week ?? "?"}
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
