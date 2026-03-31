using System;
using System.Text;


namespace ContactFinder.Core
{
    public sealed record Contact
    {
        public string Name { get; }
        public string Email { get; }
        public string Phone { get; }

        public Contact(string name, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required.", nameof(name));
            }
            if(string.IsNullOrWhiteSpace(email))
            { 
                throw new ArgumentException("Email is required. ", nameof(email));
            }
            if(string.IsNullOrWhiteSpace(phone))
            { 
                throw new ArgumentException("Phone is required. ", nameof(phone));
            }
            Name = name.Trim();
            Email = email.Trim();
            Phone = NormalizePhone(phone);
        }

        public static string NormalizePhone(string phone)
        {
            var sb = new StringBuilder(phone.Length);
            foreach (var c in phone)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }
            if (sb.Length < 7)
            {
                throw new ArgumentException("Phone number must contain at least 7 digits.", nameof(phone));
            }
            
            return sb.ToString();
        }
    }
}
