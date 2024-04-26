<script setup lang="ts">
import {ref, computed} from 'vue';
import {LoginUser} from '@/models/LoginUser.ts';
import {usersService} from '@/services/users.api';
import InputGroup from "primevue/inputgroup";
import InputGroupAddon from "primevue/inputgroupaddon";

const user = ref<LoginUser>({
  email: '',
  password: '',
});

const isValid = computed(() => {
  return user.value.email && user.value.password;
});

async function login() {
  await usersService.loginUser(user.value);
}
</script>

<template>
  <div class="wrapper p-8 m-7">
    <div class="flex justify-content-center flex-wrap p-8">
      <div class="p-fluid align-content-center">
        <h3 class="text-center">Вход в аккаунт</h3>
        <div class="field">
          <label for="email">Email</label>
          <InputText id="email" v-model="user.email"/>
        </div>
        <div class="field">
          <label for="password">Пароль</label>
          <Password id="password" v-model="user.password" toggleMask/>
        </div>
        <div class="field">
          <Button class="align-items-center justify-content-center" :disabled="!isValid" type="submit" @click="login">
            Войти
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>