using System;

namespace ContactFinder.Simple
{
    public record Contact
    {
        public string Name { get;}
        public string Email { get;}
        public Contact(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));

            Name = name.Trim();
            Email = email.Trim();
        }
    }
}
