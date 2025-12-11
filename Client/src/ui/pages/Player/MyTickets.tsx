import {useEffect, useState} from "react";
import {useAtom} from "jotai";
import {activeGamesAtom} from "@core/atoms/game.ts";
import {gameApi} from "@core/api/controllers/game.ts";
import {GameInstanceDto} from "@core/types/game.ts";

export default function MyTickets() {

    const [selectedGame, setSelectedGame] = useState("lottery16");
    const [picked, setPicked] = useState<number[]>([]);
    const [repeat, setRepeat] = useState(1);
    const [gameTemplate, setGameTemplate] = useAtom(activeGamesAtom);
    const [choosenGameTemplate, setChoosenGameTemplate] = useState<GameInstanceDto | null>(null);
    const canSubmit =
        choosenGameTemplate !== null &&
        picked.length >= choosenGameTemplate.template!.minNumbersPerTicket;


    useEffect(() => {
        if (gameTemplate.length === 0) {
            gameApi.getAllActiveGames().then((data) => {
                setGameTemplate(data);
                if (choosenGameTemplate === null) {
                    setChoosenGameTemplate(data[0]);
                }
            });
        }
    }, []);

    // Price map
    const priceTable: Record<number, number> = {
        5: 20,
        6: 40,
        7: 80,
        8: 160,
    };

    const basePrice = priceTable[picked.length] ?? 0;
    const totalPrice = basePrice * repeat;

    const toggleNumber = (num: number) => {
        if (picked.includes(num)) {
            setPicked(picked.filter(n => n !== num));
        } else {
            if (picked.length < 8) setPicked([...picked, num]);
        }
    };

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
                            setChoosenGameTemplate(game ?? null)

                            setPicked([])
                            setRepeat(1)
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

                    {choosenGameTemplate !== null && (
                        <div>
                            <p className="text-sm mb-2 text-gray-600">
                                Select {choosenGameTemplate.template?.minNumbersPerTicket}–
                                {choosenGameTemplate.template?.maxNumbersPerTicket} numbers.
                            </p>

                            <div
                                className="grid gap-3 max-w-sm"
                                style={{
                                    gridTemplateColumns: `repeat(${Math.ceil(
                                        Math.sqrt(choosenGameTemplate.template?.poolOfNumbers ?? 0)
                                    )}, 1fr)`
                                }}
                            >
                                {[...Array(choosenGameTemplate.template?.poolOfNumbers)].map((_, i) => {
                                    const num = i + 1;
                                    const selected = picked.includes(num);

                                    return (
                                        <button
                                            key={num}
                                            onClick={() => toggleNumber(num)}
                                            disabled={
                                                !selected &&
                                                picked.length >= (choosenGameTemplate.template?.maxNumbersPerTicket ?? 1)
                                            }
                                            className={`p-3 rounded-lg text-center border font-semibold transition-all
                                            ${selected
                                                ? "bg-green-600 text-white border-green-700"
                                                : "bg-white border-gray-300 hover:bg-gray-100"}
                                            ${!selected && picked.length >= (choosenGameTemplate.template?.maxNumbersPerTicket ?? 0)
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
                            min={1}
                            value={repeat}
                            onChange={e => setRepeat(Number(e.target.value))}
                        />
                    </div>

                    <div>
                        <p className="text-gray-700 font-semibold">Total:</p>
                        <p className="text-xl font-bold">{totalPrice} kr.</p>
                    </div>
                    {/* Submit Button */}
                    <button
                        disabled={!canSubmit}
                        className={`btn btn-primary ml-6 ${
                            !canSubmit ? "btn-disabled opacity-50 cursor-not-allowed" : ""
                        }`}
                        onClick={() => console.log("Submit ticket", picked, repeat)}
                    >
                        Submit Ticket
                    </button>
                </div>

                {/* Active Tickets */}
                <div className="bg-amber-100 p-6 rounded-xl shadow-md">
                    <h2 className="text-xl font-semibold mb-4">Active Tickets</h2>

                    <p className="text-gray-500">
                        No active tickets yet. This section will show the user's ongoing repeating tickets.
                    </p>
                </div>
            </div>
        </div>
    );
}
