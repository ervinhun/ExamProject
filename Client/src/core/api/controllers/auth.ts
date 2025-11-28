import type { AuthResponseDto, LoginRequestDto } from "../../types/auth"
import { api } from "../Api";
import { type User } from "../../types/users";

const endpoint = "/api/auth";

export const authApi = {
    login: async (loginRequest: LoginRequestDto): Promise<AuthResponseDto> => {
        return await api<{ user: User }>(`${endpoint}/login`, {
            init: {
                method: "POST",
                body: JSON.stringify(loginRequest)
            }
        }).then((raw) => {
            const response: AuthResponseDto = {
                user: raw.user ?? null
            };
            return response;
        }).catch((err) => {
            throw err;
        });
    },

    logout: async (): Promise<void> => {
        return await api<void>(`${endpoint}/logout`, {
                init: {
                    method: "POST"
                }
            }).catch((err) => { err.message; throw err; });
    },

    profile: async (): Promise<User> => {
        return await api<User>(`${endpoint}/profile`, {
                init: {
                    method: "GET"
                }
            }).catch((err) => { err.message; throw err; });
    }
};
