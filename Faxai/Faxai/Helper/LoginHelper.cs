using Faxai.Models.PageSystemModels;
using System.Data;

namespace Faxai.Helper
{
    public static class LoginHelper
    {
        public static void SendToUserWarning(string currentUri,string userEmail)
        {
            try
            {
                string userID = string.Empty;
                if (ExistsInDataBase(userEmail,ref userID))
                {
                    Dictionary<string, string> dataToChange = new Dictionary<string, string>();
                    string localBlockUserUrl = currentUri + "/PageSystem/UserBlocking" + "?userId="+userID;
                    dataToChange.Add("blokavimas", localBlockUserUrl);
                    EmailHelper.SendMail("vidmantaszuk@gmail.com", "IncorectPasswordMail", dataToChange);
                }
            }
            catch (Exception e)
            {
                PageLog log = new PageLog();
                log.UserID = 0;
                log.Description = e.Message;
                log.Category = "SystemError";
                log.Url = "PageSystemController.GetObject";
                log.Date = DateTime.Now;
                log.SaveToDataBase();
            }
        }
        private static bool ExistsInDataBase(string email,ref string userID)
        {
            DataView userData = DataSource.ExecuteSelectSQL($"SELECT * FROM Naudotojas WHERE El_Pastas = '{email}'");
            if (userData != null && userData.Count > 0)
            {
                userID = userData.Table.Rows[0]["ID"].ToString();
                return true;
            }
            else
                return false;
        }
        public static bool BlockUserByID(string userID)
        {
            DateTime from = DateTime.Now;
            DateTime to = DateTime.Now.AddDays(10);

            return DataSource.UpdateDataSQL(
                $"INSERT INTO [dbo].[Naudotojo_Uzdraudimas]([Pradzios_Data],[Pabaigos_Data],[NaudotojasID])" +
                $" VALUES ('{from.ToString()}','{to.ToString()}',{userID})");
        }
        public static bool IsBannedUser(string email)
        {
            DataView userData = DataSource.ExecuteSelectSQL(
                $"SELECT NU.Pabaigos_Data AS Pabaigos_Data FROM Naudotojo_Uzdraudimas NU " +
                $" LEFT JOIN Naudotojas ON Naudotojas.ID = NU.NaudotojasID" +
                $" WHERE GETDATE() BETWEEN NU.Pradzios_Data AND NU.Pabaigos_Data AND " +
                $" Naudotojas.El_Pastas = '{email}'");
            if (userData != null && userData.Count > 0)
            {
                return true;
            }
            else
                return false;
        }
    }
}
