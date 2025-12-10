import {MyTicketDto} from "@core/types/ticket.ts";
import {atom} from "jotai";


export const myTicketsAtom = atom<MyTicketDto[]>([]);