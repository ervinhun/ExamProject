import { api } from '../Api';
import type { WalletDto } from '../../types/wallet';
import { PlayerSchema, type Player } from '../../types/users';
import { DepositRequestDto } from '@core/types/transaction';

const endpoint = '/api/players';
export const playerApi = {
    getAll: async (): Promise<Player[]> => {
        return await api<Player[]>(`${endpoint}/all-players`, {
            init: {
                method: 'GET'
            }
        });
    },

    getById: async (id: string): Promise<Player> => {
        return await api<Player>(`${endpoint}/get-player/${id}`, {
            init: {
                method: 'GET'
            }
        });
    },

    create: async (player: Omit<Player, 'id'>): Promise<Player> => {
        return await api<Player>(`${endpoint}/register-player`, {
            // schema: PlayerSchema,
            init: {
                credentials: 'include',
                method: 'POST',
                body: JSON.stringify(player),
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        });
    },

    update: async (id: string, player: Partial<Player>): Promise<Player> => {
        return await api<Player>(`${endpoint}/update-player/${id}`, {
            schema: PlayerSchema,
            init: {
                method: 'PUT',
                body: JSON.stringify(player),
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        });
    },

    getWalletForPlayerId: async (playerId: string): Promise<WalletDto> => {
        return await api<WalletDto>(`${endpoint}/${playerId}/wallet`, {
            init: {
                method: 'GET'
            }
        });
    },

    requestForDeposit: async (depositRequest: DepositRequestDto): Promise<void> => {
        return await api<void>(`${endpoint}/${depositRequest.playerId}/wallet/deposit`, {
            init: {
                method: 'POST',
                body: JSON.stringify(depositRequest),
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        });
    }   
};