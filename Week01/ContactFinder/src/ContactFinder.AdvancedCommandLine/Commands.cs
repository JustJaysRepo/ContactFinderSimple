using System;
using System.CommandLine;
using System.Linq;
using ContactFinder.Core;

namespace ContactFinder.AdvancedCommandLine
{
    public static class Commands
    {
        // ─── add ──────────────────────────────────────────────────────────────
        // Usage: contactfinder add --name "Alice Johnson" --email alice@example.com --phone "555-123-4567"
        public static Command BuildAddCommand(IContactRepository repo)
        {
            var nameOption = new Option<string>("--name") { Required = true, Description = "Contact name" };
            var emailOption = new Option<string>("--email") { Required = true, Description = "Email address" };
            var phoneOption = new Option<string>("--phone") { Required = true, Description = "Phone number" };

            var command = new Command("add") { Description = "Add or update a contact" };
            command.Add(nameOption);
            command.Add(emailOption);
            command.Add(phoneOption);

            command.SetAction(parseResult =>
            {
                var name = parseResult.GetValue(nameOption);
                var email = parseResult.GetValue(emailOption);
                var phone = parseResult.GetValue(phoneOption);

                try
                {
                    repo.Add(new Contact(name!, email!, phone!));
                    Console.WriteLine($"Added: {name}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });

            return command;
        }

        // ─── find ─────────────────────────────────────────────────────────────
        // Usage: contactfinder find --email alice@example.com
        public static Command BuildFindCommand(IContactRepository repo)
        {
            var emailOption = new Option<string>("--email") { Required = true, Description = "Email address" };

            var command = new Command("find") { Description = "Find a contact by email" };
            command.Add(emailOption);

            command.SetAction(parseResult =>
            {
                var email = parseResult.GetValue(emailOption);
                var contact = repo.FindByEmail(email!);

                Console.WriteLine(contact is null
                    ? $"Not found: {email}"
                    : FormatContact(contact));
            });

            return command;
        }

        // ─── find-phone ───────────────────────────────────────────────────────
        // Usage: contactfinder find-phone --phone "555-123-4567"
        public static Command BuildFindPhoneCommand(IContactRepository repo)
        {
            var phoneOption = new Option<string>("--phone") { Required = true, Description = "Phone number" };

            var command = new Command("find-phone") { Description = "Find a contact by phone number" };
            command.Add(phoneOption);

            command.SetAction(parseResult =>
            {
                var phone = parseResult.GetValue(phoneOption);

                try
                {
                    var contact = repo.FindByPhone(phone!);
                    Console.WriteLine(contact is null
                        ? $"Not found: {phone}"
                        : FormatContact(contact));
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });

            return command;
        }

        // ─── list ─────────────────────────────────────────────────────────────
        // Usage: contactfinder list
        public static Command BuildListCommand(IContactRepository repo)
        {
            var command = new Command("list") { Description = "List all contacts" };

            command.SetAction(_ =>
            {
                if (repo.Count == 0)
                {
                    Console.WriteLine("No contacts.");
                    return;
                }

                var sorted = repo.GetAll()
                                 .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase);

                Console.WriteLine($"Contacts ({repo.Count}):");

                foreach (var c in sorted)
                    Console.WriteLine($"  {c.Name,-20}  {c.Email,-28}  {c.Phone}");
            });

            return command;
        }

        // ─── remove ───────────────────────────────────────────────────────────
        // Usage: contactfinder remove --email alice@example.com
        public static Command BuildRemoveCommand(IContactRepository repo)
        {
            var emailOption = new Option<string>("--email") { Required = true, Description = "Email address" };

            var command = new Command("remove") { Description = "Remove a contact by email" };
            command.Add(emailOption);

            command.SetAction(parseResult =>
            {
                var email = parseResult.GetValue(emailOption);
                bool removed = repo.RemoveByEmail(email!);

                Console.WriteLine(removed
                    ? $"Removed: {email}"
                    : $"Not found: {email}");
            });

            return command;
        }

        // ─── Shared helper ────────────────────────────────────────────────────
        private static string FormatContact(Contact c)
            => $"{c.Name}  <{c.Email}>  {c.Phone}";
    }
}
