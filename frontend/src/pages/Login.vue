<template>
  <button
    @click="themeStore.toggleTheme()"
    class="theme-toggle"
  >
    <span v-if="themeStore.theme === 'light'" class="light-icon">🌙 深色模式</span>
    <span v-else class="dark-icon">☀️ 淺色模式</span>
  </button>

  <div class="login-container">
    <div class="login-left">
      <div class="login-card">
        <div class="logo">
          <h1>核能安全委員會<br>會內知識平台</h1>
          <h2>使用者登入</h2>
        </div>

        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>

        <form @submit.prevent="handleLogin">
          <div class="form-group">
            <label for="username">使用者名稱</label>
            <input
              id="username"
              v-model="credentials.username"
              type="text"
              required
              autocomplete="username"
              placeholder="請輸入使用者名稱"
            />
          </div>

          <div class="form-group">
            <label for="password">密碼</label>
            <input
              id="password"
              v-model="credentials.password"
              type="password"
              required
              autocomplete="current-password"
              placeholder="請輸入密碼"
            />
          </div>

          <button type="submit" :disabled="loading" class="btn btn-primary">
            {{ loading ? '登入中...' : '登入' }}
          </button>

          <a href="#" class="forgot-password">忘記密碼</a>
        </form>
      </div>
    </div>

    <div class="login-right">
      <div class="brand-content">
        <h2>會議室預約與內容管理<br>雛型展示</h2>
        <p>透過柏通公司的設計方案<br>簡化會議室管理和內容發佈流程</p>

        <div class="brand-features">
          <div class="feature-item">
            <div class="feature-icon">📅</div>
            <span>智慧會議室預約系統</span>
          </div>
          <div class="feature-item">
            <div class="feature-icon">📝</div>
            <span>專業內容管理平台</span>
          </div>
          <div class="feature-item">
            <div class="feature-icon">📁</div>
            <span>安全檔案管理系統</span>
          </div>
          <div class="feature-item">
            <div class="feature-icon">👥</div>
            <span>進階使用者權限控制</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import { useThemeStore } from '@/stores/themeStore'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const themeStore = useThemeStore()

const credentials = ref({
  username: '',
  password: '',
})

const loading = ref(false)
const errorMessage = ref('')

const handleLogin = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    await authStore.login(credentials.value)

    const redirect = (route.query.redirect as string) || '/reservations'
    router.push(redirect)
  } catch (error: any) {
    console.error('Login failed:', error)
    errorMessage.value = error.response?.data?.error?.message || '登入失敗，請檢查您的使用者名稱和密碼'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  themeStore.initialize()
})
</script>

<style>
/* Copy exact CSS from prototype - NO scoped */
body {
  margin: 0;
  padding: 0;
  overflow-x: hidden;
}

body.light {
  background-color: #F5F7FA;
  color: #0F172A;
}

body.dark {
  background-color: #0F172A;
  color: #F8FAFC;
}

.login-container {
  min-height: 100vh;
  display: flex;
  width: 100%;
}

.login-left {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 32px;
}

.login-right {
  flex: 1;
  background: linear-gradient(135deg, #2563EB 0%, #1E40AF 100%);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: white;
  padding: 64px 48px;
  position: relative;
}

.brand-content {
  max-width: 480px;
  text-align: left;
}

.brand-content h2 {
  font-size: 2.5rem;
  font-weight: 900;
  font-family: 'Noto Sans TC', sans-serif;
  margin-bottom: 24px;
  line-height: 1.3;
  letter-spacing: 0.02em;
}

.brand-content p {
  font-size: 1.125rem;
  font-weight: 500;
  font-family: 'Noto Sans TC', sans-serif;
  opacity: 0.95;
  line-height: 1.8;
  margin-bottom: 16px;
}

.brand-features {
  margin-top: 32px;
  padding-top: 32px;
  border-top: 1px solid rgba(255, 255, 255, 0.2);
}

.feature-item {
  display: flex;
  align-items: center;
  margin-bottom: 16px;
  font-size: 1rem;
  font-weight: 500;
}

.feature-icon {
  width: 28px;
  height: 28px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 12px;
  font-size: 1rem;
}

.login-card {
  width: 100%;
  max-width: 420px;
  padding: 40px;
  border: 1px solid;
  border-radius: 4px;
  transition: border-color 200ms, background-color 200ms, box-shadow 200ms;
}

.light .login-card {
  background-color: #FFFFFF;
  border-color: #D1D5DB;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08), 0 2px 8px rgba(0, 0, 0, 0.04);
}

.dark .login-card {
  background-color: #1E293B;
  border-color: #334155;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.4), 0 2px 8px rgba(0, 0, 0, 0.3);
}

.logo {
  margin-bottom: 40px;
  text-align: center;
}

.logo h1 {
  font-size: 1.875rem;
  font-weight: 900;
  font-family: 'Noto Sans TC', sans-serif;
  margin-bottom: 12px;
  line-height: 1.5;
  letter-spacing: 0.02em;
}

.light .logo h1 {
  color: #0F172A;
}

.dark .logo h1 {
  color: #F8FAFC;
}

.logo h2 {
  font-size: 1.25rem;
  font-weight: 900;
  font-family: 'Noto Sans TC', sans-serif;
  margin-bottom: 8px;
  color: #2563EB;
  letter-spacing: 0.02em;
}

.error-message {
  margin-bottom: 24px;
  padding: 12px 16px;
  background-color: #FEE2E2;
  border: 1px solid #F87171;
  border-radius: 4px;
  color: #B91C1C;
  font-size: 0.875rem;
}

.dark .error-message {
  background-color: rgba(127, 29, 29, 0.3);
  border-color: #B91C1C;
  color: #F87171;
}

.forgot-password {
  display: block;
  margin-top: 16px;
  text-align: center;
  font-size: 0.875rem;
  font-weight: 500;
  font-family: 'Noto Sans TC', sans-serif;
  text-decoration: none;
  transition: color 200ms;
}

.light .forgot-password {
  color: #2563EB;
}

.dark .forgot-password {
  color: #60A5FA;
}

.forgot-password:hover {
  text-decoration: underline;
}

.form-group {
  margin-bottom: 24px;
}

.form-group label {
  display: block;
  font-size: 0.875rem;
  font-weight: 700;
  font-family: 'Noto Sans TC', sans-serif;
  margin-bottom: 8px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

input[type="text"],
input[type="password"] {
  width: 100%;
  padding: 10px 14px;
  border: 1px solid;
  border-radius: 4px;
  font-size: 15px;
  font-weight: 500;
  font-family: 'Noto Sans TC', sans-serif;
  transition: border-color 150ms, background-color 200ms, color 200ms;
}

.light input[type="text"],
.light input[type="password"] {
  background-color: #FFFFFF;
  border-color: #D1D5DB;
  color: #0F172A;
}

.dark input[type="text"],
.dark input[type="password"] {
  background-color: #0F172A;
  border-color: #334155;
  color: #F8FAFC;
}

input[type="text"]:focus,
input[type="password"]:focus {
  outline: none;
  border-color: #2563EB;
}

input::placeholder {
  font-weight: 400;
}

.btn {
  width: 100%;
  padding: 12px 20px;
  border: 1px solid;
  border-radius: 4px;
  font-size: 15px;
  font-weight: 700;
  font-family: 'Noto Sans TC', sans-serif;
  cursor: pointer;
  transition: all 150ms ease;
  text-transform: uppercase;
  letter-spacing: 0.1em;
}

.btn-primary {
  background-color: #2563EB;
  border-color: #2563EB;
  color: #FFFFFF;
}

.btn-primary:hover:not(:disabled) {
  background-color: #1E40AF;
  border-color: #1E40AF;
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.theme-toggle {
  position: fixed;
  top: 16px;
  right: 16px;
  padding: 10px 18px;
  background: transparent;
  border: 1px solid;
  border-radius: 4px;
  font-size: 13px;
  font-weight: 700;
  font-family: 'Noto Sans TC', sans-serif;
  cursor: pointer;
  transition: all 150ms ease;
  z-index: 100;
  letter-spacing: 0.05em;
}

.light .theme-toggle {
  border-color: #D1D5DB;
  background-color: #FFFFFF;
  color: #0F172A;
}

.dark .theme-toggle {
  border-color: #334155;
  background-color: #1E293B;
  color: #F8FAFC;
}

.light .theme-toggle:hover {
  background-color: #F5F7FA;
}

.dark .theme-toggle:hover {
  background-color: #334155;
}

@media (max-width: 768px) {
  .login-right {
    display: none;
  }
}
</style>
