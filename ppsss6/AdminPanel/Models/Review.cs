using System;

namespace AdminPanel.Models
{
    public partial class Review : IItem
    {
        public int ReviewId { get; set; }
        public int? OrderId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime? ReviewDate { get; set; }

        public string OrderInfo { get; set; }

        public int Id
        {
            get => ReviewId;
            set => ReviewId = value;
        }

        public string Name
        {
            get => $"Отзыв #{ReviewId}";
            set { }
        }
    }
}