/**
 * Format a number to Danish Kroner currency format
 * @param amount - The amount to format
 * @param decimals - Number of decimal places (default: 0)
 */
export const formatCurrency = (amount: number, decimals: number = 0): string => {
    return new Intl.NumberFormat('da-DK', {
        style: 'currency',
        currency: 'DKK',
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals
    }).format(amount);
};

/**
 * Calculate exponential ticket price
 * Price doubles for each additional number
 */
export const calculateExponentialPrice = (
    numCount: number,
    minNumbers: number,
    basePrice: number
): number => {
    if (numCount < minNumbers) return 0;
    return basePrice * Math.pow(2, numCount - minNumbers);
};

/**
 * Calculate linear ticket price
 * Price increases linearly per additional number
 */
export const calculateLinearPrice = (
    numCount: number,
    minNumbers: number,
    basePrice: number,
    incrementPerNumber: number = 10
): number => {
    if (numCount < minNumbers) return 0;
    return basePrice + ((numCount - minNumbers) * incrementPerNumber);
};
