<template>
  <div class="test-page">
    <h1>HELLO WORLD TEST</h1>
    <p>This is a brand new test page</p>
  </div>
</template>

<script setup lang="ts">
// Simple test page
</script>

<style scoped>
.test-page {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: lime;
  color: black;
}

h1 {
  font-size: 48px;
  font-weight: bold;
}

p {
  font-size: 24px;
}
</style>
