import type { GameInstanceDto, GameTemplate, GameTemplateDto } from "@core/types/game";
import { GameTemplateSchema } from "@core/types/game";
import { api } from "../Api";

const endpoint = "/api/games";

export const gameApi = {
    createGameTemplate: async (template: GameTemplateDto): Promise<GameTemplateDto> => {
        const gameTemplate = await api<GameTemplate>(`${endpoint}/templates/create`, {
            schema: GameTemplateSchema,
            init: {
                method: "POST",
                body: JSON.stringify(template)
            }
        });
        return gameTemplate as GameTemplateDto;
    },
    
    startGameInstance: async (gameInstance: Partial<GameInstanceDto>): Promise<GameInstanceDto> => {
        return await api<GameInstanceDto>(`${endpoint}/start`, {
            init: {
                method: "POST",
                body: JSON.stringify(gameInstance)
            }
        });
    },

    getAllGameTemplates: async (): Promise<GameTemplateDto[]> => {
        return await api<GameTemplateDto[]>(`${endpoint}/templates/all`, {
            init: {
                method: "GET"
            }
        });
    },

    getAllGameInstances: async (): Promise<GameInstanceDto[]> => {
        return await api<GameInstanceDto[]>(`${endpoint}/all`, {
            init: {
                method: "GET"
            }
        });
    },

    getAllActiveGames: async (): Promise<GameInstanceDto[]> => {
        return await api<GameInstanceDto[]>(`${endpoint}/active`, {
            init: {
                method: "GET"
            }
        });
    }
}