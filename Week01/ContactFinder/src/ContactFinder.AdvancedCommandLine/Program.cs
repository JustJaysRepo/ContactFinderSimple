using ContactFinder.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using System.CommandLine;
using ContactFinder.Core;
using ContactFinder.AdvancedCommandLine;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

var connectionString = config.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string not found.");

// ─── Test the connection ───────────────────────────────────────────────

//Console.WriteLine("Testing connection...");

//try
//{
//    using var connection = new SqlConnection(connectionString);
//    connection.Open();
//    Console.WriteLine($"Connected successfully to: {connection.DataSource}");
//    Console.WriteLine($"Database: {connection.Database}");
//    Console.WriteLine($"Server version: {connection.ServerVersion}");
//}
//catch (SqlException ex)
//{
//    Console.WriteLine($"Connection failed: {ex.Message}");
//}

// ─── Run the CLI ──────────────────────────────────────────────────────
// ─── Bootstrap ───────────────────────────────────────────────────────────────
// Ensure the Contacts table exists before any command runs.
// Safe to call every time — does nothing if the table is already there.
DbInitializer.EnsureCreated(connectionString);

// The repository is the only object that knows about SQL Server.
// Every command handler receives it through a closure — none of them
// reference the connection string or ADO.NET directly.
var repo = new SqlContactRepository(connectionString);

// ─── Root command ─────────────────────────────────────────────────────────────
// System.CommandLine works by building a tree of commands.
// The root command is the entry point — sub-commands hang off it.
// Running the app with no arguments shows the help text automatically.
var rootCommand = new RootCommand("ContactFinder — manage contacts from the command line")
{
    Commands.BuildAddCommand(repo),
    Commands.BuildFindCommand(repo),
    Commands.BuildFindPhoneCommand(repo),
    Commands.BuildListCommand(repo),
    Commands.BuildRemoveCommand(repo)
};

// InvokeAsync parses args, routes to the correct handler, and returns an
// exit code. 0 = success, non-zero = error. The OS receives this exit code.
await rootCommand.Parse(args).InvokeAsync();
return 0;