import {api} from "@core/api/Api.ts";
import * as MyTicket from "@core/types/ticket.ts";

const endpoint = "/api/tickets";

export const ticketApi = {
    playTicket: async (template: MyTicket.CreateTicketToGameDto): Promise<MyTicket.MyTicketDto> => {
        return await api<MyTicket.MyTicketDto>(`${endpoint}/api/games/createTicket`, {
            schema: MyTicket.MyTicketDtoSchema,
            init: {
                method: "POST",
                body: JSON.stringify(template)
            }
        });
    },

    getAllActiveTickets: async (): Promise<MyTicket.MyTicketDto[]> => {
        return await api<MyTicket.MyTicketDto[]>(`${endpoint}/api/games/all-my-tickets`, {
            init: {
                method: "GET"
            }
        });
    }
}