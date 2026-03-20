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

**Focus:** Improved command parsing and interactive experience.

Planned improvements:
- Quote-aware parsing (supports names with spaces)
- Better command handling structure
- Additional indexing (e.g., phone number lookup)
- Improved user feedback and error handling

---

### v3 - Command Line + Persistence (`ContactFinder.AdvancedCommandLine`)

**Focus:** Real-world CLI tooling and persistence.

Planned improvements:
- Structured CLI using `System.CommandLine`
- SQL Server integration for persistent storage
- Separation of concerns (data layer, services, CLI layer)
- More robust validation and error handling