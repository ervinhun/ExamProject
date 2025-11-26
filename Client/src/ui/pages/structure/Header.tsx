import {authAtom, isLoggedInAtom, logoutAtom} from "@core/atoms/auth";
import Dock from "./Dock.tsx";
import {useAtom} from "jotai";

export default function Header() {

    const [authUser,] = useAtom(authAtom);
    const [, logout] = useAtom(logoutAtom);
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
                            Hello, {authUser?.name ?? authUser?.email ?? 'User'}!
                        </span>

                        <button
                            className="btn btn-sm btn-error text-white"
                            onClick={logout}
                            hidden={!isLoggedIn}
                        >
                            Logout
                        </button>
                    </>
                )}
            </div>
        );
    }

    return (
        <div className="flex justify-center">
            <div className="w-full max-w-5xl px-6 py-12 bg-base-300 rounded-xl shadow-md space-y-6 text-center">
                {getTitle()}
                <div className="flex justify-center">
                    <Dock/>
                </div>
            </div>
        </div>
    );
}
