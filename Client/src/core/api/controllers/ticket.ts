import {api} from "@core/api/Api.ts";
import { MyTicketDto, PurchaseTicketDto } from "@core/types/ticket.ts";

const endpoint = "/api/tickets";

export const ticketApi = {
    purchaseTicket: async (template: PurchaseTicketDto): Promise<void> => {
        await api<PurchaseTicketDto>(`${endpoint}/purchase-ticket`, {
            init: {
                method: "POST",
                body: JSON.stringify(template)
            }
        });
    },

    fetchAllMyTickets: async (): Promise<MyTicketDto[]> => {
        return await api<MyTicketDto[]>(`${endpoint}/all-my-tickets`, {
            init: {
                method: "GET"
            }
        });
    },

    // getAllActiveTickets: async (): Promise<MyTicket.MyTicketDto[]> => {
    //     return await api<MyTicket.MyTicketDto[]>(`${endpoint}/all-my-tickets`, {
    //         init: {
    //             method: "GET"
    //         }
    //     });
    // },

    // getTicketsForGameInstance: async (gameInstanceId: string, isWinning: boolean | null): Promise<MyTicket.MyTicketDto[]> =>
    // {
    //     let url = `${endpoint}/tickets-for-game/${gameInstanceId}`;

    //     if (isWinning !== undefined) {
    //         url += `?winning=${isWinning}`;
    //     }

    //     return await api<MyTicket.MyTicketDto[]>(url, {
    //         init: {method: "GET"}
    //     });
    // }
}