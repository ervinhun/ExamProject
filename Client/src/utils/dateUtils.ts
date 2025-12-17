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
    
    // Parse as UTC to prevent timezone conversion
    const dateString = typeof dateStr === 'string' ? dateStr : dateStr.toISOString();
    const date = new Date(dateString);
    
    // Use UTC methods to get the exact time from the database
    const day = date.getUTCDate().toString().padStart(2, '0');
    const month = (date.getUTCMonth() + 1).toString().padStart(2, '0');
    const year = date.getUTCFullYear();
    const hours = date.getUTCHours().toString().padStart(2, '0');
    const minutes = date.getUTCMinutes().toString().padStart(2, '0');
    
    return `${day}/${month}/${year}, ${hours}:${minutes}`;
};

/**
 * Map day number to day name
 */
export const mapDayOfWeek = (dayNum: number | undefined): string => {
    const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    if (dayNum === undefined || dayNum < 0 || dayNum > 6) return "N/A";
    return days[dayNum];
};
