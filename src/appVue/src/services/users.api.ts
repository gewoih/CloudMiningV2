import {RegisterUser} from '@/models/RegisterUser.ts';
import {LoginUser} from "@/models/LoginUser.ts";
import router from "@/router.ts";
import {apiService} from "@/services/api.ts";

class UsersService {
    async createUser(userData: RegisterUser) {
        return apiService.axiosInstance.post("/users", userData);
    }

    async loginUser(userData: LoginUser) {
        const response = await apiService.axiosInstance.post("/users/login", userData);
        const token = response.data;

        localStorage.setItem('access_token', token);
        await router.push({name: 'home'});
        return token;
    }
}

export const usersService = new UsersService();