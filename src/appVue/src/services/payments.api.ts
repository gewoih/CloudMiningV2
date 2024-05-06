import {ShareablePayment} from '@/models/ShareablePayment';
import {PaymentType} from '@/enums/PaymentType';
import {apiService} from '@/services/api.ts';
import {ShareablePaymentList} from "@/models/ShareablePaymentList.ts";

class PaymentsService {
    async createPayment(paymentData: ShareablePayment) {
        return apiService.axiosInstance.post("/payments", paymentData)
    }

    async getPayments(skip: number, paymentType: PaymentType): Promise<ShareablePaymentList> {
        skip = (skip-1)*10;
        const response = await apiService.axiosInstance
            .get("/payments", {params: {paymentType, skip}});
        
        return response.data;
    }
}

export const paymentsService = new PaymentsService();