import { NavLink } from "react-router-dom";
import { useState } from "react";

export default function DockPlayer() {
    const [openBoards, setOpenBoards] = useState(false);
    const [openHistory, setOpenHistory] = useState(false);

    return (
        <div className="flex gap-4 items-start">
            {/* Balance */}
            <NavLink
                to="/balance"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center`
                }
            >
                <img
                    src="https://img.icons8.com/?size=100&id=LOoB110eJTzD&format=png&color=FE9900"
                    alt="Balance"
                    className="dock-icon size-[1.7em]"
                />
                <span className="dock-label text-accent">Balance</span>
            </NavLink>

            {/* Boards dropdown */}
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

                <ul
                    tabIndex={0}
                    className="dropdown-content menu p-2 shadow bg-base-200 rounded-box w-52"
                >
                    <li>
                        <NavLink to="/boards">My Boards</NavLink>
                    </li>
                    <li>
                        <NavLink to="/boards/new">Create Board</NavLink>
                    </li>
                    <li>
                        <NavLink to="/boards/repeating">Repeating Boards</NavLink>
                    </li>
                </ul>
            </div>

            {/* Game History dropdown */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenHistory(!openHistory)}
                    className={`dock-button flex items-center ${openHistory ? "dock-active" : ""}`}
                >
                    <img
                        src="https://img.icons8.com/?size=100&id=6904&format=png&color=FE9900"
                        alt="History"
                        className="dock-icon size-[1.7em]"
                    />
                    <span className="dock-label text-accent">History</span>
                </div>

                <ul
                    tabIndex={0}
                    className="dropdown-content menu p-2 shadow bg-base-200 rounded-box w-52"
                >
                    <li>
                        <NavLink to="/games">Game History</NavLink>
                    </li>
                    <li>
                        <NavLink to="/games/wins">Winning Boards</NavLink>
                    </li>
                </ul>
            </div>

            {/* Profile */}
            <NavLink
                to="/profile"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center`
                }
            >
                <img
                    src="https://img.icons8.com/?size=100&id=23264&format=png&color=FE9900"
                    alt="Profile"
                    className="dock-icon size-[1.7em]"
                />
                <span className="dock-label text-accent">Profile</span>
            </NavLink>
        </div>
    );
}
