namespace LangLang.Models
{
    public class Director : User
    {
        public Director(string firstName, string lastName, string email, string password, Gender gender, string phone) :
            base(firstName, lastName, email, password, gender, phone)
        {
        }

        public Director()
        {
        }
    }
}