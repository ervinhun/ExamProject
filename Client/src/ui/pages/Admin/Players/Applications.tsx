import {useAtom, useSetAtom} from "jotai";
import {fetchPlayersAtom, playersAtom, togglePlayerStatusAtom} from "@core/atoms/players.ts";
import {addNotificationAtom} from "@core/atoms/error.ts";
import {useEffect, useState} from "react";
import {userApi} from "@core/api/controllers/user.ts";
import {AppliedUser} from "@core/types/users.ts";

export default function Applications() {
    const [setPlayers, players] = useAtom(playersAtom);
    const [setFetchPlayers, fetchPlayers] = useAtom(fetchPlayersAtom);
    const togglePlayerStatus = useSetAtom(togglePlayerStatusAtom);
    const addNotification = useSetAtom(addNotificationAtom);
    const [appliedPlayers, setAppliedPlayers] = useState<AppliedUser[]>([]);

    useEffect(() => {
        if (appliedPlayers.length === 0) {
            userApi.getAllAppliedUsers().then((data) => {
                setAppliedPlayers(data);
            });
        }
    }, []);

    useEffect(() => {
        if (players.length === 0) {
            fetchPlayers();
        }
    }, []);

    const confirmPlayer = async (userId: string, isApproved: boolean, isActive: boolean) => {
        const result = await userApi.confirmAppliedUsers(userId, isApproved, isActive);
        if (result)
            setAppliedPlayers(prev =>
                prev.map(player =>
                    player.id === userId
                        ? {...player, status: "Confirmed"}
                        : player));
    }


    return (
        <div className="container mx-auto px-4 py-6 my-7">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary">Player applications</h1>
                        <p className="text-base text-base-content/70 mt-1">
                            Total pending applications: {appliedPlayers.length}
                        </p>
                    </div>
                </div>

                {/* Players Table */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <div className="overflow-x-auto">
                            <table className="table table-zebra">
                                <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Email</th>
                                    <th>Phone</th>
                                    <th>Age</th>
                                    <th>Request Date</th>
                                    <th>Status</th>
                                    <th>
                                        <div className="flex items-center">Actions</div>
                                    </th>
                                </tr>
                                </thead>
                                <tbody>
                                {appliedPlayers.map((item) => (
                                    <tr key={item.id}>
                                        <td>{item.player.firstName} {item.player.lastName}</td>
                                        <td>{item.player.email}</td>
                                        <td>{item.player.phoneNumber}</td>
                                        <td>{item.age ?? 18}</td>
                                        <td>{new Date(item.createdAt).toLocaleDateString()}{" "}
                                            {new Date(item.createdAt).toLocaleTimeString([], {
                                                hour: "2-digit",
                                                minute: "2-digit"
                                            })}
                                        </td>
                                        <td>{item.status}</td>
                                        <td>
                                            <div className="flex items-center gap-2">
                                                <span className="cursor-pointer">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                                                     viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                                     stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
                                                     className="lucide lucide-octagon-x-icon lucide-octagon-x"
                                                     onClick={() => confirmPlayer(item.player.id!, false, false)}><path
                                                    d="m15 9-6 6"/><path
                                                    d="M2.586 16.726A2 2 0 0 1 2 15.312V8.688a2 2 0 0 1 .586-1.414l4.688-4.688A2 2 0 0 1 8.688 2h6.624a2 2 0 0 1 1.414.586l4.688 4.688A2 2 0 0 1 22 8.688v6.624a2 2 0 0 1-.586 1.414l-4.688 4.688a2 2 0 0 1-1.414.586H8.688a2 2 0 0 1-1.414-.586z"/><path
                                                    d="m9 9 6 6"/><title>Decline</title></svg>
                                            </span>

                                                <span className="cursor-pointer">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                                                     viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                                     stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
                                                     className="lucide lucide-check-icon lucide-check"
                                                     onClick={() => confirmPlayer(item.player.id!, true, false)}><path
                                                    d="M20 6 9 17l-5-5"/><title>Approve</title></svg></span>{" "}

                                                <span className="cursor-pointer">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                                                     viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                                     stroke-width="2" stroke-linecap="round"
                                                     stroke-linejoin="round"
                                                     className="lucide lucide-check-check-icon lucide-check-check"
                                                     onClick={() => confirmPlayer(item.player.id!, true, true)}>
                                                    <path d="M18 6 7 17l-5-5"/>
                                                    <path d="m22 10-7.5 7.5L13 16"/>
                                                    <title>Approve and activate</title>
                                                    </svg>
                                                </span>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}