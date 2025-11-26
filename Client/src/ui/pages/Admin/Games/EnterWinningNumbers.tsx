import { useState } from "react";

export default function EnterWinningNumbers() {
    const [numbers, setNumbers] = useState(["", "", ""]);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [isSubmitted, setIsSubmitted] = useState(false);

    function updateNumber(index: number, value: string) {
        const clone = [...numbers];
        clone[index] = value;
        setNumbers(clone);
        setError("");
        setSuccess("");
    }

    function submit() {
        const nums = numbers.map(n => Number(n));

        // Validation
        if (nums.some(isNaN)) {
            setError("All fields must be numbers.");
            return;
        }
        if (nums.some(n => n < 1 || n > 16)) {
            setError("Numbers must be between 1 and 16.");
            return;
        }
        if (new Set(nums).size !== 3) {
            setError("Numbers must be unique.");
            return;
        }

        // Submit API goes here
        setSuccess("Winning numbers submitted successfully! New week is starting.");
        setIsSubmitted(true);
    }

    return (
        <div>

            <h1 className="text-3xl text-center font-bold mb-6">
                Enter Winning Numbers
            </h1>

            {/* Input section */}
            <div className="flex justify-center gap-6 mb-6">

                {numbers.map((num, i) => (
                    <div key={i} className="flex flex-col items-center">

                        <div className="w-16 h-16 rounded-full bg-primary text-white flex
                                        items-center justify-center text-2xl font-bold mb-3 shadow">
                            {num || "-"}
                        </div>

                        <input
                            type="number"
                            id={`number-${i}`}
                            min={1}
                            max={16}
                            value={num}
                            onChange={(e) => updateNumber(i, e.target.value)}
                            className="input input-bordered w-20 text-center"
                            disabled = {isSubmitted}
                        />
                    </div>
                ))}

            </div>

            {/* Error */}
            {error && (
                <div className="text-red-500 font-semibold text-center mb-3">
                    {error}
                </div>
            )}

            {/* Success */}
            {success && (
                <div className="text-green-500 font-semibold text-center mb-3">
                    {success}
                </div>
            )}

            {/* Submit button */}
            <div className="flex justify-center">
                <button
                    onClick={submit}
                    className="btn btn-primary btn-wide text-lg"
                    disabled = {isSubmitted}
                >
                    Submit
                </button>
            </div>
        </div>
    );
}
