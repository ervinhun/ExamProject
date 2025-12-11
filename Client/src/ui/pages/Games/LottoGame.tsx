
import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAtom } from "jotai";
import { activeGamesAtom, fetchActiveGamesAtom } from "@core/atoms/game";
import type { GameInstanceDto } from "@core/types/game";

export default function LottoGame() {
    const { gameId } = useParams<{ gameId: string }>();
    const navigate = useNavigate();
    const [activeGames] = useAtom(activeGamesAtom);
    const [, fetchActiveGames] = useAtom(fetchActiveGamesAtom);
    const [game, setGame] = useState<GameInstanceDto | null>(null);
    const [selectedNumbers, setSelectedNumbers] = useState<number[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const loadGame = async () => {
            if (activeGames.length === 0) {
                await fetchActiveGames();
            }
            const foundGame = activeGames.find(g => g.id === gameId);
            if (foundGame) {
                setGame(foundGame);
            }
            setLoading(false);
        };
        loadGame();
    }, [gameId, activeGames, fetchActiveGames]);

    const toggleNumber = (number: number) => {
        if (selectedNumbers.includes(number)) {
            setSelectedNumbers(selectedNumbers.filter(n => n !== number));
        } else {
            const maxNumbers = game?.template?.maxNumbersPerTicket || 0;
            if (selectedNumbers.length < maxNumbers) {
                setSelectedNumbers([...selectedNumbers, number].sort((a, b) => a - b));
            }
        }
    };

    const handleQuickPick = () => {
        const poolSize = game?.template?.poolOfNumbers || 0;
        const numbersToPick = game?.template?.maxNumbersPerTicket || 0;
        const numbers: number[] = [];
        
        while (numbers.length < numbersToPick) {
            const randomNum = Math.floor(Math.random() * poolSize) + 1;
            if (!numbers.includes(randomNum)) {
                numbers.push(randomNum);
            }
        }
        
        setSelectedNumbers(numbers.sort((a, b) => a - b));
    };

    const handleClear = () => {
        setSelectedNumbers([]);
    };

    const handleSubmit = () => {
        // TODO: Implement ticket purchase logic
        console.log("Selected numbers:", selectedNumbers);
        alert("Ticket purchase functionality to be implemented");
    };

    if (loading) {
        return (
            <div className="container mx-auto flex items-center justify-center min-h-[60vh]">
                <span className="loading loading-spinner loading-lg text-primary"></span>
            </div>
        );
    }

    if (!game) {
        return (
            <div className="container mx-auto">
                <div className="alert alert-error">
                    <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <span>Game not found</span>
                </div>
                <button onClick={() => navigate('/games')} className="btn btn-primary mt-4">
                    Back to Games
                </button>
            </div>
        );
    }

    const poolOfNumbers = game.template?.poolOfNumbers || 0;
    const minNumbers = game.template?.minNumbersPerTicket || 0;
    const maxNumbers = game.template?.maxNumbersPerTicket || 0;
    const basePrice = game.template?.basePrice || 0;
    const canSubmit = selectedNumbers.length >= minNumbers && selectedNumbers.length <= maxNumbers;

    return (
        <div className="container mx-auto space-y-6 pb-8">
            {/* Header */}
            <div className="pb-4 border-b-2 border-primary">
                <button 
                    onClick={() => navigate('/games')} 
                    className="btn btn-ghost btn-sm mb-3"
                >
                    ← Back to Games
                </button>
                <div className="flex flex-col lg:flex-row items-start lg:items-center justify-between gap-4">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary">{game.template?.name}</h1>
                        <p className="text-base text-base-content/70 mt-1">{game.template?.description}</p>
                    </div>
                    <div className="flex flex-wrap gap-3 items-center">
                        <div className="stats shadow bg-base-200">
                            <div className="stat py-3 px-4">
                                <div className="stat-title text-xs">Numbers to Pick</div>
                                <div className="stat-value text-primary text-xl">{minNumbers}-{maxNumbers}</div>
                            </div>
                        </div>
                        <div className="stats shadow bg-base-200">
                            <div className="stat py-3 px-4">
                                <div className="stat-title text-xs">Selected</div>
                                <div className="stat-value text-secondary text-xl">{selectedNumbers.length}/{maxNumbers}</div>
                            </div>
                        </div>
                        <div className="stats shadow bg-base-200">
                            <div className="stat py-3 px-4">
                                <div className="stat-title text-xs">Number Pool</div>
                                <div className="stat-value text-xl">1-{poolOfNumbers}</div>
                            </div>
                        </div>
                        <div className="stats shadow bg-base-200">
                            <div className="stat py-3 px-4">
                                <div className="stat-title text-xs">Ticket Price</div>
                                <div className="stat-value text-primary text-xl">{basePrice} DKK</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Selected Numbers Display */}
            <div className="card bg-base-200 shadow-lg">
                <div className="card-body">
                    <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 mb-3">
                        <h3 className="card-title text-xl">Your Selected Numbers</h3>
                        <div className="flex gap-2">
                            <button 
                                className="btn btn-outline btn-sm" 
                                onClick={handleQuickPick}
                            >
                                🎲 Quick Pick
                            </button>
                            <button 
                                className="btn btn-outline btn-sm" 
                                onClick={handleClear}
                                disabled={selectedNumbers.length === 0}
                            >
                                Clear
                            </button>
                        </div>
                    </div>
                    <div className="flex flex-wrap gap-3 min-h-[70px] items-center p-4 bg-base-300 rounded-lg">
                        {selectedNumbers.length === 0 ? (
                            <p className="text-base-content/50 italic">Select numbers below or use Quick Pick</p>
                        ) : (
                            selectedNumbers.map(num => (
                                <div 
                                    key={num} 
                                    className="badge badge-lg badge-primary font-bold text-lg px-4 py-3 cursor-pointer hover:badge-error transition-colors"
                                    onClick={() => toggleNumber(num)}
                                    title="Click to remove"
                                >
                                    {num}
                                </div>
                            ))
                        )}
                    </div>
                </div>
            </div>

            {/* Number Grid */}
            <div className="card bg-base-200 shadow-lg">
                <div className="card-body">
                    <h3 className="card-title text-xl mb-6">Pick Your Numbers</h3>
                    <div className="grid grid-cols-5 sm:grid-cols-8 md:grid-cols-10 lg:grid-cols-12 gap-3">
                        {Array.from({ length: poolOfNumbers }, (_, i) => i + 1).map(number => {
                            const isSelected = selectedNumbers.includes(number);
                            const isDisabled = !isSelected && selectedNumbers.length >= maxNumbers;
                            
                            return (
                                <button
                                    key={number}
                                    onClick={() => toggleNumber(number)}
                                    disabled={isDisabled}
                                    className={`
                                        btn btn-square h-12 min-h-12 w-12 text-base font-semibold
                                        ${isSelected 
                                            ? 'btn-primary text-white shadow-lg scale-105' 
                                            : isDisabled 
                                                ? 'btn-disabled opacity-30' 
                                                : 'btn-outline hover:btn-primary hover:scale-105'
                                        }
                                        transition-all duration-200
                                    `}
                                >
                                    {number}
                                </button>
                            );
                        })}
                    </div>
                </div>
            </div>

            {/* Submit Section */}
            <div className="card bg-base-200 shadow-xl sticky bottom-4 border-2 border-primary/20">
                <div className="card-body">
                    <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
                        <div className="flex-1">
                            {!canSubmit ? (
                                <p className="text-warning text-sm font-medium">
                                    ⚠️ Please select {minNumbers}-{maxNumbers} numbers to continue
                                </p>
                            ) : (
                                <p className="text-success text-sm font-medium">
                                    ✓ Ready to purchase your ticket!
                                </p>
                            )}
                        </div>
                        <button 
                            className="btn btn-primary btn-lg w-full sm:w-auto px-8 shadow-lg"
                            onClick={handleSubmit}
                            disabled={!canSubmit}
                        >
                            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                            </svg>
                            Purchase Ticket ({basePrice} DKK)
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
