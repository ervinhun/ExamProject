import { authAtom } from "@core/atoms/auth";
import { isLoggedInAtom, logoutAtom } from "@core/atoms/auth";
import Dock from "./Dock.tsx";
import {useAtom} from "jotai";
import {useNavigate} from "react-router-dom";
// import {AuthAtom} from "@core/atoms/auth";

export default function Header() {

    const [authUser,] = useAtom(authAtom);
    // const   [currentUser, setCurrentUser] = useAtom(currentUserAtom);
    const [,logout] = useAtom(logoutAtom);
    const navigate = useNavigate();
    const [isLoggedIn,] = useAtom(isLoggedInAtom);



    function getTitle() {
        console.log("Rendering Header, isLoggedIn:", isLoggedIn, "authAtom:", authUser);
        return (
            <div className="flex items-center space-x-4 mb-3">
                <img
                    src="https://jerneif.dk/cms/Clubjerneif/images/logo.png"
                    alt="Jerne IF"
                    className="w-16 h-16 object-cover"
                />
                <h1 className="text-4xl md:text-6xl font-bold text-default-400">
                    Lottery App
                </h1>
                {isLoggedIn && (
                    <>
                        <span className="font-medium">
                            Hello, {authUser?.email ?? authUser?.name ?? 'User'}!
                        </span>

                        <button
                            className="btn btn-sm btn-error text-white"
                            onClick={logout}
                        >
                            Logout
                        </button>
                    </>
                )}
                {!isLoggedIn && (
                    <button
                        className="btn btn-sm btn-primary text-white"
                        onClick={() => navigate("/login")}
                    >
                        Login
                    </button>
                )}
            </div>
        );
    }

    return (
        <div className="flex justify-center">
            <div className="w-full max-w-5xl px-6 py-12 bg-base-300 rounded-xl shadow-md space-y-6 text-center">
                {getTitle()}
                <div className="flex justify-center">
                    <Dock />
                </div>
            </div>
        </div>
    );
}
