import { Link } from "react-router-dom";

export default function Forbidden403() {
    return (
        <div className="mt-6 flex flex-col items-center justify-center text-center">

            <div className="flex flex-col items-center justify-center mb-3">
                <h1 className="text-7xl font-bold text-error">403</h1>
                <h2 className="text-2xl font-bold text-error">Unauthorized</h2>
            </div>

            <p className="mt-2 opacity-80 max-w-md">
                You don't have permission to access this page.
            </p>

            <div className="mt-8 flex flex-col gap-4">
                <Link to="/" className="btn btn-primary w-full">
                    Go Home
                </Link>
            </div>

        </div>
    );
}
