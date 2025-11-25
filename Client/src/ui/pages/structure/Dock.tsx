import {NavLink} from "react-router-dom";
import DockPlayer from "./DockPlayer.tsx";
import DockAdmin from "./DockAdmin.tsx";
// import { authUserAtom } from "@core/atoms/auth.ts";
import { authAtom } from "@core/atoms/auth";
import { useAtom } from "jotai";
import { useEffect } from "react";
import { envConfig } from "@core/config/EnvConfig";
import { isLoggedInAtom } from "@core/atoms/auth.ts";

export default function Dock() {
    const [authUser,] = useAtom(authAtom)
    const [isLoggedIn,] = useAtom(isLoggedInAtom)

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
            {!isLoggedIn && (

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



            {isLoggedIn && authUser.roles.includes(Number(envConfig.PLAYER_ROLE)) && (
                <DockPlayer />
            )}


            {isLoggedIn && (authUser.roles.includes(Number(envConfig.ADMIN_ROLE)) || authUser.roles.includes(Number(envConfig.SUPERADMIN_ROLE))) && (
                <DockAdmin />
            )}
        </div>
    );
}
