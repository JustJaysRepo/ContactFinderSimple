using ContactFinder.AdvancedRepl;
using ContactFinder.Core;
using System;
using System.Collections.Generic;
using System.Linq;

var repo = new ContactRepository();

Dictionary<string, CommandSpec>? commands = null;

commands = new Dictionary<string, CommandSpec>(StringComparer.OrdinalIgnoreCase)
{
    ["add"] = new CommandSpec(
        Usage: "add <name> <email> <phone>\n  Example: add \"Alice Johnson\" alice@example.com (555) 123-4567",
        Handler: Add),

    ["find"] = new CommandSpec(
        Usage: "find <email>\n  Example: find alice@example.com",
        Handler: Find),

    ["find-phone"] = new CommandSpec(
        Usage: "find-phone <phone>\n  Example: find-phone 555-123-4567",
        Handler: FindPhone),

    ["list"] = new CommandSpec(
        Usage: "list",
        Handler: List),

    ["remove"] = new CommandSpec(
        Usage: "remove <email>\n  Example: remove alice@example.com",
        Handler: Remove),

    ["help"] = new CommandSpec(
        Usage: "help [command]\n  Examples: help  |  help add",
        Handler: Help),

    ["quit"] = new CommandSpec(Usage: "quit", Handler: _ => false),
    ["exit"] = new CommandSpec(Usage: "exit", Handler: _ => false),
};

Console.WriteLine("Contact Finder (REPL)");
Console.WriteLine("Type 'help' for a list of commands.");
Console.WriteLine();

while (true)
{
    Console.Write("> ");

     var(cmd, parts) = InputParser.Parse(Console.ReadLine());

    if (string.IsNullOrEmpty(cmd))
        continue;

    if (!commands.TryGetValue(cmd, out var spec))
    {
        Console.WriteLine("Unknown command. Type 'help' for a list of commands.");
        continue;
    }

    try
    {
        if (!spec.Handler(parts))
            break;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

// ********** Handlers ********************

bool Add(string[] args)
{
    // Expects at least: <name> <email> <phone>
    // The name may span multiple tokens (e.g. "Alice Johnson" becomes one token
    // via the parser's quote handling, but unquoted first-last also works).
    if (args.Length < 3)
    {
        PrintUsage("add");
        return true;
    }

    // Last arg = phone, second-to-last = email, everything before = name.
    var phone = args[^1];
    var email = args[^2];
    var name = string.Join(' ', args.Take(args.Length - 2));

    repo.Add(new Contact(name, email, phone));
    Console.WriteLine($"Added: {name}");
    return true;
}

bool Find(string[] args)
{
    if (args.Length != 1)
    {
        PrintUsage("find");
        return true;
    }

    var contact = repo.GetByEmail(args[0]);
    Console.WriteLine(contact is null
        ? $"Not found: {args[0]}"
        : FormatContact(contact));

    return true;
}

bool FindPhone(string[] args)
{
    if (args.Length != 1)
    {
        PrintUsage("find-phone");
        return true;
    }

    var contact = repo.GetByPhone(args[0]);
    Console.WriteLine(contact is null
        ? $"Not found: {args[0]}"
        : FormatContact(contact));

    return true;
}

bool List(string[] _)
{
    if (repo.Count == 0)
    {
        Console.WriteLine("No contacts.");
        return true;
    }

    var sorted = repo.GetAll()
                     .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase);

    Console.WriteLine($"Contacts ({repo.Count}):");

    foreach (var c in sorted)
        Console.WriteLine($"  {c.Name,-20}  {c.Email,-28}  {c.Phone}");

    return true;
}

bool Remove(string[] args)
{
    if (args.Length != 1)
    {
        PrintUsage("remove");
        return true;
    }

    bool removed = repo.RemoveByEmail(args[0]);
    Console.WriteLine(removed
        ? $"Removed: {args[0]}"
        : $"Not found: {args[0]}");

    return true;
}

bool Help(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Commands:");

        foreach (var key in commands.Keys.OrderBy(k => k))
            Console.WriteLine($"  {key}");

        Console.WriteLine("\nType 'help <command>' for usage details.");
        return true;
    }

    if (commands.TryGetValue(args[0], out var spec))
        Console.WriteLine(spec.Usage);
    else
        Console.WriteLine($"Unknown command: {args[0]}");

    return true;
}

// ********** Helpers ********************

void PrintUsage(string cmd)
{
    if (commands.TryGetValue(cmd, out var spec))
        Console.WriteLine($"Usage: {spec.Usage}");
}

static string FormatContact(Contact c)
    => $"{c.Name}  <{c.Email}>  {c.Phone}";
