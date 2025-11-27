import { useAtom } from "jotai";
import { createUserAtom } from "@core/atoms/user";
import { useState } from "react";
import { errorAtom } from "@core/atoms/error";
import ErrorPopUp from "../../Errors/ErrorPopUp";
import { createPlayerAtom } from "@core/atoms/players";

export default function AddPlayer() {    
    const [,createPlayer] = useAtom(createPlayerAtom)
    const [error,] = useAtom(errorAtom);
    
    // Form state
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [address, setAddress] = useState("");
    const [zipCode, setZipCode] = useState("");
    const [city, setCity] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [birthDate, setBirthDate] = useState("");
    const [gender, setGender] = useState("");
    const [email, setEmail] = useState("");
    const [confirmEmail, setConfirmEmail] = useState("");
    
    async function handleSubmit(e:React.FormEvent<HTMLFormElement>){
        e.preventDefault();
        await createPlayer({
            firstName,
            lastName,
            email,
            phoneNumber
        });
    }

    return (
        <div className="flex justify-center mt-10 w-full">
            <ErrorPopUp />

            <form className="w-full max-w-lg p-6 bg-base-200 rounded-xl shadow-md space-y-4" onSubmit={handleSubmit}>
                <h2 className="text-3xl font-semibold text-center mb-4">Registration</h2>

                {/* First Name */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">First Name</span>
                    </label>
                    <input 
                        type="text" 
                        className="input input-bordered w-full" 
                        value={firstName}
                        onChange={(e) => setFirstName(e.target.value)}
                        required 
                    />
                </div>

                {/* Last Name */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Last Name</span>
                    </label>
                    <input 
                        type="text" 
                        className="input input-bordered w-full" 
                        value={lastName}
                        onChange={(e) => setLastName(e.target.value)}
                        required 
                    />
                </div>

                {/* Address */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Address</span>
                    </label>
                    <input 
                        type="text" 
                        className="input input-bordered w-full" 
                        value={address}
                        onChange={(e) => setAddress(e.target.value)}
                        required 
                    />
                </div>

                {/* ZIP + City */}
                <div className="grid grid-cols-2 gap-4">
                    <div className="form-control">
                        <label className="label">
                            <span className="label-text">ZIP Code</span>
                        </label>
                        <input 
                            type="text" 
                            className="input input-bordered w-full" 
                            value={zipCode}
                            onChange={(e) => setZipCode(e.target.value)}
                            required 
                        />
                    </div>

                    <div className="form-control">
                        <label className="label">
                            <span className="label-text">City</span>
                        </label>
                        <input 
                            type="text" 
                            className="input input-bordered w-full" 
                            value={city}
                            onChange={(e) => setCity(e.target.value)}
                            required 
                        />
                    </div>
                </div>

                {/* Phone Numbers */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Mobile Phone</span>
                    </label>
                    <input 
                        type="tel" 
                        className="input input-bordered w-full" 
                        value={phoneNumber}
                        onChange={(e) => setPhoneNumber(e.target.value)}
                        required 
                    />
                </div>

                {/* Birthdate */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Date of Birth</span>
                    </label>
                    <input 
                        type="date" 
                        className="input input-bordered w-full" 
                        value={birthDate}
                        onChange={(e) => setBirthDate(e.target.value)}
                        required 
                    />
                </div>

                {/* Gender */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Gender</span>
                    </label>
                    <select 
                        className="select select-bordered w-full" 
                        value={gender}
                        onChange={(e) => setGender(e.target.value)}
                        required
                    >
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
                    <input 
                        type="email" 
                        className="input input-bordered w-full" 
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required 
                    />
                </div>

                {/* Confirm Email */}
                <div className="form-control">
                    <label className="label">
                        <span className="label-text">Confirm Email</span>
                    </label>
                    <input 
                        type="email" 
                        className="input input-bordered w-full" 
                        value={confirmEmail}
                        onChange={(e) => setConfirmEmail(e.target.value)}
                        required 
                    />
                </div>

                {/* Submit */}
                <button className="btn btn-primary w-full mt-2" type="submit">
                    Register
                </button>
            </form>
        </div>
    );
}

