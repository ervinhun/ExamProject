import { authApi } from '../api/controllers/auth';
import { atom } from 'jotai'
import type { User } from '../types/users';
import { atomWithStorage } from 'jotai/utils';
import { errorAtom } from './error';
import type { AuthUser } from '@core/types/auth';
import { getDefaultStore } from 'jotai/vanilla';

// Auth user shape used by the client convenience atoms


export const authAtom = atomWithStorage<AuthUser>("auth", {
    id: null,
    name: null,
    email: null,
    roles: []
    // token: null
});

authAtom.debugLabel = "Auth User";

export const isLoggedInAtom = atom<boolean, [boolean], void>(
    (get) => {
        const auth = get(authAtom);
        return auth?.id !== null;
    },
    (_get, _set, _newValue) => {
        // Write function for compatibility - actual state derived from authAtom
    }
);
isLoggedInAtom.debugLabel = "Is Logged In";

// Verify auth session on mount
isLoggedInAtom.onMount = (setAtom) => {
    const verifySession = async () => {
        const store = getDefaultStore();
        const currentAuth = store.get(authAtom);
        
        // Set initial state based on stored auth
        setAtom(currentAuth?.id !== null);
        
        // If we have auth data, verify it with the server
        if (currentAuth?.id) {
            console.log('[Auth] Found auth data, verifying session with server...');
            try {
                const authUser = await authApi.profile();
                if (authUser?.id) {
                    console.log('[Auth] Session valid, updating with fresh data');
                    // Update atom with fresh data from server
                    store.set(authAtom, {
                        id: authUser.id,
                        name: `${authUser.firstName} ${authUser.lastName}`,
                        email: authUser.email,
                        roles: authUser.roles ?? []
                    });
                    setAtom(true);
                    return;
                }
            } catch (err) {
                console.log('[Auth] Session invalid, clearing auth...');
                store.set(authAtom, {
                    id: null,
                    name: null,
                    email: null,
                    roles: []
                });
                localStorage.removeItem('auth');
                setAtom(false);
                return;
            }
        } else {
            console.log('[Auth] No auth data found, trying to verify session...');
            try {
                const authUser = await authApi.profile();
                if (authUser?.id) {
                    console.log('[Auth] Session still valid, setting auth data');
                    // Update atom with fresh data from server
                    const newAuthUser = {
                        id: authUser.id,
                        name: `${authUser.firstName} ${authUser.lastName}`,
                        email: authUser.email,
                        roles: authUser.roles ?? []
                    };
                    store.set(authAtom, newAuthUser);
                    localStorage.setItem('auth', JSON.stringify(newAuthUser));
                    setAtom(true);
                    return;
                }
            } catch (err) {
                console.log('[Auth] No valid session found, user must login...');
                store.set(authAtom, {
                    id: null,
                    name: null,
                    email: null,
                    roles: []
                });
                localStorage.removeItem('auth');
                setAtom(false);
            }
        }
    };
    
    verifySession();
};

export const loginAtom = atom(null,
    async (_, set, credentials: {email: string, password: string}) => {
        await authApi.login(credentials).then((response)=>{
            console.log("LoginAtom response:", response);
            const user: User = {...response.user} as User;
            const authUser = {
                id: user.id ?? null,
                name: `${user.firstName} ${user.lastName}`,
                email: user.email,
                roles: user.roles ?? [],
            };
            set(authAtom, authUser);
            set(isLoggedInAtom, true);
            localStorage.setItem('auth', JSON.stringify(authUser));
        }).catch((err) => {
            set(errorAtom, err.message)
        })
    }
)

export const logoutAtom = atom(null, 
    async (_, set) => {
        await authApi.logout().then(() => {
            set(authAtom, {
                id: null,
                name: null,
                email: null,
                roles: []
            });
            set(isLoggedInAtom, false);
            localStorage.removeItem('auth');
        }).catch((err) => {
            set(errorAtom, err.message);
            throw err;
        });
    }
);
