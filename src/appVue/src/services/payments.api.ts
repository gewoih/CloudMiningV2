import axios, { AxiosInstance } from 'axios';
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
    
      async createPayment(paymentData: ShareablePayment) {
        return this.axiosInstance.post("/api/payments", paymentData)
      }

      async getPayments(paymentType: PaymentType): Promise<ShareablePayment[]> {
          const response = await this.axiosInstance.get("/Payments",{ data: { paymentType } });
        return response.data;
      }
}

export const shareablePaymentsService = new ShareablePaymentsService();