
import {PaymentType} from '@/enums/PaymentType';
import {apiService} from '@/services/api.ts';
import {PaymentList} from "@/models/PaymentList.ts";

class PaymentsService {
    async getPayments(skip: number, take: number, paymentType: PaymentType): Promise<PaymentList> {
        skip = (skip-1)*take;
        const response = await apiService.axiosInstance
            .get("/payments", {params: {paymentType, skip, take}});
        
        return response.data;
    }
}

export const paymentsService = new PaymentsService();