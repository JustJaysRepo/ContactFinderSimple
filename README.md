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

**Focus:** Real-world CLI tooling and persistence.

Planned improvements:
- Structured CLI using `System.CommandLine`
- SQL Server integration for persistent storage
- Separation of concerns (data layer, services, CLI layer)
- More robust validation and error handling