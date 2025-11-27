/// <reference types="vite/client" />
import * as z from 'zod'
import { getDefaultStore } from 'jotai/vanilla'
import { authAtom } from '../atoms/auth'

export async function api<T>(
    url: string,
    options: {
        schema?: z.ZodType<T>,
        init: RequestInit
    }
): Promise<T> {
    const {schema, init} = options || {};
    console.log(schema, init);

    let token = ''
    // try {
    //     const store = getDefaultStore()
    //     const auth = store.get(authAtom)
    //     token = auth?.token || ''
    // } catch {
    //     // fallback if store not initialized
    //     try {
    //         const authData = localStorage.getItem('auth')
    //         if (authData) {
    //             const parsed = JSON.parse(authData)
    //             token = parsed?.token || ''
    //         }
    //     } catch {
    //         token = ''
    //     }
    // }

    const headers = {
        "Content-Type": "application/json",
        ...init?.headers
    } as Record<string, string>
    
    if (token) {
        headers["authorization"] = `Bearer ${token}`;
    }

    const  response = await fetch(url, {
        credentials: "include",
        headers,
        ...init
    });

    if(!response.ok){
        const error = await response.json().catch(()=>({}))
        throw new Error(error.message || error.errors || `HTTP ${response.status}`)
    }

    const data = await response.json();

    if(!schema){
        return data as T;
    }
    return schema.parse(data);

}
