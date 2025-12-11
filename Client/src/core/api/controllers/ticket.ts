import {api} from "@core/api/Api.ts";
import * as MyTicket from "@core/types/ticket.ts";

const endpoint = "/api/tickets";

export const ticketApi = {
    playTicket: async (template: MyTicket.CreateTicketToGameDto): Promise<MyTicket.MyTicketDto> => {
        return await api<MyTicket.MyTicketDto>(`${endpoint}/create-ticket`, {
            schema: MyTicket.MyTicketDtoSchema,
            init: {
                method: "POST",
                body: JSON.stringify(template)
            }
        });
    },

    getAllActiveTickets: async (): Promise<MyTicket.MyTicketDto[]> => {
        return await api<MyTicket.MyTicketDto[]>(`${endpoint}/all-my-tickets`, {
            init: {
                method: "GET"
            }
        });
    },

    getTicketsForGameInstance: async (gameInstanceId: string, isWinning: boolean | null): Promise<MyTicket.MyTicketDto[]> =>
    {
        let url = `${endpoint}/tickets-for-game/${gameInstanceId}`;

        if (isWinning !== undefined) {
            url += `?winning=${isWinning}`;
        }

        return await api<MyTicket.MyTicketDto[]>(url, {
            init: {method: "GET"}
        });
    }
}