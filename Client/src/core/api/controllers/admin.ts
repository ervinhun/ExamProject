import { api } from "@core/api/Api";
import type { CreateUserDto, User } from "@core/types/users";

const endpoint = "/api/admin";

export const adminApi = {
    registerAdmin: (data: CreateUserDto): Promise<void> => {
        return api<void>(`${endpoint}/register`, {
            init: {
                method: "POST",
                body: JSON.stringify(data),
            },
        });
    },
    getAllAdmins: (): Promise<User[]> => {
        return api<User[]>(`${endpoint}/list-admins`, {
            init: {
                method: "GET",
            },
        });
    },
}
