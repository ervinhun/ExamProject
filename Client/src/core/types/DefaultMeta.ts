import * as z from 'zod'
export const DefaultMeta = z.object({
    pagination: z
        .object({
            page: z.number().optional(),
            limit: z.number().optional(),
            total: z.number().optional(),
            pages: z.number().optional(),
            hasNext: z.boolean().optional(),
            hasPrev: z.boolean().optional(),
        })
        .optional(),

    timestamp: z.iso.datetime(),
    requestId: z.uuid(),
    rateLimit: z
        .object({
            limit: z.number(),
            remaining: z.number(),
            reset: z.number(),
        })
        .optional(),
})

export type DefaultMeta = z.infer<typeof DefaultMeta>;