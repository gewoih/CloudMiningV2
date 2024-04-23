<script lang="ts">
import Registration from './components/Registration.vue'
import Authentication from "@/components/Authentication.vue";

export default {
  components: {
    Authentication,
    Registration
  }
}
</script>

<template>
  <Registration />
  <Authentication />
</template>
