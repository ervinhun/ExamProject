export default function AddPlayer() {
    return (
        <div className="flex justify-center mt-10 w-full">
            <form className="w-full max-w-lg p-6 bg-base-200 rounded-xl shadow-md space-y-4">
                <h2 className="text-3xl font-semibold text-center mb-4">Registration</h2>

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

                {/* Address */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Address</span>
                    </label>
                    <input type="text" className="input input-bordered w-full" required />
                </div>

                {/* ZIP + City */}
                <div className="grid grid-cols-2 gap-4">
                    <div className="form-control">
                        <label className="label">
                            <span className="label-text">ZIP Code</span>
                        </label>
                        <input type="text" className="input input-bordered w-full" required />
                    </div>

                    <div className="form-control">
                        <label className="label">
                            <span className="label-text">City</span>
                        </label>
                        <input type="text" className="input input-bordered w-full" required />
                    </div>
                </div>

                {/* Municipality */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Municipality</span>
                    </label>
                    <input type="text" className="input input-bordered w-full" />
                </div>

                {/* Phone Numbers */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Mobile Phone</span>
                    </label>
                    <input type="tel" className="input input-bordered w-full" required />
                </div>

                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Additional Mobile</span>
                    </label>
                    <input type="tel" className="input input-bordered w-full" />
                </div>

                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Landline</span>
                    </label>
                    <input type="tel" className="input input-bordered w-full" />
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

                {/* Extra Emails */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Extra Email</span>
                    </label>
                    <input type="email" className="input input-bordered w-full" />
                </div>

                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Extra Email 2</span>
                    </label>
                    <input type="email" className="input input-bordered w-full" />
                </div>

                {/* Submit */}
                <button className="btn btn-primary w-full mt-2" type="submit">
                    Register
                </button>
            </form>
        </div>
    );
}