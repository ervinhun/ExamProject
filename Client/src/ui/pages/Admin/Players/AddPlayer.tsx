import { useAtom } from "jotai";
import { createUserAtom } from "@core/atoms/user";

export default function AddPlayer() {    
    const [,createUser] = useAtom(createUserAtom)
   async function handleSubmit(e:React.FormEvent<HTMLFormElement>){

        e.preventDefault();
        await createUser({
            firstName: "abc",
            lastName: "def",
            email: "abc@mail.com",
            phoneNumber:"123456123"
        });
    }
    return (
        <div className="flex justify-center mt-10 w-full">
            <form className="w-full max-w-lg p-6 bg-base-200 rounded-xl shadow-md space-y-4" onSubmit={handleSubmit}>
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

                {/* Submit */}
                <button className="btn btn-primary w-full mt-2" type="submit">
                    Register
                </button>
            </form>
        </div>
    );
}