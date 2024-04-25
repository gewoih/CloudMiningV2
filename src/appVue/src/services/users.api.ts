import axios, {AxiosInstance} from 'axios';
import {RegisterUser} from '@/models/RegisterUser.ts';
import {LoginUser} from "@/models/LoginUser.ts";
import router from "@/router.ts";

export default class UsersService {
    private axiosInstance: AxiosInstance;

    constructor() {
        this.axiosInstance = axios.create({
            baseURL: 'https://localhost:5000/api',
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }
    
    async createUser(userData: RegisterUser) {
        return this.axiosInstance.post("/users", userData);
    }

    async loginUser(userData: LoginUser) {
        const response = await this.axiosInstance.post("/users/login", userData);
        const token = response.data;

        localStorage.setItem('access_token', token);
        await router.push({ name: 'Registration' });
    }
}

export const usersService = new UsersService();