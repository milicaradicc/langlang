using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LangLang.Models
{
    public class Language
    {
        private string _name = null!;

        public Language() { }

        public Language(string name, LanguageLevel level)
        {
            Name = name;
            Level = level;
        }

        public string Name
        {
            get => _name;
            set
            {
                ValidateName(value);
                _name = value;
            }
        }    
        public int Id {  get; set; }

        public LanguageLevel Level { get; set; }
        [JsonIgnore]
        public virtual List<Teacher> Teachers { get; set; } = new();

        private void ValidateName(string name)
        {
            switch (name)
            {
                case null:
                    throw new ArgumentNullException(nameof(name));
                case "":
                    throw new InvalidInputException("Name must include at least one character.");
            }
        }

        public override string ToString()
        {
            return $"{Name} {Level}";
        }

        public override bool Equals(object? obj)
        {
            return obj is Language language &&
                   Name == language.Name &&
                   Level == language.Level;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, (int)Level);
        }
    }
}