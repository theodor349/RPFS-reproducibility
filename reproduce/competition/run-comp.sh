#!/bin/bash

cd "$(dirname "$0")"
BINS="../bin"
MODELS="../MCC2022"
mkdir -p output/{base,RPFS}

for b in base RPFS ; do 
    for t in Reachability{Cardinality,Fireability} ; do
        for m in $(ls "$MODELS") ; do
            export MODEL_PATH="$MODELS/$m"
            export VERIFYPN="$BINS/$b"
            export BK_EXAMINATION="$t"

            if [ "$b" == "RPFS" ]; then
                export USE_RPFS=true
            else
                export USE_RPFS=false
            fi

            ./tapaal.sh 2> /dev/null > "output/$b/$m.$t"
        done
    done
done
