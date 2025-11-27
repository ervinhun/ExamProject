import { atom } from 'jotai';
import type { Player, User } from '../types/users';
import { playerApi } from '@core/api/controllers/player';
import { error } from 'console';
import { errorAtom } from './error';


export const playersAtom = atom<Player[]>([]);

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
                        set(playersAtom,[...players,res]);
                        return res;
                })
                .catch((err) => {
                    set(errorAtom, err.message);
                    throw err})
                .finally(() =>{});
       

    }

);