import { useState } from "react";
import ErrorPopUp from "../../Errors/ErrorPopUp";
import type { GameTemplateDto } from "@core/types/game";
import { useAtom } from "jotai";
import { createGameTemplateAtom } from "@core/atoms/game";

export const CreateGameTemplate: React.FC = () => {
    const [ , createGameTemplate ] = useAtom(createGameTemplateAtom);

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

        await createGameTemplate(templateData);

        
        
    }

    return (
        <div className="flex justify-center mt-10 w-full px-4">
            <ErrorPopUp />

            <form 
                className="w-full max-w-3xl p-8 bg-base-200 rounded-xl shadow-lg space-y-6" 
                onSubmit={handleSubmit}
            >
                <h2 className="text-3xl font-bold text-center mb-6">
                    Create Game Template
                </h2>

                {/* Basic Information */}
                <div className="space-y-4">
                    <h3 className="text-xl font-semibold text-primary">Basic Information</h3>
                    
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

                {/* Number Configuration */}
                <div className="space-y-4">
                    <h3 className="text-xl font-semibold text-primary">Number Configuration</h3>
                    
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

                {/* Pricing */}
                <div className="space-y-4">
                    <h3 className="text-xl font-semibold text-primary">Pricing</h3>
                    
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
                <div className="pt-4">
                    <button className="btn btn-primary w-full text-lg" type="submit">
                        Create Template
                    </button>
                </div>
            </form>
        </div>
    );
}