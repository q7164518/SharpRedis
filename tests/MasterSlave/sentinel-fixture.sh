#!/bin/sh
set -eu
for port in 17379 17380 17381; do
  mkdir -p /data/$port
  cat > /data/$port/redis.conf <<EOF
port $port
bind 0.0.0.0
protected-mode no
requirepass sharpredis-data-test
masterauth sharpredis-data-test
daemonize yes
pidfile /data/$port/redis.pid
logfile /data/$port/redis.log
dir /data/$port
replica-announce-ip 127.0.0.1
replica-announce-port $port
EOF
  if [ "$port" != 17379 ]; then echo "replicaof 127.0.0.1 17379" >> /data/$port/redis.conf; fi
  redis-server /data/$port/redis.conf
done
for port in 27379 27380 27381; do
  mkdir -p /data/$port
  cat > /data/$port/sentinel.conf <<EOF
port $port
bind 0.0.0.0
protected-mode no
requirepass sharpredis-sentinel-test
daemonize yes
pidfile /data/$port/redis.pid
logfile /data/$port/redis.log
dir /data/$port
sentinel monitor mymaster 127.0.0.1 17379 2
sentinel auth-pass mymaster sharpredis-data-test
sentinel down-after-milliseconds mymaster 1000
sentinel failover-timeout mymaster 5000
sentinel parallel-syncs mymaster 1
sentinel announce-ip 127.0.0.1
sentinel announce-port $port
EOF
  redis-server /data/$port/sentinel.conf --sentinel
done
