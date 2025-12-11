import {useEffect, useMemo, useState} from "react";
import {useAtom, useSetAtom} from "jotai";
import {GameInstanceDto} from "@core/types/game.ts";
import {activeGamesAtom} from "@core/atoms/game.ts";
import {walletAtom} from "@core/atoms/wallet.ts";
import {addNotificationAtom} from "@core/atoms/error.ts";
import {playerApi} from "@core/api/controllers/player.ts";
import {gameApi} from "@core/api/controllers/game.ts";
import {CreateTicketToGameDto, MyTicketDto} from "@core/types/ticket.ts";
import {ticketApi} from "@core/api/controllers/ticket.ts";
import {myTicketsAtom} from "@core/atoms/tickets.ts";
import {useParams} from "react-router-dom";

export default function Play() {
    const [chosenGameTemplate, setChosenGameTemplate] = useState<GameInstanceDto | null>(null);
    const [picked, setPicked] = useState<number[]>([]);
    const [repeat, setRepeat] = useState(0);
    const [isSubscription, setIsSubscription] = useState(false);

    const [gameInstance, setGameInstance] = useAtom(activeGamesAtom);
    const [wallet, setWallet] = useAtom(walletAtom);
    const [, setMyTickets] = useAtom(myTicketsAtom)
    const {gameId} = useParams();

    const addNotification = useSetAtom(addNotificationAtom);


    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        if (wallet?.balance === null)
            playerApi.getWalletForPlayerId(wallet?.id ?? "").then(setWallet);
    }, [wallet?.balance]);

    useEffect(() => {
        if (gameInstance.length === 0) {
            gameApi.getAllActiveGames().then((data) => {
                setGameInstance(data);
            });
        }
    }, []);
    useEffect(() => {
        if (chosenGameTemplate === null)
            setChosenGameTemplate(gameInstance.find(d => d.id === gameId) ?? null)
    }, [])

    const priceTableJson = useMemo(() => {
        const rule = chosenGameTemplate?.template?.priceGrowthRule;
        if (rule) return JSON.parse(rule);

        const min = chosenGameTemplate?.template?.minNumbersPerTicket ?? 1;
        const max = chosenGameTemplate?.template?.maxNumbersPerTicket ?? 1;
        const base = chosenGameTemplate?.template?.basePrice ?? 1;

        const generated: string[] = [];
        let m = 0;

        for (let i = min; i <= max; i++) {
            generated.push(`"${i}": ${2 ** m * base}`);
            m++;
        }
        const result = JSON.parse(`{${generated.join(",")}}`);
        console.log("Price table:", result);
        console.log("Min numbers:", min, "Max numbers:", max);
        return result;
    }, [chosenGameTemplate?.template]);

    const canSubmit =
        chosenGameTemplate?.template !== undefined &&
        picked.length >= chosenGameTemplate.template.minNumbersPerTicket


    const wouldExceedBalance = (newPickedLength: number, newRepeat: number) => {
        // Don't check balance if we don't have enough numbers selected
        const minNumbers = chosenGameTemplate?.template?.minNumbersPerTicket ?? 0;
        if (newPickedLength < minNumbers) {
            return false; // Don't block selection when building up to minimum
        }
        
        const newBasePrice = priceTableJson[newPickedLength] ?? 0;
        if (newBasePrice === 0) return false; // No price defined, don't block
        
        const newTotal = newBasePrice * (newRepeat + 1);
        return wallet?.balance !== undefined && newTotal > wallet.balance;
    };

    const overBalance = wouldExceedBalance(picked.length, repeat);

    const basePrice = priceTableJson[picked.length] ?? 0;
    const totalPrice = basePrice * (repeat + 1);

    const toggleNumber = (num: number) => {
        const selected = picked.includes(num);
        if (selected) {
            setPicked(picked.filter(n => n !== num));
            return;
        }

        const newLen = picked.length + 1;
        const maxNumbers = chosenGameTemplate?.template?.maxNumbersPerTicket ?? 0;
        
        console.log("Trying to add number:", num);
        console.log("Current picked length:", picked.length);
        console.log("New length would be:", newLen);
        console.log("Max numbers allowed:", maxNumbers);
        
        if (newLen > maxNumbers) {
            console.log("BLOCKED: Exceeds max numbers");
            addNotification({type: "warning", message: `You can only pick up to ${maxNumbers} numbers`});
            return;
        }
        
        const balanceCheck = wouldExceedBalance(newLen, repeat);
        console.log("Balance check result:", balanceCheck);
        
        if (balanceCheck) {
            console.log("BLOCKED: Would exceed balance");
            addNotification({type: "warning", message: "Insufficient balance for this selection"});
            return;
        }

        console.log("SUCCESS: Adding number");
        setPicked([...picked, num].sort((a, b) => a - b));
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

    const handleClearNumbers = () => {
        setPicked([]);
    };

    const onSubmit = async (values: CreateTicketToGameDto) => {
        try {
            setIsSubmitting(true);
            const payload = {
                gameInstanceId: values.gameInstanceId,
                gameTemplateId: values.gameTemplateId,
                selectedNumbers: values.selectedNumbers,
                repeat: values.repeat
            };

            const response = await ticketApi.playTicket(payload);

            if (response?.id == null) {
                addNotification({type: "error", message: "Failed to create ticket. Please try again later."});
                return;
            }
            addNewTicketToMyTickets(response);
            addNotification({type: "success", message: "Ticket has been created"});
            if (wallet != null && wallet?.balance !== null) {
                const newBalance = wallet.balance - (response.ticketPrice ?? totalPrice);
                setWallet({...wallet, balance: newBalance});
            }
            reset();
        } catch (ex) {
            addNotification({type: "error", message: `Unexpected error. ${ex}`});
        } finally {
            setIsSubmitting(false);
        }
    };

    const addNewTicketToMyTickets = (ticket: MyTicketDto) => {
        if (ticket.gameInstanceId === "") return;
        setMyTickets(prevTickets => [...prevTickets, ticket]);
    }

    const reset = () => {
        setChosenGameTemplate(null);
        setPicked([]);
        setRepeat(0);
        setIsSubscription(false);
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
                            {chosenGameTemplate !== null && (
                                <>
                                    <h1 className="text-3xl font-bold">Playing: {chosenGameTemplate?.template?.name}</h1>
                                    <p className="text-gray-600">Select {chosenGameTemplate.template?.minNumbersPerTicket}–
                                        {chosenGameTemplate.template?.maxNumbersPerTicket} numbers.</p>
                                </>)}
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
                                    disabled={chosenGameTemplate === null}
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

                        {chosenGameTemplate !== null && (
                            <div>
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
                                        const maxNumbers = chosenGameTemplate.template?.maxNumbersPerTicket ?? 0;
                                        const isDisabled =
                                            (!selected && picked.length >= maxNumbers)
                                            ||
                                            (!selected && wouldExceedBalance(picked.length + 1, repeat))
                                        return (
                                            <button
                                                key={num}
                                                onClick={() => toggleNumber(num)}
                                                disabled={isDisabled}
                                                className={`p-3 rounded-lg text-center border font-semibold transition-all
                                            ${selected
                                                    ? "bg-green-600 text-white border-green-700 scale-105 shadow-lg"
                                                    : "bg-white border-gray-300 hover:bg-gray-100"}
                                            ${isDisabled
                                                    ? "opacity-40 cursor-not-allowed"
                                                    : ""}
                                        `}
                                            >
                                                {num}
                                            </button>
                                        );
                                    })}
                                </div>
                            </div>
                        )}
                    </div>

                    {/* Price + Repeat */}
                    <div className="bg-amber-50 p-5 rounded-xl shadow-md">
                        <div className="flex justify-center items-center gap-6 mx-auto w-full flex-wrap">
                            <div>
                                <p className="text-gray-700 font-semibold">Base Price:</p>
                                <p className="text-lg font-bold">{basePrice} kr.</p>
                            </div>

                            <div>
                                <p className="text-gray-700 font-semibold">Repeat:</p>
                                <input
                                    type="number"
                                    className="input input-bordered w-24"
                                    min={0}
                                    value={repeat}
                                    onChange={e => {
                                        const newRepeat = Number(e.target.value);
                                        if (!wouldExceedBalance(picked.length, newRepeat)) {
                                            setRepeat(newRepeat);
                                        }
                                    }}
                                    max={52}
                                />
                            </div>

                            <div>
                                <p className="text-gray-700 font-semibold">Total:</p>
                                <p className="text-xl font-bold">{totalPrice} kr.</p>
                            </div>
                        </div>

                        {/* Subscription Checkbox - Only for auto-renewable games */}
                        {chosenGameTemplate?.isAutoRepeatable && (
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
                                onClick={() => {
                                    console.log("Game template: ", chosenGameTemplate)
                                    console.log("Is subscription: ", isSubscription)
                                    void onSubmit({
                                        gameInstanceId: chosenGameTemplate?.id ?? "",
                                        gameTemplateId: chosenGameTemplate?.template?.id ?? "",
                                        selectedNumbers: picked,
                                        repeat
                                    })
                                }}
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                                </svg>
                                {isSubscription ? "Start Subscription" : "Submit Ticket"}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}
