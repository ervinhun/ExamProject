export type AuthUser = {
    name: string | null;
    email: string | null;
    role: string[];
    token: string | null;
};
