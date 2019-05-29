﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum ErrorCodes
    {
        LongDescriptionTruncated,
        ShortDescriptionTruncated,
        QuestNumberOutOfBounds,
        NameTruncated,
        UnrecognizedQuestLanguage,
        UnknownEventCode
    }
}
