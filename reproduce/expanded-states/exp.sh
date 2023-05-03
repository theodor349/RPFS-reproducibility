#!/bin/bash

cd "$(dirname "$0")"
BINARY="../bin/RPFS"

N=100000
sum=0
i=1
while [ "$i" -le "$N" ]; do
    sum=$((sum + $($BINARY -n -r 0 -x 1 -s RPFS --seed-offset "$i" model.pnml query.xml | grep expanded | awk '{print $3}')))
    i=$((i + 1))
done

echo "N: $N"
echo -n "avg: " ; echo "$sum / $N" | bc -l
