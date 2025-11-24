import RequireRole from "./RequireRole.tsx";

export default function RequireAdmin() {

    return <RequireRole roles={["Admin", "SuperAdmin"]}/>
}
