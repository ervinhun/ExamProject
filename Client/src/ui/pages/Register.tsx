import {useLocation} from "react-router-dom";
import {authApi} from "@core/api/controllers/auth.ts";


function playerApplicationOnSubmit(e) {
    e.preventDefault();
    const data = new FormData(e.currentTarget);

    const firstName = data.get("inputFirstName") as string;
    const lastName = data.get("inputLastName") as string;
    const phoneNo = data.get("inputPhoneNo") as string;
    const birthDate = data.get("inputDob") as string;
    const gender = data.get("inputGender") as string;
    const email = data.get("inputEmail") as string;
    const email2 = data.get("inputEmail2") as string;
    const password = data.get("inputPassword") as string;
    const confirmPassword = data.get("inputPassword2") as string;
    const dob = new Date(birthDate + "T00:00:00Z").toISOString();
    if (email !== email2) {
        alert("Emails do not match!");
        return;
    }
    if (password !== confirmPassword) {
        alert("Passwords do not match!");
        return;
    }
    const player = {
        "firstName": firstName,
        lastName,
        email,
        dob,
        gender,
        phoneNo,
        password
    };
    console.log(player);
    authApi.register(player)
        .then(async res => {
            if (res) {
                alert("Player registered successfully!");
            } else {
                alert("Registration failed.");
            }
        })
        .catch(err => {
            alert("Failed to register player. " + err.message);
        })
        .finally(() => {
            globalThis.location.href = "/";
        });
}

export default function Register() {
    const location = useLocation();

    let title: string;
    if (location.pathname === "/admin/players/register") {
        title = "Register player";
    } else
        title = "Application for membership";


    return (
        <div className="flex justify-center mt-10 w-full">
            <form className="w-full max-w-lg p-6 bg-base-200 rounded-xl shadow-md space-y-4"
                  onSubmit={(e) => {
                      e.preventDefault();
                      playerApplicationOnSubmit(e);
                  }}>
                <h2 className="text-3xl font-semibold text-center mb-4">{title}</h2>

                {/* First Name */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">First Name</span>
                    </label>
                    <input type="text" className="input input-bordered w-full" name="inputFirstName" required/>
                </div>

                {/* Last Name */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Last Name</span>
                    </label>
                    <input type="text" className="input input-bordered w-full" name="inputLastName" required/>
                </div>

                {/* Phone Numbers */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Mobile Phone</span>
                    </label>
                    <input type="tel" className="input input-bordered w-full" name="inputPhoneNo" required/>
                </div>

                {/* Birthdate */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Date of Birth</span>
                    </label>
                    <input type="date" className="input input-bordered w-full" name="inputDob" required/>
                </div>

                {/* Gender */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Gender</span>
                    </label>
                    <select className="select select-bordered w-full" name="inputGender" required>
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
                    <input type="email" className="input input-bordered w-full" name="inputEmail" required/>
                </div>

                {/* Confirm Email */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Confirm Email</span>
                    </label>
                    <input type="email" className="input input-bordered w-full" name="inputEmail2" required/>
                </div>

                {/* Password */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Password</span>
                    </label>
                    <input type="password" className="input input-bordered w-full" name="inputPassword" required/>
                </div>

                {/* Confirm Password */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Confirm Password</span>
                    </label>
                    <input type="password" className="input input-bordered w-full" name="inputPassword2" required/>
                </div>

                {/* Submit */}
                <button className="btn btn-primary w-full mt-2">
                    Register
                </button>
            </form>
        </div>
    );
}
