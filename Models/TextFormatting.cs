using System;
namespace FirstApp.Models
{
    public class TextFormatting
    {
        public TextFormatting()
        {
        }
        public string ReadableDate(DateTime date)
        {
            return date.ToString("d");
        }
    }
}
