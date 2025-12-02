import DockPlayer from "./DockPlayer.tsx";
import DockAdmin from "./DockAdmin.tsx";
import { authAtom } from "@core/atoms/auth";
import { useAtom } from "jotai";
import { envConfig } from "@core/config/EnvConfig";
import { isLoggedInAtom } from "@core/atoms/auth";
import { NavLink } from "react-router-dom";

export default function Dock() {
    const [authUser,] = useAtom(authAtom)
    const [isLoggedIn,] = useAtom(isLoggedInAtom)

    return (
        <div id="dock-root" className="flex space-x-6 items-center">
            {!isLoggedIn && (
                <div className="flex gap-4">
                    <NavLink
                        to="/about"
                        className={({ isActive }) =>
                            `dock-button ${isActive ? "dock-active" : ""} flex items-center gap-2`
                        }
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" className="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <span className="dock-label text-accent text-base">About</span>
                    </NavLink>
                    <NavLink
                        to="/contact"
                        className={({ isActive }) =>
                            `dock-button ${isActive ? "dock-active" : ""} flex items-center gap-2`
                        }
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" className="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                        </svg>
                        <span className="dock-label text-accent text-base">Contact</span>
                    </NavLink>
                </div>
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
