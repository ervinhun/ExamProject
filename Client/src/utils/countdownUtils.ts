import type { GameInstanceDto } from "../core/types/game";

/**
 * Get the ISO week number of a date
 */
export const getWeekNumber = (date: Date): number => {
    const start = new Date(date.getFullYear(), 0, 1);
    const diff = date.getTime() - start.getTime();
    const oneWeek = 1000 * 60 * 60 * 24 * 7;
    return Math.ceil(diff / oneWeek);
};

/**
 * Get the next draw time for a game
 * Follows year → week → day → time hierarchy for repeatable games
 */
export const getNextDrawTime = (game: GameInstanceDto): Date | null => {
    const now = new Date();
    const currentYear = now.getFullYear();
    const currentWeek = getWeekNumber(now);
    const currentDayOfWeek = now.getDay(); // 0-6 (Sunday-Saturday)
    const currentTimeOfDay = now.toTimeString().substring(0, 5); // "HH:MM"
    
    if (game.isAutoRepeatable && game.drawDayOfWeek !== undefined && game.drawTimeOfDay) {
        const gameDayOfWeek = game.drawDayOfWeek;
        const [hours, minutes] = game.drawTimeOfDay.substring(0, 5).split(':').map(Number);
        const correctedHours = hours - 1; // Timezone correction
        
        // Check if in current year
        if (currentYear !== game.year) {
            return null;
        }
        
        // Check year and week to determine if game is still active
        const isPastScheduledWeek = currentWeek > game.week;
        const isSameWeek = currentWeek === game.week;
        
        if (isPastScheduledWeek) {
            return null;
        }
        
        if (isSameWeek) {
            // We're in the game's scheduled week
            const correctedDrawTime = `${correctedHours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
            
            if (currentDayOfWeek < gameDayOfWeek) {
                // Draw day is still ahead this week
                const target = new Date(now);
                const daysUntilDraw = gameDayOfWeek - currentDayOfWeek;
                target.setDate(target.getDate() + daysUntilDraw);
                target.setHours(correctedHours, minutes, 0, 0);
                return target;
            } else if (currentDayOfWeek === gameDayOfWeek) {
                // Same day - check time
                if (currentTimeOfDay < correctedDrawTime) {
                    const target = new Date(now);
                    target.setHours(correctedHours, minutes, 0, 0);
                    return target;
                } else {
                    return null;
                }
            } else {
                return null;
            }
        } else {
            // Future week - calculate the exact date
            const weeksAhead = game.week - currentWeek;
            const daysToAdd = weeksAhead * 7;
            
            const targetWeekStart = new Date(now);
            targetWeekStart.setDate(targetWeekStart.getDate() + daysToAdd);
            
            const targetWeekStartDay = targetWeekStart.getDay();
            const daysToDrawDay = (gameDayOfWeek - targetWeekStartDay + 7) % 7;
            
            const target = new Date(targetWeekStart);
            target.setDate(targetWeekStart.getDate() + daysToDrawDay);
            target.setHours(correctedHours, minutes, 0, 0);
            
            return target;
        }
    } else if (game.drawDate) {
        // For non-repeatable games, use the exact draw date
        const drawDate = new Date(game.drawDate);
        const correctedDrawTime = new Date(drawDate.getTime() - (60 * 60 * 1000));
        
        return correctedDrawTime > now ? correctedDrawTime : null;
    }
    
    return null;
};

/**
 * Find the closest upcoming game from a list of games
 */
export const getNextGame = (games: GameInstanceDto[]): GameInstanceDto | null => {
    const activeGames = games.filter(game => game.status === 0 || game.status === "Active");
    if (activeGames.length === 0) return null;
    
    let closestGame: GameInstanceDto | null = null;
    let closestTime: Date | null = null;
    
    for (const game of activeGames) {
        const drawTime = getNextDrawTime(game);
        if (drawTime && (!closestTime || drawTime < closestTime)) {
            closestTime = drawTime;
            closestGame = game;
        }
    }
    
    return closestGame;
};

/**
 * Calculate countdown values from a target date
 * Returns days, hours, minutes, and seconds remaining
 */
export const calculateCountdown = (targetDate: Date | null): {
    days: number;
    hours: number;
    minutes: number;
    seconds: number;
} => {
    if (!targetDate) {
        return { days: 0, hours: 0, minutes: 0, seconds: 0 };
    }
    
    const remaining = Math.max(0, targetDate.getTime() - Date.now());
    const totalSeconds = Math.max(0, Math.floor(remaining / 1000));

    const days = Math.floor(totalSeconds / (3600 * 24));
    const hours = Math.floor((totalSeconds % (3600 * 24)) / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;

    return { days, hours, minutes, seconds };
};
