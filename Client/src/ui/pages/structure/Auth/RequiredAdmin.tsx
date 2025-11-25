import RequireRole from "./RequireRole.tsx";
import { envConfig } from "@core/config/EnvConfig";
export default function RequireAdmin() {

    return <RequireRole roles={[Number(envConfig.ADMIN_ROLE), Number(envConfig.SUPERADMIN_ROLE)]}/>
}
