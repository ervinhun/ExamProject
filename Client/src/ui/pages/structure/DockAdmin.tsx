import { NavLink } from "react-router-dom";
import { useState } from "react";
import { useAtom } from "jotai";
import { authAtom } from "@core/atoms/auth";
import { envConfig } from "@core/config/EnvConfig";
import dashboardIcon from "@ui/assets/layout-dashboard.svg";
import userIcon from "@ui/assets/user.svg";
import dicesIcon from "@ui/assets/dices.svg";
import transactionIcon from "@ui/assets/arrow-left-right.svg";
import settingsIcon from "@ui/assets/cog.svg";

export default function DockAdmin() {
    const [openUsers, setOpenUsers] = useState(false);
    const [openGames, setOpenGames] = useState(false);
    const [openTransactions, setOpenTransactions] = useState(false);
    const [authUser] = useAtom(authAtom);
    
    const isSuperAdmin = authUser?.roles.includes(Number(envConfig.SUPERADMIN_ROLE));

    return (
        <div id="dock" className="flex gap-10 items-center">
            {/* Dashboard */}
            <NavLink
                to="/admin/dashboard"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center gap-2`
                }
            >
                <img
                    src={dashboardIcon}
                    alt="Dashboard"
                    className="dock-icon h-7 w-7 object-contain"
                />
                <span className="dock-label text-accent text-base">Dashboard</span>
            </NavLink>

            {/* Players Management */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenUsers(!openUsers)}
                    className={`dock-button flex items-center gap-2 ${openUsers ? "dock-active" : ""}`}
                >
                    <img
                        src={userIcon}
                        alt="Users"
                        className="dock-icon h-7 w-7 object-contain"
                    />
                    <span className="dock-label text-accent text-base cursor-default">Users</span>
                    <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className={`h-4 w-4 transition-transform ${openUsers ? 'rotate-180' : ''}`}
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                    >
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                    </svg>
                </div>

                <ul tabIndex={0} className="dropdown-content menu p-2 shadow bg-base-200 rounded-box w-52">
                    <li>
                        <NavLink to="/admin/players">All Players</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/players/register">Register Player</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/players/applications">Player Applications </NavLink>
                    </li>
                    {isSuperAdmin && (
                        <li>
                            <NavLink to="/admin/create-admin">Create Admin</NavLink>
                        </li>
                    )}
                </ul>
            </div>

            {/* Games Management */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenGames(!openGames)}
                    className={`dock-button flex items-center gap-2 ${openGames ? "dock-active" : ""}`}
                >
                    <img
                        src={dicesIcon}
                        alt="Games"
                        className="dock-icon h-7 w-7 object-contain"
                    />
                    <span className="dock-label text-accent text-base cursor-default">Games</span>
                    <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className={`h-4 w-4 transition-transform ${openGames ? 'rotate-180' : ''}`}
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                    >
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                    </svg>
                </div>

                <ul tabIndex={0} className="dropdown-content menu p-2 shadow bg-base-200 rounded-box w-52">
                    <li>
                        <NavLink to="/admin/games/overview">Overview</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/games/start">Start Game</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/games/history">Game History</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/games/templates/create">Create Game Template</NavLink>
                    </li>
                </ul>
            </div>

            {/* Transactions */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenTransactions(!openTransactions)}
                    className={`dock-button flex items-center gap-2 ${openTransactions ? "dock-active" : ""}`}
                >
                    <img
                        src={transactionIcon}
                        alt="Transactions"
                        className="dock-icon h-7 w-7 object-contain"
                    />
                    <span className="dock-label text-accent text-base cursor-default">Transactions</span>
                    <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className={`h-4 w-4 transition-transform ${openTransactions ? 'rotate-180' : ''}`}
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                    >
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                    </svg>
                </div>

                <ul tabIndex={0} className="dropdown-content menu p-2 shadow bg-base-200 rounded-box w-52">
                    <li>
                        <NavLink to="/admin/transactions/pending">Pending</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/transactions/history">History</NavLink>
                    </li>
                </ul>
            </div>

            {/* Settings */}
            <NavLink
                to="/admin/settings"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center gap-2`
                }
            >
                <img
                    src={settingsIcon}
                    alt="Settings"
                    className="dock-icon h-7 w-7 object-contain"
                />
                <span className="dock-label text-accent text-base">Settings</span>
            </NavLink>
        </div>
    );
}