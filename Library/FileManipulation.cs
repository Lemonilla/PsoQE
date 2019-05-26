using System;
using System.Collections.Generic;
using System.IO;
using Models;

namespace Library
{
    public static class FileManipulation
    {

        public static Quest LoadQstFromFile(string path)
        {
            try
            {
                Quest quest = new Quest();
                byte[] rawQst = File.ReadAllBytes(path);
                quest = ParseQuestFromQstBinary(rawQst, quest);

                return quest;
            }
            catch (ArgumentException e)
            {
                // path is 0 length
                return null;
            }
            catch(FileNotFoundException e)
            {
                // no such file
                return null;
            }
            catch (Exception e)
            {
                // something else
                return null;
            }
        }
        

        private static Quest ParseQuestFromQstBinary(byte[] rawQst, Quest quest)
        {
            List<byte> bin = new List<byte>();
            List<byte> dat = new List<byte>();

            UInt32 headerHeader = 0x00440058;
            UInt32 bodyHeader = 0x0013041c;
            string writeTo = "";
            int binChunkNumber = 0;
            int datChunkNumber = 0;

            using (MemoryStream stream = new MemoryStream(rawQst))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (reader.BaseStream.CanRead)
                    {
                        UInt32 chunkHeader = reader.ReadUInt32();
                        if (chunkHeader == headerHeader)
                        {
                            var questNumber = reader.ReadUInt16(); // quest number
                            reader.ReadBytes(38); // whitespace
                            string fileName = "";
                            for (int i = 0; i < 16; i++) fileName += reader.ReadChar();
                            var fileSize = reader.ReadUInt32();
                            reader.ReadBytes(24); // Jpn name
                            writeTo = "";
                            if (fileName.ToLower().Contains(".bin")) writeTo = "Bin";
                            if (fileName.ToLower().Contains(".dat")) writeTo = "Dat";
                        }
                        else if (chunkHeader == bodyHeader)
                        {
                            int chunkNumber = reader.ReadByte();
                            reader.ReadBytes(3); //whitespace

                            byte[] data = reader.ReadBytes(1024);
                            UInt32 dataSize = reader.ReadUInt32();

                            UInt32 validation = reader.ReadUInt32();
                            if (validation != 0) throw new Exception("Unexpected Body Chunk termination: " + writeTo + chunkNumber.ToString());

                            if (writeTo == "Bin" && chunkNumber == (binChunkNumber + 1))
                            {
                                for (int i = 0; i < dataSize; i++) bin.Add(data[i]);
                                binChunkNumber += 1;
                            }
                            if (writeTo == "Dat" && chunkNumber == (datChunkNumber + 1))
                            {
                                for (int i = 0; i < dataSize; i++) dat.Add(data[i]);
                                datChunkNumber += 1;
                            }
                            if (writeTo == "") throw new Exception("Unexpected Headerless Body Chunk");

                        }
                        else
                        {
                            throw new Exception("Chunk Header not recognized: 0x" + chunkHeader.ToString("X8"));
                        }
                    }

                }
            }

            quest = ParseQuestFromBinBinary(Prs.Decompress(bin.ToArray()), quest);
            quest = ParseQuestFromDatBinary(Prs.Decompress(dat.ToArray()), quest);

            return quest;
        }

        private static Quest ParseQuestFromBinBinary(byte[] rawBin, Quest quest)
        {
            UInt32 object_code_offset;
            UInt32 function_offset_table_offset;
            using (MemoryStream stream = new MemoryStream(rawBin))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    object_code_offset = reader.ReadUInt32();
                    function_offset_table_offset = reader.ReadUInt32();
                    var bin_size = reader.ReadUInt32();
                    var language = reader.ReadUInt16();

                    if (language == 0xFFFF)
                    { // BB Quest
                        quest.ClientVersion = Models.Version.Blueburst;
                        reader.ReadUInt16(); //whitespace.  Should be 0xFFFF
                        quest.LanguageCode = reader.ReadUInt32();
                        quest.Number = reader.ReadUInt32();
                        quest.Name = reader.ReadChars(32).ToString();
                        quest.ShortDescription = reader.ReadChars(128).ToString();
                        quest.LongDescription = reader.ReadChars(288).ToString();
                        reader.ReadUInt32();//whitespace
                        List<UInt32> items = new List<UInt32>();
                        for (int i = 0; i < 932; i++) items.Add(reader.ReadUInt32());
                        ItemList itemList = new ItemList();
                        itemList.itemList = items.ToArray();
                        quest.Items = itemList;
                    }
                    else // GC or before
                    {
                        quest.LanguageCode = language;
                        quest.ClientVersion = Models.Version.Gamecube;
                        quest.Number = reader.ReadUInt16();
                        quest.Name = reader.ReadChars(32).ToString();
                        quest.ShortDescription = reader.ReadChars(128).ToString();
                        quest.LongDescription = reader.ReadChars(288).ToString();
                    }
                }
            }
            
            // TODO: Implement scripting

            return quest;
        }

        private static Quest ParseQuestFromDatBinary(byte[] rawDat, Quest quest)
        {

            Dictionary<UInt32, Map> maps = new Dictionary<uint, Map>();


            using (MemoryStream stream = new MemoryStream(rawDat))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (reader.BaseStream.CanRead)
                    {
                        // read header
                        var type = reader.ReadUInt32();
                        var table_size = reader.ReadUInt32();
                        var area = reader.ReadUInt32();
                        var table_body_size = reader.ReadUInt32();

                        // if we don't have anything on the map yet, add it to the list
                        if (!maps.ContainsKey(area)) maps.Add(area, new Map());

                        // process body
                        switch (type)
                        {
                            case 1: // Object
                                var entries = table_body_size / 68;
                                for (int entryNumber = 0; entryNumber < entries; entryNumber++)
                                {
                                    var obj = new Models.Object();
                                    UInt16 object_type = reader.ReadUInt16();
                                    reader.ReadBytes(6); //unknown
                                    UInt16 id = reader.ReadUInt16();
                                    UInt16 group = reader.ReadUInt16();
                                    UInt16 section = reader.ReadUInt16();
                                    reader.ReadBytes(2); //unknown
                                    Single x = reader.ReadSingle();
                                    Single y = reader.ReadSingle();
                                    Single z = reader.ReadSingle();
                                    UInt32 rotx = reader.ReadUInt32();
                                    UInt32 roty = reader.ReadUInt32();
                                    UInt32 rotz = reader.ReadUInt32();
                                    reader.ReadBytes(6); //unknown
                                    UInt32 objectId = reader.ReadUInt32();
                                    UInt32 action = reader.ReadUInt32();
                                    reader.ReadBytes(14); //unknown

                                    // TODO: Convert to object && add to map

                                }
                                break;
                            case 2: // NPC
                                entries = table_body_size / 72;
                                for (int entryNumber = 0; entryNumber < entries; entryNumber++)
                                {
                                }
                                break;
                            case 3: // Wave
                                break;
                            default: // Unhandled tables
                                break;
                        }
                        
                    }
                }
            }

            return quest;
        }
    }
}
