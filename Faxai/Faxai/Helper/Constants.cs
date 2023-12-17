using System.Net.Mail;

namespace Faxai.Helper
{
    public class Constants
    {
        /// <summary>
        /// Server = Nurodomas serverio pavadinimas prie kurio jungiamasi
        /// Database = Duomenu bazės pavadinimas
        /// User = Prisijungimo vardas
        /// Password = Naudotojo slaptažodis
        /// </summary>
        public static string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Faxai;User=Faxas;Password=Faxas123;";


        /// <summary>
        /// Duomenu bazės pagrindinių veiksmų plėtiniai. Juos naudoti nebūtina, bet palengvina ir pagražina kodavimą.
        /// </summary>
        public static string SQLInset = "_Insert";
        public static string SQLUpdate = "_Update";
        public static string SQLSelect = "_Select";
        public static string SQLSelectAll = "_SelectAll";
        public static string SQLDelete = "_Delete";
        public static string SMTPServer = "smtp.gmail.com";
        public static int SMTPPort = 587;
        public static string SMTPUserEmail= "vidmantaszukitas@gmail.com";
        public static string SMTPUserPassword = "vmoa brzh iwbg wggz";
    }
}
