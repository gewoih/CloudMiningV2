
import {PaymentType} from '@/enums/PaymentType';
import {apiService} from '@/services/api.ts';
import {PaymentList} from "@/models/PaymentList.ts";
import {PaymentShare} from "@/models/PaymentShare.ts";
import {CreatePayment} from "@/models/CreatePayment.ts";

class AdminService {
    async createPayment(paymentData: CreatePayment) {
        return apiService.axiosInstance.post("/admin", paymentData)
    }

    async getPayments(skip: number, take: number, paymentType: PaymentType): Promise<PaymentList> {
        skip = (skip-1)*take;
        const response = await apiService.axiosInstance
            .get("/admin/payments", {params: {paymentType, skip, take}});

        return response.data;
    }
    async getShares(paymentId: string): Promise<PaymentShare[]> {
        const response = await apiService.axiosInstance
            .get("/admin/shares", {params: {paymentId}});

        return response.data;
    }
}

export const adminService = new AdminService();