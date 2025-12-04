export interface TransactionDto{
    id: string;
    name: string;
    amount: number;
    type: number | string;
    status: number | string;
    createdAt: string;
}

export interface DepositRequestDto{
    amount: number;
    walletId: string;
    playerId: string;
    mobilePayTransactionNumber: string;
}

// Transaction Status Mapping
export const mapTransactionStatus = (status: number | string): string => {
    if (typeof status === "number") {
        switch (status) {
            case 0: return "Pending";
            case 1: return "Rejected";
            case 2: return "Canceled";
            case 3: return "Approved";
            default: return "Unknown";
        }
    }
    return status;
};

// Transaction Type Mapping
export const mapTransactionType = (type: number | string): string => {
    if (typeof type === "number") {
        switch (type) {
            case 0: return "Deposit";
            case 1: return "Withdrawal";
            case 2: return "TicketPurchase";
            case 3: return "RewardPayout";
            case 4: return "Refund";
            case 5: return "SystemAdjustment";
            default: return "Unknown";
        }
    }
    return type;
};