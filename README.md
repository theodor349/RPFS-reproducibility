# RPFS Reproducability
The models which were used to benchmark RPFS can be found at `reproduce/MCC2022.tar.gz`.

The data file which all tables and plots are based on is found here: `latex/data_nonreduced.csv`.
The binary used for the benchmark is found at `reproduce/bin/RPFS.bin`, and all code for it can be found at [theodor349/P7-verifypn](https://github.com/theodor349/P7-verifypn/tree/RPFS).

## Guides
- [Generate all tables and figures](#generate-all-tables-and-figures)
- [Generate table 1, figures 3 + 4 (from scratch)](#generate-table-1-figures-3--4-from-scratch)
- [Generate table 2 (from scratch)](#generate-table-2-from-scratch)
- [Average expanded states](#average-expanded-states)

## Generate all tables and figures
Follow the guide to produce all tables and figures, using precomputed results.

### Requirements
 - Git LFS
 - .NET 6 Runtime + SDK
 - 4 GB free storage space

### Guide
    git clone https://github.com/theodor349/RPFS-reproducibility
    cd RPFS-reproducability
    git lfs pull
    cd latex
    ./run-cdpr.sh 

## Generate table 1, figures 3 + 4 (from scratch)

> **Warning**
>
> The results used in the paper were computed using a CPU cluster. 
> This will take a *long* time to run on a single CPU. (~1 week)

### Requirements 
 - OS: Linux
 - Git LFS
 - time(1), timeout(1), bc(1), awk(1)
 - .NET 6 runtime + SDK
 - 25 GB free storage space
 - 16 GB RAM

### Guide 
    git clone https://github.com/theodor349/RPFS-reproducibility
    cd RPFS-reproducability
    git lfs pull
    cd reproduce
    tar xf MCC2022.tar.gz       # Extract the MCC models
    cd general 
    ./start.sh                  # Generate all data (all models, all strategies)
    ./archiver.sh               # Combine all data into a single `data.csv` file
    cd ../..
    mv reproduce/general/results/data.csv latex/data_nonreduced.csv
    cd latex
    ./run-cdpr.sh

## Generate table 2 (from scratch)
Run the competition benchmark with and without RPFS, and produce a table comparing the results.

> **Warning**
>
> The results used in the paper were computed using a CPU cluster. 
> This will take a *very long* time to run on a single CPU. (~2 weeks)

### Requirements 
 - OS: Linux
 - Git LFS
 - time(1), timeout(1), parallel(1), bc(1), awk(1)
 - .NET 6 runtime + SDK
 - 25 GB free storage space
 - 16 GB RAM

### Guide 
    git clone https://github.com/theodor349/RPFS-reproducibility
    cd RPFS-reproducability
    git lfs pull
    cd reproduce
    tar xf MCC2022.tar.gz       # Extract the MCC models
    cd competition
    ./run-comp.sh               # Start the competition benchmark
    ./compare.sh                # Check consistency and write statistics to `stats.txt`
    cd ../..
    mv reproduce/competition/stats.txt latex/CompetitionResults.txt
    cd latex
    ./run-cdpr.sh

## Average expanded states
Calculate the average number of expanded markings when RPFS is run 100000 times on the Petri net in figure&nbsp;1, with the query `p3 >= 20`.

Note: this may take up to 30 minutes to run (depending on hardware).

### Requirements 
 - OS: Linux
 - GNU bc(1)

### Guide 
    git clone https://github.com/theodor349/RPFS-reproducibility
    cd RPFS-reproducability
    cd reproduce/expanded-states
    ./exp.sh
