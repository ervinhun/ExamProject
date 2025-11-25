import { api } from "../Api";
import type { User } from "@core/types/users";

const endpoint = "api/users";

export async function fetchAllUsers(): Promise<User[]> {
    return await api<User[]>(`${endpoint}/all`, {
        init: {
            method: "GET"
        }
    });
}

export async function fetchUserById(userId: string): Promise<User> {
    return await api<User>(`${endpoint}/${userId}`, {
        init: {
            method: "GET"
        }
    });
}