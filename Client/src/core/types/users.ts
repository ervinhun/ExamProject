import {z} from "zod"

export const UserSchema = z.object({
    id: z.uuid().optional(),
    firstName: z.string(),
    lastName: z.string(),
    email: z.email(),
    roles: z.array(z.number()).optional(),
    createdAt: z.string().optional(),
    updatedAt: z.string().nullable().optional(),
});

export type User = z.infer<typeof UserSchema>;

export interface CreateUserDto {
    firstName: string,
    lastName: string,
    email: string,
    phoneNumber: string,
}

export interface AppliedUser {
    player: Player;
    id: string;
    status: string;
    age: number;
    createdAt: string;
    updatedAt: string;
    verifiedBy: string;
}

export const PlayerSchema = z.object({
    id: z.uuid().optional(),
    firstName: z.string(),
    lastName: z.string(),
    email: z.email(),
    isActive: z.boolean().default(true).optional(),
    phoneNumber: z.string(),
    createdAt: z.string().optional(),
    updatedAt: z.string().nullable().optional(),
});


export type Player = z.infer<typeof PlayerSchema>;