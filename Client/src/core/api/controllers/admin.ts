import type { User } from '@core/types/users';
import { atom } from 'jotai';

// Atom for the list of users
export const usersAtom = atom<User[]>([]);

// Atom for the currently selected user
export const selectedUserAtom = atom<User | null>(null);

// Atom for loading state
export const usersLoadingAtom = atom<boolean>(false);

// Atom for error state
export const usersErrorAtom = atom<string | null>(null);
