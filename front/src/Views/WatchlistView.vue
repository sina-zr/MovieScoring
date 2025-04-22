<script setup>
import { authApi } from "@/services/auth";
import { onMounted, reactive } from "vue";
import { useToast } from "vue-toastification";

const toast = useToast();
const watchlist = reactive({
  movies: [],
  watchlistName: ''
})
onMounted(async () => {
  await loadWatchlist();
});

const loadWatchlist = async () => {
  try {
    const res = await authApi.get("/api/GetUserWatchlist")
    if (res.status == 204) {
      return;
    }
    Object.assign(watchlist, res.data)
  } catch (error) {
    console.error("Error getting watchlist: ", error);
  }
};

const handleRecommendWatchlist = async () => {
  try {
    const res = await authApi.get('/api/RecommendWatchlist')
    if (res.status == 200) {
      watchlist.movies = res.data
    }
  } catch (error) {
    if (error.response?.status == 400) {
      toast.error(error.response?.data)
    }
    console.error(error.response?.data)
  }
}
</script>

<template>
  <section class="bg-orange-50 min-h-screen flex items-center justify-center">
    <div v-if="watchlist.movies.length == 0">
      <p class="text-center mb-5 text-red-400">No Watchlist</p>
      <button @click="handleRecommendWatchlist" class="bg-transparent hover:bg-blue-500 text-blue-700 font-semibold hover:text-white py-2 px-4 border border-blue-500 hover:border-transparent rounded">
        Recommend Watchlist
      </button>
    </div>
    <div v-else class="flex flex-wrap lg:w-4/5 sm:mx-auto sm:mb-2 -mx-2 p-4">
      <div v-for="movie in watchlist.movies" :key="movie.movieId" class="p-2 sm:w-1/2 w-full">
        <div class="bg-gray-100 rounded flex p-4 h-full items-center">
          <svg
            fill="none"
            stroke="currentColor"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="3"
            class="text-indigo-500 w-6 h-6 flex-shrink-0 mr-4"
            viewBox="0 0 24 24"
          >
            <path d="M22 11.08V12a10 10 0 11-5.93-9.14"></path>
            <path d="M22 4L12 14.01l-3-3"></path>
          </svg>
          <span class="font-medium">{{ movie.movieTitle }}</span>
        </div>
      </div>
    </div>
  </section>
</template>