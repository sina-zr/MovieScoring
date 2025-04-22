<script setup>
import Card from "@/components/Card.vue";
import { RouterLink } from "vue-router";
</script>

<template>
  <!-- Developers and Employers -->
  <section class="py-4">
    <div class="container-xl lg:container m-auto">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4 p-4 rounded-lg">
        <Card bg="bg-red-100">
          <h2 class="text-2xl font-bold">For Users</h2>
          <p class="mt-2 mb-4">
            Browse our movies and score them right away
          </p>
          <RouterLink
            to="/movies"
            class="inline-block bg-black text-white rounded-lg px-4 py-2 hover:bg-gray-700"
          >
            Browse movies
          </RouterLink>
        </Card>

        <Card bg="bg-orange-200">
          <h2 class="text-2xl font-bold">For Managers</h2>
          <p class="mt-2 mb-4">
            enter your directors and actors to create a movie
          </p>
          <RouterLink
            to="/movies/add"
            class="inline-block bg-orange-500 text-white rounded-lg px-4 py-2 hover:bg-orange-700"
          >
            Add Movie
          </RouterLink>
        </Card>
      </div>
    </div>
  </section>
</template>