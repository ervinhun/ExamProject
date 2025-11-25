import {atom} from "jotai";
import type { AuthUser } from "@core/types/auth";

export const AuthAtom = atom<AuthUser>();
AuthAtom.debugLabel = "Auth User";

export const TokenAtom = atom({
    accessToken: null as string | null,
    refreshToken: null as string | null
})
