import { useMemo, useState } from "react";
import { useAtom, useSetAtom } from "jotai";
import { activeGamesAtom } from "@core/atoms/game.ts";
import { walletAtom } from "@core/atoms/wallet.ts";
import { addNotificationAtom } from "@core/atoms/error.ts";
import { authAtom } from "@core/atoms/auth.ts";
import { PurchaseTicketDto } from "@core/types/ticket.ts";
import { ticketApi } from "@core/api/controllers/ticket.ts";
import { useParams, useNavigate } from "react-router-dom";
import { calculateExponentialPrice } from "@utils/priceUtils.ts";

export default function Play() {
    const { gameId } = useParams();
    const navigate = useNavigate();
    const addNotification = useSetAtom(addNotificationAtom);
    
    const [gameInstance] = useAtom(activeGamesAtom);
    const [wallet, setWallet] = useAtom(walletAtom);
    const [authUser] = useAtom(authAtom);
    
    const [picked, setPicked] = useState<number[]>([]);
    const [isSubscription, setIsSubscription] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const chosenGameTemplate = useMemo(
        () => gameInstance.find(g => g.id === gameId) ?? null,
        [gameInstance, gameId]
    );

    const minNumbers = chosenGameTemplate?.template?.minNumbersPerTicket ?? 0;
    const maxNumbers = chosenGameTemplate?.template?.maxNumbersPerTicket ?? 0;
    const baseTicketPrice = chosenGameTemplate?.template?.basePrice ?? 0;
    
    const canSubmit = picked.length >= minNumbers;
    
    // Exponential pricing - doubles for each additional number
    const totalPrice = calculateExponentialPrice(picked.length, minNumbers, baseTicketPrice);
    const overBalance = wallet?.balance !== undefined && totalPrice > wallet.balance;

    const toggleNumber = (num: number) => {
        if (picked.includes(num)) {
            setPicked(prev => prev.filter(n => n !== num));
            return;
        }

        if (picked.length >= maxNumbers) {
            addNotification({ type: "warning", message: `You can only pick up to ${maxNumbers} numbers` });
            return;
        }

        setPicked(prev => [...prev, num].sort((a, b) => a - b));
    };

    const handleQuickPick = () => {
        const poolSize = chosenGameTemplate?.template?.poolOfNumbers || 0;
        const numbersToPick = chosenGameTemplate?.template?.maxNumbersPerTicket || 0;
        const numbers: number[] = [];
        
        while (numbers.length < numbersToPick) {
            const randomNum = Math.floor(Math.random() * poolSize) + 1;
            if (!numbers.includes(randomNum)) {
                numbers.push(randomNum);
            }
        }
        
        setPicked(numbers.sort((a, b) => a - b));
    };

    const handleClearNumbers = () => setPicked([]);

    const onSubmit = async () => {
        if (!authUser?.id || !wallet?.id) {
            addNotification({ type: "error", message: "Missing required information" });
            return;
        }

        if (picked.length < minNumbers || picked.length > maxNumbers) {
            addNotification({ type: "warning", message: `Please select between ${minNumbers} and ${maxNumbers} numbers` });
            return;
        }

        try {
            setIsSubmitting(true);
            
            // Purchase the ticket for current game
            if (!gameId) {
                throw new Error("Game ID is required");
            }
            
            const payload: PurchaseTicketDto = {
                gameInstanceId: gameId,
                playerId: authUser.id,
                walletId: wallet.id,
                fullPrice: totalPrice,
                pickedNumbers: picked.map(n => Number(n))
            };

            console.log("Submitting ticket payload:", payload);
            await ticketApi.purchaseTicket(payload);

            // Update wallet balance
            if (wallet?.balance !== undefined) {
                setWallet({ ...wallet, balance: wallet.balance - totalPrice });
            }
            
            // If subscription is checked, also create a subscription
            if (isSubscription) {
                if (!chosenGameTemplate?.template?.id) {
                    throw new Error("Game template ID is required for subscription");
                }
                
                const subscriptionPayload = {
                    gameTemplateId: chosenGameTemplate.template.id,
                    playerId: authUser.id,
                    walletId: wallet.id,
                    price: totalPrice,
                    pickedNumbers: picked.map(n => Number(n))
                };
                
                console.log("Creating subscription:", subscriptionPayload);
                await ticketApi.startSubscriptionForTicket(subscriptionPayload);
            }
            
            const successMessage = isSubscription 
                ? "Ticket purchased and subscription created! Your numbers will play in every draw."
                : "Ticket purchased successfully!";
            addNotification({ type: "success", message: successMessage });
            
            // Reset form and navigate
            setPicked([]);
            setIsSubscription(false);
            
            setTimeout(() => navigate('/tickets'), 1000);
        } catch (ex) {
            console.error("Purchase error:", ex);
            const errorMsg = ex instanceof Error ? ex.message : String(ex);
            addNotification({ type: "error", message: errorMsg || "Failed to purchase ticket" });
        } finally {
            setIsSubmitting(false);
        }
    };

    if (!chosenGameTemplate) {
        return (
            <div className="container mx-auto px-4 py-6">
                <div className="alert alert-warning">
                    <span>Game not found</span>
                </div>
            </div>
        );
    }

    return (
        <>
            {isSubmitting && (
                <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
                    <span className="loading loading-dots loading-lg text-white"></span>
                </div>
            )}

            <div className="container mx-auto px-4 py-6 my-7">
                <div className="space-y-8">
                    {/* Title */}
                    <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                        <div className="flex-1">
                            <h1 className="text-3xl font-bold">Playing: {chosenGameTemplate.template?.name}</h1>
                            <p className="text-gray-600">
                                Select {chosenGameTemplate.template?.minNumbersPerTicket}–
                                {chosenGameTemplate.template?.maxNumbersPerTicket} numbers.
                            </p>
                        </div>
                    </div>

                    {/* Number Board */}
                    <div className="bg-amber-50 p-5 rounded-xl shadow-md">
                        <div className="flex items-center justify-between mb-4">
                            <h2 className="text-xl font-semibold">Pick Your Numbers</h2>
                            <div className="flex gap-2">
                                <button 
                                    className="btn btn-outline btn-sm"
                                    onClick={handleQuickPick}
                                >
                                    🎲 Quick Pick
                                </button>
                                <button 
                                    className="btn btn-outline btn-sm"
                                    onClick={handleClearNumbers}
                                    disabled={picked.length === 0}
                                >
                                    Clear
                                </button>
                            </div>
                        </div>

                        {/* Selected Numbers Display */}
                        {picked.length > 0 && (
                            <div className="mb-4 p-3 bg-green-100 rounded-lg">
                                <p className="text-sm font-semibold mb-2 text-gray-700">Selected Numbers:</p>
                                <div className="flex flex-wrap gap-2">
                                    {picked.map(num => (
                                        <div 
                                            key={num} 
                                            className="badge badge-lg badge-success font-bold cursor-pointer hover:badge-error transition-colors"
                                            onClick={() => toggleNumber(num)}
                                            title="Click to remove"
                                        >
                                            {num}
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}

                        {/* Number Grid */}
                        <div
                            className="grid gap-3 max-w-sm content-center mx-auto"
                            style={{
                                gridTemplateColumns: `repeat(${Math.ceil(
                                    Math.sqrt(chosenGameTemplate.template?.poolOfNumbers ?? 0)
                                )}, 1fr)`
                            }}
                        >
                            {[...Array(chosenGameTemplate.template?.poolOfNumbers)].map((_, i) => {
                                const num = i + 1;
                                const selected = picked.includes(num);
                                const isDisabled = !selected && picked.length >= maxNumbers;
                                
                                return (
                                    <button
                                        key={num}
                                        onClick={() => toggleNumber(num)}
                                        disabled={isDisabled}
                                        className={`p-3 rounded-lg text-center border font-semibold transition-all
                                            ${selected
                                                ? "bg-green-600 text-white border-green-700 scale-105 shadow-lg"
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

                    {/* Price + Repeat */}
                    <div className="bg-amber-50 p-5 rounded-xl shadow-md">
                        <div className="flex justify-center items-center gap-6 mx-auto w-full flex-wrap mb-4">
                            <div>
                                <p className="text-gray-700 font-semibold">Ticket Price:</p>
                                <p className="text-xl font-bold text-primary">{totalPrice} DKK</p>
                            </div>
                            {picked.length > 0 && (
                                <div>
                                    <p className="text-gray-700 font-semibold">Selected:</p>
                                    <p className="text-xl font-bold">{picked.length} numbers</p>
                                </div>
                            )}
                        </div>

                        {/* Subscription Checkbox - Only for auto-renewable games */}
                        {chosenGameTemplate.isAutoRepeatable && (
                            <div className="mt-4 pt-4 border-t border-gray-300">
                                <div className="form-control">
                                    <label className="label cursor-pointer justify-center gap-3">
                                        <input 
                                            type="checkbox" 
                                            className="checkbox checkbox-primary" 
                                            checked={isSubscription}
                                            onChange={(e) => setIsSubscription(e.target.checked)}
                                        />
                                        <span className="label-text font-semibold">
                                            🔄 Make this a repeating subscription (auto-renew every week)
                                        </span>
                                    </label>
                                    {isSubscription && (
                                        <p className="text-sm text-gray-600 text-center mt-2">
                                            These numbers will automatically play in future draws until you cancel
                                        </p>
                                    )}
                                </div>
                            </div>
                        )}

                        {/* Submit Button */}
                        <div className="flex justify-center mt-4">
                            <button
                                disabled={!canSubmit || overBalance}
                                className={`btn btn-primary btn-lg ${
                                    !canSubmit || overBalance ? "btn-disabled opacity-50 cursor-not-allowed" : ""
                                }`}
                                onClick={onSubmit}
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                                </svg>
                                {isSubscription ? "Start Subscription" : "Purchase Ticket"}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}
