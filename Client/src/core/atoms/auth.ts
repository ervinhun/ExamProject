import { loginRequest, logoutRequest } from '../api/controllers/auth';
import { atom } from 'jotai'
import type { User } from '../types/users';
import { atomWithStorage } from 'jotai/utils';
import { errorAtom } from './error';
import { is } from 'zod/locales';
import { log } from 'console';

// Auth user shape used by the client convenience atoms
export type AuthUser = {
    id: string | null;
    name: string | null;
    email: string | null;
    roles: number[];
    token: string | null;
};

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
            console.log("LoginAtom response:", response);
            const user: User = {...response.user} as User;
            set(authAtom, {
                id: user.id ?? null,
                name: `${user.firstName} ${user.lastName}`,
                email: user.email,
                roles: user.roles ?? [],
                token: response.accessToken
            });
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