# Frontend Dockerfile for NuclearWeb Vue.js 3 Application
# Multi-stage build for optimized production image

# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source files
COPY . .

# Build for production
RUN npm run build

# Production stage
FROM nginx:1.25-alpine AS production
WORKDIR /usr/share/nginx/html

# Remove default nginx static assets
RUN rm -rf ./*

# Copy built assets from build stage
COPY --from=build /app/dist .

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Create non-root user for security
RUN addgroup -g 1001 -S nuclearweb && \
    adduser -S nuclearweb -u 1001

# Set ownership
RUN chown -R nuclearweb:nuclearweb /usr/share/nginx/html && \
    chown -R nuclearweb:nuclearweb /var/cache/nginx && \
    chown -R nuclearweb:nuclearweb /var/log/nginx && \
    chown -R nuclearweb:nuclearweb /etc/nginx/conf.d && \
    touch /var/run/nginx.pid && \
    chown -R nuclearweb:nuclearweb /var/run/nginx.pid

# Switch to non-root user
USER nuclearweb

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/ || exit 1

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
