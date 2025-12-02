import { TransactionDto } from "./transaction";

export interface WalletDto{
    id: string;
    balance: number;
    transactions: TransactionDto[];
    updatedAt: string;
}
