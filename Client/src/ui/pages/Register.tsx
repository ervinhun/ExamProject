import {useLocation} from "react-router-dom";

export default function Register() {
    const location = useLocation();

    let title:string;
    if (location.pathname === "/admin/players/register") {
        title = "Register player";
    }
    else
        title = "Application for membership";
    return (
        <div className="flex justify-center mt-10 w-full">
            <form className="w-full max-w-lg p-6 bg-base-200 rounded-xl shadow-md space-y-4">
                <h2 className="text-3xl font-semibold text-center mb-4">{title}</h2>

                {/* First Name */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">First Name</span>
                    </label>
                    <input type="text" className="input input-bordered w-full" required />
                </div>

                {/* Last Name */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Last Name</span>
                    </label>
                    <input type="text" className="input input-bordered w-full" required />
                </div>

                {/* Phone Numbers */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Mobile Phone</span>
                    </label>
                    <input type="tel" className="input input-bordered w-full" required />
                </div>

                {/* Birthdate */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Date of Birth</span>
                    </label>
                    <input type="date" className="input input-bordered w-full" required />
                </div>

                {/* Gender */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Gender</span>
                    </label>
                    <select className="select select-bordered w-full" required>
                        <option value="">Select gender</option>
                        <option>Male</option>
                        <option>Female</option>
                        <option>Other</option>
                    </select>
                </div>

                {/* Email */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Email</span>
                    </label>
                    <input type="email" className="input input-bordered w-full" required />
                </div>

                {/* Confirm Email */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Confirm Email</span>
                    </label>
                    <input type="email" className="input input-bordered w-full" required />
                </div>

                {/* Password */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Password</span>
                    </label>
                    <input type="password" className="input input-bordered w-full" required />
                </div>

                {/* Confirm Password */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Confirm Password</span>
                    </label>
                    <input type="password" className="input input-bordered w-full" required />
                </div>

                {/* Submit */}
                <button className="btn btn-primary w-full mt-2">
                    Register
                </button>
            </form>
        </div>
    );
}
