/**
 * Format a date string to Danish locale format
 */
export const formatDate = (dateStr: string | Date | undefined) => {
    if (!dateStr) return "N/A";
    const date = new Date(dateStr);
    return date.toLocaleDateString("da-DK", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric"
    });
};

/**
 * Format a date string to Danish locale format with time
 */
export const formatDateTime = (dateStr: string | Date | undefined) => {
    if (!dateStr) return "N/A";
    const date = new Date(dateStr);
    return date.toLocaleDateString("da-DK", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        hour12: false
    });
};

/**
 * Map day number to day name
 */
export const mapDayOfWeek = (dayNum: number | undefined): string => {
    const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    if (dayNum === undefined || dayNum < 0 || dayNum > 6) return "N/A";
    return days[dayNum];
};
