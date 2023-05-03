#!/bin/bash

cd "$(dirname "$0")"
mkdir -p answers

main () {
    for t in Reachability{Cardinality,Fireability} ; do
        for b in $1 $2 ; do
            grep -hoP '(?<=FORMULA ).*(?= TECHNIQUES)' output/$b/*.$t | sed "s/-$t//g" | sort | uniq > answers/$b.$t
        done

        LEFT=$(comm -13 answers/$1.$t answers/$2.$t | grep -oP '.*(?= (TRUE|FALSE))')
        RIGHT=$(comm -23 answers/$1.$t answers/$2.$t | grep -oP '.*(?= (TRUE|FALSE))')
        echo "DIFFERENCES IN $t : "

        comm -12 <(echo "$LEFT" | sort) <(echo "$RIGHT" | sort) | awk 'NF'
        C1=$(wc -l < answers/$1.$t)
        C2=$(wc -l < answers/$2.$t)
        echo "$1.$t -> $C1"
        echo "$2.$t -> $C2"
    done
}

main base RPFS | tee stats.txt
