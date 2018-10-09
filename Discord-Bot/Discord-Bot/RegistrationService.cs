using System.Collections.Generic;

namespace Discord_Bot
{
    internal class RegistrationService
    {
        public enum RegistrationStep
        {
            NotStarted,
            Name,
            Surname,
            Sex,
            Email,
            Phone,
            Done
        }

        public RegistrationService()
        {
            Users = new List<User>();
            //todo load users from json
        }

        public List<User> Users { get; set; }
    }
}