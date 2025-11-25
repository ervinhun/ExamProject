import { Navigate, Outlet, useLocation } from "react-router-dom";
// import { useApi } from "../../../../utils/useApi";
import { authAtom } from "@core/atoms/auth";
import { useAtom } from "jotai";
import { isLoggedInAtom } from "@core/atoms/auth";

interface RequireRoleProps {
    roles: number[];
}

export default function RequireRole({ roles }: Readonly<RequireRoleProps>) {
    const [authUser, _] = useAtom(authAtom);
    const [isLoggedIn, ] = useAtom(isLoggedInAtom);
    
    const location = useLocation();

    // If not logged in at all
    if (!isLoggedIn) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    // If logged in but does not have the required role
    const userRoles = authUser.roles || [];
    console.log("RequireRole userRoles:", userRoles, "required roles:", roles);
    const hasAccess = roles.some(r => {
        return userRoles.includes(r);
    });


    if (!hasAccess) {
        return <Navigate to="/403" replace />;
    }

    // User is allowed
    return <Outlet />;
}
