import { defineStore } from 'pinia';
import { UserRole } from "@/enums/UserRole.ts";

export const useUserStore = defineStore('user', {
    state: () => {
        const token = localStorage.getItem('access_token');
        const role = token ? getUserRoleFromToken(token) : UserRole.User;

        return {
            token: token,
            role: role,
        };
    },
    actions: {
        setToken(newToken: string | null) {
            if (newToken) {
                localStorage.setItem('access_token', newToken);
                this.token = newToken;
                this.role = getUserRoleFromToken(newToken);
            } else {
                localStorage.removeItem('access_token');
                this.token = null;
                this.role = null;
            }
        }
    },
    getters: {
        isAdmin: (state) => state.role === UserRole.Admin,
        isAuthenticated: (state) => !!state.token
    },
});

const getUserRoleFromToken = (token: string) => {
    try {
        const decodedJwt = JSON.parse(atob(token.split('.')[1]));
        return decodedJwt["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || UserRole.User;
    } catch (error) {
        console.error("Failed to decode JWT:", error);
        return UserRole.User;
    }
};