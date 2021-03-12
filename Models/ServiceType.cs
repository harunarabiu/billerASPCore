using System;

namespace FirstApp.Models
{
    public class ServiceType : BaseModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
