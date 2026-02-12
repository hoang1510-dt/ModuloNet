# ModuloNet

**Modular Monolith · Vertical Slice · Opinionated Minimal .NET**

A minimal, opinionated .NET template for building APIs with enforced boundaries. One feature = one folder = one mental unit. No layers to chase, no generic repositories, no premature architecture.

[![.NET](https://github.com/hoangnam-dt/ModuloNet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hoangnam-dt/ModuloNet/actions/workflows/dotnet.yml) [![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## Why ModuloNet?

**If you mix features by layer** (Controllers / Services / Repositories / DTOs), you end up jumping across six folders to understand one feature. Refactoring gets expensive, so it stops—and the architecture rots.

**Vertical slice avoids this.** Each feature lives in one place. You can read, change, and later extract it without touching the rest of the system.

ModuloNet is **open source**. Use it, fork it, and adapt it. We optimize for clarity and cognitive load, not cleverness.

---

## Getting Started

### Install the template

```bash
# From this repo (when published as a template)
dotnet new install ModuloNet

# Or clone and install from path
git clone https://github.com/hoangnam-dt/ModuloNet.git
cd ModuloNet
dotnet new install ./
```

### Create a new project

```bash
dotnet new modulonet -n YourAppName
cd YourAppName
dotnet restore
dotnet run --project src/ModuloNet.Api
```

Replace `YourAppName` with your solution name; project names will follow (e.g. `YourAppName.Api`, `YourAppName.Application`).

---

## Folder Structure

Minimal. Opinionated. Enforced boundaries.

```
/src
├── ModuloNet.Api
│   ├── Endpoints
│   ├── Middlewares
│   ├── Filters
│   ├── Auth
│   ├── Swagger
│   └── Program.cs
│
├── ModuloNet.Application
│   ├── Features
│   │   ├── Courses
│   │   │   ├── Create
│   │   │   │   ├── Command.cs
│   │   │   │   ├── Handler.cs
│   │   │   │   ├── Validator.cs
│   │   │   │   └── Endpoint.cs
│   │   │   ├── GetById
│   │   │   └── Delete
│   │   └── Users
│   │
│   ├── Abstractions
│   │   ├── IApplicationDbContext.cs
│   │   ├── ICurrentUser.cs
│   │   └── IDateTimeProvider.cs
│   │
│   └── Behaviors
│         ├── ValidationBehavior.cs
│         ├── LoggingBehavior.cs
│         └── TransactionBehavior.cs
│
├── ModuloNet.Domain (optional)
│   ├── Entities
│   ├── ValueObjects
│   ├── DomainEvents
│   └── Exceptions
│
├── ModuloNet.Infrastructure
│   ├── Persistence
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations
│   │   ├── Migrations
│   │   └── Seed
│   ├── Identity
│   ├── Services
│   ├── Dapper
│   └── DependencyInjection.cs
│
└── ModuloNet.Tests
```

**Why this works:** one feature = one folder = one mental unit. No cross-folder hunting.

---

## Rules the Template Enforces

Templates that don’t enforce rules turn into chaos. These are the rules ModuloNet is built around.

### RULE 1 — No cross-feature dependency

`Features/Courses` cannot directly call `Features/Users`.

- If they need something: use an interface in **Abstractions**, or publish a domain event.
- Cross-calling features = hidden coupling. The question to ask: *“Will I regret this in 12 months?”*

### RULE 2 — No repository pattern (EF is already one)

- No `ICourseRepository`, no generic repository, no custom `UnitOfWork`.
- EF Core `DbContext` already tracks changes and handles transactions. Extra repositories add indirection without benefit.

### RULE 3 — Read and write separation

- **Writes** → EF Core.
- **Reads** → Dapper (direct SQL) where it matters.

Using EF for heavy dashboard/read queries often means complex joins and tracking overhead. Separation is built in from day one.

### RULE 4 — One feature = one entry point

Every feature exposes exactly one of:

- **Command** (write)
- **Query** (read)

If one “feature” has seven methods in one handler, it’s not one feature—split it. We optimize for clarity.

### RULE 5 — Infrastructure never leaks upward

- Application defines interfaces: `IApplicationDbContext`, `ICurrentUser`, `IEmailSender`.
- Infrastructure implements them.
- API never references EF or infrastructure details directly.

That keeps the option to extract a feature into a service, replace the database, or test in isolation.

### RULE 6 — Domain is earned, not forced

Start **without** `ModuloNet.Domain`.

Introduce it only when:

- Aggregates grow
- Invariants get complex
- Business rules no longer fit neatly in handlers

Premature DDD is optional complexity; add it when the problem demands it.

### RULE 7 — No magic base classes

No `BaseEntity`, `BaseService`, `BaseController`, `BaseHandler`.

Base classes become dumping grounds. Prefer composition over inheritance.

---

## What We Intentionally Do *Not* Support

Great templates are defined by what they reject. ModuloNet stays focused.

| Not included | Why |
|--------------|-----|
| **Multi-tenancy (by default)** | Pollutes every query, complicates migrations, increases test surface. Add it only if the business truly needs it. |
| **Generic CRUD generator** | No reflection-based CRUD or dynamic filtering engines. They encourage anemic models and database-driven design. |
| **Event bus by default** | No Kafka, RabbitMQ, or MassTransit out of the box. Add when you actually need async decoupling. |
| **Microservices template** | Start as a monolith. The vertical slice layout makes extraction to a service straightforward when the time comes. |
| **Clean Architecture “layers” overload** | No Application / Domain / Infrastructure / Persistence / Shared / Common / Contracts / CrossCutting sprawl unless complexity justifies it. We optimize for cognitive load per feature. |

---

## How This Template Evolves

| Stage | Shape |
|-------|--------|
| **1 — Solo API** | Small team, single DB, single deployable. This is where you start. |
| **2 — Feature gets heavy** | Extract that feature into e.g. `ModuloNet.Courses.Service`. Vertical slices are already portable. |
| **3 — System grows** | Add distributed caching, background jobs, or an event bus when you need them—not before. |

Start simple. Add structure when the problem asks for it.

---

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or the version specified in the repo)

---

## Contributing

Contributions are welcome. Open an issue or a pull request. Please keep changes aligned with the rules and philosophy above.

---

## License

This project is open source under the [MIT License](LICENSE).
