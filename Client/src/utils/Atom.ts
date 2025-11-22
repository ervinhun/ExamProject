import {atom} from "jotai";
import type {Book} from "./models/Book.ts";
import type {Author} from "./models/Author.ts";
import type {Genre} from "./models/Genre.ts";

export const ShowBookAtom = atom("all");
ShowBookAtom.debugLabel = "ShowBookAtom";

export type FilterType = "author" | "genre";

export const FilterAtom = atom<
    { type: FilterType; id?: string; value: string } | null
>(null);
FilterAtom.debugLabel = "FilterAtom";

export type SortedBy = "title" | "pages" | "author" | "genre"; // adjust as needed

export const SortingAtom = atom<{ type: SortedBy; value: "asc" | "desc" }>({
    type: "title",
    value: "asc",
});
SortingAtom.debugLabel = "SortingAtom";

export const AuthorAtom = atom<Author[]>([]);
AuthorAtom.debugLabel = "AuthorAtom";

export const GenreAtom = atom<Genre[]>([]);
GenreAtom.debugLabel = "GenreAtom";
