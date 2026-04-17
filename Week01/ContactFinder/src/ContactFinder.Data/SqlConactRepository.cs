using System.Collections.Generic;
using ContactFinder.Core;
using Microsoft.Data.SqlClient;

namespace ContactFinder.Data
{
    // SQL Server implementation of IContactRepository.
    // Used at runtime by both ContactFinder.AdvancedCommandLine and
    // ContactFinder.Wpf — written once, referenced by both.
    //
    // Each method follows the same three-step pattern:
    //   1. Open a connection (closed automatically by the using block)
    //   2. Build a parameterized command
    //   3. Execute and map the result
    //
    // ─── Why parameterized queries? ─────────────────────────────────────────
    // Never concatenate user input into a SQL string:
    //   BAD:  "SELECT * FROM Contacts WHERE Email = '" + email + "'"
    //   GOOD: "SELECT * FROM Contacts WHERE Email = @Email"
    //         command.Parameters.AddWithValue("@Email", email)
    //
    // Concatenation allows SQL injection — a user could enter:
    //   alice@example.com'; DROP TABLE Contacts; --
    // and destroy the database. Parameters prevent this entirely.
    // ────────────────────────────────────────────────────────────────────────

    public sealed class SqlContactRepository : IContactRepository
    {
        private readonly string _connectionString;

        public SqlContactRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ─── Count ───────────────────────────────────────────────────────────
        public int Count
        {
            get
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                using var command = new SqlCommand(
                    "SELECT COUNT(*) FROM Contacts", connection);

                // ExecuteScalar returns the first column of the first row.
                // Ideal for COUNT, SUM, and other single-value queries.
                return (int)command.ExecuteScalar()!;
            }
        }

        // ─── Add (insert or update) ───────────────────────────────────────
        public void Add(Contact contact)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // MERGE handles both insert and update in one statement.
            // If a contact with this email already exists it is updated.
            // If it does not exist it is inserted.
            // This mirrors the behaviour of the in-memory ContactRepository.
            const string sql = """
                MERGE Contacts AS target
                USING (VALUES (@Email, @Name, @Phone))
                      AS source (Email, Name, Phone)
                ON target.Email = source.Email
                WHEN MATCHED THEN
                    UPDATE SET Name = source.Name, Phone = source.Phone
                WHEN NOT MATCHED THEN
                    INSERT (Email, Name, Phone)
                    VALUES (source.Email, source.Name, source.Phone);
                """;

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", contact.Email);
            command.Parameters.AddWithValue("@Name", contact.Name);
            command.Parameters.AddWithValue("@Phone", contact.Phone);

            command.ExecuteNonQuery();
        }

        // ─── FindByEmail ──────────────────────────────────────────────────
        public Contact? FindByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand(
                "SELECT Name, Email, Phone FROM Contacts WHERE Email = @Email",
                connection);

            command.Parameters.AddWithValue("@Email",
                email.Trim().ToLowerInvariant());

            // ─── SqlDataReader ───────────────────────────────────────────
            // ExecuteReader returns a forward-only cursor over the result set.
            // reader.Read() advances to the next row and returns false when
            // there are no more rows. For a single-row lookup, one Read()
            // call is enough.
            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Contact(
                name: reader.GetString(0),
                email: reader.GetString(1),
                phone: reader.GetString(2));
        }

        // ─── FindByPhone ──────────────────────────────────────────────────
        public Contact? FindByPhone(string phone)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand(
                "SELECT Name, Email, Phone FROM Contacts WHERE Phone = @Phone",
                connection);

            // Normalize before querying so format variations still match.
            command.Parameters.AddWithValue("@Phone",
                Contact.NormalizePhone(phone));

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Contact(
                name: reader.GetString(0),
                email: reader.GetString(1),
                phone: reader.GetString(2));
        }

        // ─── RemoveByEmail ────────────────────────────────────────────────
        public bool RemoveByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand(
                "DELETE FROM Contacts WHERE Email = @Email", connection);

            command.Parameters.AddWithValue("@Email",
                email.Trim().ToLowerInvariant());

            // ExecuteNonQuery returns the number of rows affected.
            // 1 means the contact was found and deleted.
            // 0 means no contact with that email existed.
            return command.ExecuteNonQuery() > 0;
        }

        // ─── GetAll ───────────────────────────────────────────────────────
        public IReadOnlyList<Contact> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand(
                "SELECT Name, Email, Phone FROM Contacts ORDER BY Name",
                connection);

            using var reader = command.ExecuteReader();

            var results = new List<Contact>();

            // Read() returns true for each row, false when the result set
            // is exhausted. This is the standard pattern for reading
            // multiple rows from a SqlDataReader.
            while (reader.Read())
            {
                results.Add(new Contact(
                    name: reader.GetString(0),
                    email: reader.GetString(1),
                    phone: reader.GetString(2)));
            }

            return results;
        }
    }
}
