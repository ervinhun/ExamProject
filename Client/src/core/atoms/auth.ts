import { loginRequest, logoutRequest } from '../api/controllers/auth';
import { atom } from 'jotai'
import type { User } from '../types/users';
import { atomWithStorage } from 'jotai/utils';

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
            set(authAtom, {...user, roles: user.roles ?? [], token: response.accessToken} as AuthUser);
        }).catch((err) => {throw err})
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