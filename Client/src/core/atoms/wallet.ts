import { walletApi } from "@core/api/controllers/wallet";
import { WalletDto } from "@core/types/wallet";
import { errorAtom } from "./error";
import { atom } from "jotai";
import { DepositRequestDto } from "@core/types/transaction";
import { authAtom } from "./auth";

export const walletAtom = atom<WalletDto | null>(null);

export const getWalletForPlayerIdAtom = atom(null,
    async (_get, set, playerId: string) => {
        await walletApi.getWalletForPlayerId(playerId)
            .then((res) => {
                console.log('Fetched wallet for player:', res);
                set(walletAtom, res);
                return res;
            })
            .catch((err) => {
                set(errorAtom, err.message);
                throw err;
            })
            .finally(() => {});
    }
);

export const requestDepositAtom = atom(null,
    async (get, set, depositRequest: DepositRequestDto) => {
        try {
            const authUser = get(authAtom);
            const wallet = get(walletAtom);
            
            if (!authUser?.id) {
                throw new Error("User not authenticated");
            }
            
            if (!wallet?.id) {
                throw new Error("Wallet not found");
            }
            
            if (depositRequest.amount <= 0) {
                throw new Error("Amount must be greater than 0");
            }

            if (depositRequest.mobilePayTransactionNumber.length < 11){
                throw new Error("Wrong MobilePay transaction number");
            }

            const response = await walletApi.requestForDeposit({
                amount: Number(depositRequest.amount),
                walletId: wallet.id,
                playerId: authUser.id,
                mobilePayTransactionNumber: depositRequest.mobilePayTransactionNumber
            });

            // Refetch wallet to get updated balance and transactions
            await walletApi.getWalletForPlayerId(authUser.id)
                .then((updatedWallet) => {
                    set(walletAtom, updatedWallet);
                });

            return response;
        } catch (error: any) {
            const errorMessage = error.response?.data?.message || error.message;
            set(errorAtom, errorMessage);
            console.error("Deposit failed:", error);
            throw error;
        }
    }
);