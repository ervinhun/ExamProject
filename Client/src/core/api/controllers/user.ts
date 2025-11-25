import { api } from "../Api";
import { UserSchema, type CreateUserDto, type User } from "@core/types/users";

const endpoint = "/api/users";

export async function fetchAllUsers(): Promise<User[]> {
    return await api<User[]>(`${endpoint}/all`, {
    
        init: {
            method: "GET"
        }
    });
}

export async function fetchUserById(userId: string): Promise<User> {
    return await api<User>(`${endpoint}/${userId}`, {
        schema:UserSchema,
        init: {
            method: "GET"
        }
    });
}

export async function createUser(userDto: CreateUserDto): Promise<User>{
    return await api<User>(`${endpoint}/create`, {
        schema:UserSchema,
        init: {
            method:"POST"
        }
    })
}


//createUser
//deleteUserById
//updateUserById
