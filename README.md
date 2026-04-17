# ContactFinder - C# and .NET Learning Lab

A progressive C# learning project built around a single domain - a contact management CLI - implemented multiple times with increasing complexity, tools, and architectural patterns.

The goal is deliberate repetition: same core problem, escalating architecture.

---

## Project Structure

This repository is organized by weekly progression and architectural complexity.

```
ContactFinderSimple/
|- Week01/
| |- ContactFinder/
| |- src/
| | |- ContactFinder.Core/ # Core domain models and interfaces (records, services)
| | |- ContactFinder.Simple/ # v1 Basic in-memory contact manager (Dictionary, O(1) lookup)
| | |- ContactFinder.AdvancedRepl/ # v2 Interactive REPL with improved parsing and indexing
| | |- ContactFinder.AdvancedCommandLine/ # v3 Command-line app with SQL Server persistence
| | |- ContactFinder.Wpf/                 # v4 WPF desktop GUI (MVVM)
| |
| |- tests/
| | |- ContactFinder.Simple.Tests/
| | |- ContactFinder.AdvancedRepl.Tests/
| | |- ContactFinder.AdvancedCommandLine.Tests/
| | |-ContactFinder.Wpf.Tests
| |
| |- ContactFinder.sln

```

---

## Versions

### v1 - Simple CLI (`ContactFinder.Simple`)

**Focus:** Core C# fundamentals - records, dictionaries, basic I/O, exception handling.

A minimal command-line app that stores contacts (name + email) in memory using a `Dictionary<string, Contact>`. No persistence, no phone numbers, and no advanced parsing - just the essential structure.

**Key concepts practiced:**
- `record` types and value semantics
- `Dictionary<TKey, TValue>` as an in-memory store
- Email normalization (`Trim().ToLowerInvariant()`) for consistent key lookup
- `IReadOnlyCollection<T>` for safe data exposure
- Basic `switch` command dispatch inside a `while (true)` loop

**Commands:**

| Command | Usage |
|--------|------|
| add    | add <name> <email> |
| find   | find <email> |
| list   | list |
| remove | remove <email> |
| quit / exit | Exit the application |

**Limitations (by design):**
- Names with spaces break parsing (e.g., `add Alice Johnson alice@example.com`)
- No phone number support
- No persistence (data is lost on exit)
- Minimal validation beyond constructor guards

---

### v2 - Advanced REPL (`ContactFinder.AdvancedRepl`)

**Focus:** Cleaner architecture, quote-aware parsing, dual-index lookup, command table pattern.

Adds a phone field, normalizes phone numbers to digits-only for consistent lookup,
and replaces the simple `Split()` parser with a proper tokenizer that respects
quoted strings. Command dispatch moves from a `switch` to a
`Dictionary<string, CommandSpec>` for extensibility.

**Key concepts practiced:**
- Referencing a shared project (`ContactFinder.Core`)
- Quote-aware tokenization (`StringBuilder` + state machine)
- Dual dictionary index - email -> contact, phone -> email key for O(1) lookup by either field
- Phone normalization -`(555) 123-4567`, `555-123-4567`, and `5551234567` all resolve to the same record
- Command table pattern (`Dictionary<string, CommandSpec>`) vs. `switch`
- `Func<string[], bool>` as a handler delegate
- Single-responsibility file separation (`InputParser`, `CommandSpec`, `Program`)

| File | Project | Responsibility |
|---|---|---|
| `Contact.cs` | `ContactFinder.Core` | Record with constructor validation and phone normalization |
| `ContactRepository.cs` | `ContactFinder.Core` | Dual-index in-memory store |
| `InputParser.cs` | `ContactFinder.AdvancedRepl` | Quote-aware tokenizer  CLI specific |
| `CommandSpec.cs` | `ContactFinder.AdvancedRepl` | Command name - usage + handler mapping |
| `Program.cs` | `ContactFinder.AdvancedRepl` | REPL loop and command handlers |

**Commands:**

| Command | Usage |
|---|---|
| `add` | `add "Alice Johnson" alice@example.com 555-123-4567` |
| `find` | `find alice@example.com` |
| `find-phone` | `find-phone (555) 123-4567` |
| `list` | `list` |
| `remove` | `remove alice@example.com` |
| `help` | `help` or `help <command>` |
| `quit` / `exit` | Exit the application |

---

### v3 - Command Line + Persistence (`ContactFinder.AdvancedCommandLine`)

**Focus:** Production-style CLI tooling, SQL Server persistence, and the repository pattern.

Replaces the manual REPL loop with `System.CommandLine` for structured argument
parsing. Contacts are now stored in SQL Server instead of memory — data persists
between runs. The SQL logic lives in a shared `ContactFinder.Data` project so
both this version and the WPF version can use it without duplication.

**Key concepts practiced:**
- `System.CommandLine` — named options, automatic `--help`, exit codes
- ADO.NET — `SqlConnection`, `SqlCommand`, `SqlDataReader`
- Parameterized queries — preventing SQL injection with `@Parameter` syntax
- `MERGE` statement — insert or update in a single SQL statement
- `IContactRepository` — interface introduced here so both v3 and v4 depend
  on a contract rather than a concrete class
- Repository pattern — data access isolated behind an interface
- `DbInitializer` — table creation on first run, safe to call every startup
- `appsettings.json` — connection string externalised from code
- `appsettings.Development.json` — local overrides, excluded from Git

**Project files:**

| File | Project | Responsibility |
|---|---|---|
| `Contact.cs` | `ContactFinder.Core` | Record with validation and phone normalization |
| `ContactRepository.cs` | `ContactFinder.Core` | In-memory implementation (used in tests) |
| `IContactRepository.cs` | `ContactFinder.Core` | Contract both repositories implement |
| `DbInitializer.cs` | `ContactFinder.Data` | Creates the Contacts table on first run |
| `SqlContactRepository.cs` | `ContactFinder.Data` | ADO.NET implementation of IContactRepository |
| `Commands.cs` | `ContactFinder.AdvancedCommandLine` | All command definitions and handlers |
| `Program.cs` | `ContactFinder.AdvancedCommandLine` | Bootstrap, configuration, wiring, root command |
| `appsettings.json` | `ContactFinder.AdvancedCommandLine` | Connection string placeholder — committed to Git |
| `appsettings.Development.json` | `ContactFinder.AdvancedCommandLine` | Real local connection string — Git ignored |

**Commands:**

| Command | Usage |
|---|---|
| `add` | `dotnet run -- add --name "Alice Johnson" --email alice@example.com --phone "555-123-4567"` |
| `find` | `dotnet run -- find --email alice@example.com` |
| `find-phone` | `dotnet run -- find-phone --phone "555-123-4567"` |
| `list` | `dotnet run -- list` |
| `remove` | `dotnet run -- remove --email alice@example.com` |
| `--help` | `dotnet run -- --help` or `dotnet run -- add --help` |

**What changed from v2:**

| v2 REPL | v3 Command Line |
|---|---|
| Manual `InputParser` tokenizer | `System.CommandLine` handles all parsing |
| Positional args (`add "Alice" alice@test.com 555...`) | Named options (`--name --email --phone`) |
| `Dictionary<string, CommandSpec>` dispatch | Command tree built with `Command` objects |
| In-memory storage — lost on exit | SQL Server — persists between runs |
| No interface needed | `IContactRepository` introduced |
| Connection config not applicable | `appsettings.json` + `appsettings.Development.json` |

**Prerequisites:**
- SQL Server 2019+ or a named SQL Server instance
- Database must be created manually before first run:
  ```sql
  CREATE DATABASE ContactFinderDb;
  ```
- The `Contacts` table is created automatically by `DbInitializer` on first run
- Connection string configured in `appsettings.Development.json`:
  ```json
  {
    "ConnectionStrings": {
      "Default": "Server=YOUR_SERVER_NAME;Database=ContactFinderDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
  }
  ```

> `appsettings.Development.json` is listed in `.gitignore` and will never be
> committed. `appsettings.json` contains a safe placeholder and is committed.

**Notes on package versions:**

This version was built using preview packages (`System.CommandLine 3.0.0-preview`,
`Microsoft.Extensions.Configuration 11.0.0-preview`). These generate build
warnings against `net9.0` but run correctly. To suppress the warnings add this
to `ContactFinder.AdvancedCommandLine.csproj`:

```xml
<SuppressTfmSupportBuildWarnings>true</SuppressTfmSupportBuildWarnings>
```
