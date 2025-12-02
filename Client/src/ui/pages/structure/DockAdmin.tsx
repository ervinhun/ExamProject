import { NavLink } from "react-router-dom";
import { useState } from "react";

export default function DockAdmin() {
    const [openPlayers, setOpenPlayers] = useState(false);
    const [openGames, setOpenGames] = useState(false);
    const [openTransactions, setOpenTransactions] = useState(false);

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
                    src="https://img.icons8.com/?size=100&id=sUJRwjfnGwbJ&format=png&color=FE9900"
                    alt="Dashboard"
                    className="dock-icon h-7 w-7"
                />
                <span className="dock-label text-accent text-base">Dashboard</span>
            </NavLink>

            {/* Players Management */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenPlayers(!openPlayers)}
                    className={`dock-button flex items-center gap-2 ${openPlayers ? "dock-active" : ""}`}
                >
                    <img
                        src="https://img.icons8.com/?size=100&id=23264&format=png&color=FE9900"
                        alt="Players"
                        className="dock-icon h-7 w-7"
                    />
                    <span className="dock-label text-accent text-base cursor-default">Players</span>
                    <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className={`h-4 w-4 transition-transform ${openPlayers ? 'rotate-180' : ''}`}
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
                        <NavLink to="/admin/players/applications">Applications (3)</NavLink>
                    </li>
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
                        src="https://img.icons8.com/?size=100&id=8Bv7Az7nb1dV&format=png&color=FE9900"
                        alt="Games"
                        className="dock-icon h-7 w-7"
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
                        src="https://img.icons8.com/?size=100&id=qXJsrsUbVH3B&format=png&color=FE9900"
                        alt="Transactions"
                        className="dock-icon h-7 w-7"
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
                    src="https://img.icons8.com/?size=100&id=364&format=png&color=FE9900"
                    alt="Settings"
                    className="dock-icon h-7 w-7"
                />
                <span className="dock-label text-accent text-base">Settings</span>
            </NavLink>
        </div>
    );
}