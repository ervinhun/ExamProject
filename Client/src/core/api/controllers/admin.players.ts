import {AppliedUser} from "@core/types/users.ts";

const endpoint = "/api/users";

// Player applications

import {api} from "@core/api/Api.ts";

export const appliedUsersApi = {
    getAppliedUsersList: (): Promise<AppliedUser[]> => {
        return api<AppliedUser[]>(`${endpoint}/get-applied-users`, {
            init: {
                method: "GET"
            }
        });
    }
}