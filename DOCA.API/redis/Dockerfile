# Sử dụng Redis image chính thức
FROM redis:latest

# Copy file cấu hình redis.conf (nếu có)
COPY redis.conf /usr/local/etc/redis/redis.conf

# Mở cổng Redis
EXPOSE 6379

# Chạy Redis với file config (hoặc mặc định nếu không có)
CMD ["redis-server", "/usr/local/etc/redis/redis.conf"]
