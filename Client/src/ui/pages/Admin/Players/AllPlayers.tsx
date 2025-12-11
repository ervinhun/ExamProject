import {fetchPlayersAtom, playersAtom, togglePlayerStatusAtom} from "@core/atoms/players";
import {useAtom, useSetAtom} from "jotai";
import {useEffect} from "react";
import {NavLink} from "react-router-dom";
import {addNotificationAtom} from "@core/atoms/error";
import formatDate from "@ui/helpers/FormatDate.ts";

export default function AllPlayers() {
    const [players,] = useAtom(playersAtom);
    const [, fetchPlayers] = useAtom(fetchPlayersAtom);
    const togglePlayerStatus = useSetAtom(togglePlayerStatusAtom);
    const addNotification = useSetAtom(addNotificationAtom);

    useEffect(() => {
        if (players.length === 0) {
            fetchPlayers();
        }
    }, []);

    const handleToggleStatus = async (userId: string, currentStatus: boolean, playerName: string) => {
        try {
            // await userApi.toggleStatus(userId);
            await togglePlayerStatus(userId);
            addNotification({
                message: `${playerName} has been ${currentStatus ? 'deactivated' : 'activated'} successfully`,
                type: 'success'
            });
        } catch (error) {
            console.error('Failed to toggle user status:', error);
            addNotification({
                message: 'Failed to update player status. Please try again.',
                type: 'error'
            });
        }
    }

    return (
        <div className="container mx-auto">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary">All Players</h1>
                        <p className="text-base text-base-content/70 mt-1">
                            Manage all registered players - Total: {players.length} players
                        </p>
                    </div>
                    <NavLink to="/admin/players/register" className="btn btn-primary">
                        + Register Player
                    </NavLink>
                </div>

                {/* Players Table */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Players List</h2>

                        <div className="overflow-x-auto">
                            <table className="table table-zebra">
                                <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Email</th>
                                    <th>Phone</th>
                                    <th>Status</th>
                                    <th>Joined Date</th>
                                    <th>Actions</th>
                                </tr>
                                </thead>
                                <tbody>
                                {players.length === 0 ? (
                                    <tr>
                                        <td colSpan={6} className="text-center py-8 text-base-content/60">
                                            No players found
                                        </td>
                                    </tr>
                                ) : (
                                    players.map((player) => (
                                        <tr key={player.id}>
                                            <td className="font-semibold">
                                                {player.firstName} {player.lastName}
                                            </td>
                                            <td>{player.email}</td>
                                            <td>{player.phoneNumber}</td>
                                            <td>
                                                    <span
                                                        className={`badge ${player.isActive ? 'badge-success' : 'badge-error'}`}>
                                                        {player.isActive ? 'Active' : 'Inactive'}
                                                    </span>
                                            </td>
                                            <td>{player.createdAt ? formatDate(player.createdAt) : "N/A"}</td>
                                            <td>
                                                <div className="flex gap-2">
                                                    <button className="btn btn-xs btn-info">View</button>
                                                    <button className="btn btn-xs btn-ghost">Edit</button>
                                                    <button
                                                        className={`btn btn-xs ${player.isActive ? 'btn-warning' : 'btn-success'}`}
                                                        onClick={() => handleToggleStatus(
                                                            player.id!,
                                                            player.isActive!,
                                                            `${player.firstName} ${player.lastName}`
                                                        )}
                                                    >
                                                        {player.isActive ? 'Deactivate' : 'Activate'}
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))
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