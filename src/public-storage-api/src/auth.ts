function hasAdminHeader(request: Request, env: Env): boolean {
    var adminToken = request.headers.get('x-admin-token');

    if (adminToken === null || adminToken === undefined) {
        return false;
    }

    if (adminToken.toLowerCase() === env.ADMIN_TOKEN.toLowerCase()) {
        return true;
    }

    return false;
}

export default {
    isAdmin(request: Request, env: Env): boolean {
        return hasAdminHeader(request, env);
    },

    async canWriteFileAsync(request: Request, env: Env, scope: string): Promise<boolean> {
        return hasAdminHeader(request, env);
    },
};

