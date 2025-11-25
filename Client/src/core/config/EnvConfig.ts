import * as z from 'zod';


const envSchema = z.object({
    VITE_API_PORT:z .string().default("5152"),
    VITE_API_HOST: z.string(),
    VITE_ADMIN: z.string(),
    VITE_PLAYER: z.string(),
    VITE_SUPERADMIN: z.string(),
    VITE_ENVIRONMENT: z.string().default("development"),
    VITE_SECURE: z.string().default("true"),
    VITE_CLITEN_PORT: z.string().default("5173"),

})

type Env = z.infer<typeof envSchema>;

class EnvConfig{
    private static _instance: EnvConfig;
    protected _env: Env;
    private envImport: ImportMetaEnv = import.meta.env

    private constructor(){
        this._env = this.validateEnv();
    }

    private validateEnv(): Env{
        try{
            // console.log(envSchema.parse(this.envImport));
            
            return envSchema.parse(this.envImport)
        }catch (e){
            if(e instanceof z.ZodError){
                throw new Error(`ZodError: ${e}`);
            }
            throw e;
        }
    }

    public static getInstance(): EnvConfig {
        if (!EnvConfig._instance) {
            EnvConfig._instance = new EnvConfig();
        }
        return EnvConfig._instance;
    }

    public get API_HOST() { return this._env.VITE_API_HOST }
    public get ADMIN_ROLE() { return this._env.VITE_ADMIN }
    public get PLAYER_ROLE() { return this._env.VITE_PLAYER }
    public get SUPERADMIN_ROLE() { return this._env.VITE_SUPERADMIN }
    public get SECURE() { return this._env.VITE_SECURE }
    public get CLITEN_PORT() { return Number.parseInt(this._env.VITE_CLITEN_PORT) }
    public get ENVIRONMENT() { return this._env.VITE_ENVIRONMENT }
    public get API_PORT() { return Number.parseInt(this._env.VITE_API_PORT) }

    // public get isProd() { return this._env.environment === "production"; }
    // public get isTest() { return this._env.environment === "test"; }

}

export const envConfig = EnvConfig.getInstance();
