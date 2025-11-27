import { loginRequest, logoutRequest } from '../api/controllers/auth';
import { atom } from 'jotai'
import type { User } from '../types/users';
import { atomWithStorage } from 'jotai/utils';
import { errorAtom } from './error';
import { AuthUser } from '@core/types/auth';

// Auth user shape used by the client convenience atoms


export const authAtom = atomWithStorage<AuthUser>("auth", {
    id: null,
    name: null,
    email: null,
    roles: [],
    token: null,
});
authAtom.debugLabel = "Auth User";


export const loginAtom = atom(null,
    async (_, set, credentials: {email: string, password: string}) => {
        await loginRequest(credentials).then((response)=>{
            const token = response.accessToken;
            console.log("LoginAtom response:", response);
            const user: User = {...response.user} as User;
            try{
                set(authAtom, {
                    id: user.id ?? null,
                    name: `${user.firstName} ${user.lastName}`,
                    email: user.email,
                    roles: user.roles ?? [],
                    token: response.accessToken ?? null
                });
            }catch(err){
                localStorage.setItem('auth', token || '');
                throw err;
            }

        }).catch((err) => {
            set(errorAtom, err.message)
        })
        .finally(() =>{});
    }
)

export const logoutAtom = atom(null, 
    async (_, set) => {
        await logoutRequest()
                    .catch((err) => {throw err})
                    .finally(() => set(authAtom, {id: null, name: null, email: null, roles: [], token: null}));
    }
);

export const isLoggedInAtom = atom((get) => {
    const auth = get(authAtom);
    return auth.token !== null && auth.token !== undefined;
});