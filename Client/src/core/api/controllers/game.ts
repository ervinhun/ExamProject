import {GameInstanceDto, GameTemplate, GameTemplateDto, GameTemplateSchema} from "@core/types/game";
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
        return await api<GameInstanceDto[]>(`${endpoint}/all-instances`, {
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


}