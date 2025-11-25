import {useEffect, useState} from "react";
import {useAtom} from "jotai";
import {useNavigate} from "react-router-dom";
import { isLoggedInAtom, loginAtom } from "@core/atoms/auth";
import { authAtom } from "@core/atoms/auth";

export default function Login() {

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [isLoggedIn, ] = useAtom(isLoggedInAtom);

    const [,login] = useAtom(loginAtom);

    const navigate = useNavigate();

    useEffect(() => {
        if(isLoggedIn) {
            navigate("/");
            return;
        }  
    }, [isLoggedIn]);

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        await login({email, password}).finally(() => {
            navigate("/");
        }); 

    }


    return (
        <div className="flex justify-center mt-10 w-full">
            <form
                onSubmit={handleSubmit}
                className="w-full max-w-sm p-6 bg-base-200 rounded-xl shadow-md space-y-4"
            >
                <h2 className="text-2xl font-semibold text-center">Login</h2>

                {/* Email */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Email</span>
                    </label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="you@example.com"
                        className="input input-bordered w-full"
                        required
                    />
                </div>

                {/* Password */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Password</span>
                    </label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="••••••••"
                        className="input input-bordered w-full"
                        required
                    />
                    <label className="label">
                        <a href="#" className="label-text-alt link link-hover">
                            Forgot password?
                        </a>
                    </label>
                </div>

                {/* Submit */}
                <button className="btn btn-primary w-full">Login</button>

                {/* Register.tsx */}
                <p className="text-center text-sm mt-2">
                    Don’t have an account?{" "}
                    <a href="/register" className="link link-hover text-primary">
                        Register
                    </a>
                </p>
            </form>
        </div>
    );
}
