import { api } from '../Api';
import { PlayerSchema, type Player } from '../../types/users';

const endpoint = '/api/players';
export const playerApi = {
    getAll: async (): Promise<Player[]> => {
        return await api<Player[]>(`${endpoint}/all`, {
            init: {
                method: 'GET'
            }
        });
    },

    getById: async (id: string): Promise<Player> => {
        return await api<Player>(`${endpoint}/${id}`, {
            init: {
                method: 'GET'
            }
        });
    },

    create: async (player: Omit<Player, 'id'>): Promise<Player> => {
        return await api<Player>(`${endpoint}/create`, {
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
        return await api<Player>(`${endpoint}/${id}`, {
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

    // delete: async (id: string): Promise<void> => {
    //     // await api.delete(`${endpoint}/${id}`);
    // },
};