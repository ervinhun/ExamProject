import { atom, useSetAtom } from 'jotai';
import type { Player } from '../types/users';
import { playerApi } from '@core/api/controllers/player';
import { errorAtom } from './error';
import { WalletDto } from '@core/types/wallet';
import { addNotificationAtom } from './error';
import { userApi } from '@core/api/controllers/user';


export const playersAtom = atom<Player[]>([]);
export const walletAtom = atom<WalletDto | null>(null);

export const fetchPlayersAtom = atom(null, 
    async (_, set) => {
        await playerApi.getAll()
                    .then((res)=>set(playersAtom, res))
                    .catch((err) => {
                        set(errorAtom, err.message);
                        throw err})
                    .finally(() =>{});
    }
);

export const createPlayerAtom = atom(null,
    async (get,set,player: Omit<Player, 'id'>)=>{
        const players = get(playersAtom)
        
        await playerApi.create(player)
                .then((res)=>{
                        console.log('Created player:', res);
                        set(playersAtom,[...players,res]);
                        return res;
                })
                .catch((err) => {
                    set(errorAtom, err.message);
                    throw err})
                .finally(() =>{});
       

    }

);

export const getWalletForPlayerIdAtom = atom(null,
    async (_,set,playerId: string)=>{
        await playerApi.getWalletForPlayerId(playerId)
                .then((res)=>{
                        console.log('Fetched wallet for player:', res);
                        set(walletAtom,res);
                        return res;
                })
                .catch((err) => {
                    set(errorAtom, err.message);
                    throw err})
                .finally(() =>{});
    }
);

export const togglePlayerStatusAtom = atom(null,
    async (get,set,playerId: string)=>{
        const players = get(playersAtom);
        const player = players.find(p => p.id === playerId);
        if(!player){
            set(errorAtom, `Player with id ${playerId} not found.`);
            return;
        }

        const updatedPlayerData: Partial<Player> = {
            isActive: !player.isActive
        };

        await userApi.toggleStatus(playerId);
        const updatedPlayer = {...player, ...updatedPlayerData};

        set(playersAtom, players.map(p => p.id === playerId ? updatedPlayer : p));
        return updatedPlayer;
    });

    export const getAllAppliedUsers = atom(null,
        async (get,set)=> {
            await playerApi.getAllAppliedPlayer()
                .then((res) => set(playersAtom, res))
                .catch((err) => {
                    set(errorAtom, err.message);
                    throw err
                })
                .finally(() => {
                });
        }
);