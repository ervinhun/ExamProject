import { envConfig } from "@core/config/EnvConfig.ts";
import RequireRole from "./RequireRole.tsx";

export default function RequirePlayer() {
    return <RequireRole roles={[Number(envConfig.PLAYER_ROLE)]}/>
}
