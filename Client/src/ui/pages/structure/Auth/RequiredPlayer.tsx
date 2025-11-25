import { envConfig } from "@core/config/EnvConfig";
import RequireRole from "./RequireRole.tsx";

export default function RequirePlayer() {
    return <RequireRole roles={[Number(envConfig.PLAYER_ROLE)]}/>
}
