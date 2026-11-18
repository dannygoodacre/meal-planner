# API changes: DannyGoodacre.Cqrs 1.0.0-alpha.2 rewrite

Compared with commit `2bd7675`. Routes, HTTP methods and success responses are unchanged: `POST` returns `201` with the new id, `GET` and `PUT` return `200`, and `DELETE` returns `204`.

The frontend has already been updated for the request shape changes (`schema.ts`, `AddNewMealPlan.tsx`, `MealPlanEditor.tsx`).

## Breaking changes

### Meal plan meals no longer reference template meals

Meal plans hold their own copy of each meal. To add a meal from a template, copy its name and ingredients into the request.

`POST /api/mealplan`, each item in `meals`:

| Field | Before | Now |
| --- | --- | --- |
| `mealId` | Optional. Copied the template meal's name and ingredients on the server. | Removed |
| `name` | Nullable. Had to be null when `mealId` was set. | Required |
| `ingredients` | Had to be empty when `mealId` was set. | Required, non-empty |

`PUT /api/mealplan/{id}`, each item in `meals`:

| Field | Before | Now |
| --- | --- | --- |
| `templateMealId` | Optional. Copied a template meal, with optional name and ingredient overrides. | Removed |
| `id` | Optional. Updates an existing meal in the plan; null or empty adds a new one. | Unchanged |
| `name` | Required unless `templateMealId` was set | Required |
| `ingredients` | Could be empty when `templateMealId` was set | Required, non-empty |

Unknown JSON properties are ignored, so a client that still sends `mealId` or `templateMealId` doesn't get an error for that field. But it gets `400` if it relied on the template to fill in `name` or `ingredients`.

### Food `calories` is an integer

`calories` in every food response (`GET /api/food/{id}`, food search, and each `food` nested in meal and meal plan responses) is now an integer. Requests already took an integer.

The `FoodCaloriesInt` migration converts existing values, and Postgres rounds them half away from zero (52.5 becomes 53).

## Status code changes

The "Before" codes come from reading the old code; I didn't run them against it. The two meal plan `Restrict` failures were reproduced against the old database model.

| Request | Before | Now |
| --- | --- | --- |
| `DELETE /api/food/{id}` for a food used by a meal or meal plan | `500` (foreign key violation) | `409` "The food is used by a meal or meal plan" |
| `PUT /api/food/{id}` renaming to another food's name | `500` (unique index) | `409` |
| `PUT /api/meal/{id}` renaming to another meal's name | `500` (unique index) | `409` |
| `PUT /api/meal/{id}` that removes an ingredient | `500` (collection modified during enumeration) | `200` |
| `POST /api/mealplan` with a food id that doesn't exist | `500` (the missing-food check never fired) | `422` |
| `PUT /api/mealplan/{id}` that removes a meal from the plan | `500` (`Restrict` delete behaviour) | `200` |
| `DELETE /api/mealplan/{id}` for a plan that has meals | `500` (`Restrict` delete behaviour) | `204` |
| The same food twice in one meal's `ingredients` (meal or meal plan, add or update) | `500` (duplicate key) | `400` |
| `PUT /api/food/{id}` or `PUT /api/meal/{id}` with an empty GUID | `404` | `400` |
| `PUT /api/meal/{id}` with an invalid body (blank name, no ingredients, empty food id, quantity ≤ 0) | Not validated: a blank name, no ingredients or quantity ≤ 0 were saved; an empty food id gave `422` | `400` |

## Validation error bodies

Still `ValidationProblemDetails` with `errors` keyed by field name.

- `POST /api/meal`: errors for an ingredient's food id or quantity are now keyed `FoodId` and `Quantity`, not `Ingredients`. This matches the meal plan endpoints.
- New error, keyed `Ingredients`: "Must not contain the same food more than once."
- A blank required string now says "Must not be null or empty." instead of "Must not be null, empty, or whitespace."

## Error message text

`409` and `422` bodies are plain strings. These changed:

| Request | Before | Now |
| --- | --- | --- |
| `POST /api/meal` name conflict | "A meal with the same name already exists." | "A meal with the same name already exists" |
| `PUT /api/mealplan/{id}` name conflict | "A meal plan already exists by that name" | "A meal plan with the same name already exists" |
| `PUT /api/mealplan/{id}` unknown food | "The following foods were not found: …" | "The following foods are missing: …" |
| Template meal not found (add or update meal plan) | `422` "The following … meals … were not found …" | Removed along with templates |
