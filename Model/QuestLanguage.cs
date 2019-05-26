using System;

namespace Models
{
    

    public static class QuestLangauge
    {
        public enum Language
        {
            Unknown = 0xFFFF
        }

        public static Language VerifyQuestLanguage(UInt32 code)
        {
            foreach(Language lang in Language.GetValues(typeof(Language)))
            {
                if ((UInt32)lang == code) return lang;
            }
            return Language.Unknown;
        }
    }
}