import { api } from "../Api";
import { UserSchema, type CreateUserDto, type User } from "../../types/users";

const endpoint = "/api/users";

export const userApi ={
    getAll: async (): Promise<User[]> => {
        return await api<User[]>(`${endpoint}/all`, {
            init: {
                method: "GET"
            }
        });
    },
    getById: async (id: string): Promise<User> => {
        return await api<User>(`${endpoint}/${id}`, {
            init: {
                method: "GET"
            }
        });
    },
    createUser: async (userDto: CreateUserDto): Promise<User> => {
        return await api<User>(`${endpoint}/create`, {
            // schema:UserSchema,
            init: {
                method:"POST",
                body: JSON.stringify(userDto)
            }
        })
    }
}


//createUser
//deleteUserById
//updateUserById
