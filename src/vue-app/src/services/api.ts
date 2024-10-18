import axios, {AxiosInstance} from 'axios';

class ApiService {
    public axiosInstance: AxiosInstance;

    constructor() {
        const API_URL = import.meta.env.VITE_APP_API_URL;
        
        this.axiosInstance = axios.create({
            baseURL: `${API_URL}/api`,
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