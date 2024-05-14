<template>

  <div class="w-8">

    <Toolbar class="mb-4">
      <template #end>
        <Dropdown v-model="selectedPaymentType" :options="paymentTypes" optionLabel="name" optionValue="value" @change="fetchPayments" class="w-full md:w-14rem" />
      </template>
    </Toolbar>

    <DataTable :value="payments" dataKey="id">
      <Column field="isCompleted" header="Статус">
        <template #body="slotProps">
          <Tag :value="slotProps.data.isCompleted ? 'Завершен' : 'Ожидание'" :severity="getStatusSeverity(slotProps.data.isCompleted)" />
        </template>
      </Column>
      <Column field="amount" header="Сумма"/>
      <Column field="date" header="Дата">
        <template #body="slotProps">
          {{ getDateOnly(slotProps.data.date) }}
        </template>
      </Column>
      <Column field="caption" header="Комментарий"></Column>
    </DataTable>
    <Paginator :rows="pageSize" :totalRecords="totalPaymentsCount" :rowsPerPageOptions="[5, 10, 15]" @page="pageChange"></Paginator>
  </div>

</template>

<script setup lang="ts">
import {ref} from 'vue';
import {paymentsService} from "@/services/payments.api.ts";
import {format} from 'date-fns'
import {CurrencyCode} from "@/enums/CurrencyCode.ts";
import {PaymentType} from "@/enums/PaymentType.ts";
import {Payment} from "@/models/Payment.ts";
import {CreatePayment} from "@/models/CreatePayment.ts";
import {PaymentShare} from "@/models/PaymentShare.ts";

</script>
