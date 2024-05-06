import {ShareablePayment} from '@/models/ShareablePayment';
import {PaymentType} from '@/enums/PaymentType';
import {apiService} from '@/services/api.ts';

class PaymentsService {
    async createPayment(paymentData: ShareablePayment) {
        return apiService.axiosInstance.post("/payments", paymentData)
    }

    async getPayments(skip: number, paymentType: PaymentType): Promise<ShareablePayment[]> {
        skip = (skip-1)*10;
        const response = await apiService.axiosInstance
            .get("/payments", {params: {paymentType, skip}});
        
        return response.data;
    }
}

export const paymentsService = new PaymentsService();