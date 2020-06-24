using System;
using System.Collections.Generic;
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
        
        private List<string> Translates { get; set; } = new List<string>();

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

        public List<string> GetTranslates()
        {
            if (Translates.Count != 0)
                return Translates;
                
            Translates.Add(Default);
            Translates.Add(LineRus);
            return Translates;
        }

        public bool Contains(string text)
        {
            return GetTranslates().Contains(text);
        }
    }
}