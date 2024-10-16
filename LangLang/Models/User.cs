using LangLang.FormTable;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LangLang.Models
{
    public abstract class User
    {
        // = null! to suppress nullable warning because the values are validated
        private string _firstName = null!;
        private string _lastName = null!;
        private string _email = null!;
        private string _password = null!;
        private string _phone = null!;

        protected User(string? firstName, string? lastName, string? email, string? password, Gender gender, string? phone)
        {
            FirstName = firstName!;
            LastName = lastName!;
            Email = email!;
            Password = password!;
            Gender = gender;
            Phone = phone!;
            Deleted = false;
        }

        public User()
        {
        }

        [TableItem(1)]
        public int Id { get; set; }
        public bool Deleted { get; set; }
        [TableItem(2)]
        public string FirstName
        {
            get => _firstName;
            set
            {
                ValidateFirstName(value);
                _firstName = value;
            }
        }
        [TableItem(3)]
        public string LastName
        {
            get => _lastName;
            set
            {
                ValidateLastName(value);
                _lastName = value;
            }
        }

        [TableItem(4)]
        public string Email
        {
            get => _email;
            private set
            {
                ValidateEmail(value);
                _email = value;
            }
        }
        [TableItem(5)]
        public string Password
        {
            get => _password;
            set
            {
                ValidatePassword(value);
                _password = value;
            }
        }

        [TableItem(6)]
        public Gender Gender { get; set; }

        [TableItem(7)]
        public string Phone
        {
            get => _phone;
            set
            {
                ValidatePhoneNumber(value);
                _phone = value;
            }
        }

        private static void ValidateFirstName(string firstName)
        {
            switch (firstName)
            {
                case null:
                    throw new ArgumentNullException(nameof(firstName));
                case "":
                    throw new InvalidInputException("First name must include at least one character.");
            }
        }

        private static void ValidateLastName(string lastName)
        {
            switch (lastName)
            {
                case null:
                    throw new ArgumentNullException(nameof(lastName));
                case "":
                    throw new InvalidInputException("Last name must include at least one character.");
            }
        }

        private static void ValidateEmail(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (!Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"))
            {
                throw new InvalidInputException("Email not valid");
            }
        }

        private static void ValidatePassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (password.Length < 8)
            {
                throw new InvalidInputException("Password must be at least eight characters long");
            }

            if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
            {
                throw new InvalidInputException("Password must contain at least one uppercase letter, one lower case and one digit.");
            }
        }

        private static void ValidatePhoneNumber(string phoneNumber)
        {
            if(phoneNumber==null)
                throw new ArgumentNullException(nameof(phoneNumber));


            if (phoneNumber.Length < 10)
            {
                throw new InvalidInputException("Phone number must contain at least 10 numbers.");
            }

            if (!Regex.IsMatch(phoneNumber, "^\\d+$"))
            {
                throw new InvalidInputException("Phone number must contain only numbers.");
            }
        }
    }
}