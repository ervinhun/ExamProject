export default function Home() {
    return (
        <div className="container mx-auto px-4 py-8">
            <div className="max-w-4xl mx-auto">
                {/* Header */}
                <div className="mb-8">
                    <h1 className="text-4xl font-bold text-primary mb-3">
                        Welcome to Jerne IF 🐦
                    </h1>
                    <p className="text-xl text-base-content/70">
                        Dead Pigeons — Supporting our community through play
                    </p>
                </div>

                {/* Introduction */}
                <div className="prose prose-lg max-w-none mb-8">
                    <p className="text-base-content/80">
                        At Jerne IF, we're more than a sports club — we're a community. Our supporters are the lifeblood of everything we do. With "Dead Pigeons," you can now help power our club in a fun, engaging way, all while potentially winning weekly prizes.
                    </p>
                </div>

                {/* How It Works */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">How It Works</h2>
                        <div className="space-y-3 text-base-content/80">
                            <p>
                                Each week, we draw <strong>three winning numbers</strong> and compare them to the sequences on the boards you've purchased.
                            </p>
                            <p>
                                You choose <strong>5–8 numbers</strong> (from 1 to 16) on each board, and every matching sequence could land you a piece of the prize pool.
                            </p>
                            <p>
                                <strong>70%</strong> of all board sales goes back to the winners — the remaining <strong>30%</strong> supports Jerne IF.
                            </p>
                        </div>
                    </div>
                </div>

                {/* Getting Started */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">Getting Started</h2>
                    <div className="space-y-3 text-base-content/80">
                        <div className="flex gap-3">
                            <span className="font-semibold text-primary">1.</span>
                            <p>Apply for membership at one of our clubs.</p>
                        </div>
                        <div className="flex gap-3">
                            <span className="font-semibold text-primary">2.</span>
                            <p>Create a balance in your account (you can top-up via MobilePay — just include your transaction number for us to verify).</p>
                        </div>
                        <div className="flex gap-3">
                            <span className="font-semibold text-primary">3.</span>
                            <p>Buy as many boards as you like, for as many weeks as you want — and even reuse the same board week after week.</p>
                        </div>
                        <div className="flex gap-3">
                            <span className="font-semibold text-primary">4.</span>
                            <p>Join the game before <strong>Saturday at 17:00</strong> (local Danish time), and once the winning numbers are in, the next round begins automatically.</p>
                        </div>
                    </div>
                </div>

                {/* Transparency */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Transparency & Fairness</h2>
                        <p className="text-base-content/80">
                            Our admins oversee everything — they have full visibility into registered players, transactions, and game history. They also manually calculate and distribute prizes in coordination with physical players, ensuring fairness for everyone.
                        </p>
                    </div>
                </div>

                {/* Closing */}
                <div className="text-center py-6">
                    <p className="text-lg text-base-content/70">
                        Whether you're here to support your club, take part in something fun, or try your luck — welcome aboard.
                    </p>
                    <p className="text-lg font-semibold text-primary mt-3">
                        Play smart, play often — and thank you for helping Jerne IF thrive!
                    </p>
                </div>
            </div>
        </div>
    );
}

        