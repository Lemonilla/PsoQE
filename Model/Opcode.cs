using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Opcode
    {
        public byte table;
        public byte code;
        public string name;
        public bool push_stack;
        public bool pop_stack;
        public List<OpcodeArgument> arguments;
    }

    public abstract class OpcodeArgument {
        public OpcodeArgumentType enumType;
        public Type type;
    }

    public class RegisterOpcodeArgument : OpcodeArgument
    {
        public RegisterOpcodeArgument() { enumType = OpcodeArgumentType.Register; type = typeof(byte);}
        public byte value;
    }

    public class ByteOpcodeArgument : OpcodeArgument
    {
        public ByteOpcodeArgument() { enumType = OpcodeArgumentType.Byte; type = typeof(byte);}
        public byte value;
    }

    public class WordOpcodeArgument : OpcodeArgument
    {
        public WordOpcodeArgument() { enumType = OpcodeArgumentType.Word; type = typeof(UInt16);}
        public UInt16 value;
    }

    public class DwordOpcodeArgument : OpcodeArgument
    {
        public DwordOpcodeArgument() { enumType = OpcodeArgumentType.Dword; type = typeof(UInt32);}
        public UInt32 value;
    }

    public class SignedWordOpcodeArgument : OpcodeArgument
    {
        public SignedWordOpcodeArgument() { enumType = OpcodeArgumentType.SignedWord; type = typeof(Int16);}
        public Int16 value;
    }

    public class SignedDwordOpcodeArgument : OpcodeArgument
    {
        public SignedDwordOpcodeArgument() { enumType = OpcodeArgumentType.SignedDword; type = typeof(Int32); }
        public Int32 value;
    }

    public class LabelOpcodeArgument : OpcodeArgument
    {
        public LabelOpcodeArgument() { enumType = OpcodeArgumentType.Label; type = typeof(UInt32); }
        public UInt32 value;
    }

    public class FloatOpcodeArgument : OpcodeArgument
    {
        public FloatOpcodeArgument() { enumType = OpcodeArgumentType.Float; type = typeof(Single); }
        public Single value;
    }

    public class ListLabelOpcodeArgument : OpcodeArgument
    {
        public ListLabelOpcodeArgument() { enumType = OpcodeArgumentType.ListLabels; type = typeof(List<LabelOpcodeArgument>); }
        public List<LabelOpcodeArgument> value;
    }

    public class ListRegisterOpcodeArgument : OpcodeArgument
    {
        public ListRegisterOpcodeArgument() { enumType = OpcodeArgumentType.ListRegisters; type = typeof(List<RegisterOpcodeArgument>); }
        public List<RegisterOpcodeArgument> value;
    }

    public class StringOpcodeArgument : OpcodeArgument
    {
        public StringOpcodeArgument() { enumType = OpcodeArgumentType.String; type = typeof(string); }
        public string value;
    }
   
    public enum OpcodeArgumentType
    {
        Register,
        Byte,
        Word,
        Dword,
        SignedWord,
        SignedDword,
        Label,
        Float,
        ListLabels,
        ListRegisters,
        String
    }
}
