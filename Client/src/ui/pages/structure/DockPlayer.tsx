import { NavLink } from "react-router-dom";
import { useState } from "react";

export default function DockPlayer() {
    const [openHistory, setOpenHistory] = useState(false);

    return (
        <div id="dock"className="flex gap-10 items-center">
            {/* Wallet */}
            <NavLink
                to="/wallet"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center gap-2`
                }
            >
                <img
                    src="https://img.icons8.com/?size=100&id=LOoB110eJTzD&format=png&color=FE9900"
                    alt="Wallet"
                    className="dock-icon h-7 w-7"
                />
                <span className="dock-label text-accent text-base">Wallet</span>
            </NavLink>

            {/* My Tickets */}
            <NavLink
                to="/tickets"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center gap-2`
                }
            >
                <img
                    src="https://img.icons8.com/?size=100&id=85462&format=png&color=FE9900"
                    alt="My Tickets"
                    className="dock-icon h-7 w-7"
                />
                <span className="dock-label text-accent text-base">My Tickets</span>
            </NavLink>

            {/* Game History dropdown */}
            <div className="dropdown dropdown-hover">
                <div
                    tabIndex={0}
                    role="button"
                    onClick={() => setOpenHistory(!openHistory)}
                    className={`dock-button flex items-center gap-2 ${openHistory ? "dock-active" : ""}`}
                >
                    <img
                        src="https://img.icons8.com/?size=100&id=6904&format=png&color=FE9900"
                        alt="History"
                        className="dock-icon h-7 w-7"
                    />
                    <span className="dock-label text-accent text-base cursor-default">History</span>
                    <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className={`h-4 w-4 transition-transform ${openHistory ? 'rotate-180' : ''}`}
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                    >
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                    </svg>
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

            {/* Account */}
            <NavLink
                to="/profile"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center gap-2`
                }
            >
                <img
                    src="https://img.icons8.com/?size=100&id=23264&format=png&color=FE9900"
                    alt="Account"
                    className="dock-icon h-7 w-7"
                />
                <span className="dock-label text-accent text-base">Account</span>
            </NavLink>
        </div>
    );
}
