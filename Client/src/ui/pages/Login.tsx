import {useState} from "react";
import {useAtom} from "jotai";
import {AuthAtom} from "../../utils/Atom.ts";
import {useNavigate} from "react-router-dom";

export default function Login() {

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [accessToken, setAccessToken] = useState<string | null>(null);
    const [, setUser] = useState<string | null>(null);
    const [, setAuth] = useAtom(AuthAtom);
    const navigate = useNavigate();

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();


        const response = await fetch("http://localhost:5152/api/auth/login", {
            method: "POST",
            credentials: "include",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({email, password})
        });

        if (!response.ok) {
            alert("Login failed");
            return;
        }

        const data = await response.json();

        // Save user + token globally
        setUser(data.user);

        setAuth({
            name: data.user.name,
            email: data.user.email,
            role: data.user.roles,
            token: data.accessToken
        })
        console.log( data.user.name);
        console.log( data.user.email);
        console.log(data.user.roles);
        console.log(data.accessToken);


        // Go to homepage
        navigate("/");
    }

    // Generic API wrapper with auto-refresh logic
    async function apiFetch(url: string, options: any = {}) {

        const res = await fetch(url, {
            ...options,
            credentials: "include", // send refresh token cookie
            headers: {
                "Authorization": `Bearer ${accessToken}`,
                ...(options.headers || {})
            }
        });

        if (res.status !== 401) {
            return res;
        }

        // Try refresh token
        const refreshRes = await fetch("http://localhost:5152/api/auth/refresh", {
            method: "POST",
            credentials: "include"
        });

        if (refreshRes.ok) {
            const data = await refreshRes.json();
            setAccessToken(data.accessToken);

            // Retry the original request
            return apiFetch(url, options);
        }

        // Refresh failed → logout user
        logout();
        return res;
    }

    function logout() {
        setUser(null);
        setAccessToken(null);
        fetch("http://localhost:5152/api/auth/logout", {
            method: "POST",
            credentials: "include"
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
