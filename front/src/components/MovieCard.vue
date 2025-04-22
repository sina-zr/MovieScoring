<script setup>
import { ref, computed } from "vue";
import { RouterLink } from "vue-router";

const props = defineProps({
  movie: Object,
});

</script>

<template>
  <div class="p-4">
    <div class="mb-6">
      <h3 class="text-xl font-bold text-center">{{ movie.title }}</h3>
    </div>
    
    <div class="mb-5 text-center">
      {{ movie.year }}
    </div>
    <div class="text-center">
      <span class="text-gray-600 my-2 mx-1" v-for="g in movie.genres" :key="g">{{ g }}</span>
    </div>

    <div class="border border-gray-100 mb-5"></div>

    <div class="flex flex-col lg:flex-row justify-between mb-4">
      <div class="text-yellow-500 mt-2">
        <i class="pi pi-star-fill text-lg"></i>
        {{ movie.score }}
      </div>
      <RouterLink
        :to="`/movies/${movie.movieId}`"
        class="bg-red-300 hover:bg-red-600 text-white px-4 py-2 rounded-lg text-center text-sm"
      >
        Read More
      </RouterLink>
    </div>
  </div>
</template>