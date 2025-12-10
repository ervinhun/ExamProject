import {z} from "zod";

export const CreateTicketToGameDtoSchema = z.object({
    gameInstanceId: z.string(),
    selectedNumbers: z.array(z.number().positive()),
    repeat: z.number()
});

export type CreateTicketToGameDto = z.infer<typeof CreateTicketToGameDtoSchema>;

export const TicketResultDtoSchema = z.object({
    gameInstanceId: z.string(),
    numbers: z.array(z.number()),
    matchedNumbers: z.array(z.number()).optional(),
    isWinning: z.boolean().optional(),
    prizeWon: z.number().optional(),
    drawDate: z.string().or(z.date()),
});

export type TicketResultDto = z.infer<typeof TicketResultDtoSchema>;

export const MyTicketDtoSchema = z.object({
    id: z.string().optional(),
    playerId: z.string(),
    gameInstanceId: z.string(),
    selectedNumbers: z.array(z.number()),
    repeat: z.number().optional(),
    results: z.array(TicketResultDtoSchema).optional(),
});

export type MyTicketDto = z.infer<typeof MyTicketDtoSchema>;




