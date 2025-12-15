import {z} from "zod";

export const MyTicketSchema = z.object({
    id: z.string(),
    gameInstanceId: z.string(),
    playerId: z.string(),
    fullPrice: z.number(),
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

