import {z} from "zod";
import { GameTemplateDto } from "./game";

export const MyTicketSchema = z.object({
    id: z.string(),
    gameInstanceId: z.string(),
    playerId: z.string(),
    fullPrice: z.number(),
    player: z.object({
        id: z.string(),
        firstName: z.string(),
        lastName: z.string(),
        email: z.email(),
    }).optional(),
    pickedNumbers: z.array(z.number()),
    isWinning: z.boolean().nullable(),
    boughtAt: z.string(),
});

export type MyTicketDto = z.infer<typeof MyTicketSchema>;

export interface PurchaseTicketDto {
    gameInstanceId: string;
    playerId: string;
    walletId: string;
    fullPrice : number;
    pickedNumbers: number[];
}

export interface TicketSubscriptionDto {
    gameTemplateId: string;
    playerId: string;
    pickedNumbers: number[];
    isExpired?: boolean;
    price: number;
    gameTemplate?: GameTemplateDto;
}