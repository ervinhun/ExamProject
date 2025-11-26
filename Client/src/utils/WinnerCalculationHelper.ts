export interface PlayerResult {
    id: string;
    name: string;
    numbersPlayed: number; // 5–8
}

export interface Week {
    id: string;
    date: string;
    weekNumber: number;
    onlineIncome: number;
    players: PlayerResult[];
}

export function calculateWinnersShare(
    onlineIncome: number,
    offlineIncome: number,
    totalWinners: number
): number {
    if (totalWinners === 0) return 0;

    const totalIncome = onlineIncome + offlineIncome;
    const winnersPool = totalIncome * 0.7;
    return Math.ceil(winnersPool / totalWinners);
}

function getISOWeekNumber(date: Date): number {
    const temp = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
    const day = temp.getUTCDay() || 7;

    temp.setUTCDate(temp.getUTCDate() + 4 - day);
    const yearStart = new Date(Date.UTC(temp.getUTCFullYear(), 0, 1));

    const diffInMs = temp.getTime() - yearStart.getTime();
    return Math.ceil((diffInMs / 86400000 + 1) / 7);
}

export function generateWeeks(): Week[] {
    const weeks: Week[] = [];

    for (let i = 0; i < 35; i++) {
        const today = new Date();
        const date = new Date(today);

        const day = today.getDay(); // 0=Sun…6=Sat
        const diff = day === 6 ? 0 : day + 1; // days after Saturday
        date.setDate(today.getDate() - diff - i * 7);

        const isoWeek = getISOWeekNumber(date);
        const dateStr = date.toISOString().split("T")[0];

        // Random count of players
        const playerCount = Math.floor(Math.random() * 10) + 5;

        const players: PlayerResult[] = Array.from({ length: playerCount }, (_, idx) => ({
            id: `p-${i}-${idx}`,
            name: `Player ${idx + 1}`,
            numbersPlayed: Math.floor(Math.random() * 4) + 5 // between 5–8
        }));

        // Each player buys 1 ticket with random price
        const ticketPrices = [20, 40, 80, 160];
        const ticketPrice = ticketPrices[Math.floor(Math.random() * ticketPrices.length)];
        const totalIncome = players.length * ticketPrice;

        weeks.push({
            id: `${date.getFullYear()}-W${isoWeek.toString().padStart(2, "0")}`,
            date: dateStr,
            weekNumber: isoWeek,
            players,
            onlineIncome: totalIncome
        });
    }

    return weeks;
}
