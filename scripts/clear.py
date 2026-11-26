#!/usr/bin/env python3
"""Wipe all foods, meals and meal plans from the dev database.

Usage: python scripts/clear.py

DESTRUCTIVE. Schema and migration history are kept, so the API can stay running.
Uses the same SEED_DB_CONTAINER, SEED_DB_NAME and SEED_DB_USER overrides as seed.py, e.g. for the deployed dev server:
    DOCKER_HOST=ssh://<user>@<host> SEED_DB_CONTAINER=<container> SEED_DB_NAME=meal_planner SEED_DB_USER=meal_planner \\
        python scripts/clear.py
"""
from seed import wipe

if __name__ == "__main__":
    wipe()
    print("Wiped database")
