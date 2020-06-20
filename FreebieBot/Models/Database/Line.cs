using System.ComponentModel.DataAnnotations;

namespace FreebieBot.Models.Database
{
    public class Line
    {
        [Key]
        public string Name { get; set; }
        public string Default { get; set; }
        public string LineRus { get; set; }
    }
}