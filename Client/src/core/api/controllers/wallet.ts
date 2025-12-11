import { WalletDto } from "@core/types/wallet";
import { api } from "../Api";
import { DepositRequestDto, TransactionDto } from "@core/types/transaction";

export const walletApi = {
    getWalletForPlayerId: async (playerId: string): Promise<WalletDto> => {
        return await api<WalletDto>(`/api/players/${playerId}/wallet`, {
            init: {
                method: 'GET'
            }
        });
    },

    requestForDeposit: async (depositRequest: DepositRequestDto): Promise<TransactionDto> => {
        return await api<TransactionDto>(`/api/players/${depositRequest.playerId}/wallet/deposit`, {
            init: {
                method: 'POST',
                body: JSON.stringify(depositRequest),
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        });
    }   
}