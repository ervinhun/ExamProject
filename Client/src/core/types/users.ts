import { z } from "zod"

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
