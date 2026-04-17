using Microsoft.Data.SqlClient;

namespace ContactFinder.Data
{
    // Runs once at application startup.
    // Creates the Contacts table if it does not already exist.
    // This means the app works on a fresh database with no manual setup.
    public static class DbInitializer
    {
        public static void EnsureCreated(string connectionString)
        {
            // ─── What is a using statement here? ────────────────────────────
            // SqlConnection implements IDisposable. The using statement
            // guarantees the connection is closed and its resources are
            // released when the block exits — even if an exception is thrown.
            // This is the standard pattern for all ADO.NET work.
            // ────────────────────────────────────────────────────────────────
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // IF NOT EXISTS means this is safe to run on every startup.
            // If the table already exists it does nothing.
            const string sql = """
                IF NOT EXISTS (
                    SELECT 1 FROM sysobjects
                    WHERE name = 'Contacts' AND xtype = 'U'
                )
                CREATE TABLE Contacts (
                    Email NVARCHAR(255) NOT NULL PRIMARY KEY,
                    Name  NVARCHAR(255) NOT NULL,
                    Phone NVARCHAR(50)  NOT NULL
                )
                """;

            using var command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}
