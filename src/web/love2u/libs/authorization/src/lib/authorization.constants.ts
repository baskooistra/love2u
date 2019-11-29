export class AuthorizationConfiguration {
    readonly authority: string;
    readonly applicationName: string;
}

export const AuthorizationPaths: AuthorizationPathTypes = {
    CONFIGURATION_URL: '',
    LOGIN: "authentication/login",
    LOGIN_CALLBACK: "authentication/login-callback",
    LOGIN_FAILED: "authentication/login-failed",
    LOGOUT: "authentication/logout",
    LOGGED_OUT: "authentication/logged-out",
    LOGOUT_CALLBACK: "authentication/logout-callback",
    LOGOUT_FAILED: "authentication/logout-failed"
}

class AuthorizationPathTypes {
    readonly CONFIGURATION_URL: string;
    readonly LOGIN: string;
    readonly LOGIN_FAILED: string;
    readonly LOGIN_CALLBACK: string;
    readonly LOGOUT: string;
    readonly LOGOUT_FAILED: string;
    readonly LOGGED_OUT: string;
    readonly LOGOUT_CALLBACK: string;
}