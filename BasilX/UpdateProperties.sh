#! /bin/bash
# BAT file run when project built to update property files
#    with build time and source version.
date > "$1/BuildDate.txt"
git rev-parse HEAD > "$1/GitCommit.txt"
