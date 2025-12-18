import { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
import { addNotificationAtom } from "@core/atoms/error";
import { useSetAtom } from "jotai";
import { formatDate } from "@utils/dateUtils";
import type { User } from "@core/types/users";
import { adminApi } from "@core/api/controllers/admin";

export default function AdminList() {
    const [admins, setAdmins] = useState<User[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const addNotification = useSetAtom(addNotificationAtom);

    useEffect(() => {
        adminApi.getAllAdmins()
            .then((data) => {
                setAdmins(data);
                setLoading(false);
            })
            .catch((err) => {
                setError("Failed to fetch admins");
                setLoading(false);
                addNotification({
                    message: "Failed to fetch admins.",
                    type: "error"
                });
            });
    }, []);

    return (
        <div className="container mx-auto">
            <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                <div className="flex-1">
                    <h1 className="text-4xl font-bold text-primary">Admin List</h1>
                    <p className="text-base text-base-content/70 mt-1">All registered admins in the system</p>
                </div>
                <NavLink to="/admin/register" className="btn btn-primary">
                    + Register Admin
                </NavLink>
            </div>
            {loading ? (
                <div className="text-center py-8">Loading...</div>
            ) : error ? (
                <div className="text-center text-error py-8">{error}</div>
            ) : (
                <div className="overflow-x-auto mt-8">
                    <table className="table w-full">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Phone</th>
                                <th>Date of Birth</th>
                                <th>Created</th>
                            </tr>
                        </thead>
                        <tbody>
                            {admins.map((admin) => (
                                <tr key={admin.id}>
                                    <td>{admin.firstName} {admin.lastName}</td>
                                    <td>{admin.email}</td>
                                    <td>{admin.DOB ? formatDate(admin.DOB) : "-"}</td>
                                    <td>{admin.createdAt ? formatDate(admin.createdAt) : "-"}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}
