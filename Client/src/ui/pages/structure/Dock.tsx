import {NavLink} from "react-router-dom";
import {useApi} from "../../../utils/useApi.ts";
import DockPlayer from "./DockPlayer.tsx";
import DockAdmin from "./DockAdmin.tsx";

export default function Dock() {
    const {isLoggedIn, getRole} = useApi();

    return (
        <div className="flex space-x-10 bg-base-300 shadow-sm navbar-center w-full">
            <NavLink
                to="/"
                className={({isActive}) =>
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
            {!isLoggedIn() && (

            <NavLink
                to="/login"
                className={({isActive}) =>
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
            )}



            {isLoggedIn() && getRole()?.includes("Player") && (
                <DockPlayer />
            )}


            {isLoggedIn() && (getRole()?.includes("Admin") || getRole()?.includes("SuperAdmin")) && (
                <DockAdmin />
            )}
        </div>
    );
}
