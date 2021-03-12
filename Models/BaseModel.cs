using System;
namespace FirstApp.Models
{
    public class BaseModel
    {
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
