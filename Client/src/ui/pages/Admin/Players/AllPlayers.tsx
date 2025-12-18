import {fetchPlayersAtom, playersAtom, togglePlayerStatusAtom} from "@core/atoms/players";
import {useAtom, useSetAtom} from "jotai";
import {useEffect, useState} from "react";
import {NavLink} from "react-router-dom";
import {addNotificationAtom} from "@core/atoms/error";
import {formatDate} from "@utils/dateUtils";
import {Player} from "@core/types/users";
import { userApi } from "@core/api/controllers/user";

export default function AllPlayers() {
    const [players,] = useAtom(playersAtom);
    const [, fetchPlayers] = useAtom(fetchPlayersAtom);
    const togglePlayerStatus = useSetAtom(togglePlayerStatusAtom);
    const addNotification = useSetAtom(addNotificationAtom);
    
    const [editingPlayer, setEditingPlayer] = useState<Player | null>(null);
    const [editForm, setEditForm] = useState({
        firstName: "",
        lastName: "",
        email: "",
        phoneNumber: ""
    });

    useEffect(() => {
        if (players.length === 0) {
            fetchPlayers();
        }
    }, []);

    const handleToggleStatus = async (userId: string, currentStatus: boolean, playerName: string) => {
        try {
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
    };

    const handleEditClick = (player: Player) => {
        setEditingPlayer(player);
        setEditForm({
            firstName: player.firstName || "",
            lastName: player.lastName || "",
            email: player.email || "",
            phoneNumber: player.phoneNumber || ""
        });
    };

    const handleEditSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        try {
            await userApi.updateUser(editingPlayer!.id!, {
                firstName: editForm.firstName,
                lastName: editForm.lastName,
                email: editForm.email,
                phoneNumber: editForm.phoneNumber
            });
            
            addNotification({
                message: 'Player updated successfully',
                type: 'success'
            });
            
            setEditingPlayer(null);
            fetchPlayers(); // Refresh the list
        } catch (error) {
            console.error('Failed to update player:', error);
            addNotification({
                message: 'Failed to update player. Please try again.',
                type: 'error'
            });
        }
    };

    const handleCloseModal = () => {
        setEditingPlayer(null);
        setEditForm({
            firstName: "",
            lastName: "",
            email: "",
            phoneNumber: ""
        });
    };

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
                                                    <button 
                                                        className="btn btn-xs btn-info"
                                                        onClick={() => handleEditClick(player)}
                                                    >
                                                        Edit
                                                    </button>
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

            {/* Edit Player Modal */}
            {editingPlayer && (
                <div className="modal modal-open">
                    <div className="modal-box max-w-2xl">
                        <h3 className="font-bold text-2xl mb-6 flex items-center gap-2">
                            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                            </svg>
                            Edit Player
                        </h3>
                        
                        <form onSubmit={handleEditSubmit} className="space-y-4">
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-semibold">First Name</span>
                                    </label>
                                    <input
                                        type="text"
                                        placeholder="First name"
                                        className="input input-bordered w-full"
                                        value={editForm.firstName}
                                        onChange={(e) => setEditForm({...editForm, firstName: e.target.value})}
                                        required
                                    />
                                </div>

                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-semibold">Last Name</span>
                                    </label>
                                    <input
                                        type="text"
                                        placeholder="Last name"
                                        className="input input-bordered w-full"
                                        value={editForm.lastName}
                                        onChange={(e) => setEditForm({...editForm, lastName: e.target.value})}
                                        required
                                    />
                                </div>
                            </div>

                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-semibold">Email</span>
                                </label>
                                <input
                                    type="email"
                                    placeholder="Email address"
                                    className="input input-bordered w-full"
                                    value={editForm.email}
                                    onChange={(e) => setEditForm({...editForm, email: e.target.value})}
                                    required
                                />
                            </div>

                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-semibold">Phone Number</span>
                                </label>
                                <input
                                    type="tel"
                                    placeholder="Phone number"
                                    className="input input-bordered w-full"
                                    value={editForm.phoneNumber}
                                    onChange={(e) => setEditForm({...editForm, phoneNumber: e.target.value})}
                                    required
                                />
                            </div>

                            <div className="alert alert-info">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="stroke-current shrink-0 w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                                </svg>
                                <span className="text-sm">Changes will be saved to the database</span>
                            </div>

                            <div className="modal-action">
                                <button 
                                    type="button" 
                                    className="btn btn-ghost"
                                    onClick={handleCloseModal}
                                >
                                    Cancel
                                </button>
                                <button type="submit" className="btn btn-primary">
                                    Save Changes
                                </button>
                            </div>
                        </form>
                    </div>
                    <div className="modal-backdrop" onClick={handleCloseModal}></div>
                </div>
            )}
        </div>
    );
}