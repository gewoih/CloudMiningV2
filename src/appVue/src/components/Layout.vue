<template>
  <div class="d-flex">
    <div class="d-flex flex-column flex-shrink-0 p-3 text-bg-dark side-menu">
      <router-link to="/" class="d-flex align-items-center mb-3 mb-md-0 me-md-auto text-white text-decoration-none">
        <i class="bi bi-currency-bitcoin me-2 fs-2"></i>
        <span class="fs-3">CloudMining</span>
      </router-link>
      <hr>
      <ul class="nav nav-pills flex-column mb-auto">
        <li class="nav-item">
          <router-link to="/user/register" class="nav-link text-white text-decoration-none hover-shadow bg-hover-light" active-class="active">
            <i class="bi bi-currency-bitcoin me-2"></i>
            <span class="fs-4">Регистрация</span>
          </router-link>
        </li>
        <router-link to="/user/login" class="nav-link text-white text-decoration-none hover-shadow bg-hover-light" active-class="active">
          <i class="bi bi-currency-bitcoin me-2"></i>
          <span class="fs-4">Вход</span>
        </router-link>
      </ul>
      <hr>
      <div class="dropdown">
        <a href="#" class="d-flex align-items-center text-white text-decoration-none dropdown-toggle"
           data-bs-toggle="dropdown" aria-expanded="false">
          <i class="bi bi-person-circle me-2 fs-5"></i>
          <strong class="fs-6">Мой профиль</strong>
        </a>
        <ul class="dropdown-menu dropdown-menu-dark text-small shadow">
          <li><a class="dropdown-item" href="#">Настройки</a></li>
          <li>
            <hr class="dropdown-divider">
          </li>
          <li><a class="dropdown-item" href="#">Выйти</a></li>
        </ul>
      </div>
    </div>

    <div class="content">
      <router-view></router-view>
    </div>

  </div>
</template>

<script setup lang="ts">
</script>

<style>
.side-menu {
  width: 20vw;
  height: 100vh;
}

.content {
  flex-grow: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: lightgray;
}
</style>