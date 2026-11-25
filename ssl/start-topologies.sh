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

for port in 30000 30001 30002 30003 30010 30011 30012 30013 30014 30015 30020 30021; do
    dir="/data/$port"
    mkdir -p "$dir"
    config="$dir/redis.conf"
    if [ ! -f "$config" ]; then
        {
            printf 'bind 0.0.0.0\nprotected-mode no\ndir %s\nsave ""\nappendonly no\n' "$dir"
            case "$port" in
                30020|30021) printf 'port %s\n' "$port" ;;
                *)
                    printf 'port 0\ntls-port %s\n' "$port"
                    cat <<'TLS'
tls-cert-file /tls/server.pem
tls-key-file /tls/server-key.pem
tls-client-cert-file /tls/client.pem
tls-client-key-file /tls/client-key.pem
tls-ca-cert-file /tls/ca.pem
tls-auth-clients yes
tls-replication yes
TLS
                    ;;
            esac
            case "$port" in
                30001|30002)
                    printf 'requirepass tls-data-test\nmasterauth tls-data-test\n'
                    if [ "$port" = 30002 ]; then
                        printf 'replicaof 127.0.0.1 30001\nreplica-announce-ip 127.0.0.1\nreplica-announce-port 30002\n'
                    fi
                    ;;
                30003)
                    printf 'requirepass tls-sentinel-test\nsentinel monitor tlsmaster 127.0.0.1 30001 1\nsentinel auth-pass tlsmaster tls-data-test\nsentinel down-after-milliseconds tlsmaster 1500\n'
                    ;;
                3001?)
                    printf 'cluster-enabled yes\ntls-cluster yes\ncluster-config-file nodes.conf\ncluster-node-timeout 2000\ncluster-announce-ip 127.0.0.1\ncluster-announce-tls-port %s\n' "$port"
                    ;;
                30021) printf 'replicaof 127.0.0.1 30020\n' ;;
            esac
        } > "$config"
    fi
    if [ "$port" = 30003 ]; then
        redis-server "$config" --sentinel &
    else
        redis-server "$config" &
    fi
    pids="$pids $!"
done
wait
