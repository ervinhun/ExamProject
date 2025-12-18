import type {GameInstanceDto, GameTemplate, GameTemplateDto} from "@core/types/game";
import {GameTemplateSchema} from "@core/types/game";
import {api} from "../Api";

const endpoint = "/api/games";

export const gameApi = {
    createGameTemplate: async (template: GameTemplateDto): Promise<GameTemplateDto> => {
        const gameTemplate = await api<GameTemplate>(`${endpoint}/templates/create-template`, {
            schema: GameTemplateSchema,
            init: {
                method: "POST",
                body: JSON.stringify(template)
            }
        });
        return gameTemplate as GameTemplateDto;
    },

    startGameInstance: async (gameInstance: Partial<GameInstanceDto>): Promise<GameInstanceDto> => {
        return await api<GameInstanceDto>(`${endpoint}/start-game`, {
            init: {
                method: "POST",
                body: JSON.stringify(gameInstance)
            }
        });
    },

    getAllGameTemplates: async (): Promise<GameTemplateDto[]> => {
        return await api<GameTemplateDto[]>(`${endpoint}/templates/all-templates`, {
            init: {
                method: "GET"
            }
        });
    },

    getAllGameInstances: async (): Promise<GameInstanceDto[]> => {
        return await api<GameInstanceDto[]>(`${endpoint}/all-games`, {
            init: {
                method: "GET"
            }
        });
    },

    getAllActiveGames: async (): Promise<GameInstanceDto[]> => {
        return await api<GameInstanceDto[]>(`${endpoint}/active-games`, {
            init: {
                method: "GET"
            }
        });
    },

    drawNumbersForGame: async (gameId: string, drawnNumbers: number[]): Promise<void> => {
        return await api<void>(`${endpoint}/draw-numbers/${gameId}`, {
            init: {
                method: "POST",
                body: JSON.stringify(drawnNumbers)
            }
        });
    }

}