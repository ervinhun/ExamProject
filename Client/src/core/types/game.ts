import z from "zod";

export const GameTemplateSchema = z.object({
    id: z.uuid().optional(),
    name: z.string(),
    description: z.string(),
    gameType: z.enum(["Lotto", "Bingo"]),
    poolOfNumbers: z.number(),
    maxWinningNumbers: z.number(),
    minNumbersPerTicket: z.number().min(1),
    maxNumbersPerTicket: z.number().min(1),
    basePrice: z.number().min(0),
    priceGrowthRule: z.string().optional(),
    createdAt: z.string().optional(),
    updatedAt: z.string().nullable().optional(),
});

export type GameTemplate = z.infer<typeof GameTemplateSchema>;

export interface GameTemplateDto {
    id?: string;
    name: string;
    description: string;
    gameType: "Lotto" | "Bingo";
    poolOfNumbers: number;
    maxWinningNumbers: number;
    minNumbersPerTicket: number;
    maxNumbersPerTicket: number;
    basePrice: number;
    priceGrowthRule?: string;
    createdAt?: string;
    updatedAt?: string;
}

export interface GameInstanceDto {
    id?: string;
    createdById: string;
    template?: GameTemplateDto;
    status: "Active" | "Completed" | "Pending Draw" | 0 | 1 | 2;
    isAutoRepeatable: boolean;
    expirationDate?: string;
    expirationDayOfWeek?: number; // 0 (Sunday) to 6 (Saturday) for weekly repeats
    expirationTimeOfDay?: string; // "HH:MM" format for daily repeats
    week: number;
    drawDate?: Date;
    isDrawn: boolean;
    participants: number;
    isExpired: boolean;
    winningNumbers: number[];
    createdAt: string;
    updatedAt: string;
}

export const mapGameStatus = (status: number | string): "Active" | "Completed" | "Pending Draw" => {
    if (typeof status === "number") {
        switch (status) {
            case 0:
                return "Active";
            case 1:
                return "Completed";
            case 2:
                return "Pending Draw";
            default:
                return "Active";
        }
    }
    return status as "Active" | "Completed" | "Pending Draw";
};
