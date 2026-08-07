# Expenses App

A personal income & expense tracker built with ASP.NET Core Blazor.

This is my **first project build entirely with AI** (Claude Code, Anthropic's coding agent) — from the initial architecture and database design through every feature, the UI, tests, and this README. I described what I wanted and reviewed the results; the AI wrote the code. I'm keeping this note here on purpose, as an honest record of how the project came together.

## What it does

- Track income and expense **transactions**, filterable by type, category, and date range
- Organize transactions into user-defined **categories** (e.g. Groceries, Salary)
- Set monthly **budgets** per category and see spend-vs-budget progress
- View a monthly **report** with income/expense breakdowns by category
- A **dashboard** summarizing the current month at a glance
- Per-user accounts via login/registration (each user only sees their own data)

## Tech stack

| Layer | Technology |
|---|---|
| UI | Blazor Server (Interactive Server render mode), Bootstrap 5 + a custom design system |
| Backend | ASP.NET Core 10 |
| Data access | Entity Framework Core 10 + SQLite |
| Auth | ASP.NET Core Identity |
| Validation | FluentValidation |
| Tests | xUnit, FluentAssertions, NSubstitute |
| Package management | NuGet Central Package Management (`Directory.Packages.props`) |

### Architecture

The solution follows a layered ("Clean Architecture"-style) structure so business rules don't depend on EF Core, Blazor, or any other framework detail:

```
src/
  ExpensesApp.Domain/           Entities, enums, and domain rules — no dependencies
  ExpensesApp.Application/      Use cases (commands/queries + handlers), DTOs, validation
  ExpensesApp.Infrastructure/   EF Core DbContext, migrations, Identity, EF configurations
  ExpensesApp.Web/              Blazor Server UI, Program.cs, pages/components

tests/
  ExpensesApp.Domain.UnitTests/
  ExpensesApp.Application.UnitTests/
  ExpensesApp.Infrastructure.IntegrationTests/
  ExpensesApp.Web.IntegrationTests/
```

Each feature (e.g. "Create Transaction") lives together as a command/query + validator + handler under `Application/Features/...`, and Blazor pages call those handlers directly.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (see `global.json` for the exact pinned version)

### Run it

```bash
# 1. Restore & build
dotnet build ExpensesApp.slnx

# 2. Create the local SQLite database
dotnet ef database update --project src/ExpensesApp.Infrastructure --startup-project src/ExpensesApp.Web

# 3. Run the app
dotnet run --project src/ExpensesApp.Web
```

Then open **https://localhost:7260** (or the URL printed in the console) and register a new account to get started.

> If step 2 fails with `dotnet-ef: command not found`, install the tool first: `dotnet tool install --global dotnet-ef`.

### Run the tests

```bash
dotnet test ExpensesApp.slnx
```

## Project structure at a glance

- **Domain** — `Transaction`, `Category`, `Budget` entities with validation baked into their constructors/setters, plus domain-specific exceptions.
- **Application** — one folder per feature (`Features/Transactions`, `Features/Budgets`, ...), each containing a command/query record, a FluentValidation validator, and a handler class.
- **Infrastructure** — `ApplicationDbContext` (EF Core + ASP.NET Identity), entity configurations, and migrations.
- **Web** — Blazor Server pages under `Components/Pages`, using constructor-injected handlers directly (no separate API layer, since UI and backend live in the same process).

## Why this exists

I wanted to learn how a real, working ASP.NET Core app is put together — project structure, EF Core, auth, validation, testing — by watching an AI build one from scratch and iterating on it with real requests, the same way I'd work with a human collaborator: describing features, reporting bugs, and asking for design changes. Every commit in this repo's history reflects that back-and-forth.
