<script setup>
import { ref } from "vue";
import { useAuth } from "@/services/auth";

const { login, authError } = useAuth();
const username = ref("");
const password = ref("");

const onSubmit = () => login(username.value, password.value);
</script>

<template>
  <section
    class="m-0 bg-yellow-50 min-h-screen flex items-center p-0 justify-center"
  >
    <div class="max-w-sm rounded overflow-hidden shadow-lg bg-white">
      <form @submit.prevent="onSubmit" class="max-w-sm mx-auto p-4 justify-center">
        <h1 class="text-2xl font-bold mb-4 text-center">Log in</h1>
        <div class="mb-2">
          <label>Username</label>
          <input
            v-model="username"
            type="text"
            class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
          />
        </div>
        <div class="mb-4">
          <label>Password</label>
          <input
            v-model="password"
            type="password"
            class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
          />
        </div>
        <div class="flex justify-center">

          <button
          type="submit"
          class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
          >
          Log In
        </button>
      </div>
        <p v-if="authError" class="text-red-500 mt-2">{{ authError }}</p>
      </form>
    </div>
  </section>
</template>
