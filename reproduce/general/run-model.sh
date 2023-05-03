#!/bin/bash
#SBATCH --mem=15000
#SBATCH --output=/dev/null
#SBATCH --partition=naples

TIME_CMD="/usr/bin/time -f \"@@@%M@@@\" "

# THIS SCRIPT IS NOT INTENDED TO BE RUN MANUALLY, LOOK AT 'start.sh' SCRIPT

main () {
    BINARY=$1
    TIME_LIMIT=$2 # in seconds per query
    STRAT_STR="$3"
    MODEL_PATH="$4"
    RESULT_PATH="$5"

    DATE=$(date -u +"%d/%m/%Y %H.%M.%S")
    OUTPUT="model\tquery\tsolved\tresult\tstrategy\ttime\tdate\ttime-limit\tmemory\texit-code\tformula\ttimed-out\terror-msg\tdiscoveredStates\texploredStates\texpandedStates\tmaxTokens\tsearchTime"

    MODEL=$(basename "$MODEL_PATH")

    deserialize_array STRAT_STR STRATEGIES '|'

    for STRAT in "${STRATEGIES[@]}"; do
        for QUERY in $(seq 16); do
            START_TIME=$(date +%s.%N)
            TMP=$($TIME_CMD timeout $TIME_LIMIT $BINARY -n -x $QUERY -s $STRAT $MODEL_PATH/model.pnml $MODEL_PATH/ReachabilityCardinality.xml 2>&1)
            RETVAL=$?

            ERROR_MSG=""
            if [[ "$RETVAL" -ne 0 ]]; then
                ERROR_MSG=$(echo "$TMP" | tr -d '\n' | sed 's|\t|   |g')
            fi

            END_TIME=$(date +%s.%N)
            TIME=$(echo "$END_TIME - $START_TIME" | bc -l)
            if [[ $TIME =~ ^[.] ]]; then
                TIME="0$TIME"
            fi 

            MEM_USED=$(grep -oP "(?<=@@@).*(?=@@@)" <<< "$TMP")

            TIMED_OUT="FALSE"
            if grep -qm 1 'Command exited with non-zero status 124' <<< "$TMP"; then
                TIMED_OUT="TRUE"
            fi

            FORMULA="FALSE"
            if grep -qm 1 'FORMULA' <<< "$TMP"; then
                FORMULA="TRUE"
            fi

            SOLVED="FALSE"
            if grep -qm 1 'was solved' <<< "$TMP"; then
                SOLVED="TRUE"
            fi 

            RESULT="NULL"
            if grep -qm 1 'Query is satisfied.' <<< "$TMP"; then
                RESULT="TRUE"
            elif grep -qm 1 'Query is NOT satisfied.' <<< "$TMP"; then
                RESULT="FALSE"
            fi 

            DISCOVERED_STATES=$(echo "$TMP" | grep -m 1 'discovered states:' | awk '{print $3}')
            EXPLORED_STATES=$(echo "$TMP" | grep -m 1 'explored states:' | awk '{print $3}')
            EXPANDED_STATES=$(echo "$TMP" | grep -m 1 'expanded states:' | awk '{print $3}')
            MAX_TOKENS=$(echo "$TMP" | grep -m 1 'max tokens:' | awk '{print $3}')
            SEARCH_TIME=$(echo "$TMP" | grep -m 1 'search time taken:' | awk '{print $4}')
            OUTPUT+="\n$MODEL\t$QUERY\t$SOLVED\t$RESULT\t$STRAT\t$TIME\t$DATE\t$TIME_LIMIT\t$MEM_USED\t$RETVAL\t$FORMULA\t$TIMED_OUT\t$ERROR_MSG\t$DISCOVERED_STATES\t$EXPLORED_STATES\t$EXPANDED_STATES\t$MAX_TOKENS\t$SEARCH_TIME"
        done
    done

    echo -e "$OUTPUT" > "$RESULT_PATH"
}

deserialize_array() {
    IFS="${3:-$'\x01'}" read -r -a "${2}" <<<"${!1}" # -a => split on IFS
}

main "$@"
