import { api } from "../Api";
import {AppliedUser, type CreateUserDto, type User} from "../../types/users";

const endpoint = "/api/users";

export const userApi ={
    getAll: async (): Promise<User[]> => {
        return await api<User[]>(`${endpoint}/all-users`, {
            init: {
                method: "GET"
            }
        });
    },
    getById: async (id: string): Promise<User> => {
        return await api<User>(`${endpoint}/get-user/${id}`, {
            init: {
                method: "GET"
            }
        });
    },
    createUser: async (userDto: CreateUserDto): Promise<User> => {
        return await api<User>(`${endpoint}/register-user`, {
            // schema:UserSchema,
            init: {
                method:"POST",
                body: JSON.stringify(userDto)
            }
        })
    },
    toggleStatus: async (id: string): Promise<User> => {
        return await api<User>(`${endpoint}/toggle-status/${id}`, {
            init: {
                method: "PATCH"
            }
        });
    },
    getAllAppliedUsers: async (): Promise<AppliedUser[]> => {
        return await api<AppliedUser[]>(`${endpoint}/get-applied-users`, {
            init: {
                method: "GET"
            }
        });
    },

    confirmAppliedUsers: async (userId:string, isApproved:boolean, isActive:boolean): Promise<boolean> => {
        console.log(`URL which is sent: ${endpoint}/approve-user/${userId}?isApproved=${isApproved}&isActive=${isActive}`)
        return await api<boolean>(
            `${endpoint}/approve-user/${userId}?isApproved=${isApproved}&isActive=${isActive}`, {
                init: {
                    method: "PUT"
                }
            });
    }

}


//createUser
//deleteUserById
//updateUserById
