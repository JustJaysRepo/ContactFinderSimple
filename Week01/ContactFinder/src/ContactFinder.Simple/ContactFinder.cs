using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactFinder.Simple
{
    public class ContactFinder
    {
        private readonly Dictionary<string, Contact> _contacts = new();

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        public void Add(Contact contact)
        {
            _contacts[NormalizeEmail(contact.Email)] = contact;

        }
        public void Remove(Contact contact)
        {
            _contacts.Remove(NormalizeEmail(contact.Email));
        }
        public Contact? Find(string email)
        {
            _contacts.TryGetValue(NormalizeEmail(email), out var contact);
            return contact;
        }
        public IReadOnlyCollection<Contact> GetAll()
        {
            return _contacts.Values.ToList().AsReadOnly();
        }
        public int Count => _contacts.Count;

    }
}
