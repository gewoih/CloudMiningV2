<script setup lang="ts">
import {ref, computed} from 'vue';
import {LoginUser} from '@/models/LoginUser.ts';
import {usersService} from '@/services/users.api';

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
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <form @submit.prevent="login" class="card p-4 shadow">
          <h2 class="mb-4">Вход в аккаунт</h2>

          <div class="mb-2">
            <label for="email" class="form-label">Email</label>
            <input type="email" class="form-control" id="email" v-model="user.email" required>
          </div>

          <div class="mb-2">
            <label for="password" class="form-label">Пароль</label>
            <input type="password" class="form-control" id="password" v-model="user.password" required>
          </div>

          <button type="submit" :disabled="!isValid" class="btn btn-primary">Войти</button>
        </form>
      </div>
    </div>
  </div>
</template>