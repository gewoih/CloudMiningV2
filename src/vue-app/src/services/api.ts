﻿import axios, {AxiosInstance} from 'axios';

class ApiService {
    public axiosInstance: AxiosInstance;

    constructor() {
        const apiUrl = import.meta.env.VITE_API_URL;
        
        this.axiosInstance = axios.create({
            baseURL: `${apiUrl}/api/`,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        this.axiosInstance.interceptors.request.use(
            config => {
                const token = localStorage.getItem('access_token');
                if (token) {
                    config.headers.Authorization = `Bearer ${token}`;
                }
                return config;
            },
            error => {
                return Promise.reject(error);
            }
        );
    }
}

export const apiService = new ApiService();