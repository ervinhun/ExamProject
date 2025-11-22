import {NavLink} from "react-router-dom";

export default function Dock() {
    return (
        <div className="flex space-x-10 bg-base-300 shadow-sm navbar-center w-full">
            <NavLink
                to="/"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center`
                }
            >
                <img
                    src="https://img.icons8.com/?size=100&id=14096&format=png&color=FE9900"
                    alt="Home"
                    className="dock-icon size-[1.7em]"
                />
                <span className="dock-label text-accent">Home</span>
            </NavLink>

            <NavLink
                to="/login"
                className={({ isActive }) =>
                    `dock-button ${isActive ? "dock-active" : ""} flex items-center`
                }
            >
                <img
                    src="https://img.icons8.com/?size=100&id=14312&format=png&color=FE9900"
                    alt="Login"
                    className="dock-icon size-[1.7em]"
                />
                <span className="dock-label text-accent">Login</span>
            </NavLink>
        </div>
    );
}
