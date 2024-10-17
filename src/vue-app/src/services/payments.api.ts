import {PaymentType} from '@/enums/PaymentType';
import {apiService} from '@/services/api.ts';
import {PaymentList} from "@/models/PaymentList.ts";
import {PaymentShare} from "@/models/PaymentShare.ts";
import {CreatePayment} from "@/models/CreatePayment.ts";

class PaymentsService {
    async createPayment(paymentData: CreatePayment) {
        const response = await apiService.axiosInstance
            .post("/payments", paymentData);
        return response.data;
    }

    async getPayments(skip: number, take: number, paymentType: PaymentType): Promise<PaymentList> {
        skip = (skip - 1) * take;
        const response = await apiService.axiosInstance
            .get("/payments", {params: {paymentType, skip, take}});

        return response.data;
    }

    async getShares(paymentId: string): Promise<PaymentShare[]> {
        const response = await apiService.axiosInstance
            .get("/payments/shares", {params: {paymentId}});

        return response.data;
    }

    async switchPaymentStatus(shareId: string) {
        return apiService.axiosInstance.patch("/payments/status", shareId)
    }
}

export const paymentsService = new PaymentsService();