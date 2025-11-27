import type { AuthResponseDto, LoginRequestDto } from "../../types/auth"
import { api } from "../Api";
import { type User } from "../../types/users";

const endpoint = "/api/auth";
export const loginRequest = async (loginRequest: LoginRequestDto): Promise<AuthResponseDto> =>{
        const raw = await api<{ accessToken?: string | null; user: User }>(`${endpoint}/login`, {
                        init: {
                            method: "POST",
                            body: JSON.stringify(loginRequest)
                        }
                    });

                const response: AuthResponseDto = {
                    accessToken: raw.accessToken ?? null,
                    user: raw.user ?? null
                };
                return response;
}

export const logoutRequest = async (): Promise<void> => {
    try{
        return await api<void>(`${endpoint}/logout`, {
                init: {
                    method: "POST"
                }
            });
    }catch(err){
        // throw err;
        console.error("Logout error:", err);
        return;
    }
}


export const meRequest = async (): Promise<AuthResponseDto> => {
    try{
        const raw = await api<{accessToken?: string | null; user: User}>(`${endpoint}/me`, {
                init: {
                    method: "GET"
                }
            });
        const response: AuthResponseDto = {
            accessToken: raw.accessToken ?? null,
            user: raw.user ?? null
        };
        return response;
    }catch(err){
        console.error("Me error:", err);
        return {
            accessToken: null,
            user: null
        };
    }
}