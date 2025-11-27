import { atom } from "jotai";

export const errorAtom = atom<string | null>(null);
errorAtom.debugLabel = "Error Message";

