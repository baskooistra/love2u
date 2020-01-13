import { environment } from '../environments/environment';

export const ApplicationUrls: ApplicationUrlTypes = {
    IDENTITY_PROVIDER: environment.identityProviderUrl      
}

class ApplicationUrlTypes {
    readonly IDENTITY_PROVIDER: string;    
}