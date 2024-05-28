<template>
  <div class="w-8">
    <DataTable v-model:expandedRows="expandedRows" :value="members" dataKey="id"
               @rowExpand="fetchDeposits">
      <Column expander/>
      <Column header="ФИО">
        <template v-slot:body="slotProps">
          {{
            slotProps.data.user.lastName + ' ' + slotProps.data.user.firstName + ' ' + slotProps.data.user.patronymic
          }}
        </template>
      </Column>
      <Column header="Доля участника">
        <template v-slot:body="slotProps">
          {{
            getTruncatedAmount(slotProps.data.share, 2) + ' ' + "%"
          }}
        </template>
      </Column>
      <Column header="Сумма инвестиций">
        <template v-slot:body="slotProps">
          {{
            getTruncatedAmount(slotProps.data.sharedAmount, slotProps.data.currency.precision) + ' ' + slotProps.data.currency.shortName
          }}
        </template>
      </Column>
      <Column field="date" header="Дата регистрации">
        <template v-slot:body="slotProps">
          {{ getDateOnly(slotProps.data.date) }}
        </template>
      </Column>
      <template v-slot:expansion="slotProps">
        <div class="p-3">
          <DataTable :value="depositsMap[slotProps.data.id]">
            <Column field="date" header="Дата">
              <template v-slot:body="shareSlotProps">
                {{ getDateOnly(shareSlotProps.data.date) }}
              </template>
            </Column>
            <Column field="amount" header="Сумма">
              <template v-slot:body="shareSlotProps">
                {{
                  getTruncatedAmount(shareSlotProps.data.amount, slotProps.data.currency.precision) + ' ' + slotProps.data.currency.shortName
                }}
              </template>
            </Column>
            <Column field="caption" header="Комментарий"></Column>
          </DataTable>
        </div>
      </template>
    </DataTable>
  </div>
</template>

<script setup lang="ts">
import {ref} from "vue";
import {format} from "date-fns";
import {paymentsService} from "@/services/payments.api.ts";
import {Member} from "@/models/Member.ts";
import {MemberDeposit} from "@/models/MemberDeposit.ts";

const expandedRows = ref({});
const depositsMap = ref<{ [key: string]: MemberDeposit[] }>({});
const members = ref<Member[]>();
const deposits = ref<MemberDeposit[]>();

const getDateOnly = (date: Date) => {
  return format(date, 'dd.MM.yyyy');
};

const getTruncatedAmount = (value: number, precision: number) => {
  const factor = 10 ** precision;
  return Math.trunc(value * factor) / factor;
}

const fetchDeposits = async (event) => {
  const memberId = event.data.id;
  if (!depositsMap.value[memberId]) {
    depositsMap.value[memberId] = await paymentsService.getDeposits(memberId);
  }
  deposits.value = depositsMap.value[memberId];

};

const fetchMembers = async () => {
  const response = await paymentsService.getMembers();
  members.value = response.items;
};

fetchMembers();

</script>
