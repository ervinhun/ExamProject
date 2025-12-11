import {useEffect, useState} from "react";
import {useAtom} from "jotai";
import {activeGamesAtom} from "@core/atoms/game.ts";
import {gameApi} from "@core/api/controllers/game.ts";
import {GameInstanceDto} from "@core/types/game.ts";
import {ticketApi} from "@core/api/controllers/ticket.ts";
import {myTicketsAtom} from "@core/atoms/tickets.ts";

export default function MyTickets() {
    const [chosenGameTemplate, setChosenGameTemplate] = useState<GameInstanceDto | null>(null)

    const [gameInstance, setGameInstance] = useAtom(activeGamesAtom)
    const [myTickets, setMyTickets] = useAtom(myTicketsAtom)

    useEffect(() => {
        if (gameInstance.length === 0) {
            gameApi.getAllActiveGames().then((data) => {
                setGameInstance(data);
                if (chosenGameTemplate === null) {
                    setChosenGameTemplate(data[0]);
                }
            });
        }
    }, []);

    useEffect(() => {
        if (myTickets.length === 0) {
            ticketApi.getAllActiveTickets().then(setMyTickets);
        }
    }, [myTickets, setMyTickets]);


    return (
        <div className="container mx-auto px-4 py-6 my-7">
            <div className="space-y-8">
                {/* Title */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-3xl font-bold">Lottery Tickets</h1>
                        <p className="text-gray-600">Check your tickets</p>
                    </div>
                </div>
                {/* Active Tickets */}
                <div className="bg-amber-100 p-6 rounded-xl shadow-md">
                    <h2 className="text-xl font-semibold mb-4">Active Tickets</h2>

                    {myTickets.length === 0 ? (
                        <p className="text-gray-500">You don't have any active tickets yet.</p>
                    ) : (
                        <table className="w-full border-collapse">
                            <thead>
                            <tr className="bg-gray-200 text-left">
                                <th className="p-2">Game</th>
                                <th className="p-2">Numbers</th>
                                <th className="p-2">Repeat</th>
                            </tr>
                            </thead>

                            <tbody>
                            {myTickets.map((t) => {
                                const gameInstanceColor = gameInstance.find(
                                    gi => gi.id === t.gameInstanceId
                                );

                                // Default values if something missing
                                const status = gameInstanceColor?.status ?? 0;
                                const result = t.results?.[0]?.isWinning ?? false;

                                // Color selection
                                let rowClass = "";
                                if (result) {
                                    rowClass = "bg-green-100";
                                } else if (!result && status !== 0) {
                                    rowClass = "bg-red-100";
                                } else if (status === 0) {
                                    rowClass = "bg-yellow-100";
                                }

                                return (
                                    <tr key={t.id} className={rowClass}>
                                        <td className="p-2 font-medium">
                                            {gameInstanceColor?.template?.name ?? "Unknown Game Template"}{" (W"}{gameInstanceColor?.week}{")"}
                                        </td>
                                        <td className="p-2">
                                            <div className="flex flex-wrap gap-2">
                                                {t.selectedNumbers.map(n => (
                                                    <div
                                                        key={`${t.id}-${n}`}
                                                        className="w-8 h-8 flex items-center justify-center rounded-full bg-orange-500 text-white font-semibold shadow"
                                                    >
                                                        {n}
                                                    </div>
                                                ))}
                                            </div>
                                        </td>
                                        <td className="p-2">
                                            {t.repeat}
                                        </td>
                                    </tr>
                                );
                            })}
                            </tbody>
                        </table>
                    )}
                </div>

            </div>
        </div>
    )
}
