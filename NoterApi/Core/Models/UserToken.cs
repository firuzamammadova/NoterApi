using System;
namespace Core.Models
{
    public class UserToken
    {
        public long Id { get; set; }
        public DateTime AddedDate { get; set; }
        public int Type { get; set; }
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
