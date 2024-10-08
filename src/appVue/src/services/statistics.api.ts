import {apiService} from "@/services/api.ts";
import {StrategyType} from "@/enums/StrategyType.ts";
import {Statistics} from "@/models/Statistics.ts";

class StatisticsService{

    async getStatistics(statisticsCalculationStrategy: StrategyType): Promise<Statistics[]> {
        const response = await apiService.axiosInstance
            .get("/statistics", {params: {statisticsCalculationStrategy}});

        return response.data;
    }
}

export const statisticsService = new StatisticsService();