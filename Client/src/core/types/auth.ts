import type { User } from "./users";

export interface JwtResponse {
  token: string;
  refreshToken: string;
  claims: []
}

export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

// Keep naming consistent with backend DTO names AccessToken = accessToken
export interface AuthResponseDto {
  accessToken: string | null;
  user: User | null;
}

export type AuthUser = {
    id: string | null;
    name: string | null;
    email: string | null;
    roles: number[];
    token: string | null;
};