import { z } from "zod"

export const UserSchema = z.object({
    id: z.guid(),
    name: z.string(),
    email: z.email(),
    roles: z.array(z.number()),
    createdAt: z.string().optional(),
    updatedAt: z.string().optional(),
});

export type User = z.infer<typeof UserSchema>;

export interface CreateUserDto {
    fullName: string,
    email: string,
    phoneNo: string,
}
