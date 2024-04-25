<script setup lang="ts">
import {ref} from 'vue';
import {ShareablePayment} from "@/models/ShareablePayment.ts";
import {shareablePaymentsService} from "@/services/payments.api.ts";
import {CurrencyCode} from "@/enums/CurrencyCode.ts";
import {PaymentType} from "@/enums/PaymentType.ts";

const selectedPaymentType = ref(PaymentType.Electricity);
// const isLoading = ref(false);
const payments = ref<ShareablePayment[]>([]);
const newPayment = ref<ShareablePayment>({ 
  caption: null,
  currencyCode: CurrencyCode.RUB,
  paymentType: PaymentType.Electricity,
  date: new Date(),
  amount: 0,
  isCompleted: false
});
const isModalOpen = ref(false);

const fetchPayments = async () => {
  // isLoading.value = true;
  payments.value = await shareablePaymentsService.getPayments(selectedPaymentType.value);
  console.log(payments.value);
  // isLoading.value = false;
};

const showModal = () => {
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};
const createPayment = async () => {
    newPayment.value.paymentType = selectedPaymentType.value;
    await shareablePaymentsService.createPayment(newPayment.value);
    await fetchPayments();
    newPayment.value = { 
      caption: null,
      currencyCode: CurrencyCode.RUB,
      paymentType: selectedPaymentType.value,
      date: new Date(),
      amount: 0,
      isCompleted: false
    };
    closeModal();
};

fetchPayments();

</script>

<template>
<!--  <div class="loading-container" v-if="isLoading">-->
<!--    <div class="loading-spinner"></div>-->
<!--    <p class="loading-message">Загрузка данных...</p>-->
<!--  </div>-->

  <div class="container-fluid">
    <div class="row justify-content-end">
      <div class="col-auto">
        <select class="form-select" v-model="selectedPaymentType" @change="fetchPayments">
          <option :value="PaymentType.Electricity">Электричество</option>
          <option :value="PaymentType.Purchase">Покупки</option>
        </select>
      </div>
    </div>
    <div class="row">
      <div class="col">
        <div class="table-responsive m-3">
          <table class="table table-striped table-hover display">
            <thead class="sticky-top">
            <tr>
              <th>Сумма</th>
              <th>Дата</th>
              <th>Комментарий</th>
            </tr>
            </thead>
            <tbody>
            <tr v-for="payment in payments" :class="{ 'table-success': payment.isCompleted, 'table-danger': !payment.isCompleted }">
              <td>{{ payment.amount }}</td>
              <td>{{ new Date(payment.date).toLocaleDateString('ru-RU') }}</td>
              <td>{{ payment.caption }}</td>
            </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    <div class="row justify-content-end">
      <div class="col-auto">
        <button type="button" class="btn btn-primary m-3" v-on:click="showModal()">Добавить платеж</button>
      </div>
    </div>
  </div>

  <!-- Modal -->
  <div class="modal fade" :class="{ 'show': isModalOpen }" id="exampleModal" tabindex="-1" aria-labelledby="addPaymentModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="addPaymentModalLabel">Добавление нового платежа</h5>
          <button type="button" class="btn-close" aria-label="Close" @click="closeModal"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3" id="addPaymentForm">
            <div class="form-group">
              <label for="paymentCaption">Комментарий</label>
              <input type="text" class="form-control" id="paymentCaption" v-model="newPayment.caption">
            </div>
            <div class="form-group">
              <label for="paymentDate">Дата</label>
              <input type="date" class="form-control" id="paymentDate" required v-model="newPayment.date">
            </div>
            <div class="form-group">
              <label for="paymentAmount">Сумма</label>
              <input type="number" class="form-control" id="paymentAmount" required v-model="newPayment.amount">
            </div>
          </div>
        </div>
        <div class="modal-footer border-top-0">
          <button type="button" class="btn btn-secondary" @click="closeModal">Отмена</button>
          <button type="button" class="btn btn-primary" @click="createPayment">Сохранить</button>
        </div>
      </div>
    </div>
  </div>
</template>
