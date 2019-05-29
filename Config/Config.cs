using System;
using System.Collections.Generic;
using System.IO;
using Models;
using System.Configuration;

namespace Config
{
    public class Config
    {
        private Config _instance;

        public List<Opcode> Opcodes;
       
        private Config()
        {
            Opcodes = _readOpcodesFromFile();
        }
        public Config GetInstance() {
            if (_instance == null)
                _instance = new Config();
            return _instance;
        }


        private List<Opcode> _readOpcodesFromFile()
        {
            var list = new List<Opcode>();
            using (StreamReader file = new StreamReader(ConfigurationManager.AppSettings["opcodeConfigListFileName"]))
            {
                var line = "";
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith(";")) continue; // skip comment lines
                    var opcode = new Opcode();
                    var s = line.Split(',');
                    var num = Int32.Parse(s[0]);
                    opcode.table = (byte)(num & 0x00FF);
                    opcode.code = (byte)((num & 0xFF00) >> 16);
                    opcode.name = s[1];
                    for(int i = 0; i < s[2].Length; i++)
                    {
                        var arg = s[2][i];
                        switch (arg)
                        {
                            case 'p':
                                opcode.push_stack = true;
                                break;
                            case 'a':
                                opcode.pop_stack = true;
                                break;
                            case 'B':
                                opcode.arguments.Add(new ByteOpcodeArgument());
                                break;
                            case 'W':
                            case 'w':
                                opcode.arguments.Add(new WordOpcodeArgument());
                                break;
                            case 'L':
                                opcode.arguments.Add(new DwordOpcodeArgument());
                                break;
                            case 'l':
                                opcode.arguments.Add(new SignedDwordOpcodeArgument());
                                break;
                            case 'f':
                            case 'F':
                                opcode.arguments.Add(new FloatOpcodeArgument());
                                break;
                            case 'r':
                            case 'R':
                                opcode.arguments.Add(new RegisterOpcodeArgument());
                                break;
                            case 'j':
                            case 'J':
                                opcode.arguments.Add(new ListLabelOpcodeArgument());
                                break;
                            case 't':
                            case 'T':
                                opcode.arguments.Add(new ListRegisterOpcodeArgument());
                                break;
                            case 's':
                            case 'S':
                                opcode.arguments.Add(new StringOpcodeArgument());
                                break;
                            default:
                                break;
                        }
                    }
                    opcode.arguments.Reverse(); //we're appending to front, so we need to reverse to have proper order
                    list.Add(opcode);
                }
            }
            return list;
        }
    }   
}
