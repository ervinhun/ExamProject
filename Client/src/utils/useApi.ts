// import { useAtom } from "jotai";
// import { authAtom } from "@core/atoms/Atom";

// export function useApi() {
//     const [auth, setAuth] = useAtom(authAtom);

//     // 🔥 Clean and consistent
//     function getAuth() {
//         return auth; // never null because Atom always has a valid shape
//     }

//     function isLoggedIn() {
//         return !!auth.token;
//     }

//     function getRole() {
//         return auth.role; // always an array
//     }

//     function resetAuth() {
//         setAuth({
//             name: null,
//             email: null,
//             role: [],
//             token: null,
//         });
//     }

//     // 🔥 Fully robust API fetch with silent token refresh
//     async function apiFetch(url: string, options: RequestInit  = {}) {
//         const res = await fetch(url, {
//             ...options,
//             credentials: "include",
//             headers: {
//                 "Authorization": auth.token ? `Bearer ${auth.token}` : "",
//                 ...options.headers,
//             },
//         });

//         // If request is NOT unauthorized, return as-is
//         if (res.status !== 401) return res;

//         // Try refreshing token
//         const refresh = await fetch("http://localhost:5152/api/auth/refresh", {
//             method: "POST",
//             credentials: "include",
//         });

//         // If refresh succeeds → update token + retry
//         if (refresh.ok) {
//             const data = await refresh.json();

//             setAuth(prev => ({
//                 ...prev,
//                 token: data.accessToken,
//             }));

//             return apiFetch(url, options);
//         }

//         // Refresh failed → logout completely
//         resetAuth();
//         return res;
//     }

//     return {
//         auth,
//         getAuth,
//         isLoggedIn,
//         getRole,
//         resetAuth,
//         apiFetch,
//     };
// }
