import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useApi } from "../../../../utils/useApi";

interface RequireRoleProps {
    roles: string[];
}

export default function RequireRole({ roles }: Readonly<RequireRoleProps>) {
    const { getAuth } = useApi();
    const location = useLocation();

    // If not logged in at all
    if (!getAuth().token) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    // If logged in but does not have the required role
    const userRoles = getAuth().role || [];
    const hasAccess = roles.some(r => userRoles.includes(r));

    if (!hasAccess) {
        return <Navigate to="/403" replace />;
    }

    // User is allowed
    return <Outlet />;
}
