import { NavLink } from "react-router-dom";
import { useState } from "react";

export default function DockAdmin() {
    const [openPlayers, setOpenPlayers] = useState(false);
    const [openGames, setOpenGames] = useState(false);
    const [openTransactions, setOpenTransactions] = useState(false);
    const [openBoards, setOpenBoards] = useState(false);

    return (
        <div className="flex gap-4 items-start">
            {/* Dashboard */}
            <NavLink
                to="/admin/dashboard"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center`
                }
            >
                <img
                    src="https://img.icons8.com/?size=100&id=sUJRwjfnGwbJ&format=png&color=FE9900"
                    alt="Dashboard"
                    className="dock-icon size-[1.7em]"
                />
                <span className="dock-label text-accent">Dashboard</span>
            </NavLink>

            {/* Players Management */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenPlayers(!openPlayers)}
                    className={`dock-button flex items-center ${openPlayers ? "dock-active" : ""}`}
                >
                    <img
                        src="https://img.icons8.com/?size=100&id=23264&format=png&color=FE9900"
                        alt="Players"
                        className="dock-icon size-[1.7em]"
                    />
                    <span className="dock-label text-accent">Players</span>
                </div>

                <ul tabIndex={0} className="dropdown-content menu p-2 shadow bg-base-200 rounded-box w-52">
                    <li>
                        <NavLink to="/admin/players">All Players</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/players/new">Add Player</NavLink>
                    </li>
                </ul>
            </div>

            {/* Games Management */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenGames(!openGames)}
                    className={`dock-button flex items-center ${openGames ? "dock-active" : ""}`}
                >
                    <img
                        src="https://img.icons8.com/?size=100&id=8Bv7Az7nb1dV&format=png&color=FE9900"
                        alt="Games"
                        className="dock-icon size-[1.7em]"
                    />
                    <span className="dock-label text-accent">Games</span>
                </div>

                <ul tabIndex={0} className="dropdown-content menu p-2 shadow bg-base-200 rounded-box w-52">
                    <li>
                        <NavLink to="/admin/games">Game History</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/games/winning-numbers">Enter Winning Numbers</NavLink>
                    </li>
                </ul>
            </div>

            {/* Transactions */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenTransactions(!openTransactions)}
                    className={`dock-button flex items-center ${openTransactions ? "dock-active" : ""}`}
                >
                    <img
                        src="https://img.icons8.com/?size=100&id=qXJsrsUbVH3B&format=png&color=FE9900"
                        alt="Transactions"
                        className="dock-icon size-[1.7em]"
                    />
                    <span className="dock-label text-accent">Transactions</span>
                </div>

                <ul tabIndex={0} className="dropdown-content menu p-2 shadow bg-base-200 rounded-box w-52">
                    <li>
                        <NavLink to="/admin/transactions/pending">Pending</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/transactions/all">All Transactions</NavLink>
                    </li>
                </ul>
            </div>

            {/* Boards Overview */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenBoards(!openBoards)}
                    className={`dock-button flex items-center ${openBoards ? "dock-active" : ""}`}
                >
                    <img
                        src="https://img.icons8.com/?size=100&id=85462&format=png&color=FE9900"
                        alt="Boards"
                        className="dock-icon size-[1.7em]"
                    />
                    <span className="dock-label text-accent">Boards</span>
                </div>

                <ul tabIndex={0} className="dropdown-content menu p-2 shadow bg-base-200 rounded-box w-52">
                    <li>
                        <NavLink to="/admin/boards">All Boards</NavLink>
                    </li>
                    <li>
                        <NavLink to="/admin/boards/winning">Winning Boards</NavLink>
                    </li>
                </ul>
            </div>

            {/* Settings */}
            <NavLink
                to="/admin/settings"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center`
                }
            >
                <img
                    src="https://img.icons8.com/?size=100&id=364&format=png&color=FE9900"
                    alt="Settings"
                    className="dock-icon size-[1.7em]"
                />
                <span className="dock-label text-accent">Settings</span>
            </NavLink>
        </div>
    );
}