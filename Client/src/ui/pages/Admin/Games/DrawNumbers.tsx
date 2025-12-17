import { useState } from "react";
import { useAtom, useSetAtom } from "jotai";
import { useParams, useNavigate } from "react-router-dom";
import { activeGamesAtom } from "@core/atoms/game.ts";
import { gameApi } from "@core/api/controllers/game.ts";
import { addNotificationAtom } from "@core/atoms/error";

export default function DrawNumbers() {
    const { gameId } = useParams<{ gameId: string }>();
    const navigate = useNavigate();
    const [activeGames] = useAtom(activeGamesAtom);
    const addNotification = useSetAtom(addNotificationAtom);
    const [drawnNumbers, setDrawnNumbers] = useState<number[]>([]);
    const [isDrawing, setIsDrawing] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const selectedGame = activeGames.find(g => g.id === gameId);
    const poolSize = selectedGame?.template?.poolOfNumbers ?? 0;
    const numbersToDrawCount = selectedGame?.template?.maxWinningNumbers!;

    const handleRandomDraw = () => {
        if (!selectedGame) return;
        
        setIsDrawing(true);
        const numbers: number[] = [];
        
        // Animate the drawing
        const interval = setInterval(() => {
            if (numbers.length < numbersToDrawCount) {
                let randomNum;
                do {
                    randomNum = Math.floor(Math.random() * poolSize) + 1;
                } while (numbers.includes(randomNum));
                
                numbers.push(randomNum);
                setDrawnNumbers([...numbers]);
            } else {
                clearInterval(interval);
                setIsDrawing(false);
                setDrawnNumbers(numbers.sort((a, b) => a - b));
            }
        }, 300);
    };

    const handleManualToggle = (num: number) => {
        if (drawnNumbers.includes(num)) {
            setDrawnNumbers(drawnNumbers.filter(n => n !== num));
        } else if (drawnNumbers.length < numbersToDrawCount) {
            setDrawnNumbers([...drawnNumbers, num].sort((a, b) => a - b));
        } else {
            alert(`You can only draw ${numbersToDrawCount} numbers`);
        }
    };

    const handleSubmit = async () => {
        if (!selectedGame) {
            alert("Please select a game");
            return;
        }

        if (drawnNumbers.length !== numbersToDrawCount) {
            alert(`Please draw exactly ${numbersToDrawCount} numbers`);
            return;
        }

        try {
            setIsSubmitting(true);
            
            console.log("Attempting to draw numbers:", { 
                gameId, 
                drawnNumbers,
                count: drawnNumbers.length,
                expectedCount: numbersToDrawCount
            });
            
            await gameApi.drawNumbersForGame(gameId!, drawnNumbers);
            addNotification({ type: 'success', message: 'Numbers drawn successfully!' });
            // Navigate back
            navigate('/admin/games/overview');
            
        } catch (err: any) {
            console.error("Draw numbers error:", err);
            addNotification({ type: 'error', message: err?.message || err?.toString() || "Failed to draw numbers" });
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleClear = () => {
        setDrawnNumbers([]);
    };

    if (!selectedGame) {
        return (
            <div className="container mx-auto px-4 py-6 my-7">
                <div className="space-y-8">
                    <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                        <div className="flex-1">
                            <h1 className="text-3xl font-bold">Draw Winning Numbers</h1>
                        </div>
                    </div>

                    <div className="bg-amber-50 p-8 rounded-xl shadow-md text-center">
                        <p className="text-gray-600 text-lg">
                            Game not found. Please select a valid game.
                        </p>
                        <button 
                            className="btn btn-primary mt-4"
                            onClick={() => navigate('/admin/games/overview')}
                        >
                            Back to Games
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="container mx-auto px-4 py-6 my-7">
            <div className="space-y-8">
                {/* Title */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-3xl font-bold">Draw Winning Numbers</h1>
                        <p className="text-gray-600">
                            {selectedGame.template?.name} - Week {selectedGame.week}
                        </p>
                    </div>
                    <button 
                        className="btn btn-ghost"
                        onClick={() => navigate('/admin/games/overview')}
                        disabled={isSubmitting || isDrawing}
                    >
                        ← Back
                    </button>
                </div>

                {/* Game Info */}
                <div className="bg-amber-50 p-6 rounded-xl shadow-md">
                    <h2 className="text-xl font-semibold mb-4">Game Information</h2>
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <p className="text-sm text-gray-600">Numbers to Draw:</p>
                            <p className="text-lg font-bold">{numbersToDrawCount}</p>
                        </div>
                        <div>
                            <p className="text-sm text-gray-600">Pool Size:</p>
                            <p className="text-lg font-bold">1 - {poolSize}</p>
                        </div>
                    </div>
                </div>

                {/* Drawn Numbers Display */}
                {selectedGame && (
                    <div className="bg-amber-50 p-6 rounded-xl shadow-md">
                        <div className="flex items-center justify-between mb-4">
                            <h2 className="text-xl font-semibold">
                                Drawn Numbers ({drawnNumbers.length}/{numbersToDrawCount})
                            </h2>
                            <div className="flex gap-2">
                                <button 
                                    className="btn btn-primary btn-sm"
                                    onClick={handleRandomDraw}
                                    disabled={isDrawing || isSubmitting || drawnNumbers.length > 0}
                                >
                                    {isDrawing ? "Drawing..." : "🎲 Random Draw"}
                                </button>
                                <button 
                                    className="btn btn-outline btn-sm"
                                    onClick={handleClear}
                                    disabled={isDrawing || isSubmitting || drawnNumbers.length === 0}
                                >
                                    Clear
                                </button>
                            </div>
                        </div>

                        {drawnNumbers.length > 0 ? (
                            <div className="p-4 bg-green-100 rounded-lg mb-4">
                                <div className="flex flex-wrap gap-3 justify-center">
                                    {drawnNumbers.map(num => (
                                        <div 
                                            key={num}
                                            className="w-14 h-14 flex items-center justify-center rounded-full bg-green-600 text-white font-bold text-xl shadow-lg"
                                        >
                                            {num}
                                        </div>
                                    ))}
                                </div>
                            </div>
                        ) : (
                            <div className="p-8 bg-gray-100 rounded-lg text-center text-gray-500">
                                No numbers drawn yet. Click "Random Draw" or select numbers manually below.
                            </div>
                        )}

                        {/* Manual Selection Grid */}
                        <div className="mt-6">
                            <h3 className="text-lg font-semibold mb-3">Manual Selection</h3>
                            <div 
                                className="grid gap-2"
                                style={{
                                    gridTemplateColumns: `repeat(${Math.ceil(Math.sqrt(poolSize))}, 1fr)`
                                }}
                            >
                                {[...Array(poolSize)].map((_, i) => {
                                    const num = i + 1;
                                    const isDrawn = drawnNumbers.includes(num);
                                    const isDisabled = !isDrawn && drawnNumbers.length >= numbersToDrawCount;

                                    return (
                                        <button
                                            key={num}
                                            onClick={() => handleManualToggle(num)}
                                            disabled={isDrawing || isSubmitting || isDisabled}
                                            className={`p-2 rounded-lg border font-semibold transition-all text-sm
                                                ${isDrawn
                                                    ? "bg-green-600 text-white border-green-700 scale-105 shadow-md"
                                                    : "bg-white border-gray-300 hover:bg-gray-100"
                                                }
                                                ${isDisabled ? "opacity-40 cursor-not-allowed" : ""}
                                            `}
                                        >
                                            {num}
                                        </button>
                                    );
                                })}
                            </div>
                        </div>
                    </div>
                )}

                {/* Submit Button */}
                {selectedGame && drawnNumbers.length > 0 && (
                    <div className="bg-amber-50 p-6 rounded-xl shadow-md">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-gray-600">
                                    Ready to finalize the draw for {selectedGame.template?.name}?
                                </p>
                                <p className="text-xs text-gray-500 mt-1">
                                    This will calculate winners and cannot be undone.
                                </p>
                            </div>
                            <button
                                className="btn btn-success btn-lg"
                                onClick={handleSubmit}
                                disabled={isSubmitting || drawnNumbers.length !== numbersToDrawCount}
                            >
                                {isSubmitting ? (
                                    <>
                                        <span className="loading loading-spinner loading-sm"></span>
                                        Processing...
                                    </>
                                ) : (
                                    <>
                                        ✓ Finalize Draw
                                    </>
                                )}
                            </button>
                        </div>
                    </div>
                )}
            </div>

            {/* Loading Overlay */}
            {isSubmitting && (
                <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
                    <div className="bg-white p-8 rounded-xl shadow-2xl text-center">
                        <span className="loading loading-spinner loading-lg"></span>
                        <p className="mt-4 text-lg font-semibold">Calculating winners...</p>
                    </div>
                </div>
            )}
        </div>
    );
}
