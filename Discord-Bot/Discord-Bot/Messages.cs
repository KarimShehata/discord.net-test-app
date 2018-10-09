namespace Discord_Bot
{
    internal class Messages
    {
        public static string WelcomeMessage = "Hallo!\n" +
                                              "Ich bin der UltimateLinz-Bot und werde dich durch die Anmeldung führen.\n" +
                                              "Bitte beantworte die folgenden Fragen wahrheitsgemäß.\n" +
                                              "Ein Admin wird sich deine Anmeldung dann ansehen und dich freischalten.";

        public static string AskName = "Wie lautet dein Vorname?";
        public static string AskSurName = "Wie lautet dein Nachname?";
        public static string AskSex = "Was is dein Geschlecht?";
        public static string AskBirthdate = "Was is dein Geburtsdatum?";
        public static string AskEmail = "Wie lautet deine E-Mail Adresse?";
        public static string AskPhone = "Wie lautet deine Telefonnummer?";
        public static string Done = "Glückwunsch du hast die Anmeldung abgeschlossen.";

        public static string[] RegistrationForm = { AskName, AskSurName, AskSex, AskBirthdate, AskEmail, AskPhone, Done };
    }
}