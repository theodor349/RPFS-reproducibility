#!/bin/bash

cd "$(dirname "$0")"
RESULTS_PATH="./results"

aggregate_files() {
    ALL_ROWS=$(awk FNR-1 "$RESULTS_PATH"/*.csv 2>/dev/null | sort -t$'\t' -k6,6 -n) 
    STRATS=$(echo "$ALL_ROWS" | awk -F '\t' '{print $5}' | sort | uniq)

    OUTPUT=""
    while read -r STRAT; do
        [ -z "$STRAT" ] && continue
        ROWS=$(echo "$ALL_ROWS" | grep -P "\t$STRAT\t")
        OUTPUT+="$ROWS\n"
    done <<< "$STRATS"

    echo -e "$(header)\n$OUTPUT" | head -n-1
}

header() {
    echo -e "model\tquery\tsolved\tresult\tstrategy\ttime\tdate\ttime-limit\tmemory\texit-code\tformula\ttimed-out\terror-msg\tdiscoveredStates\texploredStates\texpandedStates\tmaxTokens\tsearchTime"
}

if [[ -f "$RESULTS_PATH/data.csv" ]]; then 
    rm "$RESULTS_PATH/data.csv"
fi
aggregate_files > "$RESULTS_PATH/data.csv"
