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
             '<br/><b>Доход (Hold)</b> - доход при учёте хранения полного объема полученных средств с начала участия в проекте. ' +
              '<br/><b>Доход (Receive&Sell)</b> - доход при учёте продажи полного объема полученных средств в день получения.',
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
      <Card class="shadow-3 w-2">
        <template #title>
          <i class="pi pi-wallet mr-1"></i>
          Доходы
        </template>
        <template #content>Card content</template>
        <template #footer>
          <div class="mb-4">Card footer</div>
        </template>
      </Card>
      <Card class="shadow-3 w-2">
        <template #title>
          <i class="pi pi-calendar-clock mr-1"></i>
          Расходы
        </template>
        <template #content>
          <div class="mb-1">Card content</div>
        </template>
        <template #footer>
          <div class="mb-1">Card footer</div>
          <div>Card footer</div>
        </template>
      </Card>
      <Card class="shadow-3 w-2">
        <template #title>
          <i class="pi pi-chart-line mr-1"></i>
          Прибыль
        </template>
        <template #content>
          <div class="mb-1">Card content</div>
        </template>
        <template #footer>
          <div class="mb-1">Card footer</div>
          <div>Card footer</div>
        </template>
      </Card>
    </div>
    <div class="flex align-items-center justify-content-between mt-7 mb-7">
      <Card class="shadow-3 w-5 pt-1 pb-1">
        <template #title>
          <Toolbar class="border-none pt-0 pb-0">
            <template #start>
              Доходы
            </template>
          </Toolbar>
        </template>
        <template #content>
          <Chart type="line" :data="incomeChartData" :options="incomeChartOptions" class="h-19rem mb-1"/>
        </template>
      </Card>
      <Card class="shadow-3 w-5">
        <template #title>
          <Toolbar class="border-none pt-0 pb-0">
            <template #start>
              Расходы
            </template>
            <template #end>
              <Dropdown v-model="selectedExpenseType" :options="expenseTypes" class="w-full" optionLabel="name"
                        optionValue="value"/>
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
const selectedExpenseType = ref(1)
const incomeChartData = ref();
const incomeChartOptions = ref();
const expenseChartData = ref();
const expenseChartOptions = ref();
const incomeTypes = ref([
  {name: 'Доход (Hold)', value: 1},
  {name: 'Доход (Receive&Sell)', value: 2},
]);
const expenseTypes = ref([
  {name: 'Общие', value: 1},
  {name: 'Электричество', value: 2},
  {name: 'Покупки', value: 3},
]);

const setIncomeChartData = () => {

  return {
    labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
    datasets: [
      {
        label: 'First Dataset',
        data: [28, 48, 40, 19, 86, 27, 90],
        fill: false,
        borderColor: ['rgb(139, 92, 246)'],
        backgroundColor: ['rgb(139, 92, 246)'],
        tension: 0.4
      }
    ]
  };
};
const setIncomeChartOptions = () => {
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
        label: 'Sales',
        data: [540, 325, 702, 620, 325, 702, 620],
        backgroundColor: ['rgb(139, 92, 246)', 'rgb(139, 92, 246)', 'rgb(139, 92, 246)', 'rgb(139, 92, 246)', 'rgb(139, 92, 246)', 'rgb(139, 92, 246)', 'rgb(139, 92, 246)'],
        borderColor: ['rgb(139, 92, 246)', 'rgb(139, 92, 246)', 'rgb(139, 92, 246)', 'rgb(139, 92, 246)'],
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
  incomeChartData.value = setIncomeChartData();
  incomeChartOptions.value = setIncomeChartOptions();
  expenseChartData.value = setExpenseChartData();
  expenseChartOptions.value = setExpenseChartOptions();
});
</script>


