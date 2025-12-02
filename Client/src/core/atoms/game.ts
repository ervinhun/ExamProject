import { atom } from "jotai";
import type { GameInstanceDto, GameTemplate, GameTemplateDto } from "../types/game";
import { gameApi } from "../api/controllers/game";

export const gameTemplatesAtom = atom<GameTemplate[]>([]);
export const activeGamesAtom = atom<GameInstanceDto[]>([]);

export const isCreatingGameTemplateAtom = atom<boolean>(false);

export const createGameTemplateAtom = atom(null,
    async (get, set, template: GameTemplateDto) => {
        set(isCreatingGameTemplateAtom, true);
        await gameApi.createGameTemplate(template).then((res) => {
            const currentTemplates = get(gameTemplatesAtom);
            set(gameTemplatesAtom, [...currentTemplates, res]);
            return res;
        }).catch((err) => {
            throw err;
        }).finally(() => {
            set(isCreatingGameTemplateAtom, false);
        });
    }
);

export const startGameInstanceAtom = atom(null,
    async (get, set, gameInstance: Partial<GameInstanceDto>) => {
        await gameApi.startGameInstance(gameInstance).then((res) => {
            return res;
        }).catch((err) => {
            throw err;
        });
    }
);

export const fetchGameTemplatesAtom = atom(null,
    async (get, set) => {
        await gameApi.getAllGameTemplates().then((res) => {
            set(gameTemplatesAtom, res);
        }).catch((err) => {
            throw err;
        });
    }
);

export const fetchGameInstancesAtom = atom(null,
    async (get, set) => {
        await gameApi.getAllGameInstances().then((res) => {
            // Handle game instances as needed
            set(activeGamesAtom, res);
            return res;
        }).catch((err) => {
            throw err;
        });
    }
);

export const fetchActiveGamesAtom = atom(null,
    async (get, set) => {
        await gameApi.getAllActiveGames().then((res) => {
            set(activeGamesAtom, res);
            return res;
        }).catch((err) => {
            throw err;
        });
    }
);