import {ShareablePayment} from '@/models/ShareablePayment';
import {PaymentType} from '@/enums/PaymentType';
import {apiService} from '@/services/api.ts';

class PaymentsService {
    async createPayment(paymentData: ShareablePayment) {
        return apiService.axiosInstance.post("/api/payments", paymentData)
    }

    async getPayments(paymentType: PaymentType): Promise<ShareablePayment[]> {
        const response = await apiService.axiosInstance
            .get("/api/payments", {params: {paymentType}});
        
        return response.data;
    }
}

export const paymentsService = new PaymentsService();