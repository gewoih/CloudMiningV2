import {apiService} from "@/services/api.ts";
import {Member} from "@/models/Member.ts";
import {MemberDeposit} from "@/models/MemberDeposit.ts";

class MembersService {
    async getMembers(): Promise<Member[]> {
        const response = await apiService.axiosInstance
            .get("/members");

        return response.data;
    }

    async getDeposits(userId: string): Promise<MemberDeposit[]> {
        const response = await apiService.axiosInstance
            .get("/members/deposits", {params: {userId}});

        return response.data;
    }
}
export const membersService = new MembersService();