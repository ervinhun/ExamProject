import Dock from "./Dock.tsx";
import {useNavigate} from "react-router-dom";
import {useApi} from "../../../utils/useApi.ts"
import {useAtom} from "jotai";
import {AuthAtom} from "../../../utils/Atom.ts";

export default function Header() {

    const navigate = useNavigate();
    const [auth] = useAtom(AuthAtom);
    const {isLoggedIn, resetAuth} = useApi();

    function logout() {
        resetAuth();
        // Optional: call backend logout to clear refresh cookie
        //fetch("http://localhost:5152/api/auth/logout", {
        //    method: "POST",
        //    credentials: "include"
        //});

        navigate("/");
    }

    function getTitle() {
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
                {isLoggedIn() && (
                    <>
                        <span className="font-medium">
                            Hello, {auth.name || auth.email}
                        </span>

                        <button
                            className="btn btn-sm btn-error text-white"
                            onClick={logout}>
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
