import { useAtom, useSetAtom } from "jotai";
import { useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import ErrorPopUp from "../../Errors/ErrorPopUp";
import { createPlayerAtom } from "@core/atoms/players";
import { addNotificationAtom } from "@core/atoms/error";

export default function AddPlayer() {    
    const [,createPlayer] = useAtom(createPlayerAtom)
    const addNotification = useSetAtom(addNotificationAtom);
    const navigate = useNavigate();
    
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
            birthDate: new Date(birthDate),
            phoneNumber, 
        }).then((res)=>{
        // Clear form
            setFirstName("");
            setLastName("");
            setAddress("");
            setZipCode("");
            setCity("");
            setPhoneNumber("");
            setBirthDate("");
            setGender("");
            setEmail("");
            setConfirmEmail("");
            
            navigate('/admin/players');
        
            addNotification({
                message: `Player ${firstName} ${lastName} created successfully!`,
                type: 'success'
            });
        })
        .catch((err)=>{
            addNotification({
                message: `Failed to create player. ${err.message}`,
                type: 'error'
            });
            return;
        }).finally(()=>{

        });


    }

    return (
        <div className="container mx-auto ">
            <ErrorPopUp />
            
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary">Register Player</h1>
                        <p className="text-base text-base-content/70 mt-1">Add a new player to the system</p>
                    </div>
                    <NavLink to="/admin/players" className="btn btn-ghost">
                        ← Back to Players
                    </NavLink>
                </div>

                {/* Form Card */}
                <div className="card bg-base-200 shadow-lg max-w-4xl mx-auto">
                    <div className="card-body">
                        <form onSubmit={handleSubmit} className="space-y-6">
                            {/* Personal Information */}
                            <div className="space-y-4">
                                <h2 className="card-title text-2xl">Personal Information</h2>
                                
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    {/* First Name */}
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">First Name</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <input 
                                            type="text" 
                                            className="input input-bordered w-full" 
                                            placeholder="Enter first name"
                                            value={firstName}
                                            onChange={(e) => setFirstName(e.target.value)}
                                            required 
                                        />
                                    </div>

                                    {/* Last Name */}
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">Last Name</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <input 
                                            type="text" 
                                            className="input input-bordered w-full" 
                                            placeholder="Enter last name"
                                            value={lastName}
                                            onChange={(e) => setLastName(e.target.value)}
                                            required 
                                        />
                                    </div>
                                </div>

                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    {/* Birthdate */}
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">Date of Birth</span>
                                            <span className="label-text-alt text-error">*</span>
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
                                            <span className="label-text font-medium">Gender</span>
                                            <span className="label-text-alt text-error">*</span>
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
                                </div>
                            </div>

                            <div className="divider"></div>

                            {/* Contact Information */}
                            <div className="space-y-4">
                                <h2 className="card-title text-2xl">Contact Information</h2>
                                
                                {/* Address */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Address</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <input 
                                        type="text" 
                                        className="input input-bordered w-full" 
                                        placeholder="Street address"
                                        value={address}
                                        onChange={(e) => setAddress(e.target.value)}
                                        required 
                                    />
                                </div>

                                {/* ZIP + City */}
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">ZIP Code</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <input 
                                            type="text" 
                                            className="input input-bordered w-full" 
                                            placeholder="e.g., 1000"
                                            value={zipCode}
                                            onChange={(e) => setZipCode(e.target.value)}
                                            required 
                                        />
                                    </div>

                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">City</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <input 
                                            type="text" 
                                            className="input input-bordered w-full" 
                                            placeholder="City name"
                                            value={city}
                                            onChange={(e) => setCity(e.target.value)}
                                            required 
                                        />
                                    </div>
                                </div>

                                {/* Phone Number */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Mobile Phone</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <input 
                                        type="tel" 
                                        className="input input-bordered w-full" 
                                        placeholder="+45 12 34 56 78"
                                        value={phoneNumber}
                                        onChange={(e) => setPhoneNumber(e.target.value)}
                                        required 
                                    />
                                </div>

                                {/* Email */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Email</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <input 
                                        type="email" 
                                        className="input input-bordered w-full" 
                                        placeholder="player@example.com"
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                        required 
                                    />
                                </div>

                                {/* Confirm Email */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Confirm Email</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <input 
                                        type="email" 
                                        className="input input-bordered w-full" 
                                        placeholder="Confirm email address"
                                        value={confirmEmail}
                                        onChange={(e) => setConfirmEmail(e.target.value)}
                                        required 
                                    />
                                </div>
                            </div>

                            {/* Submit Button */}
                            <div className="flex gap-2 justify-end pt-4">
                                <button className="btn btn-primary" type="submit">
                                    Register Player
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}

