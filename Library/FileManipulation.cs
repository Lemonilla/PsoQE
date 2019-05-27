using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Models;

namespace Library
{
    public static class FileManipulation
    {

        public static Quest LoadQstFromFile(string path)
        {
            //try
            //{
                Quest quest = new Quest();
                byte[] rawQst = File.ReadAllBytes(path);
                quest = ParseQuestFromQstBinary(rawQst, quest);

                return quest;
            //}
            //catch (ArgumentException e)
            //{
            //    // path is 0 length
            //    return null;
            //}
            //catch(FileNotFoundException e)
            //{
            //    // no such file
            //    return null;
            //}
            //catch (Exception e)
            //{
            //    // something else
            //    return null;
            //}
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
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
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
                            // we are dealing with a block
                            reader.ReadUInt32(); // skip 32 bits
                            
                            string block_Filename = new String(reader.ReadChars(16));

                            byte[] ba = new byte[1024]; //1024
                            reader.Read(ba, 0, 1024);
                            uint block_size = reader.ReadUInt32();
                            for (int i = 0; i < block_size; i++)
                            {
                                if (block_Filename.Contains(".bin")) bin.Add(ba[i]);
                                else if (block_Filename.Contains(".dat")) dat.Add(ba[i]);
                                else throw new Exception("Bad Block Filename.");
                            }
                            reader.ReadUInt32(); // skip the end padding


                        }
                        else
                        {
                            throw new Exception("Chunk Header not recognized: 0x" + chunkHeader.ToString("X8"));
                        }
                    }

                }
            }
            byte[] rawBin = bin.ToArray();
            byte[] rawDat = dat.ToArray();
            if (rawBin.Length > 0) rawBin = Prs.Decompress(rawBin);
            if (rawDat.Length > 0) rawDat = Prs.Decompress(rawDat);

            quest = ParseQuestFromBinBinary(rawBin, quest);
            quest = ParseQuestFromDatBinary(rawDat, quest);

            return quest;
        }

        private static Quest ParseQuestFromBinBinary(byte[] rawBin, Quest quest)
        {
            if (rawBin.Length == 0) return quest;

            UInt32 object_code_offset;
            UInt32 function_offset_table_offset;
            Encoding unicodeEncoder = Encoding.Unicode;
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
                        quest.Name = unicodeEncoder.GetString(reader.ReadBytes(64));
                        quest.ShortDescription = unicodeEncoder.GetString(reader.ReadBytes(256));
                        quest.LongDescription = unicodeEncoder.GetString(reader.ReadBytes(576));
                        UInt32 unknown = reader.ReadUInt32();//whitespace
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
                        quest.Name = unicodeEncoder.GetString(reader.ReadBytes(32));
                        quest.ShortDescription = unicodeEncoder.GetString(reader.ReadBytes(128));
                        quest.LongDescription = unicodeEncoder.GetString(reader.ReadBytes(288));
                    }
                }
            }
            
            // TODO: Implement scripting

            return quest;
        }

        private static Quest ParseQuestFromDatBinary(byte[] rawDat, Quest quest)
        {
            if (rawDat.Length == 0) return quest;

            Dictionary<UInt32, Map> maps = new Dictionary<uint, Map>();


            using (MemoryStream stream = new MemoryStream(rawDat))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
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
                                    UInt16 skin = reader.ReadUInt16();
                                    byte[] unknown1 = reader.ReadBytes(6); //unknown
                                    UInt16 id = reader.ReadUInt16();
                                    UInt16 group = reader.ReadUInt16();
                                    UInt16 section = reader.ReadUInt16();
                                    byte[] unknown2 = reader.ReadBytes(2); //unknown
                                    Single x = reader.ReadSingle();
                                    Single y = reader.ReadSingle();
                                    Single z = reader.ReadSingle();
                                    UInt32 rotx = reader.ReadUInt32();
                                    UInt32 roty = reader.ReadUInt32();
                                    UInt32 rotz = reader.ReadUInt32();
                                    Single F1 = reader.ReadSingle();
                                    Single F2 = reader.ReadSingle();
                                    Single F3 = reader.ReadSingle();
                                    Int32 F4 = reader.ReadInt32();
                                    Int32 F5 = reader.ReadInt32();
                                    Int32 F6 = reader.ReadInt32();
                                    Int32 F7 = reader.ReadInt32();

                                    obj.Skin = skin;
                                    obj.ObjectId = id;
                                    obj.ItemWave = group;
                                    obj.RoomNumber = section;
                                    obj.FloorNumber = area;
                                    obj.Position = new Location(x, y, z, rotx, roty, rotz);
                                    obj.F1 = F1;
                                    obj.F2 = F2;
                                    obj.F3 = F3;
                                    obj.F4 = F4;
                                    obj.F5 = F5;
                                    obj.F6 = F6;
                                    obj.F7 = F7;

                                    maps[area].Objects.Add(obj);
                                    
                                }
                                break;
                            case 2: // NPC
                                entries = table_body_size / 72;
                                for (int entryNumber = 0; entryNumber < entries; entryNumber++)
                                {
                                    var mob = new Monster();
                                    UInt16 skin = reader.ReadUInt16();
                                    byte[] unknown1 = reader.ReadBytes(4); //unknown
                                    UInt16 childcount = reader.ReadUInt16();
                                    UInt16 floorNumber = reader.ReadUInt16();
                                    UInt16 itemWave = reader.ReadUInt16();
                                    UInt16 section = reader.ReadUInt16();
                                    UInt16 wave1 = reader.ReadUInt16();
                                    UInt32 wave2 = reader.ReadUInt32();
                                    Single x = reader.ReadSingle();
                                    Single y = reader.ReadSingle();
                                    Single z = reader.ReadSingle();
                                    UInt32 rotx = reader.ReadUInt32();
                                    UInt32 roty = reader.ReadUInt32();
                                    UInt32 rotz = reader.ReadUInt32();
                                    Single F1 = reader.ReadSingle();
                                    Single F2 = reader.ReadSingle();
                                    Single F3 = reader.ReadSingle();
                                    Single F4 = reader.ReadSingle();
                                    Single F5 = reader.ReadSingle();
                                    Int32 F6 = reader.ReadInt32();
                                    Int32 F7 = reader.ReadInt32();

                                    mob.Skin = skin;
                                    mob.ChildCount = childcount;
                                    mob.FloorNumber = floorNumber;
                                    mob.RoomNumber = section;
                                    mob.Wave1 = wave1;
                                    mob.Wave2 = wave2;
                                    mob.Position = new Location(x, y, z, rotx, roty, rotz);
                                    mob.F1 = F1;
                                    mob.F2 = F2;
                                    mob.F3 = F3;
                                    mob.F4 = F4;
                                    mob.F5 = F5;
                                    mob.F6 = F6;
                                    mob.F7 = F7;

                                    maps[area].Monsters.Add(mob);
                                }
                                break;
                            case 3: // Wave
                                // read header
                                UInt32 wavesize = reader.ReadUInt32();
                                UInt32 flag = reader.ReadUInt32(); // 0x10000000
                                UInt32 wavecount = reader.ReadUInt32();
                                UInt32 zero = reader.ReadUInt32();

                                long startOfEvents = reader.BaseStream.Position + (wavecount*20);
                                long endOfChunk = reader.BaseStream.Position + (table_size - 32);

                                // read wave list
                                for (int i = 0; i < wavecount; i++)
                                {
                                    Event e = new Event();
                                    UInt32 id = reader.ReadUInt32();
                                    UInt32 flags = reader.ReadUInt32(); //0x00000100
                                    UInt16 section = reader.ReadUInt16();
                                    UInt16 wave = reader.ReadUInt16();
                                    UInt16 delay = reader.ReadUInt16();
                                    UInt16 unknown = reader.ReadUInt16();
                                    UInt32 waveClearEventOffset= reader.ReadUInt32();

                                    // build event
                                    e.EventNumber = id;
                                    e.SectionId = section;
                                    e.WaveId = wave;
                                    e.Delay = delay;

                                    // parse event commands
                                    long currentPosition = reader.BaseStream.Position;
                                    reader.BaseStream.Position = startOfEvents + waveClearEventOffset;
                                    bool hasCommands = true;
                                    while (hasCommands)
                                    {
                                        byte command = reader.ReadByte();
                                        switch(command)
                                        {
                                            case 0x01: // end of commands
                                                hasCommands = false;
                                                break;
                                            case 0x0a: // unlock
                                                e.OnCompletion.Add(new UnlockDoorEvent(reader.ReadUInt16()));
                                                break;
                                            case 0x0b: // lock
                                                e.OnCompletion.Add(new LockDoorEvent(reader.ReadUInt16()));
                                                break;
                                            case 0x0c: // call
                                                e.OnCompletion.Add(new CallEvent(reader.ReadUInt32()));
                                                break;
                                            case 08: // unhide
                                                e.OnCompletion.Add(new UnhideEvent(reader.ReadUInt16(),reader.ReadUInt16()));
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    reader.BaseStream.Position = currentPosition;
                                    maps[area].Events.Add(e);
                                }
                                reader.BaseStream.Position = endOfChunk;
                                break;
                            default: // Unhandled tables
                                break;
                        }
                        
                    }
                }
            }
            quest.Maps = new List<Map>();
            foreach (var m in maps)
                quest.Maps.Add(m.Value);

            return quest;
        }
    }
}
