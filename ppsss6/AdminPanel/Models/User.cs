namespace AdminPanel.Models
{
    public partial class User : IItem
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; } 

        public int Id
        {
            get => UserId;
            set => UserId = value;
        }

        public string Name
        {
            get => $"{FirstName} {LastName}";
            set { }
        }
    }
}