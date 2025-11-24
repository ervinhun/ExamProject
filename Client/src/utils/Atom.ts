import {atomWithStorage} from "jotai/utils";
import type {AuthUser} from "../model/AuthUser.ts";

export const AuthAtom = atomWithStorage<AuthUser>("auth", {
    name: null,
    email: null,
    role: [],
    token: null
});
AuthAtom.debugLabel = "Auth User";
