import * as z from 'zod'
import { getDefaultStore } from 'jotai/vanilla'
import { authAtom } from '../atoms/auth'

// Track if a refresh is already in progress to avoid race conditions
let isRefreshing = false;
let refreshPromise: Promise<boolean> | null = null;

async function refreshToken(): Promise<boolean> {
    // If already refreshing, wait for that to complete
    if (isRefreshing && refreshPromise) {
        console.log('[RefreshToken] Already refreshing, waiting...');
        return refreshPromise;
    }

    // Get user ID from auth atom or localStorage
    let userId = '';
    try {
        const store = getDefaultStore()
        const auth = store.get(authAtom)
        userId = auth?.id || ''
    } catch {
        try {
            const authData = localStorage.getItem('auth')
            if (authData) {
                const parsed = JSON.parse(authData)
                userId = parsed?.id || ''
            }
        } catch {
            console.error('[RefreshToken] Could not get user ID');
            return false;
        }
    }

    if (!userId) {
        console.error('[RefreshToken] No user ID found');
        return false;
    }

    console.log('[RefreshToken] Starting token refresh for user:', userId);
    isRefreshing = true;

    refreshPromise = fetch('/api/auth/refresh-token', {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userId: userId })
    })
    .then(async (response) => {
        console.log('[RefreshToken] Response status:', response.status);
        
        if (response.ok) {
            console.log('[RefreshToken] Token refreshed successfully');
            return true;
        }
        
        // Log the error response
        const errorText = await response.text().catch(() => 'Unable to read error');
        console.error('[RefreshToken] Refresh failed:', response.status, errorText);
        return false;
    })
    .catch((error) => {
        console.error('[RefreshToken] Network error:', error);
        return false;
    })
    .finally(() => {
        isRefreshing = false;
        refreshPromise = null;
    });

    return refreshPromise;
}

export async function api<T>(
    url: string,
    options: {
        schema?: z.ZodType<T>,
        init: RequestInit,
        skipRetry?: boolean // Flag to prevent infinite retry loops
    }
): Promise<T> {
    const {schema, init, skipRetry = false} = options || {};
    console.log(schema, init);

    const headers: Record<string, string> = {
        ...init?.headers
    } as Record<string, string>
    
    // Only add Content-Type if there's a body
    if (init?.body) {
        headers["Content-Type"] = "application/json";
    }
    
    if (headers["authorization"]) {
        throw new Error("Authorization header not allowed - using httpOnly cookies");
    }

    const response = await fetch(url, {
        credentials: "include", // Important: sends httpOnly cookies
        headers,
        ...init
    });

    // Handle 401 Unauthorized - try to refresh token
    if (response.status === 401 && !skipRetry) {
        console.log('[API] Got 401, attempting token refresh for:', url);
        const refreshed = await refreshToken();
        
        if (refreshed) {
            console.log('[API] Token refreshed, retrying original request:', url);
            // Retry the original request with new token
            return api<T>(url, {
                ...options,
                skipRetry: true // Prevent infinite retry loop
            });
        } else {
            console.error('[API] Token refresh failed, session expired');
            // Refresh failed - user needs to log in again
            // Optionally: dispatch logout event or redirect to login
            throw new Error("Session expired. Please log in again.");
        }
    }

    if(!response.ok){
        const error = await response.json().catch((err) => {
            throw new Error(err.message || `HTTP ${response.status}`);
        });
        throw new Error(error.message || error.errors || `HTTP ${response.status}`)
    }

    const data = await response.json();

    if(!schema){
        return data as T;
    }
    return schema.parse(data);
}
