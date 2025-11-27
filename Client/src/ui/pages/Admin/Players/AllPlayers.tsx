import { fetchPlayersAtom, playersAtom } from "@core/atoms/players";
import { useAtom } from "jotai";
import { useEffect } from "react"

export default function AllPlayers() {
    const [players, ] = useAtom(playersAtom);
    const [,fetchPlayers] = useAtom(fetchPlayersAtom);
    useEffect(() => {
        fetchPlayers();
    }, []);

    return (
        <div className="w-full p-6">
            <div className="mb-6">
                <h1 className="text-3xl font-bold">All Players</h1>
                <p className="text-gray-600 mt-2">Total: {players.length} players</p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {players.map((player) => (
                    <div 
                        key={player.id} 
                        className="card bg-base-200 shadow-lg hover:shadow-xl transition-shadow"
                    >
                        <div className="card-body">
                            <h2 className="card-title text-lg">
                                {player.firstName} {player.lastName}
                            </h2>
                            <div className="space-y-2 text-sm">
                                <p>
                                    <span className="font-semibold">Email:</span> {player.email}
                                </p>
                                <p>
                                    <span className="font-semibold">Phone:</span> {player.phoneNumber}
                                </p>
                                {player.createdAt && (
                                    <p className="text-xs text-gray-500">
                                        <span className="font-semibold">Joined:</span> {new Date(player.createdAt).toLocaleDateString()}
                                    </p>
                                )}
                            </div>
                            <div className="card-actions justify-end mt-4">
                                <button className="btn btn-sm btn-primary">View Details</button>
                                <button className="btn btn-sm btn-outline">Edit</button>
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            {players.length === 0 && (
                <div className="text-center py-12">
                    <p className="text-xl text-gray-500">No players found</p>
                </div>
            )}
        </div>
    )
}