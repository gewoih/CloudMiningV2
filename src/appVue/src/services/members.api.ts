import {apiService} from "@/services/api.ts";
import {Member} from "@/models/Member.ts";
import {MemberDeposit} from "@/models/MemberDeposit.ts";

class MembersService {
    async getMembers(): Promise<Member[]> {
        const response = await apiService.axiosInstance
            .get("");

        return response.data;
    }

    async getDeposits(memberId: string): Promise<MemberDeposit[]> {
        const response = await apiService.axiosInstance
            .get("", {params: {memberId}});

        return response.data;
    }
}
export const membersService = new MembersService();