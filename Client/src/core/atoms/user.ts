import { atom } from 'jotai';
import type { User } from '../types/users';
import { fetchAllUsers, fetchUserById } from '../api/controllers/user';
import type { CreateUserDto } from '../types/users';
import { createUser } from '../api/controllers/user';

// Primary atoms
export const userListAtom = atom<User[]>([]);

export const selectedUserAtom = atom<User | null>(null);

export const userLoadingAtom = atom<boolean>(false);

export const fetchUsersAtom = atom(null, 
    async (_, set) => {
        await fetchAllUsers()
                    .then((res: any)=>set(userListAtom, res))
                    .catch((err: any) => {throw err})
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
                .then((res: any)=>{
                        set(selectedUserAtom, res);
                        return res;
                })
                .catch((err: any) => {throw err})
                .finally(() =>{});
        }
    }

)

export const createUserAtom = atom(null,
    async (get,set,createUserDto:CreateUserDto)=>{
        const users = get(userListAtom)
        for(let i = 0; i< users.length;i++){
            if(users[i].email === createUserDto.email){
               throw new Error("This email already exists")
            }
        }
        await createUser(createUserDto)
                .then((res: any)=>{
                        set(userListAtom,[...users,res]);
                        return res;
                })
                .catch((err: any) => {throw err})
                .finally(() =>{});
       

    }

)

// createUserAtom
 // deleteUserByIdAtom
 
 // updateUserByIdAtom