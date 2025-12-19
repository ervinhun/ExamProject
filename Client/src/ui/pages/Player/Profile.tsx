import { useState, useEffect } from "react";
import { useSetAtom } from "jotai";
import { addNotificationAtom } from "@core/atoms/error";
import { authApi } from "@core/api/controllers/auth";
import type { User } from "@core/types/users";
import getAge from "@utils/getAge";

export default function Profile() {
    const addNotification = useSetAtom(addNotificationAtom);
    
    // User profile state
    const [userDetails, setUserDetails] = useState<User | null>(null);
    const [isLoadingProfile, setIsLoadingProfile] = useState(true);
    
    // Password change state
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [isChangingPassword, setIsChangingPassword] = useState(false);

    // Fetch user profile details
    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const profile = await authApi.profile();
                setUserDetails(profile);
            } catch (error) {
                const errorMessage = error instanceof Error ? error.message : String(error);
                addNotification({
                    type: "error",
                    message: errorMessage || "Failed to load profile"
                });
            } finally {
                setIsLoadingProfile(false);
            }
        };

        fetchProfile();
    }, []);

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
        } catch (error) {
            const errorMessage = error instanceof Error ? error.message : String(error);
            addNotification({
                type: "error",
                message: errorMessage || "Failed to change password"
            });
        } finally {
            setIsChangingPassword(false);
        }
    };

    return (
        <div className="flex justify-center w-full mt-10">
            <div className="w-full max-w-4xl space-y-8 px-4">
                {/* Header */}
                <div className="text-center">
                    <h1 className="text-4xl font-bold text-primary">My Profile</h1>
                    <p className="text-base text-base-content/70 mt-2">Manage your account information and settings</p>
                </div>

                {/* Profile Details Card */}
                {isLoadingProfile ? (
                    <div className="card bg-base-200 shadow-lg">
                        <div className="card-body">
                            <div className="flex justify-center items-center py-8">
                                <span className="loading loading-spinner loading-lg text-primary"></span>
                            </div>
                        </div>
                    </div>
                ) : userDetails && (
                    <div className="card bg-base-200 shadow-lg">
                        <div className="card-body">
                            <h2 className="card-title text-2xl mb-4 flex items-center gap-2">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                                </svg>
                                Personal Information
                            </h2>

                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                {/* Full Name */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-semibold text-base-content/70">Full Name</span>
                                    </label>
                                    <div className="text-lg font-medium">
                                        {userDetails.firstName} {userDetails.lastName}
                                    </div>
                                </div>

                                {/* Email */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-semibold text-base-content/70">Email Address</span>
                                    </label>
                                    <div className="text-lg font-medium">
                                        {userDetails.email}
                                    </div>
                                </div>

                                {/* Date of Birth */}
                                {userDetails.DOB && (
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-semibold text-base-content/70">Date of Birth</span>
                                        </label>
                                        <div className="text-lg font-medium">
                                            {new Date(userDetails.DOB).toLocaleDateString('en-US', {
                                                year: 'numeric',
                                                month: 'long',
                                                day: 'numeric'
                                            })}
                                        </div>
                                    </div>
                                )}

                                {/* Age */}
                                {userDetails.DOB && (
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-semibold text-base-content/70">Age</span>
                                        </label>
                                        <div className="text-lg font-medium">
                                            {getAge(new Date(userDetails.DOB))} years old
                                        </div>
                                    </div>
                                )}

                                {/* Account Created */}
                                {userDetails.createdAt && (
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-semibold text-base-content/70">Member Since</span>
                                        </label>
                                        <div className="text-lg font-medium">
                                
                                            {new Date(userDetails.createdAt).toLocaleDateString('en-US', {
                                                year: 'numeric',
                                                month: 'long',
                                                day: 'numeric'
                                            })}
                                        </div>
                                    </div>
                                )}

                                {/* User ID */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-semibold text-base-content/70">User ID</span>
                                    </label>
                                    <div className="text-sm font-mono text-base-content/60 break-all">
                                        {userDetails.id}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                )}

                {/* Password Change Card */}
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
    );
}