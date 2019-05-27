using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Quest
    {
        // private
        string _name;
        string _shortDesc;
        string _longDesc;
        long _questNumber;
        Version _version;
        List<ErrorCode> _errors;
        UInt32 _language;

        public Quest()
        {
            _errors = new List<ErrorCode>();
        }


        // DAT
        public List<Map> Maps { get; set; }

        // BIN
        public ItemList Items {get;set;}
        public Script Pasm { get; set; }

        // Metadata
        public List<ErrorCode> Errors { get {return _errors; } }
        public string LongDescription {
            get { return _longDesc; }
            set {
                if (value.Length > 288)
                {
                    _errors.Add(ErrorCode.LongDescriptionTruncated);
                    _longDesc = value.Trim('\0').Substring(0, 288);
                }
                else
                    _longDesc = value.Trim('\0');
            }
        }
        public string ShortDescription
        {
            get { return _shortDesc; }
            set {
                if (value.Length > 128)
                {
                    _errors.Add(ErrorCode.ShortDescriptionTruncated);
                    _shortDesc = value.Trim('\0').Substring(0, 128);
                }
                else
                    _shortDesc = value.Trim('\0');
            }
        }
        public long Number
        {
            get { return _questNumber; }
            set
            {
                if (value < 0 || (_version <= Version.Gamecube && value > UInt16.MaxValue) || (_version >= Version.Gamecube && value > UInt32.MaxValue))
                    _errors.Add(ErrorCode.QuestNumberOutOfBounds);
                _questNumber = value;
            }
        }
        public string Name
        {
            get { return _name; }
            set {
                if (value.Length > 32)
                {
                    _errors.Add(ErrorCode.NameTruncated);
                    _name = value.Trim('\0').Substring(0, 32);
                }
                else
                    _name = value.Trim('\0');
            }
        }
        public Version ClientVersion {
            get { return _version; }
            set { _version = value; }
        }
        public UInt32 LanguageCode {
            get { return  _language; }
            set {
                _language = (UInt32) QuestLangauge.VerifyQuestLanguage((UInt32) value);
                if (value == (UInt32) QuestLangauge.Language.Unknown) _errors.Add(ErrorCode.UnrecognizedQuestLanguage);
            }
        }

    }
}
