import { useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import ErrorPopUp from "../../Errors/ErrorPopUp";
import type { GameTemplateDto } from "@core/types/game";
import { useAtom, useSetAtom } from "jotai";
import { createGameTemplateAtom } from "@core/atoms/game";
import { addNotificationAtom } from "@core/atoms/error";
import { set } from "zod";

export const CreateGameTemplate: React.FC = () => {
    const navigate = useNavigate();
    const [ , createGameTemplate ] = useAtom(createGameTemplateAtom);
    const addNotification = useSetAtom(addNotificationAtom);

    // Form state
    const [name, setName] = useState("");
    const [description, setDescription] = useState("");
    const [gameType, setGameType] = useState<"Lotto" | "Bingo">("Lotto");
    const [poolOfNumbers, setPoolOfNumbers] = useState<number>(50);
    const [maxWinningNumbers, setMaxWinningNumbers] = useState<number>(5);
    const [minNumbersPerTicket, setMinNumbersPerTicket] = useState<number>(5);
    const [maxNumbersPerTicket, setMaxNumbersPerTicket] = useState<number>(10);
    const [basePrice, setBasePrice] = useState<number>(0);

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        
        
        const templateData: GameTemplateDto = {
            name,
            description,
            gameType,
            poolOfNumbers: poolOfNumbers,
            maxWinningNumbers: maxWinningNumbers,
            minNumbersPerTicket,
            maxNumbersPerTicket,
            basePrice,
        };

        await createGameTemplate(templateData).then(() => {
            // Optionally, redirect or reset form here
            setName("");
            setDescription("");
            setGameType("Lotto");
            setPoolOfNumbers(50);
            setMaxWinningNumbers(5);
            setMinNumbersPerTicket(5);
            setMaxNumbersPerTicket(10);
            setBasePrice(0);

            addNotification({
                message: `Game template "${templateData.name}" created successfully`,
                type: 'success'
            });
            navigate('/admin/games/start');
            return;
        }).catch((error) => {
            let errorMessage = "Unknown error";

            if (error?.message) {
                errorMessage = error.message;
            }

            addNotification({
                message: `Failed to create game template: ${errorMessage}`,
                type: 'error'
            });
            return;
        });
    }

    return (
        <div className="container mx-auto">
            <ErrorPopUp />
            
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary">Create Game Template</h1>
                        <p className="text-base text-base-content/70 mt-1">Define a new game template for lottery instances</p>
                    </div>
                    <NavLink to="/admin/games/overview" className="btn btn-ghost">
                        ← Back to Overview
                    </NavLink>
                </div>

                {/* Form Card */}
                <div className="card bg-base-200 shadow-lg max-w-4xl mx-auto">
                    <div className="card-body">
                        <form onSubmit={handleSubmit} className="space-y-6">
                            {/* Basic Information */}
                            <div className="space-y-4">
                                <h2 className="card-title text-2xl">Basic Information</h2>
                                
                                {/* Template Name */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Template Name</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <input 
                                        type="text" 
                                        className="input input-bordered w-full" 
                                        placeholder="e.g., Weekly Lottery"
                                        value={name}
                                        onChange={(e) => setName(e.target.value)}
                                        required 
                                    />
                                </div>

                                {/* Description */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Description</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <textarea 
                                        className="textarea textarea-bordered h-24 w-full" 
                                        placeholder="Describe this game template..."
                                        value={description}
                                        onChange={(e) => setDescription(e.target.value)}
                                        required 
                                    />
                                </div>

                                {/* Game Type */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Game Type</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <select 
                                        className="select select-bordered w-full"
                                        value={gameType}
                                        onChange={(e) => setGameType(e.target.value as "Lotto" | "Bingo")}
                                        required
                                    >
                                        <option value="Lotto">Lotto</option>
                                        <option value="Bingo">Bingo</option>
                                    </select>
                                </div>
                            </div>

                            <div className="divider"></div>

                            {/* Number Configuration */}
                            <div className="space-y-4">
                                <h2 className="card-title text-2xl">Number Configuration</h2>
                                
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    {/* Number Range */}
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">Pool of Numbers</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <input 
                                            type="number" 
                                            className="input input-bordered w-full" 
                                            min="1"
                                            value={poolOfNumbers}
                                            onChange={(e) => setPoolOfNumbers(Number(e.target.value))}
                                            required 
                                        />
                                    </div>

                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">Max Winning Numbers</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <input 
                                            type="number" 
                                            className="input input-bordered w-full" 
                                            min="1"
                                            value={maxWinningNumbers}
                                            onChange={(e) => setMaxWinningNumbers(Number(e.target.value))}
                                            required 
                                        />
                                    </div>


                                    {/* Numbers Per Ticket */}
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">Min Numbers Per Ticket</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <input 
                                            type="number" 
                                            className="input input-bordered w-full" 
                                            min="1"
                                            value={minNumbersPerTicket}
                                            onChange={(e) => setMinNumbersPerTicket(Number(e.target.value))}
                                            required 
                                        />
                                    </div>

                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">Max Numbers Per Ticket</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <input 
                                            type="number" 
                                            className="input input-bordered w-full" 
                                            min="1"
                                            value={maxNumbersPerTicket}
                                            onChange={(e) => setMaxNumbersPerTicket(Number(e.target.value))}
                                            required 
                                        />
                                    </div>
                                </div>
                            </div>

                            <div className="divider"></div>

                            {/* Pricing */}
                            <div className="space-y-4">
                                <h2 className="card-title text-2xl">Pricing</h2>
                                
                                <div className="form-control max-w-xs">
                                    <label className="label">
                                        <span className="label-text font-medium">Base Price</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <label className="input-group">
                                        <input 
                                            type="number" 
                                            className="input input-bordered w-full" 
                                            min="0"
                                            step="0.01"
                                            value={basePrice}
                                            onChange={(e) => setBasePrice(Number(e.target.value))}
                                            required 
                                        />
                                        <span>DKK</span>
                                    </label>
                                </div>
                            </div>

                            {/* Submit Button */}
                            <div className="flex gap-2 justify-end pt-4">
                                <button className="btn btn-primary" type="submit">
                                    Create Template
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}