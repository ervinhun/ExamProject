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
        return JSON.parse(`{${generated.join(",")}}`);
    }, [chosenGameTemplate?.template]);

    const canSubmit =
        chosenGameTemplate?.template !== undefined &&
        picked.length >= chosenGameTemplate.template.minNumbersPerTicket


    const wouldExceedBalance = (newPickedLength: number, newRepeat: number) => {
        const newBasePrice = priceTableJson[newPickedLength] ?? 0;
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
        if (newLen > (chosenGameTemplate?.template?.maxNumbersPerTicket ?? 1)) return;
        if (wouldExceedBalance(newLen, repeat)) return;

        setPicked([...picked, num]);
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

                        <h2 className="text-xl font-semibold mb-4">Pick Your Numbers</h2>

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
                                        const isDisabled =
                                            (!selected &&
                                                picked.length >= (chosenGameTemplate.template?.maxNumbersPerTicket ?? 1))
                                            ||
                                            (!selected && wouldExceedBalance(picked.length + 1, repeat + 1))
                                        return (
                                            <button
                                                key={num}
                                                onClick={() => toggleNumber(num)}
                                                disabled={isDisabled}
                                                className={`p-3 rounded-lg text-center border font-semibold transition-all
                                            ${selected
                                                    ? "bg-green-600 text-white border-green-700"
                                                    : "bg-white border-gray-300 hover:bg-gray-100"}
                                            ${!selected && picked.length >= (chosenGameTemplate.template?.maxNumbersPerTicket ?? 0)
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
                        {/* Submit Button */}
                        <button
                            disabled={!canSubmit || overBalance}
                            className={`btn btn-primary ml-6 ${
                                !canSubmit || overBalance ? "btn-disabled opacity-50 cursor-not-allowed" : ""
                            }`}
                            onClick={() => {
                                console.log("Game template: ", chosenGameTemplate)
                                void onSubmit({
                                    gameInstanceId: chosenGameTemplate?.id ?? "",
                                    gameTemplateId: chosenGameTemplate?.template?.id ?? "",
                                    selectedNumbers: picked,
                                    repeat
                                })
                            }}
                        >
                            Submit Ticket
                        </button>
                    </div>
                </div>
            </div>
        </>
    );
}
