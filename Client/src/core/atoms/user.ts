import { atom } from 'jotai';
import type { User } from '../types/users';
import { playerApi } from '../api/controllers/player';
import { userApi } from '../api/controllers/user';
import type { CreateUserDto } from '../types/users';
import { errorAtom } from './error';

// Primary atoms
export const userListAtom = atom<User[]>([]);

export const selectedUserAtom = atom<User | null>(null);

export const userLoadingAtom = atom<boolean>(false);

export const fetchUsersAtom = atom(null, 
    async (_, set) => {
        await userApi.getAll()
                    .then((res)=>set(userListAtom, res))
                    .catch((err) => {
                        set(errorAtom, err.message);
                        throw err})
                    .finally(() =>{});
    }
);

export const fetchUserByIdAtom = atom(null,
    async (get, set, userId: string) => {
        if(get(userListAtom).map(u => u.id).includes(userId)){
            return get(userListAtom).find(u => u.id === userId) as User;
        }
        else{
            await playerApi.getById(userId)
                .then((res)=>{
                        set(selectedUserAtom, res);
                        return res;
                })
                .catch((err) => {
                    set(errorAtom, err.message);
                    throw err})
                .finally(() =>{});
        }
    }

)

export const createUserAtom = atom(null,
    async (get,set,createUserDto:CreateUserDto)=>{
        const users = get(userListAtom)

        await userApi.createUser(createUserDto)
                .then((res)=>{
                        set(userListAtom,[...users,res]);
                        return res;
                })
                .catch((err) => {
                    set(errorAtom, err.message);
                    throw err;
                })
                .finally(() =>{});
       

    }

)

// createUserAtom
 // deleteUserByIdAtom
 
 // updateUserByIdAtom