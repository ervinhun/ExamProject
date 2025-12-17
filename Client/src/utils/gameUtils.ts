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
 * Determine game status based on game state and timing
 */
export const getGameStatus = (game: GameInstanceDto): string => {
    // If game is completed, show completed
    if (game.status === 1) {
        return "Completed";
    }

    const now = new Date();

    if (game.isAutoRepeatable) {
        // For repeatable games, check if draw day/time has passed
        const currentDay = now.getDay(); // 0-6 (Sunday-Saturday)
        const currentTime = now.toTimeString().substring(0, 5); // HH:MM

        if (game.drawDayOfWeek !== undefined && game.drawTimeOfDay) {
            // Check if current day is past draw day, or same day but time has passed
            if (currentDay > game.drawDayOfWeek) {
                return "Pending Draw";
            } else if (currentDay === game.drawDayOfWeek && currentTime >= game.drawTimeOfDay.substring(0, 5)) {
                return "Pending Draw";
            }
        }
    } else {
        // For non-repeatable games, check if draw date has passed
        if (game.drawDate) {
            const drawDate = new Date(game.drawDate);
            if (now >= drawDate) {
                return "Pending Draw";
            }
        }
    }

    // Otherwise, game is active and waiting for draw date/time
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
