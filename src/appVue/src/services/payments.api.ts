import axios, { AxiosInstance, AxiosRequestConfig } from 'axios';
import { ShareablePayment } from '@/models/ShareablePayment';
import { PaymentType } from '@/enums/PaymentType';

export default class ShareablePaymentsService{
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
    
      async createPayment(paymentData: ShareablePayment) {
        return this.axiosCall<string>({ method: "post", url: "/api/payments", data: paymentData })
      }
      async getPayments(paymentType: PaymentType): Promise<ShareablePayment[]> {
        try {
          const response = await this.axiosCall<ShareablePayment[]>({ method: "get", url: "/api/payments", params: {paymentType} });
          return response.data;
        } catch (error) {
          console.error('Ошибка при получении платежей:', error);
          return [];
        }
      }
}

export const shareablePaymentsService = new ShareablePaymentsService();