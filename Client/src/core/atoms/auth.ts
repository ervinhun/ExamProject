import { authApi } from '../api/controllers/auth';
import { atom } from 'jotai'
import type { User } from '../types/users';
import { atomWithStorage } from 'jotai/utils';
import { errorAtom } from './error';
import type { AuthUser } from '@core/types/auth';

// Auth user shape used by the client convenience atoms


export const authAtom = atomWithStorage<AuthUser>("auth", {
    id: null,
    name: null,
    email: null,
    roles: []
    // token: null
});

authAtom.debugLabel = "Auth User";

export const isLoggedInAtom= atom<boolean>(false);
isLoggedInAtom.debugLabel = "Is Logged In";

isLoggedInAtom.onMount = (setAtom) => {
    const checkAuth = async () => {
        // First check the authAtom for existing session
        const store = await import('jotai/vanilla').then(m => m.getDefaultStore());
        const currentAuth = store.get(authAtom);
        
        if (currentAuth?.id) {
            console.log('[Auth] Found auth in atom, verifying with server...');
            // We have auth in atom, verify it's still valid
            try {
                const authUser = await authApi.profile();
                if (authUser?.id) {
                    console.log('[Auth] Session valid');
                    setAtom(true);
                    return;
                }
            } catch (err) {
                console.log('[Auth] Session invalid from atom, clearing...');
                store.set(authAtom, {
                    id: null,
                    name: null,
                    email: null,
                    roles: []
                });
                setAtom(false);
                return;
            }
        }
        
        // Check localStorage as fallback
        try {
            const authData = localStorage.getItem('auth');
            if (authData) {
                const parsed = JSON.parse(authData);
                if (parsed?.id) {
                    console.log('[Auth] Found stored auth, verifying with server...');
                    try {
                        const authUser = await authApi.profile();
                        if (authUser?.id) {
                            console.log('[Auth] Session valid, restoring to atom');
                            store.set(authAtom, parsed);
                            setAtom(true);
                            return;
                        }
                    } catch (err) {
                        console.log('[Auth] Session invalid, clearing storage');
                        localStorage.removeItem('auth');
                        setAtom(false);
                        return;
                    }
                }
            }
        } catch (err) {
            console.error('[Auth] Error checking stored auth:', err);
        }
        
        // No stored auth or verification failed
        console.log('[Auth] No valid session');
        setAtom(false);
    };
    
    checkAuth();
}

export const loginAtom = atom(null,
    async (_, set, credentials: {email: string, password: string}) => {
        await authApi.login(credentials).then((response)=>{
            // const token = response.accessToken;
            console.log("LoginAtom response:", response);
            const user: User = {...response.user} as User;
            try{
                set(authAtom, {
                    id: user.id ?? null,
                    name: `${user.firstName} ${user.lastName}`,
                    email: user.email,
                    roles: user.roles ?? [],
                    // token: response.accessToken ?? null
                });
            }catch(err){
                localStorage.setItem('auth', JSON.stringify({
                    id: user.id ?? null,
                    name: `${user.firstName} ${user.lastName}`,
                    email: user.email,
                    roles: user.roles ?? []
                }));
                throw err;
            }
            set(isLoggedInAtom, true);
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
                // token: null
            });
            set(isLoggedInAtom, false);
        }).catch((err) => {
            set(errorAtom, err.message);
            throw err;
        });
    }
);
