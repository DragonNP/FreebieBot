using System.ComponentModel.DataAnnotations;
using FreebieBot.Models.Users;

namespace FreebieBot.Models.Translates
{
    public class Line
    {
        [Key]
        public string Name { get; set; }
        public string Default { get; set; }
        public string LineRus { get; set; }

        public string GetTranslate(UserLang lang)
        {
            switch (lang)
            {
                case UserLang.en:
                    return Default;
                case UserLang.ru:
                    return LineRus;
                default:
                    return Default;
            }
        }
    }
}