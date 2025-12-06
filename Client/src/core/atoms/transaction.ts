import { transactionApi } from "@core/api/controllers/transactions";
import { TransactionDto } from "@core/types/transaction";
import { atom } from "jotai";

export const pendingTransactionsAtom = atom<TransactionDto[]>([]);

export const fetchPendingTransactionsAtom = atom(null,
    async (_, set) => {
        await transactionApi.getPendingTransactions()
            .then((res) => set(pendingTransactionsAtom, res))
            .catch((err) => {
                set(pendingTransactionsAtom, []);
                throw err;
            });
    }
);

export const approveTransactionAtom = atom(null,
    async (get, set, transactionId: number) => {
        await transactionApi.approveTransaction(transactionId)
            .then(() => {
                const updatedTransactions = get(pendingTransactionsAtom).filter(t => t.id !== transactionId);
                set(pendingTransactionsAtom, updatedTransactions);
            })
            .catch((err) => {
                throw err;
            });
    }
);