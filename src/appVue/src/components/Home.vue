<template>
  <div class="w-9">
    <Toolbar class="mb-5 pt-0 border-none">
      <template #start>
        <h1 v-if="userRole === UserRole.Admin">Статистика</h1>
        <h1 v-else>Моя статистика</h1>
      </template>
      <template #end>
        <i class="pi pi-question-circle mr-3 text-2xl"
           v-tooltip.bottom="{
            value: 'Селектор переключения стратегии расчета дохода. ' +
             '<br/><br/><b>Доход (Hold)</b> - доход при учёте хранения полного объема полученных средств с начала участия в проекте. ' +
              '<br/><br/><b>Доход (Receive&Sell)</b> - доход при учёте продажи полного объема полученных средств в день получения.',
            showDelay: 300,
            hideDelay: 300,
            pt: {
              text:'bg-white text-color font-medium shadow-5 w-19rem'
            },
            escape: false
          }"></i>
        <Dropdown v-model="selectedStrategyType" :options="strategyTypes" class="w-15rem" optionLabel="name"
                  optionValue="value" @change="fetchStatistics"/>
        <Dropdown v-if="userRole === UserRole.Admin" v-model="selectedStatistics" :options="statisticsList"
                  class="w-15rem ml-3" :optionLabel="statisticsLabel" @change="updateCharts"/>
      </template>
    </Toolbar>
    <div class="flex align-items-center justify-content-between mb-8">
      <Card class="my-box">
        <template #title>
          <i class="pi pi-wallet mr-1"></i>
          Доходы
        </template>
        <template #content>
          <h2 class="m-0">{{ getFormattedAmount(selectedStatistics?.totalIncome || 0) }} ₽</h2>
        </template>
        <template #footer>
          <div class="mb-1"><b>{{ getFormattedAmount(selectedStatistics?.monthlyIncome || 0) }} ₽</b> в месяц</div>
        </template>
      </Card>
      <Card class="my-box">
        <template #title>
          <i class="pi pi-calendar-clock mr-1"></i>
          Расходы
        </template>
        <template #content>
          <h2 class="m-0">{{ getFormattedAmount(selectedStatistics?.totalExpense || 0) }} ₽</h2>
        </template>
        <template #footer>
          <div class="mb-2"><b>{{ getFormattedAmount(selectedStatistics?.electricityExpense || 0) }} ₽</b> электричество
          </div>
          <div><b>{{ getFormattedAmount(selectedStatistics?.purchaseExpense || 0) }} ₽</b> покупки</div>
        </template>
      </Card>
      <Card class="my-box">
        <template #title>
          <i class="pi pi-chart-line mr-1"></i>
          Прибыль
        </template>
        <template #content>
          <h2 class="m-0">{{ getFormattedAmount(selectedStatistics?.totalProfit || 0) }} ₽</h2>
        </template>
        <template #footer>
          <div class="mb-2"><b>{{ getFormattedAmount(selectedStatistics?.monthlyProfit || 0) }} ₽</b> в месяц</div>
          <div><b>{{ getFormattedAmount(selectedStatistics?.paybackPercent || 0) }}%</b> окупилось</div>
        </template>
      </Card>
    </div>
    <div class="flex align-items-center justify-content-between mt-7 mb-7">
      <Card class="my-chart">
        <template #title>
          <Toolbar class="border-none pt-0 pb-0">
            <template #start>
              <div class="font-medium">Доходы/Прибыль</div>
            </template>
            <template #end>
              <Dropdown v-model="selectedIncomeAndProfitTimeline" :options="incomeAndProfitTimelines"
                        class="custom-dropdown" optionLabel="name"
                        optionValue="value" @change="updateIncomeAndProfitChart"/>
            </template>
          </Toolbar>
        </template>
        <template #content>
          <Chart type="bar" :data="incomeAndProfitChartData" :options="incomeAndProfitChartOptions"
                 class="h-19rem mb-1"/>
        </template>
      </Card>
      <Card class="my-chart">
        <template #title>
          <Toolbar class="border-none pt-0 pb-0">
            <template #start>
              <div class="font-medium">Расходы</div>
            </template>
            <template #end>
              <Dropdown v-model="selectedExpenseType" :options="expenseTypes" class="custom-dropdown mr-2"
                        optionLabel="name" optionValue="value" @change="updateExpenseChart"/>
              <Dropdown v-model="selectedExpenseTimeline" :options="expenseTimelines" class="custom-dropdown"
                        optionLabel="name" optionValue="value" @change="updateExpenseChart"/>
            </template>
          </Toolbar>
        </template>
        <template #content>
          <Chart type="bar" :data="expenseChartData" :options="expenseChartOptions" class="h-19rem mb-1"/>
        </template>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import {ref} from "vue";
import {Statistics} from "@/models/Statistics.ts";
import {StrategyType} from "@/enums/StrategyType.ts";
import {statisticsService} from "@/services/statistics.api.ts";
import {UserRole} from "@/enums/UserRole.ts";
import {ExpenseType} from "@/enums/ExpenseType.ts";
import {TimeLine} from "@/enums/TimeLine.ts";
import {PriceBar} from "@/models/PriceBar.ts";

const statisticsList = ref<Statistics[]>();
const selectedStatistics = ref<Statistics>();
const selectedStrategyType = ref(StrategyType.Hold);
const userRole = ref(UserRole.User);
const selectedIncomeAndProfitTimeline = ref(TimeLine.AllTime);
const selectedExpenseType = ref(ExpenseType.Total);
const selectedExpenseTimeline = ref(TimeLine.AllTime);
const incomeAndProfitChartData = ref();
const incomeAndProfitChartOptions = ref();
const expenseChartData = ref();
const expenseChartOptions = ref();
const strategyTypes = ref([
  {name: 'Доход (Hold)', value: 'Hold'},
  {name: 'Доход (Receive&Sell)', value: 'ReceiveAndSell'},
]);

const incomeAndProfitTimelines = ref([
  {name: 'За всё время', value: 'AllTime'},
  {name: 'С начала года', value: 'YearToDate'},
  {name: '12 месяцев', value: 'Last12Months'},
]);

const expenseTypes = ref([
  {name: 'Общие', value: 'Total'},
  {name: 'Электричество', value: 'OnlyElectricity'},
  {name: 'Покупки', value: 'OnlyPurchases'},
]);

const expenseTimelines = ref([
  {name: 'За всё время', value: 'AllTime'},
  {name: 'С начала года', value: 'YearToDate'},
  {name: '12 месяцев', value: 'Last12Months'},
]);

const statisticsLabel = (statistics: Statistics) => {
  if (statistics.user) {
    return `${statistics.user.lastName} ${statistics.user.firstName}`;
  }
  return 'Общая статистика';
}
const fetchStatistics = async () => {
  statisticsList.value = await statisticsService.getStatistics(selectedStrategyType.value);
  if (userRole.value == UserRole.Admin) {
    selectedStatistics.value = statisticsList.value.find(stat => stat.user == null) || selectedStatistics.value;
  }
  updateCharts();
};

const getFormattedAmount = (value: number) => {
  return value.toLocaleString('ru-RU', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  });
};

const getUserRole = () => {
  const jwt = localStorage.getItem('access_token');
  const decodedJwt = jwt && JSON.parse(atob(jwt.split('.')[1]));
  userRole.value = decodedJwt["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
};

const filterDataByTimeline = (data: PriceBar[], timeline: TimeLine) => {
  const now = new Date();
  const formattedData = data.map(d => ({
    ...d,
    date: new Date(d.date)
  }));

  switch (timeline) {
    case TimeLine.YearToDate: {
      return formattedData.filter(d => d.date.getFullYear() === now.getFullYear());
    }
    case TimeLine.Last12Months: {
      const oneYearAgo = new Date(now);
      oneYearAgo.setFullYear(oneYearAgo.getFullYear() - 1);
      return formattedData.filter(d => d.date >= oneYearAgo);
    }
    case TimeLine.AllTime:
    default: {
      return formattedData;
    }
  }
};

const setIncomeAndProfitChartData = () => {
  const incomes = filterDataByTimeline(selectedStatistics.value?.incomes || [], selectedIncomeAndProfitTimeline.value);
  const profits = filterDataByTimeline(selectedStatistics.value?.profits || [], selectedIncomeAndProfitTimeline.value);

  const formattedIncomes = incomes.map(income => ({
    ...income,
    date: new Date(income.date)
  }));

  const formattedProfits = profits.map(profit => ({
    ...profit,
    date: new Date(profit.date)
  }));

  const incomeData = formattedIncomes.map(income => income.value);
  const profitData = formattedProfits.map(profit => profit.value);

  const labels = formattedProfits.map(profit => {
    return profit.date.toLocaleDateString('ru-RU', {month: 'short', year: '2-digit'});
  });

  const getBarColor = (value: number) => value < 0 ? 'rgb(255, 0, 0)' : 'rgb(139, 92, 246)';
  const getProfitColor = (value: number) => value < 0 ? 'rgb(255, 0, 0)' : 'rgb(0, 255, 195)';
  const getProfitBorderRadius = (value: number) => value < 0 ? 8 : 0;

  return {
    labels: labels,
    datasets: [
      {
        label: 'Доходы',
        data: incomeData,
        borderColor: incomeData.map(value => getBarColor(value)),
        backgroundColor: incomeData.map(value => getBarColor(value)),
        order: 2,
        borderRadius: {
          topLeft: 8,
          topRight: 8,
          bottomLeft: 0,
          bottomRight: 0
        }
      },
      {
        label: 'Прибыль',
        data: profitData,
        borderColor: profitData.map(value => getProfitColor(value)),
        backgroundColor: profitData.map(value => value < 0 ? 'rgb(255, 0, 0)' : 'rgba(255,255,255,0)'),
        borderWidth: {
          top: 2,
          bottom: 0,
          left: 0,
          right: 0
        },
        borderRadius: profitData.map(value => getProfitBorderRadius(value)),
        order: 1
      }
    ]
  };
};
const setIncomeAndProfitChartOptions = () => {
  const documentStyle = getComputedStyle(document.documentElement);
  const textColor = documentStyle.getPropertyValue('--text-color');
  const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
  const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

  return {
    maintainAspectRatio: false,
    aspectRatio: 0.6,
    plugins: {
      legend: {
        display: false,
        labels: {
          color: textColor
        }
      },
      tooltip: {
        mode: 'index',
        intersect: false,
        itemSort: (a: any, b: any) => {
          return b.dataset.order - a.dataset.order;
        }
      }
    },
    scales: {
      x: {
        stacked: true,
        ticks: {
          color: textColorSecondary
        },
        grid: {
          color: surfaceBorder
        }
      },
      y: {
        stacked: false,
        ticks: {
          color: textColorSecondary
        },
        grid: {
          color: surfaceBorder
        }
      }
    },
    interaction: {
      mode: 'index',
      intersect: true,
    },
    hover: {
      mode: 'index',
      intersect: false
    }
  };
}
const setExpenseChartData = () => {
  return {
    labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
    datasets: [
      {
        label: 'Расходы',
        data: [540, 325, 702, 620, 325, 702, 620],
        backgroundColor: ['rgb(139, 92, 246)'],
        borderColor: ['rgb(139, 92, 246)'],
        borderRadius: {
          topLeft: 8,
          topRight: 8,
          bottomLeft: 0,
          bottomRight: 0
        },
        borderWidth: 1
      }
    ]
  };
};
const setExpenseChartOptions = () => {
  const documentStyle = getComputedStyle(document.documentElement);
  const textColor = documentStyle.getPropertyValue('--text-color');
  const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
  const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

  return {
    plugins: {
      legend: {
        display: false,
        labels: {
          color: textColor
        }
      },
      tooltip: {
        mode: 'index',
        intersect: false
      }
    },
    scales: {
      x: {
        ticks: {
          color: textColorSecondary
        },
        grid: {
          color: surfaceBorder
        }
      },
      y: {
        beginAtZero: true,
        ticks: {
          color: textColorSecondary
        },
        grid: {
          color: surfaceBorder
        }
      }
    },
    interaction: {
      mode: 'index',
      intersect: false
    },
    hover: {
      mode: 'index',
      intersect: false
    }
  };
}
const updateCharts = () => {
  updateIncomeAndProfitChart();
  updateExpenseChart();
}

const updateIncomeAndProfitChart = () => {
  incomeAndProfitChartData.value = setIncomeAndProfitChartData();
  incomeAndProfitChartOptions.value = setIncomeAndProfitChartOptions();
}

const updateExpenseChart = () => {
  expenseChartData.value = setExpenseChartData();
  expenseChartOptions.value = setExpenseChartOptions();
}

getUserRole();
fetchStatistics();
updateCharts();

</script>


