import axios, { AxiosInstance } from 'axios';
import { RegisterUser } from '@/models/RegisterUser';
import {LoginUser} from "@/models/LoginUser.ts";

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

  async createUser(userData: RegisterUser) {
    return this.axiosInstance.post("/users", userData);
  }
  
  async loginUser(userData: LoginUser){
    return this.axiosInstance.post("/users/login", userData);
  }
}

export const usersService = new UsersService();