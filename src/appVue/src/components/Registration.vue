<template>
  <div class="p-fluid align-content-center p-card p-3 m-2">
    <h3 class="text-center mt-1">Регистрация</h3>
    <div class="field">
      <label for="firstName">Имя</label>
      <InputText id="firstName" v-model="user.firstName"/>
    </div>
    <div class="field">
      <label for="lastName">Фамилия</label>
      <InputText id="lastName" v-model="user.lastName"/>
    </div>
    <div class="field">
      <label for="patronymic">Отчество</label>
      <InputText id="patronymic" v-model="user.patronymic"/>
    </div>
    <div class="field">
      <label for="email">Email</label>
      <InputText id="email" v-model="user.email"/>
    </div>

    <div class="field">
      <label for="password">Пароль</label>
      <Password id="password" v-model="user.password" toggleMask/>
    </div>
    <div class="field">
      <label for="confirmPassword">Подтверждение пароля</label>
      <Password id="confirmPassword" v-model="user.confirmPassword" toggleMask/>
    </div>
    <div class="field">
      <Button :disabled="!isValid" class="align-items-center justify-content-center" type="submit" @click="register">
        Создать аккаунт
      </Button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {computed, ref} from 'vue';
import {RegisterUser} from '@/models/RegisterUser.ts';
import {usersService} from '@/services/users.api';

const user = ref<RegisterUser>({
  firstName: '',
  lastName: '',
  patronymic: '',
  email: '',
  password: '',
  confirmPassword: ''
});

const isValid = computed(() => {
  return user.value.firstName &&
      user.value.lastName &&
      user.value.patronymic &&
      user.value.email &&
      user.value.password &&
      user.value.password === user.value.confirmPassword;
});

async function register() {
  await usersService.createUser(user.value);
}
</script>