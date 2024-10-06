#!/bin/bash

branch_name="$1"

if [[ "$branch_name" =~ ^refs/heads/iter[0-9]+_[a-zA-Z]+$ ]]; then
    exit 0
else
    exit 1
fi
