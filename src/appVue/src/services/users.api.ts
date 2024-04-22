import axios, { AxiosInstance, AxiosRequestConfig } from 'axios';
import { RegisterUser } from '@/models/RegisterUser';

export default class UsersService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: 'https://localhost:5000/api',
      headers: {
        'Content-Type': 'application/json'
      }
    })
  }

  private async axiosCall<T>(config: AxiosRequestConfig) {
    return await this.axiosInstance.request<T>(config);
  }

  async createUser(userData: RegisterUser) {
    return this.axiosCall<string>({ method: "post", url: "/users", data: userData })
  }
}

export const usersService = new UsersService();