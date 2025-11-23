import * as z from 'zod'

export async function api<T>(
    url: string,
    options: {
        schema: z.ZodType<T>,
        init: RequestInit
    }
): Promise<T> {
    const {schema, init} = options || {};
    console.log(schema, init);

    // return await fetch(url, init) as T;
    const response = await fetch(url, {
        credentials: "include",
        headers: {
            "content-type": "application/json",
            ...init?.headers
        },
        ...init
    });

    if(!response.ok){
        const error = await response.json().catch(()=>({}))
        throw new Error(error.message || error.errors || `HTTP ${response.status}`)
    }

    const data = await response.json();

    console.log(response);
    return schema.parse(data);

}
