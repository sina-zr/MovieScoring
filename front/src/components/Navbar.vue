<script setup>
import logo from "@/assets/logo.ico";
import { useAuth, isAuthenticated, isAdmin } from "@/services/auth";
import { RouterLink, useRoute } from "vue-router";
const isActiveLink = (routePath) => {
  const currentRout = useRoute();
  return currentRout.path == routePath;
};
const navLinkClasses = [
  "text-white",
  "hover:bg-gray-900",
  "hover:text-white",
  "rounded-md",
  "px-3",
  "py-2",
];
const { logout } = useAuth()
</script>

<template>
  <nav class="bg-red-400 border-b border-red-800">
    <div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
      <div class="flex h-20 items-center justify-between">
        <div
          class="flex flex-1 items-center justify-center md:items-stretch md:justify-start"
        >
          <!-- Logo -->
          <RouterLink class="flex flex-shrink-0 items-center mr-4" to="/">
            <img class="h-10 w-auto" :src="logo" alt="Vue Jobs" />
            <span class="hidden md:block text-white text-2xl font-bold ml-2"
              >Movie App</span
            >
          </RouterLink>
          <div class="flex flex-shrink-0 items-center mr-4">
            <button
        v-if="isAuthenticated"
        @click="logout"
        class="bg-red-700 text-white px-3 py-1 rounded">
        Logout
      </button>
      <RouterLink v-else to="/login" class="text-white bg-red-700 px-3 py-1 rounded mx-1">Login</RouterLink>
      <RouterLink v-if="!isAuthenticated" to="/register" class="text-white bg-red-500 px-3 py-1 rounded mx-1">Register</RouterLink>
          </div>
          <div class="md:ml-auto">
            <div class="flex space-x-2">
              <RouterLink
                to="/"
                :class="[
                  isActiveLink('/') ? 'bg-red-900' : '',
                  ...navLinkClasses,
                ]"
                >Home</RouterLink
              >
              <RouterLink
                to="/movies"
                :class="[
                  isActiveLink('/movies') ? 'bg-red-900' : '',
                  ...navLinkClasses,
                ]"
                >Movies</RouterLink
              >
              <RouterLink v-if="isAdmin"
                to="/movies/add"
                :class="[
                  isActiveLink('/movies/add') ? 'bg-red-900' : '',
                  ...navLinkClasses,
                ]"
                >Add Movie</RouterLink
              >
              <RouterLink v-if="isAuthenticated"
                to="/watchlist"
                :class="[
                  isActiveLink('/watchlist') ? 'bg-red-900' : '',
                  ...navLinkClasses,
                ]"
                >My Watchlist</RouterLink
              >
            </div>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>