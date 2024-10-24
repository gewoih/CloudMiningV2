<template>
  <div class="w-9">
    <Toolbar class="mb-5 pt-0 border-none">
      <template #start>
        <h1 v-if="userStore.isAdmin">Статистика</h1>
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
        <Dropdown v-if="userStore.isAdmin" v-model="selectedStatistics" :options="statisticsList"
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
          <div><b>{{ getFormattedAmount(selectedStatistics?.depositAmount || 0) }} ₽</b> депозит</div>
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
    <div class="flex align-items-center justify-content-center mt-7 mb-7">
      <Card class="my-purchases">
        <template #title>
          <Toolbar class="border-none pt-0 pb-0">
            <template #start>
              <div class="font-medium text-lg">Детализация покупок проекта</div>
            </template>
            <template v-if="userStore.isAdmin" #end>
              <Button class="mr-2" icon="pi pi-plus" label="Добавить покупку" severity="success"
                      @click="isModalVisible = true"/>
            </template>
          </Toolbar>
        </template>
        <template #content>
          <DataTable :value="purchaseList" scrollable scrollHeight="12rem" tableStyle="min-width: 6rem" dataKey="id">
            <Column class="pl-5" header="Наименование">
              <template v-slot:body="slotProps">
                <span v-html="getFormattedCaption(slotProps.data.caption)"></span>
              </template>
            </Column>
            <Column header="Сумма">
              <template v-slot:body="slotProps">
                {{
                  getFormattedAmount(slotProps.data.amount) + ' ' + "₽"
                }}
              </template>
            </Column>
            <Column header="Дата">
              <template v-slot:body="slotProps">
                {{
                  getFormattedDate(slotProps.data.date)
                }}
              </template>
            </Column>
          </DataTable>
        </template>
      </Card>
    </div>
    <Dialog v-if="userStore.isAdmin" v-model:visible="isModalVisible" :dismissableMask="true" :draggable="false"
            header="Добавление покупки" modal>
      <div class="flex align-items-center gap-3 mb-3">
        <label class="font-semibold w-8rem" for="caption">Наименование</label>
        <InputText id="caption" v-model="newPurchase.caption" autocomplete="off" class="flex-auto"/>
      </div>
      <div class="flex align-items-center gap-3 mb-3">
        <label class="font-semibold w-8rem" for="amount">Сумма</label>
        <InputNumber id="amount" v-model="newPurchase.amount" autocomplete="off" class="flex-auto"/>
      </div>
      <div class="flex align-items-center gap-3 mb-5">
        <label class="font-semibold w-8rem" for="date">Дата</label>
        <Calendar id="date" v-model="newPurchase.date" showTime hourFormat="24" autocomplete="off" class="flex-auto" date-format="dd.mm.yy"
                  show-icon/>
      </div>
      <div class="flex justify-content-end gap-2">
        <Button label="Сохранить" type="submit" @click="createNewPurchase"></Button>
      </div>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import {ref} from "vue";
import {Statistics} from "@/models/Statistics.ts";
import {StrategyType} from "@/enums/StrategyType.ts";
import {statisticsService} from "@/services/statistics.api.ts";
import {ExpenseType} from "@/enums/ExpenseType.ts";
import {TimeLine} from "@/enums/TimeLine.ts";
import {PriceBar} from "@/models/PriceBar.ts";
import {Expense} from "@/models/Expense.ts";
import {useUserStore} from "@/stores/user.ts";
import {Purchase} from "@/models/Purchase.ts";
import {format} from "date-fns";
import {CreatePurchase} from "@/models/CreatePurchase.ts";

const statisticsList = ref<Statistics[]>();
const purchaseList = ref<Purchase[]>();
const isPurchaseListInitialized = ref(false);
const isModalVisible = ref(false);
const selectedStatistics = ref<Statistics>();
const selectedStrategyType = ref(StrategyType.Hold);
const userStore = useUserStore();
const selectedIncomeAndProfitTimeline = ref(TimeLine.Last12Months);
const selectedExpenseType = ref(ExpenseType.Total);
const selectedExpenseTimeline = ref(TimeLine.Last12Months);
const incomeAndProfitChartData = ref();
const incomeAndProfitChartOptions = ref();
const expenseChartData = ref();
const expenseChartOptions = ref();
const strategyTypes = ref([
  {name: 'Доход (Hold)', value: 'Hold'},
  {name: 'Доход (Receive&Sell)', value: 'ReceiveAndSell'},
]);

const incomeAndProfitTimelines = ref([
  {name: '12 месяцев', value: 'Last12Months'},
  {name: 'За всё время', value: 'AllTime'},
  {name: 'С начала года', value: 'YearToDate'},
]);

const expenseTypes = ref([
  {name: 'Общие', value: 'Total'},
  {name: 'Электричество', value: 'OnlyElectricity'},
  {name: 'Депозиты', value: 'OnlyDeposits'},
]);

const expenseTimelines = ref([
  {name: '12 месяцев', value: 'Last12Months'},
  {name: 'За всё время', value: 'AllTime'},
  {name: 'С начала года', value: 'YearToDate'},
]);

const newPurchase = ref<CreatePurchase>({
  caption: null,
  amount: 0,
  date: new Date()
})

const statisticsLabel = (statistics: Statistics) => {
  if (statistics.user) {
    return `${statistics.user.lastName} ${statistics.user.firstName}`;
  }
  return 'Общая статистика';
}
const fetchStatistics = async () => {
  const response = await statisticsService.getStatistics(selectedStrategyType.value);
  statisticsList.value = response.statisticsDtoList;
  
  if (!isPurchaseListInitialized.value){
    purchaseList.value = response.purchaseDtoList;
    sortPurchaseListByDate();
    isPurchaseListInitialized.value = true;
  }

  if (userStore.isAdmin) {
    selectedStatistics.value = statisticsList.value.find(stat => stat.user == null) || selectedStatistics.value;
  } else {
    selectedStatistics.value = statisticsList.value[0] || selectedStatistics.value;
  }

  updateCharts();
};

const getFormattedAmount = (value: number) => {
  return value.toLocaleString('ru-RU', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  });
};

const getFormattedDate = (date: Date) => {
  return format(date, 'dd.MM.yyyy');
};

const sortPurchaseListByDate = () => {
  const purchases = purchaseList?.value;

  if (!purchases || purchases.length === 0) {
    return [];
  }

  const sortedPurchases = [...purchases].sort((a, b) => {
    const dateA = new Date(a.date).getTime();
    const dateB = new Date(b.date).getTime();
    return dateB - dateA;
  });

  purchaseList.value = sortedPurchases;
  return sortedPurchases;
};

const createNewPurchase = async () => {
  const response = await statisticsService.createPurchase(newPurchase.value);
  purchaseList.value?.unshift(response);
  sortPurchaseListByDate();
  
  newPurchase.value = {
    caption: null,
    amount: 0,
    date: new Date()
  };
  isModalVisible.value = false;
}

const getFormattedCaption = (data: string) => {
  return `&nbsp;&nbsp;${data}`; // Используем 4 неразрывных пробела
}

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
      return formattedData.sort((a, b) => a.date.getTime() - b.date.getTime());
    }
  }
};
const filterDataByExpenseType = (data: Expense[], expenseType: ExpenseType) => {
  return data
      .filter(d => d.type === expenseType)
      .flatMap(d => d.priceBars || []);
}

const setIncomeAndProfitChartData = () => {

  const incomes = filterDataByTimeline(selectedStatistics.value?.incomes || [], selectedIncomeAndProfitTimeline.value);
  const profits = filterDataByTimeline(selectedStatistics.value?.profits || [], selectedIncomeAndProfitTimeline.value);
  
  const allDates = [...new Set([...incomes.map(i => i.date.toISOString().slice(0, 7)), ...profits.map(p => p.date.toISOString().slice(0, 7))])]
      .sort();
  
  const incomeData = allDates.map(date => {
    const income = incomes.find(i => i.date.toISOString().slice(0, 7) === date);
    return income ? income.value : 0;
  });
  
  const profitData = allDates.map(date => {
    const profit = profits.find(p => p.date.toISOString().slice(0, 7) === date);
    return profit ? profit.value : 0;
  });
  
  const labels = allDates.map(date => {
    const [year, month] = date.split('-');
    return new Date(Number(year), Number(month) - 1).toLocaleDateString('ru-RU', { month: 'short', year: '2-digit' }).replace(' г.', '');
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
        borderColor: incomeData.map(value => getBarColor(value || 0)),
        backgroundColor: incomeData.map(value => getBarColor(value || 0)),
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
        borderColor: profitData.map(value => getProfitColor(value || 0)),
        backgroundColor: profitData.map(value => value !== null && value < 0 ? 'rgb(255, 0, 0)' : 'rgba(255,255,255,0)'),
        borderWidth: {
          top: 2,
          bottom: 0,
          left: 0,
          right: 0
        },
        borderRadius: profitData.map(value => getProfitBorderRadius(value || 0)),
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
  const filteredExpensePriceBars = filterDataByExpenseType(selectedStatistics.value?.expenses || [], selectedExpenseType.value);
  const expenses = filterDataByTimeline(filteredExpensePriceBars, selectedExpenseTimeline.value);

  const expenseData = expenses.map(expense => expense.value);
  const labels = expenses.map(expense => {
    return expense.date.toLocaleDateString('ru-RU', {month: 'short', year: '2-digit'}).replace(' г.', '');
  });

  return {
    labels: labels,
    datasets: [
      {
        label: 'Расходы',
        data: expenseData,
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

fetchStatistics();

</script>


