<script setup lang="ts">
import {ref, computed} from 'vue';
import {LoginUser} from '@/models/LoginUser.ts';
import {usersService} from '@/services/users.api';
import {useUserStore} from "@/stores/user.ts";
import router from "@/router.ts";

const userStore = useUserStore();

const user = ref<LoginUser>({
  email: '',
  password: '',
});

const isValid = computed(() => {
  return user.value.email && user.value.password;
});

async function login() {
  const token = await usersService.loginUser(user.value);
  userStore.setToken(token)
  await router.push({name: 'home'});
}
</script>

<template>
  <div class="p-fluid p-card p-3 m-2">
    <h3 class="text-center mt-1">Вход в аккаунт</h3>
    <div class="field">
      <label for="email">Email</label>
      <InputText id="email" v-model="user.email"/>
    </div>
    <div class="field">
      <label for="password">Пароль</label>
      <Password id="password" v-model="user.password" toggleMask/>
    </div>
    <div class="field">
      <Button class="justify-content-center" :disabled="!isValid" type="submit" @click="login">
        Войти
      </Button>
    </div>
  </div>
</template>