#!/usr/bin/env python3
"""Wipe the dev database, then seed it with foods, meals and meal plans via the running API.

Usage: python scripts/seed.py [base_url]   (default http://localhost:5192)

DESTRUCTIVE: deletes all foods, meals and meal plans in the dev DB container before seeding.
Schema and migration history are kept, so the API can stay running.
"""
import json
import subprocess
import sys
import urllib.error
import urllib.request

BASE_URL = (sys.argv[1] if len(sys.argv) > 1 else "http://localhost:5192").rstrip("/")
DB_CONTAINER = "meal-planner-dev-db"
DB_NAME = "meal-planner"
DB_USER = "postgres"

SOURCE = "https://www.nutritionix.com"  # API requires an http(s) URL; values below are approximate

# name, unit, reference qty, kcal, protein, carbs, sugar, fat, sat fat, fibre, salt
FOODS = [
    ("Rolled Oats", "Grams", 100, 375, 11.0, 60.0, 1.0, 8.0, 1.5, 9.0, 0.01),
    ("Semi-Skimmed Milk", "Grams", 100, 50, 3.6, 4.8, 4.8, 1.8, 1.1, 0.0, 0.1),
    ("Banana", "Count", 1, 105, 1.3, 27.0, 14.0, 0.4, 0.1, 3.1, 0.0),
    ("Blueberries", "Grams", 100, 57, 0.7, 14.5, 10.0, 0.3, 0.0, 2.4, 0.0),
    ("Greek Yoghurt", "Grams", 100, 97, 9.0, 3.9, 3.9, 5.0, 3.4, 0.0, 0.1),
    ("Honey", "Grams", 100, 304, 0.3, 82.4, 82.1, 0.0, 0.0, 0.2, 0.01),
    ("Egg", "Count", 1, 78, 6.3, 0.6, 0.6, 5.3, 1.6, 0.0, 0.2),
    ("Wholemeal Bread", "Grams", 100, 247, 13.0, 41.0, 6.0, 3.4, 0.7, 7.0, 1.0),
    ("Butter", "Grams", 100, 717, 0.9, 0.1, 0.1, 81.0, 51.0, 0.0, 1.6),
    ("Chicken Breast", "Grams", 100, 165, 31.0, 0.0, 0.0, 3.6, 1.0, 0.0, 0.2),
    ("Basmati Rice", "Grams", 100, 350, 8.0, 77.0, 0.3, 1.0, 0.2, 1.3, 0.0),
    ("Broccoli", "Grams", 100, 34, 2.8, 7.0, 1.7, 0.4, 0.1, 2.6, 0.1),
    ("Olive Oil", "Grams", 100, 884, 0.0, 0.0, 0.0, 100.0, 14.0, 0.0, 0.0),
    ("Salmon Fillet", "Grams", 100, 208, 20.0, 0.0, 0.0, 13.0, 3.1, 0.0, 0.1),
    ("Sweet Potato", "Grams", 100, 86, 1.6, 20.0, 4.2, 0.1, 0.0, 3.0, 0.1),
    ("Lean Beef Mince", "Grams", 100, 176, 20.0, 0.0, 0.0, 10.0, 4.5, 0.0, 0.2),
    ("Spaghetti", "Grams", 100, 359, 12.5, 71.0, 3.5, 1.5, 0.3, 3.0, 0.01),
    ("Chopped Tomatoes", "Grams", 100, 21, 1.1, 3.8, 3.8, 0.1, 0.0, 0.9, 0.05),
    ("Onion", "Count", 1, 44, 1.2, 10.0, 4.7, 0.1, 0.0, 1.9, 0.0),
    ("Cheddar Cheese", "Grams", 100, 416, 25.0, 0.1, 0.1, 34.9, 21.7, 0.0, 1.8),
    ("Tortilla Wrap", "Count", 1, 190, 5.2, 31.0, 1.5, 4.6, 1.8, 2.0, 0.7),
    ("Spinach", "Grams", 100, 23, 2.9, 3.6, 0.4, 0.4, 0.1, 2.2, 0.2),
    ("Peanut Butter", "Grams", 100, 588, 25.0, 20.0, 9.0, 50.0, 10.0, 6.0, 0.4),
    ("Apple", "Count", 1, 95, 0.5, 25.0, 19.0, 0.3, 0.1, 4.4, 0.0),
]

# meal name -> [(food name, quantity)]; quantity is in the food's unit (grams or count)
MEALS = {
    "Porridge with Banana": [("Rolled Oats", 60), ("Semi-Skimmed Milk", 250), ("Banana", 1), ("Honey", 10)],
    "Yoghurt and Berries": [("Greek Yoghurt", 200), ("Blueberries", 80), ("Honey", 10)],
    "Scrambled Eggs on Toast": [("Egg", 3), ("Wholemeal Bread", 80), ("Butter", 10), ("Semi-Skimmed Milk", 30)],
    "Chicken, Rice and Broccoli": [("Chicken Breast", 200), ("Basmati Rice", 75), ("Broccoli", 120), ("Olive Oil", 10)],
    "Salmon and Sweet Potato": [("Salmon Fillet", 150), ("Sweet Potato", 250), ("Spinach", 80), ("Olive Oil", 5)],
    "Spaghetti Bolognese": [("Lean Beef Mince", 150), ("Spaghetti", 90), ("Chopped Tomatoes", 200), ("Onion", 1), ("Cheddar Cheese", 15)],
    "Chicken Wrap": [("Tortilla Wrap", 2), ("Chicken Breast", 120), ("Spinach", 30), ("Cheddar Cheese", 20)],
    "Apple and Peanut Butter": [("Apple", 1), ("Peanut Butter", 30)],
}

# plan name -> list of entries; a str copies that template meal, a tuple is a plan-specific meal
MEAL_PLANS = {
    "High Protein Day": [
        "Scrambled Eggs on Toast",
        "Chicken, Rice and Broccoli",
        ("Protein Yoghurt Bowl", [("Greek Yoghurt", 300), ("Peanut Butter", 15), ("Blueberries", 50)]),
        "Salmon and Sweet Potato",
    ],
    "Busy Weekday": [
        "Porridge with Banana",
        "Chicken Wrap",
        "Apple and Peanut Butter",
        ("Quick Egg Fried Rice", [("Basmati Rice", 80), ("Egg", 2), ("Onion", 1), ("Olive Oil", 10)]),
    ],
    "Weekend Treat": [
        ("Big Breakfast Toast", [("Egg", 2), ("Wholemeal Bread", 120), ("Butter", 15), ("Cheddar Cheese", 30)]),
        "Yoghurt and Berries",
        "Spaghetti Bolognese",
    ],
}


def post(path, body):
    """Return the new public id, or the existing one if the name is already taken."""
    req = urllib.request.Request(
        f"{BASE_URL}{path}", data=json.dumps(body).encode(), headers={"Content-Type": "application/json"}, method="POST"
    )
    try:
        with urllib.request.urlopen(req) as res:
            return json.load(res)  # the API returns the new public id as a JSON string
    except urllib.error.HTTPError as e:
        sys.exit(f"POST {path} '{body['name']}' failed ({e.code}): {e.read().decode()}")


def wipe():
    # CASCADE clears Ingredients, MealPlanMeals and MealPlanIngredients too
    sql = 'TRUNCATE "Foods", "Meals", "MealPlans" RESTART IDENTITY CASCADE;'
    subprocess.run(
        ["docker", "exec", DB_CONTAINER, "psql", "-v", "ON_ERROR_STOP=1", "-U", DB_USER, "-d", DB_NAME, "-c", sql],
        check=True,
    )


def ingredients(items, food_ids):
    return [{"foodId": food_ids[name], "quantity": qty} for name, qty in items]


def main():
    wipe()
    print("Wiped database")

    keys = ["unit", "referenceQuantity", "calories", "protein", "carbohydrates", "sugar", "fat", "saturatedFat", "fibre", "salt"]
    food_ids = {
        name: post("/api/foods", {"name": name, **dict(zip(keys, values)), "source": SOURCE})
        for name, *values in FOODS
    }
    print(f"Added {len(food_ids)} foods")

    for name, items in MEALS.items():
        post("/api/meals", {"name": name, "ingredients": ingredients(items, food_ids)})
    print(f"Added {len(MEALS)} meals")

    for plan_name, entries in MEAL_PLANS.items():
        meals = [(e, MEALS[e]) if isinstance(e, str) else e for e in entries]
        post("/api/meal-plans", {"name": plan_name, "meals": [{"name": n, "ingredients": ingredients(i, food_ids)} for n, i in meals]})
    print(f"Added {len(MEAL_PLANS)} meal plans")


if __name__ == "__main__":
    main()
