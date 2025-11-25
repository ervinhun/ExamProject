
// interface ViteTypeOptions {
//   // By adding this line, you can make the type of ImportMetaEnv strict
//   // to disallow unknown keys.
//   // strictImportMetaEnv: unknown
// }


interface ImportMetaEnv {

  readonly VITE_API_PORT: string

  readonly VITE_API_HOST: string

  readonly VITE_ADMIN: string

  readonly VITE_PLAYER: string

  readonly VITE_SUPERADMIN: string

  readonly VITE_ENVIRONMENT: string

  readonly VITE_SECURE: string

  readonly VITE_CLIENT_PORT: string

}

interface ImportMeta {
  readonly env: ImportMetaEnv
}