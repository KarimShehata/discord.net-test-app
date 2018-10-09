using Discord.WebSocket;

namespace Discord_Bot
{
    internal class User
    {
        public RegistrationService.RegistrationStep RegistrationStep { get; set; }

        public User(SocketGuildUser socketGuildUser)
        {
            SocketGuildUser = socketGuildUser;
            RegistrationStep = RegistrationService.RegistrationStep.NotStarted;
        }

        public SocketGuildUser SocketGuildUser { get; set; }
        public bool IsRegistrationComplete => RegistrationStep == RegistrationService.RegistrationStep.Done;
    }
}