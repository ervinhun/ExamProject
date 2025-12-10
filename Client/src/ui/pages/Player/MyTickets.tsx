import {useEffect, useMemo, useState} from "react";
import {useAtom, useSetAtom} from "jotai";
import {activeGamesAtom} from "@core/atoms/game.ts";
import {gameApi} from "@core/api/controllers/game.ts";
import {GameInstanceDto} from "@core/types/game.ts";
import {walletAtom} from "@core/atoms/players.ts";
import {playerApi} from "@core/api/controllers/player.ts";
import {addNotificationAtom} from "@core/atoms/error.ts";
import {CreateTicketToGameDto, MyTicketDto} from "@core/types/ticket.ts";
import {ticketApi} from "@core/api/controllers/ticket.ts";
import {myTicketsAtom} from "@core/atoms/tickets.ts";

export default function MyTickets() {
    const [chosenGameTemplate, setChosenGameTemplate] = useState<GameInstanceDto | null>(null)

    const [selectedGame, setSelectedGame] = useState("lottery16")
    const [picked, setPicked] = useState<number[]>([])
    const [repeat, setRepeat] = useState(1)
    const [gameTemplate, setGameTemplate] = useAtom(activeGamesAtom)
    const [wallet, setWallet] = useAtom(walletAtom)
    const [myTickets, setMyTickets] = useAtom(myTicketsAtom)
    const [isSubmitting, setIsSubmitting] = useState(false)
    const addNotification = useSetAtom(addNotificationAtom);


    useEffect(() => {
        if (wallet?.balance === null)
            playerApi.getWalletForPlayerId(wallet?.id ?? "").then(setWallet);
    }, []);


    useEffect(() => {
        if (gameTemplate.length === 0) {
            gameApi.getAllActiveGames().then((data) => {
                setGameTemplate(data);
                if (chosenGameTemplate === null) {
                    setChosenGameTemplate(data[0]);
                }
            });
        }
    }, []);

    useEffect(() => {
        if (myTickets.length === 0) {
            ticketApi.getAllActiveTickets().then(setMyTickets);
        }
    }, [myTickets]);


    // This gets the JSON of the prices per number from the entity.
    // If it does not exist, it creates one that each number can be picked is twice as much as the previous one
    const priceTableJson = useMemo(() => {
        const rule = chosenGameTemplate?.template?.priceGrowthRule;

        if (rule) {
            return JSON.parse(rule);
        }

        // auto-generate fallback
        const generatedPriceJson: string[] = [];
        let j = 0;

        const min = chosenGameTemplate?.template?.minNumbersPerTicket ?? 1;
        const max = chosenGameTemplate?.template?.maxNumbersPerTicket ?? 1;
        const base = chosenGameTemplate?.template?.basePrice ?? 1;

        for (let i = min; i <= max; i++) {
            generatedPriceJson.push(`"${i}": ${2 ** j * base}`);
            j++;
        }
        console.log("Price table json: ", generatedPriceJson.join(","))
        return JSON.parse(`{${generatedPriceJson.join(",")}}`);
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
        const isSelected = picked.includes(num);

        if (isSelected) {
            setPicked(picked.filter(n => n !== num));
            return;
        }

        const newLength = picked.length + 1;

        if (newLength > (chosenGameTemplate?.template?.maxNumbersPerTicket ?? 1)) return;
        if (wouldExceedBalance(newLength, repeat)) return; // stop selection

        setPicked([...picked, num]);
    };

    const onSubmit = async (values: CreateTicketToGameDto) => {
        try {
            setIsSubmitting(true);

            const payload = {
                gameInstanceId: values.gameInstanceId,
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
        setSelectedGame("");
        setPicked([]);
        setRepeat(0);
    }


    return (
        <div className="container mx-auto px-4 py-6 my-7">
            <div className="space-y-8">
                {/* Title */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-3xl font-bold">Lottery Tickets</h1>
                        <p className="text-gray-600">Create your lottery ticket</p>
                    </div>
                </div>

                {/* Game Template Dropdown */}
                <div className="bg-amber-100 p-5 rounded-xl shadow-md">
                    <h2 className="text-xl font-semibold mb-3">Active Game Templates</h2>

                    <select
                        className="select select-bordered w-full max-w-xs"
                        value={selectedGame}
                        onChange={e => {
                            const id = e.target.value
                            setSelectedGame(id)

                            const game = gameTemplate.find(t => t.id === id)
                            setChosenGameTemplate(game ?? null)

                            setPicked([])
                            setRepeat(0)
                        }}
                    >
                        <option value="">Choose game template</option>
                        {gameTemplate.map(t => (
                            <option key={t.id} value={t.id}>
                                {t.template?.name}
                            </option>
                        ))}
                    </select>
                </div>

                {/* Number Board */}
                <div className="bg-amber-50 p-5 rounded-xl shadow-md">

                    <h2 className="text-xl font-semibold mb-4">Pick Your Numbers</h2>

                    {chosenGameTemplate !== null && (
                        <div>
                            <p className="text-sm mb-2 text-gray-600">
                                Select {chosenGameTemplate.template?.minNumbersPerTicket}–
                                {chosenGameTemplate.template?.maxNumbersPerTicket} numbers.
                            </p>

                            <div
                                className="grid gap-3 max-w-sm"
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
                <div className="mt-6 flex items-center gap-6">
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
                            console.log("Submit ticket", picked, repeat)
                            void onSubmit({
                                gameInstanceId: chosenGameTemplate?.id ?? "",
                                selectedNumbers: picked,
                                repeat
                            })
                        }}
                    >
                        Submit Ticket
                    </button>
                </div>

                {/* Active Tickets */}
                <div className="bg-amber-100 p-6 rounded-xl shadow-md">
                    <h2 className="text-xl font-semibold mb-4">Active Tickets</h2>
                    <p className="text-gray-500">
                        {myTickets.length === 0 ? (
                            "You don't have any active tickets yet."
                        ) : (
                            <ul>
                                {myTickets.map(t => (
                                    <li key={t.id}>
                                        {t.gameInstanceId} - {t.selectedNumbers.join(", ")} - {t.repeat}
                                    </li>
                                ))}
                            </ul>
                        )}
                    </p>


                    <p className="text-gray-500">
                    </p>

                </div>
            </div>
        </div>
    );
}
