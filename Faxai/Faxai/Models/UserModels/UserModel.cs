namespace Faxai.Models.UserModels
{
    public class UserModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDay { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BankAccount { get; set; }
        public bool Newsletter{ get; set; }
        public string Password{ get; set; }
        public int TypeID { get; set; }
        public int UserID { get; set; }
    }
}
