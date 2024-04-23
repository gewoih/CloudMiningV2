<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <form @submit.prevent="register" class="card p-4 shadow">
          <h2 class="mb-4">Регистрация</h2>

          <div class="mb-2">
            <label for="firstName" class="form-label">Имя</label>
            <input type="text" class="form-control" id="firstName" v-model="user.firstName" required>
          </div>

          <div class="mb-2">
            <label for="lastName" class="form-label">Фамилия</label>
            <input type="text" class="form-control" id="lastName" v-model="user.lastName" required>
          </div>

          <div class="mb-2">
            <label for="patronymic" class="form-label">Отчество</label>
            <input type="text" class="form-control" id="patronymic" v-model="user.patronymic" required>
          </div>

          <div class="mb-2">
            <label for="email" class="form-label">Email</label>
            <input type="email" class="form-control" id="email" v-model="user.email" required>
          </div>

          <div class="mb-2">
            <label for="password" class="form-label">Пароль</label>
            <input type="password" class="form-control" id="password" v-model="user.password" required>
          </div>

          <div class="mb-4">
            <label for="confirmPassword" class="form-label">Подтверждение пароля</label>
            <input type="password" class="form-control" id="confirmPassword" v-model="user.confirmPassword" required>
          </div>

          <button type="submit" class="btn btn-primary">Создать аккаунт</button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {ref, computed} from 'vue';
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