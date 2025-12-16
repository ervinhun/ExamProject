import { ticketApi } from "@core/api/controllers/ticket";
import {MyTicketDto} from "@core/types/ticket.ts";
import {atom} from "jotai";


export const myTicketsAtom = atom<MyTicketDto[]>([]);
myTicketsAtom.debugLabel = "My Tickets";

export const fetchTicketsForPlayerAtom = atom(
    null,
    async (get, set) => {
        // Fetch tickets for the player
        const tickets = await ticketApi.fetchAllMyTickets();
        set(myTicketsAtom, tickets);
    }
);
fetchTicketsForPlayerAtom.debugLabel = "Fetch Tickets For Player";