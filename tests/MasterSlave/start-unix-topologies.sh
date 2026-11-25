#!/bin/sh
set -eu

pids=""
shutdown() {
    trap '' TERM INT
    for pid in $pids; do kill -TERM "$pid" 2>/dev/null || true; done
    wait || true
    exit 0
}
trap shutdown TERM INT

for port in 30101 30102 30103 30110 30111 30112 30113 30114 30115; do
    dir="/data/$port"
    mkdir -p "$dir"
    config="$dir/redis.conf"
    if [ ! -f "$config" ]; then
        {
            printf 'bind 127.0.0.1\nport %s\ndir %s\nsave ""\nappendonly no\n' "$port" "$dir"
            printf 'unixsocket /tmp/redis-%s.sock\nunixsocketperm 770\n' "$port"
            case "$port" in
                30102) printf 'replicaof 127.0.0.1 30101\n' ;;
                30103)
                    printf 'sentinel monitor unixmaster 127.0.0.1 30101 1\nsentinel down-after-milliseconds unixmaster 1500\n'
                    ;;
                3011?) printf 'cluster-enabled yes\ncluster-config-file nodes.conf\ncluster-node-timeout 2000\ncluster-announce-ip 127.0.0.1\n' ;;
            esac
        } > "$config"
    fi
    if [ "$port" = 30103 ]; then
        redis-server "$config" --sentinel &
    else
        redis-server "$config" &
    fi
    pids="$pids $!"
done
wait
