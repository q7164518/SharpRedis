#!/bin/sh
set -eu
for port in 18379 18380 18381 18382 18383 18384 18385; do
  mkdir -p /data/$port
  cat > /data/$port/redis.conf <<EOF
port $port
bind 0.0.0.0
protected-mode no
requirepass sharpredis-cluster-test
masterauth sharpredis-cluster-test
daemonize yes
pidfile /data/$port/redis.pid
logfile /data/$port/redis.log
dir /data/$port
cluster-enabled yes
cluster-config-file nodes.conf
cluster-node-timeout 1000
cluster-announce-ip 127.0.0.1
cluster-announce-port $port
cluster-announce-bus-port $((port + 10000))
replica-announce-ip 127.0.0.1
replica-announce-port $port
EOF
  redis-server /data/$port/redis.conf
done
sleep 1
redis-cli -a sharpredis-cluster-test --no-auth-warning --cluster create 127.0.0.1:18379 127.0.0.1:18380 127.0.0.1:18381 127.0.0.1:18382 127.0.0.1:18383 127.0.0.1:18384 --cluster-replicas 1 --cluster-yes
