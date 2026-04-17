using System.Collections.Generic;

namespace ContactFinder.Core
{
    // ─── Why an interface? ───────────────────────────────────────────────────
    // An interface defines what a class must be able to do without saying how.
    //
    // By depending on IContactRepository instead of a concrete class, the
    // ViewModels and CLI handlers never need to know whether contacts are stored
    // in memory, in SQL Server, or anywhere else.
    //
    // This has two practical benefits:
    //   1. Unit tests can pass in the fast in-memory ContactRepository instead
    //      of a real database connection.
    //   2. Swapping the storage engine (e.g. SQLite instead of SQL Server)
    //      only requires writing a new class that implements this interface —
    //      no changes to any ViewModel or command handler.
    // ────────────────────────────────────────────────────────────────────────

    public interface IContactRepository
    {
        void Add(Contact contact);

        Contact? FindByEmail(string email);

        Contact? FindByPhone(string phone);

        // Returns true if a contact was found and removed; false otherwise.
        bool RemoveByEmail(string email);

        // Returns a snapshot — callers cannot mutate internal state.
        IReadOnlyList<Contact> GetAll();

        int Count { get; }
    }
}
