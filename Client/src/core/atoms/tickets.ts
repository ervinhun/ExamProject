import { ticketApi } from "@core/api/controllers/ticket";
import {MyTicketDto, TicketSubscriptionDto} from "@core/types/ticket.ts";
import {atom} from "jotai";


export const myTicketsAtom = atom<MyTicketDto[]>([]);
myTicketsAtom.debugLabel = "My Tickets";

export const mySubscribedTicketsAtom = atom<TicketSubscriptionDto[]>([]);
mySubscribedTicketsAtom.debugLabel = "My Subscribed Tickets";

export const fetchTicketsForPlayerAtom = atom(
    null,
    async (get, set) => {
        // Fetch tickets for the player
        const tickets = await ticketApi.fetchAllMyTickets();
        set(myTicketsAtom, tickets);
    }
);
fetchTicketsForPlayerAtom.debugLabel = "Fetch Tickets For Player";

export const fetchSubscribedTicketsForPlayerAtom = atom(
    null,
    async (get, set) => {
        const subscriptions = await ticketApi.getMySubscribedTickets();
        set(mySubscribedTicketsAtom, subscriptions);
    }
);
fetchSubscribedTicketsForPlayerAtom.debugLabel = "Fetch Subscribed Tickets For Player";