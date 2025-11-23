import * as z from 'zod';
import {uint32} from "zod";


const envSchema = z.object({
    apiUrl:z.url(),
    apiPort:uint32(),
    environment: z.string(),

})

type Env = z.infer<typeof envSchema>;
class EnvConfig{
    private static _instance:EnvConfig;
    protected _env: Env;
    private constructor(){
        this._env = this.validateEnv();
    }

    private validateEnv(): Env{
        try{
            return envSchema.parse(import.meta.env);
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

    public get api_port() { return this._env.apiPort}
    public get api_host() { return this._env.apiUrl}


    // Environments
    public get isDev() { return this._env.environment === "development"; }
    public get isProd() { return this._env.environment === "production"; }
    public get isTest() { return this._env.environment === "test"; }

}

export const envConfig = EnvConfig.getInstance();