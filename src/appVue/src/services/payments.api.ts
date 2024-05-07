
import {PaymentType} from '@/enums/PaymentType';
import {apiService} from '@/services/api.ts';
import {ShareablePaymentList} from "@/models/ShareablePaymentList.ts";
import {PaymentShare} from "@/models/PaymentShare.ts";
import {CreatePayment} from "@/models/CreatePayment.ts";

class PaymentsService {
    async createPayment(paymentData: CreatePayment) {
        return apiService.axiosInstance.post("/payments", paymentData)
    }

    async getPayments(skip: number, paymentType: PaymentType): Promise<ShareablePaymentList> {
        skip = (skip-1)*10;
        const response = await apiService.axiosInstance
            .get("/payments", {params: {paymentType, skip}});
        
        return response.data;
    }
    async getShares(paymentId: string): Promise<PaymentShare[]> {
        const response = await apiService.axiosInstance
            .get("/payments/shares", {params: {paymentId}});

        return response.data;
    }
}

export const paymentsService = new PaymentsService();