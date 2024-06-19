<template>
  <div class="w-9">
    <Toolbar class="mb-5 pt-0 border-none">
      <template #start>
        <h1>Моя статистика</h1>
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
        <Dropdown v-model="selectedIncomeType" :options="incomeTypes" class="w-15rem" optionLabel="name"
                  optionValue="value"/>
      </template>
    </Toolbar>
    <div class="flex align-items-center justify-content-between mb-8">
      <Card class="my-box">
        <template #title>
          <i class="pi pi-wallet mr-1"></i>
          Доходы
        </template>
        <template #content>
          <h2 class="m-0">500 000,88 ₽</h2> 
        </template>
        <template #footer>
          <div class="mb-1"><b>4200,99 ₽</b> в месяц</div>
        </template>
      </Card>
      <Card class="my-box">
        <template #title>
          <i class="pi pi-calendar-clock mr-1"></i>
          Расходы
        </template>
        <template #content>
          <h2 class="m-0">340 000,88 ₽</h2>
        </template>
        <template #footer>
          <div class="mb-2"><b>150 000,44 ₽</b> электричество</div>
          <div><b>150 000,44 ₽</b> покупки</div>
        </template>
      </Card>
      <Card class="my-box">
        <template #title>
          <i class="pi pi-chart-line mr-1"></i>
          Прибыль
        </template>
        <template #content>
          <h2 class="m-0">220 000,66 ₽</h2>
        </template>
        <template #footer>
          <div class="mb-2"><b>6213,44 ₽</b> в месяц</div>
          <div><b>94.45%</b> окупилось</div>
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
              <Dropdown v-model="selectedIncomeAndProfitTimeline" :options="incomeAndProfitTimelines" class="custom-dropdown" optionLabel="name"
                        optionValue="value" />
            </template>
          </Toolbar>
        </template>
        <template #content>
          <Chart type="bar" :data="incomeAndProfitChartData" :options="incomeAndProfitChartOptions" class="h-19rem mb-1"/>
        </template>
      </Card>
      <Card class="my-chart">
        <template #title>
          <Toolbar class="border-none pt-0 pb-0">
            <template #start>
              <div class="font-medium">Расходы</div>
            </template>
            <template #end>
              <Dropdown v-model="selectedExpenseType" :options="expenseTypes" class="custom-dropdown mr-2" optionLabel="name"
                        optionValue="value" />
              <Dropdown v-model="selectedExpenseTimeline" :options="expenseTimelines" class="custom-dropdown" optionLabel="name"
                        optionValue="value" />
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
import {onMounted, ref} from "vue";

const selectedIncomeType = ref(1)
const selectedIncomeAndProfitTimeline = ref(1)
const selectedExpenseType = ref(1)
const selectedExpenseTimeline = ref(1)
const incomeAndProfitChartData = ref();
const incomeAndProfitChartOptions = ref();
const expenseChartData = ref();
const expenseChartOptions = ref();
const incomeTypes = ref([
  {name: 'Доход (Hold)', value: 1},
  {name: 'Доход (Receive&Sell)', value: 2},
]);

const incomeAndProfitTimelines = ref([
  {name: 'За всё время', value: 1},
  {name: 'С начала года', value: 2},
  {name: '12 месяцев', value: 3},
]);

const expenseTypes = ref([
  {name: 'Общие', value: 1},
  {name: 'Электричество', value: 2},
  {name: 'Покупки', value: 3},
]);

const expenseTimelines = ref([
  {name: 'За всё время', value: 1},
  {name: 'С начала года', value: 2},
  {name: '12 месяцев', value: 3},
]);

const setIncomeAndProfitChartData = () => {

  return {
    labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
    datasets: [
      {
        label: 'Доходы',
        data: [28, 48, 40, 19, 86, 27, 90],
        borderColor: ['rgb(139, 92, 246)'],
        backgroundColor: ['rgb(139, 92, 246)'],
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
        data: [18, 28, 30, 9, 46, 17, 50],
        borderColor: 'rgb(0, 255, 195)',
        backgroundColor: ['rgba(255,255,255,0)'],
        borderWidth: {
          top: 2,
          bottom: 0,
          left: 0,
          right: 0 
        },
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
onMounted(() => {
  incomeAndProfitChartData.value = setIncomeAndProfitChartData();
  incomeAndProfitChartOptions.value = setIncomeAndProfitChartOptions();
  expenseChartData.value = setExpenseChartData();
  expenseChartOptions.value = setExpenseChartOptions();
});
</script>


