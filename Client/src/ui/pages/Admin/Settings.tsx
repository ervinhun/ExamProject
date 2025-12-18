import { useState } from "react";
import { useSetAtom } from "jotai";
import { addNotificationAtom } from "@core/atoms/error";
import { authApi } from "@core/api/controllers/auth";

export default function Settings() {
    const addNotification = useSetAtom(addNotificationAtom);
    
    // Password change state
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [isChangingPassword, setIsChangingPassword] = useState(false);

    const handlePasswordChange = async (e: React.FormEvent) => {
        e.preventDefault();
        
        // Validation
        if (!currentPassword || !newPassword || !confirmPassword) {
            addNotification({
                type: "error",
                message: "Please fill in all password fields"
            });
            return;
        }
        
        if (newPassword !== confirmPassword) {
            addNotification({
                type: "error",
                message: "New passwords do not match"
            });
            return;
        }
        
        if (newPassword.length < 6) {
            addNotification({
                type: "error",
                message: "Password must be at least 6 characters long"
            });
            return;
        }
        
        setIsChangingPassword(true);
        
        try {
            await authApi.changePassword(currentPassword, newPassword);
            addNotification({
                type: "success",
                message: "Password changed successfully"
            });
            
            // Reset form
            setCurrentPassword("");
            setNewPassword("");
            setConfirmPassword("");
        } catch (error: any) {
            addNotification({
                type: "error",
                message: error?.message || "Failed to change password"
            });
        } finally {
            setIsChangingPassword(false);
        }
    };

    return (
        <div className="container mx-auto">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary">Settings</h1>
                        <p className="text-base text-base-content/70 mt-1">Manage your account settings</p>
                    </div>
                </div>

                {/* Password Change Card */}
                <div className="max-w-2xl mx-auto">
                    <div className="card bg-base-200 shadow-lg">
                        <div className="card-body">
                            <h2 className="card-title text-2xl mb-4 flex items-center gap-2">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
                                </svg>
                                Change Password
                            </h2>
                            
                            <form onSubmit={handlePasswordChange} className="space-y-4">
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-semibold">Current Password</span>
                                    </label>
                                    <input
                                        type="password"
                                        placeholder="Enter current password"
                                        className="input input-bordered w-full"
                                        value={currentPassword}
                                        onChange={(e) => setCurrentPassword(e.target.value)}
                                        disabled={isChangingPassword}
                                    />
                                </div>

                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-semibold">New Password</span>
                                    </label>
                                    <input
                                        type="password"
                                        placeholder="Enter new password"
                                        className="input input-bordered w-full"
                                        value={newPassword}
                                        onChange={(e) => setNewPassword(e.target.value)}
                                        disabled={isChangingPassword}
                                    />
                                    <label className="label">
                                        <span className="label-text-alt text-base-content/60">
                                            Must be at least 6 characters long
                                        </span>
                                    </label>
                                </div>

                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-semibold">Confirm New Password</span>
                                    </label>
                                    <input
                                        type="password"
                                        placeholder="Confirm new password"
                                        className="input input-bordered w-full"
                                        value={confirmPassword}
                                        onChange={(e) => setConfirmPassword(e.target.value)}
                                        disabled={isChangingPassword}
                                    />
                                </div>

                                <button
                                    type="submit"
                                    className={`btn btn-primary w-full ${isChangingPassword ? 'loading' : ''}`}
                                    disabled={isChangingPassword}
                                >
                                    {isChangingPassword ? 'Changing Password...' : 'Change Password'}
                                </button>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}