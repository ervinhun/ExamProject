import {atom} from "jotai";

export const AuthAtom = atom({
    name: null as string | null,
    email: null as string | null,
    role: null as string | null,
    token: null as string | null
});
AuthAtom.debugLabel = "Auth User";

export const TokenAtom = atom({
    accessToken: null as string | null,
    refreshToken: null as string | null
})
