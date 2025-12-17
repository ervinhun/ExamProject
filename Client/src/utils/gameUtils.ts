import type { GameInstanceDto } from "../core/types/game";

/**
 * Get badge color class based on status text
 */
export const getStatusColor = (status: string): string => {
    switch (status) {
        case "Active":
            return "badge-success";
        case "Pending Draw":
            return "badge-warning";
        case "Completed":
            return "badge-info";
        case "Approved":
            return "badge-success";
        case "Pending":
            return "badge-warning";
        case "Rejected":
            return "badge-error";
        case "Canceled":
            return "badge-ghost";
        default:
            return "badge-ghost";
    }
};

/**
 * Get the current week number of the year (ISO 8601 week)
 */
export const getCurrentWeekNumber = (): number => {
    const now = new Date();
    const start = new Date(now.getFullYear(), 0, 1);
    const diff = now.getTime() - start.getTime();
    const oneWeek = 1000 * 60 * 60 * 24 * 7;
    return Math.ceil(diff / oneWeek);
};

/**
 * Get the current year
 */
export const getCurrentYear = (): number => {
    return new Date().getFullYear();
};

/**
 * Check if game is from current week
 */
export const isCurrentWeek = (gameWeek: number): boolean => {
    return gameWeek === getCurrentWeekNumber();
};

/**
 * Get week badge class based on whether it's current week or past
 */
export const getWeekBadgeClass = (gameWeek: number): string => {
    const currentWeek = getCurrentWeekNumber();
    if (gameWeek === currentWeek) {
        return "badge-primary"; // Current week
    } else if (gameWeek < currentWeek) {
        return "badge-ghost"; // Past week
    } else {
        return "badge-neutral"; // Future week
    }
};

/**
 * Determine game status based on game state and timing
 * Matches server-side logic with proper year/week/day/time hierarchy
 */
export const getGameStatus = (game: GameInstanceDto): string => {
    // If game is completed, return Completed
    if (game.status === 1) {
        return "Completed";
    }

    const now = new Date();
    const currentYear = getCurrentYear();
    const currentWeek = getCurrentWeekNumber();
    const currentDayOfWeek = now.getDay(); // 0-6 (Sunday-Saturday)
    const currentTimeOfDay = now.toTimeString().substring(0, 5); // "HH:MM"

    if (game.isAutoRepeatable) {
        // For auto-repeatable games, check week number, day of week and time of day
        if (game.week !== undefined && game.year !== undefined && game.drawDayOfWeek !== undefined) {
            // Check if we're past the game's scheduled week/year
            const isPastScheduledWeek = currentYear > game.year || 
                                       (currentYear === game.year && currentWeek > game.week);
            
            // Check if we're in the same week/year
            const isSameWeek = currentYear === game.year && currentWeek === game.week;
            
            if (isPastScheduledWeek) {
                // Past the scheduled week/year
                return "Pending Draw";
            } else if (isSameWeek) {
                // We're in the correct week, now check day and time
                if (currentDayOfWeek > game.drawDayOfWeek) {
                    // Current day is past the draw day
                    return "Pending Draw";
                } else if (currentDayOfWeek === game.drawDayOfWeek && game.drawTimeOfDay) {
                    // Same day - check time with timezone correction
                    const [hours, minutes] = game.drawTimeOfDay.substring(0, 5).split(':').map(Number);
                    const correctedHours = hours - 1; // Subtract 1 hour for timezone
                    const correctedDrawTime = `${correctedHours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
                    
                    if (currentTimeOfDay >= correctedDrawTime) {
                        // Current time is past draw time
                        return "Pending Draw";
                    }
                }
            }
            // Future week/year or before draw day/time
            return "Active";
        }
    } else {
        // For one-time games, check draw date (DateTime includes both date and time)
        if (game.drawDate) {
            const drawDate = new Date(game.drawDate);
            
            // Subtract 1 hour (3600000 milliseconds) for timezone correction
            const correctedDrawTime = drawDate.getTime() - (60 * 60 * 1000);
            const nowTimestamp = now.getTime();
            
            if (nowTimestamp >= correctedDrawTime) {
                // Draw date/time has passed
                return "Pending Draw";
            }
        }
    }

    // Default: game is active and waiting for draw
    return "Active";
};

/**
 * Get ticket status based on game state
 */
export const getTicketStatus = (
    gameStatus: number,
    isDrawn: boolean,
    isWinning: boolean
): { badge: string; text: string } => {
    // Check if game is completed (status 1) or drawn
    if (gameStatus === 1 || isDrawn) {
        if (isWinning) {
            return { badge: "badge-success", text: "🎉 Won!" };
        } else {
            return { badge: "badge-error", text: "Lost" };
        }
    } else {
        // Game still active (status 0)
        return { badge: "badge-info", text: "Pending" };
    }
};
