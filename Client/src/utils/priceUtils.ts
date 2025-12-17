/**
 * Format a number to Danish Kroner currency format
 */
export const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('da-DK', {
        style: 'currency',
        currency: 'DKK',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
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
