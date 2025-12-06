import { TransactionDto } from "@core/types/transaction";
import { api } from "../Api";

const endpoint = "/api/transactions";
export const transactionApi = {
    getPendingTransactions: (): Promise<TransactionDto[]> => {
        return api<TransactionDto[]>(`${endpoint}/get-pending-transactions`, {
            init: {
                method: "GET"
            }
        });
    },

    approveTransaction: async (transactionId: number): Promise<void> => {
        return await api<void>(`${endpoint}/approve-transaction/${transactionId}`, {
            init: {
                method: "POST"
            }
        });
    },

    rejectTransaction: async (transactionId: number): Promise<void> => {
        return await api<void>(`${endpoint}/${transactionId}/reject-transaction`, {
            init: {
                method: "POST"
            }
        });
    }
}