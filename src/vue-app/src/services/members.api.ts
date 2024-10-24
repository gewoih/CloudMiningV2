import {apiService} from "@/services/api.ts";
import {Member} from "@/models/Member.ts";
import {Deposit} from "@/models/Deposit.ts";

class MembersService {

    async createDeposit(depositData: Deposit) {
        return apiService.axiosInstance.post("/deposits", depositData)
    }
    
    async getMembers(): Promise<Member[]> {
        const response = await apiService.axiosInstance
            .get("/members");

        return response.data;
    }

    async getDeposits(userId: string): Promise<Deposit[]> {
        const response = await apiService.axiosInstance
            .get("/members/deposits", {params: {userId}});

        return response.data;
    }
}
export const membersService = new MembersService();