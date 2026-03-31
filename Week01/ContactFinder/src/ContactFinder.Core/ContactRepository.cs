using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactFinder.Core
{
    public sealed class ContactRepository
    {
        private readonly Dictionary<string, Contact> _contactsByEmail = new();
        private readonly Dictionary<string, string> _emailByPhone = new();

        public int Count => _contactsByEmail.Count;
        // Add or update contact
        public void Add(Contact contact)
        {
            var emailKey = NormalizeEmail(contact.Email);
            var phoneKey = Contact.NormalizePhone(contact.Phone);

            // Remove existing contacts - Phone Number with the same email1
            // If a contact with the same email already exists, remove the old phone mapping
            if (_contactsByEmail.TryGetValue(emailKey, out var existing))
            {
                _emailByPhone.Remove(Contact.NormalizePhone(existing.Phone));
            }
            _contactsByEmail[emailKey] = contact;
            _emailByPhone[phoneKey] = emailKey;
        }
        // Get contact by email
        public Contact? GetByEmail(string email)
        {
            var emailKey = NormalizeEmail(email);
            return _contactsByEmail.GetValueOrDefault(emailKey);
        }


        // Get contact by phone
        public Contact? GetByPhone(string phone)
        {
            var phoneKey = Contact.NormalizePhone(phone);
            if (_emailByPhone.TryGetValue(phoneKey, out var emailKey))
            {
                return _contactsByEmail.GetValueOrDefault(emailKey);
            }
            return null;
        }
        // Remove contact by email
        public bool RemoveByEmail(string email)
        {
            var emailKey = NormalizeEmail(email);
            if (_contactsByEmail.TryGetValue(emailKey, out var contact))
            {
                _contactsByEmail.Remove(emailKey);
                _emailByPhone.Remove(Contact.NormalizePhone(contact.Phone));
                return true;
            }
            return false;
        }

        //Return a snapshot of all contacts, this so caller cannot mutate the internal state.
        public IReadOnlyCollection<Contact> GetAll()
        {
            return _contactsByEmail.Values.ToList();
        }
        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}
