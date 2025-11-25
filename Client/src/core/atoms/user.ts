import { atom } from 'jotai';
import type { User } from '@core/types/users';
import { fetchAllUsers, fetchUserById } from '@core/api/controllers/user';
// Primary atoms
export const userListAtom = atom<User[]>([]);

export const selectedUserAtom = atom<User | null>(null);

export const userLoadingAtom = atom<boolean>(false);

export const fetchUsersAtom = atom(null, 
    async (_, set) => {
        await fetchAllUsers()
                    .then(res=>set(userListAtom, res))
                    .catch(err => {throw err})
                    .finally(() =>{});
    }
);

export const fetchUserByIdAtom = atom(null,
    async (get, set, userId: string) => {
        if(get(userListAtom).map(u => u.id).includes(userId)){
            return get(userListAtom).find(u => u.id === userId) as User;
        }
        else{
            await fetchUserById(userId)
                .then(res=>{
                        set(selectedUserAtom, res);
                        return res;
                })
                .catch(err => {throw err})
                .finally(() =>{});
        }
    }
)