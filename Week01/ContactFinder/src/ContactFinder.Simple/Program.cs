using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ContactFinder.Simple
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var finder = new ContactFinder();

            Console.WriteLine("Contact Finder  ");
            Console.WriteLine("Commands: add, find, list, remove, quit/n");

            while (true)
            {
                Console.WriteLine("> ");
                var input = Console.ReadLine();
                var parts = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

                if (parts.Length == 0) { continue; }

                switch (parts[0].ToLower())
                {
                    case "add":
                        if (parts.Length < 3)
                        {
                            Console.WriteLine("Usage: add <name> <email>");
                            break;
                        }
                        var name = parts[1];
                        var email = parts[2];
                        finder.Add(new Contact(name, email));
                        Console.WriteLine($"Added: {name} ({email})");
                        break;
                    case "find":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: find <email>");
                            break;
                        }
                        var found = finder.Find(parts[1]);
                        if (found != null)
                        {
                            Console.WriteLine($"Found: {found.Name} ({found.Email})");
                        }
                        else
                        {
                            Console.WriteLine("Contact not found.");
                        }
                        break;
                    case "list":
                        var all = finder.GetAll();
                        if (all.Count == 0)
                        {
                            Console.WriteLine("No contacts found.");
                        }
                        else
                        {
                            foreach (var contact in all)
                            {
                                Console.WriteLine($"{contact.Name} ({contact.Email})");
                            }
                        }
                        break;
                    case "remove":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: remove <email>");
                            break;
                        }
                        var toRemove = finder.Find(parts[1]);
                        if (toRemove != null)
                        {
                            finder.Remove(toRemove);
                            Console.WriteLine($"Removed: {toRemove.Name} ({toRemove.Email})");
                        }
                        else
                        {
                            Console.WriteLine("Contact not found.");
                        }
                        break;
                    case "quit":
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }
    }
}