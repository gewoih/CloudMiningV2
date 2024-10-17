import {apiService} from "@/services/api.ts";
import {StrategyType} from "@/enums/StrategyType.ts";
import {StatisticsPage} from "@/models/StatisticsPage.ts";

class StatisticsService{

    async getStatistics(statisticsCalculationStrategy: StrategyType): Promise<StatisticsPage> {
        const response = await apiService.axiosInstance
            .get("/statistics", {params: {statisticsCalculationStrategy}});

        return response.data;
    }
}

export const statisticsService = new StatisticsService();