import {authAtom, isLoggedInAtom, logoutAtom} from "@core/atoms/auth";
import {useAtom} from "jotai";
import { NavLink } from "react-router-dom";

export default function Header() {

    const [authUser,] = useAtom(authAtom);
    const [, logout] = useAtom(logoutAtom);
    const [isLoggedIn,] = useAtom(isLoggedInAtom);


    function getTitle() {
        console.log("Rendering Header, isLoggedIn:", isLoggedIn, "authAtom:", authUser);
        return (
            <div className="flex items-center justify-between w-full">
                {/* Logo and Title */}
                <div className="flex items-center gap-4">
                    <img
                        src="https://jerneif.dk/cms/Clubjerneif/images/logo.png"
                        alt="Jerne IF"
                        className="w-16 h-16 md:w-18 md:h-18 lg:w-20 lg:h-20 object-contain"
                    />
                    <div className="hidden md:flex flex-col">
                        <h1 className="text-xl lg:text-2xl font-bold text-primary leading-tight">Jerne IF</h1>
                        <p className="text-sm lg:text-base text-base-content/70">Esbjerg</p>
                    </div>
                </div>

                {/* Navigation - Right Side */}
                <div className="flex items-center gap-3">
                    {isLoggedIn ? (
                        <>
                            {/* Home Button */}
                            <NavLink 
                                to="/"
                                className="btn btn-ghost gap-2"
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                                </svg>
                                <span className="hidden sm:inline text-base">Home</span>
                            </NavLink>

                            {/* User Info */}
                            <div className="hidden md:flex items-center gap-2 px-4 py-2 bg-base-200 rounded-lg">
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-primary" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                                </svg>
                                <span className="text-base font-medium">
                                    {authUser.name || authUser.email}
                                </span>
                            </div>

                            {/* Logout Button */}
                            <button
                                className="btn btn-ghost gap-2"
                                onClick={logout}
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                                </svg>
                                <span className="hidden sm:inline text-base">Logout</span>
                            </button>
                        </>
                    ) : (
                        <>
                            {/* Login Button */}
                            <NavLink 
                                to="/login"
                                className="btn btn-primary gap-2"
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1" />
                                </svg>
                                <span className="hidden sm:inline text-base">Login</span>
                            </NavLink>
                        </>
                    )}
                </div>
            </div>
        );
    }

    return (
        <div className="sticky top-0 z-50 bg-base-300 shadow-sm">
            <div className="container mx-auto px-4 py-4">
                {getTitle()}
            </div>
        </div>
    );
}
