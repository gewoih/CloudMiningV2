<template>
  <div class="w-8">
    <DataTable v-model:expandedRows="expandedRows" :value="members" dataKey="user.id"
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
            getTruncatedAmount(slotProps.data.amount, 2) + ' ' + "₽"
          }}
        </template>
      </Column>
      <Column field="registrationDate" header="Дата регистрации">
        <template v-slot:body="slotProps">
          {{ getDateOnly(slotProps.data.registrationDate) }}
        </template>
      </Column>
      <template v-slot:expansion="slotProps">
        <div class="p-3">
          <DataTable :value="depositsMap[slotProps.data.user.id]">
            <Column field="date" header="Дата">
              <template v-slot:body="slotProps">
                {{ getDateOnly(slotProps.data.date) }}
              </template>
            </Column>
            <Column field="amount" header="Сумма">
              <template v-slot:body="slotProps">
                {{
                  getTruncatedAmount(slotProps.data.amount, 2) + ' ' + "₽"
                }}
              </template>
            </Column>
          </DataTable>
        </div>
      </template>
    </DataTable>
  </div>
</template>

<script setup lang="ts">
import {ref} from "vue";
import {format} from "date-fns";
import {Member} from "@/models/Member.ts";
import {Deposit} from "@/models/MemberDeposit.ts";
import {membersService} from "@/services/members.api.ts";

const expandedRows = ref({});
const depositsMap = ref<{ [key: string]: Deposit[] }>({});
const members = ref<Member[]>();
const deposits = ref<Deposit[]>();

const getDateOnly = (date: Date) => {
  return format(date, 'dd.MM.yyyy');
};

const getTruncatedAmount = (value: number, precision: number) => {
  const factor = 10 ** precision;
  return Math.trunc(value * factor) / factor;
}

const fetchDeposits = async (event) => {
  const memberId = event.data.user.id;
  if (!depositsMap.value[memberId]) {
    depositsMap.value[memberId] = await membersService.getDeposits(memberId);
  }
  deposits.value = depositsMap.value[memberId];
};

const fetchMembers = async () => {
  members.value = await membersService.getMembers();
};

fetchMembers();

</script>
