//
//  Copyright (C) 2013-2021 getMaNGOS <https://getmangos.eu>
//  
//  This program is free software. You can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation. either version 2 of the License, or
//  (at your option) any later version.
//  
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY. Without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UpdateFieldsExtractor
{
    class Program
    {
        public static int SearchInFile(Stream f, string s, int o = 0)
        {
            f.Seek(0L, SeekOrigin.Begin);
            var r = new BinaryReader(f);
            var b1 = r.ReadBytes((int)f.Length);
            var b2 = Encoding.ASCII.GetBytes(s);
            for (int i = o, loopTo = b1.Length - 1; i <= loopTo; i++)
            {
                for (int j = 0, loopTo1 = b2.Length - 1; j <= loopTo1; j++)
                {
                    if (b1[i + j] != b2[j])
                        break;
                    if (j == b2.Length - 1)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static int SearchInFile(Stream f, int v)
        {
            f.Seek(0L, SeekOrigin.Begin);
            var r = new BinaryReader(f);
            var b1 = r.ReadBytes((int)f.Length);
            var b2 = BitConverter.GetBytes(v);
            // Array.Reverse(b2)

            for (int i = 0, loopTo = b1.Length - 1; i <= loopTo; i++)
            {
                if (i + 3 >= b1.Length)
                    break;
                if (b1[i] == b2[0] && b1[i + 1] == b2[1] && b1[i + 2] == b2[2] && b1[i + 3] == b2[3])
                {
                    return i;
                }
            }

            return -1;
        }

        public static string ReadString(FileStream f)
        {
            string r = "";
            byte t;

            // Read if there are zeros
            t = (byte)f.ReadByte();
            while (t == 0)
                t = (byte)f.ReadByte();

            // Read string
            while (t != 0)
            {
                r += Convert.ToString((char)t);
                t = (byte)f.ReadByte();
            }

            return r;
        }

        public static string ReadString(FileStream f, long pos)
        {
            string r = "";
            byte t;
            if (pos == -1)
                return "*Nothing*";
            f.Seek(pos, SeekOrigin.Begin);
            try
            {
                // Read if there are zeros
                t = (byte)f.ReadByte();
                while (t == 0)
                    t = (byte)f.ReadByte();

                // Read string
                while (t != 0)
                {
                    r += Convert.ToString((char)t);
                    t = (byte)f.ReadByte();
                }
            }
            catch
            {
            }

            return r;
        }

        public static string ToField(string sField)
        {
            // Make the first letter in upper case and the rest in lower case
            string tmp = sField.Substring(0, 1).ToUpper() + sField.Substring(1).ToLower();
            // Replace lowercase object with Object (used in f.ex Gameobject -> GameObject)
            if (tmp.IndexOf("object", StringComparison.OrdinalIgnoreCase) > 0)
            {
                if (tmp.Length > tmp.IndexOf("object", StringComparison.OrdinalIgnoreCase) + 6)
                {
                    tmp = tmp.Substring(0, tmp.IndexOf("object")) + "Object" + tmp.Substring(tmp.IndexOf("object") + 6);
                }
                else
                {
                    tmp = tmp.Substring(0, tmp.IndexOf("object")) + "Object";
                }
            }

            return tmp;
        }

        public static string ToType(int iType, bool full)
        {
            // Get the typename
            switch (iType)
            {
                case 0:
                {
                    return full ? "UF_TYPE_NONE" : "NONE";
                }
                case 1:
                {
                    return full ? "UF_TYPE_INT" : "INT";
                }

                case 2:
                {
                    return full ? "UF_TYPE_TWO_SHORT" : "TWO_SHORT";
                }

                case 3:
                {
                    return full ? "UF_TYPE_FLOAT" : "FLOAT";
                }

                case 4:
                {
                    return full ? "UF_TYPE_GUID" : "GUID";
                }

                case 5:
                {
                    return full ? "UF_TYPE_BYTES" : "BYTES";
                }
                case 6:
                {
                    return full ? "UF_TYPE_BYTES2" : "BYTES2";
                }
                default:
                {
                    return "UNK (" + iType + ")";
                }
            }
        }

        private static void AddFlag(ref string sFlags, string sFlag, bool full)
        {
            if (!full)
                sFlag = sFlag.Replace("UF_FLAG_", "");

            if (!string.IsNullOrEmpty(sFlags))
                sFlags += " + ";
            sFlags += sFlag;
        }

        public static string ToFlags(int iFlags, int build, bool full)
        {
            if (iFlags == 0)
                return full ? "UF_FLAG_NONE" : "NONE";

            string tmp = "";
            if (build <= 12340)
            {
                if (Convert.ToBoolean(iFlags & 1))
                    AddFlag(ref tmp, "UF_FLAG_PUBLIC", full);
                if (Convert.ToBoolean(iFlags & 2))
                    AddFlag(ref tmp, "UF_FLAG_PRIVATE", full);
                if (Convert.ToBoolean(iFlags & 4))
                    AddFlag(ref tmp, "UF_FLAG_OWNER_ONLY", full);
                if (Convert.ToBoolean(iFlags & 8))
                    AddFlag(ref tmp, "UF_FLAG_UNK4", full);
                if (Convert.ToBoolean(iFlags & 16))
                    AddFlag(ref tmp, "UF_FLAG_ITEM_OWNER", full);
                if (Convert.ToBoolean(iFlags & 32))
                    AddFlag(ref tmp, "UF_FLAG_SPECIAL_INFO", full);
                if (Convert.ToBoolean(iFlags & 64))
                    AddFlag(ref tmp, "UF_FLAG_GROUP_ONLY", full);
                if (Convert.ToBoolean(iFlags & 128))
                    AddFlag(ref tmp, "UF_FLAG_UNK8", full);
                if (Convert.ToBoolean(iFlags & 256))
                    AddFlag(ref tmp, "UF_FLAG_DYNAMIC", full);
            }
            else
            {
                if (Convert.ToBoolean(iFlags & 1))
                    AddFlag(ref tmp, "UF_FLAG_PUBLIC", full);
                if (Convert.ToBoolean(iFlags & 2))
                    AddFlag(ref tmp, "UF_FLAG_PRIVATE", full);
                if (Convert.ToBoolean(iFlags & 4))
                    AddFlag(ref tmp, "UF_FLAG_OWNER_ONLY", full);
                if (Convert.ToBoolean(iFlags & 8))
                    AddFlag(ref tmp, "UF_FLAG_ITEM_OWNER", full);
                if (Convert.ToBoolean(iFlags & 16))
                    AddFlag(ref tmp, "UF_FLAG_SPECIAL_INFO", full);
                if (Convert.ToBoolean(iFlags & 32))
                    AddFlag(ref tmp, "UF_FLAG_GROUP_ONLY", full);
                if (Convert.ToBoolean(iFlags & 64))
                    AddFlag(ref tmp, "UF_FLAG_UNK7", full);
                if (Convert.ToBoolean(iFlags & 128))
                    AddFlag(ref tmp, "UF_FLAG_DYNAMIC", full);
            }

            return tmp;
        }

        public static string ToTypeMask(string enumName)
        {
            if (enumName == "Object")
                return "TYPEMASK_OBJECT";
            if (enumName == "Container")
                return "TYPEMASK_CONTAINER";
            if (enumName == "Item")
                return "TYPEMASK_ITEM";
            if (enumName == "Unit")
                return "TYPEMASK_UNIT";
            if (enumName == "Player")
                return "TYPEMASK_PLAYER";
            if (enumName == "GameObject")
                return "TYPEMASK_GAMEOBJECT";
            if (enumName == "DynamicObject")
                return "TYPEMASK_DYNAMICOBJECT";
            if (enumName == "Corpse")
                return "TYPEMASK_CORPSE";
            if (enumName == "Areatrigger")
                return "TYPEMASK_AREATRIGGER";
            return "UNKNOWN_TYPEMASK";
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TypeEntry
        {
            public int Name;
            public int Offset;
            public int Size;
            public int Type;
            public int Flags;
        }

        public static void WriteFileHeader(StreamWriter file, FileVersionInfo versInfo)
        {
            file.WriteLine("// Auto generated file");
            file.WriteLine("// Patch: " + versInfo.FileMajorPart + "." + versInfo.FileMinorPart + "." + versInfo.FileBuildPart);
            file.WriteLine("// Build: " + versInfo.FilePrivatePart);
            file.WriteLine();
        }

        public static void ExtractUpdateFields()
        {
            var versInfo = FileVersionInfo.GetVersionInfo("Wow.exe");
            var f = new FileStream("wow.exe", FileMode.Open, FileAccess.Read, FileShare.Read, 10000000);
            var o1 = new FileStream("UpdateFields.cpp", FileMode.Create, FileAccess.Write, FileShare.None, 1024);
            var o2 = new FileStream("UpdateFields.h", FileMode.Create, FileAccess.Write, FileShare.None, 1024);
            var w1 = new StreamWriter(o1);
            var w2 = new StreamWriter(o2);

            // this is right after the data for size, type and flags of update fields begins
            int OBJECT_FIELD_GUID_PointerAddress = SearchInFile(f, "\u0000\u0000\u0000\u0000\u0002\u0000\u0000\u0000\u0004\u0000\u0000\u0000\u0001\u0000\u0000\u0000");
            if (OBJECT_FIELD_GUID_PointerAddress == -1)
            {
                Console.WriteLine("Cannot find where data for update field types begins!");
                return;
            }

            // read the uint32 before that address
            OBJECT_FIELD_GUID_PointerAddress -= 4;
            f.Seek(OBJECT_FIELD_GUID_PointerAddress, SeekOrigin.Begin);
            var Buffer = new byte[4];
            f.Read(Buffer, 0, 4);
            uint OBJECT_FIELD_GUID_Pointer = BitConverter.ToUInt32(Buffer, 0);

            // find the address of the first update field name
            int OBJECT_FIELD_GUID_NameAddress = SearchInFile(f, "OBJECT_FIELD_GUID");
            if (OBJECT_FIELD_GUID_NameAddress == -1)
            {
                Console.WriteLine("Cannot find OBJECT_FIELD_GUID string!");
                return;
            }
            // substract the address of the name to get the offset
            uint AddressOffset = OBJECT_FIELD_GUID_Pointer - (uint)OBJECT_FIELD_GUID_NameAddress;

            int FieldTypesBegin = OBJECT_FIELD_GUID_PointerAddress;
            int FieldNamesBegin = SearchInFile(f, "AREATRIGGER_FINAL_POS");
            if (FieldNamesBegin == -1)
            {
                FieldNamesBegin = SearchInFile(f, "CORPSE_FIELD_PAD");
            }
            if (FieldNamesBegin == -1)
            {
                FieldNamesBegin = SearchInFile(f, "CORPSE_FIELD_FLAGS");
            }
            if (FieldNamesBegin == -1)
            {
                FieldNamesBegin = SearchInFile(f, "CORPSE_FIELD_LEVEL");
            }
            if (FieldNamesBegin == -1)
            {
                Console.WriteLine("Cannot find last update field name!");
            }
            else
            {
                var Names = new List<string>();
                string Last = "";
                int Offset = FieldNamesBegin;
                f.Seek(Offset, SeekOrigin.Begin);
                while (Last != "OBJECT_FIELD_GUID")
                {
                    Last = ReadString(f);
                    Names.Add(Last);
                }

                var Info = new List<TypeEntry>();
                int Temp;
                Offset = 0;
                f.Seek(FieldTypesBegin, SeekOrigin.Begin);
                for (int i = 0, loopTo = Names.Count - 1; i <= loopTo; i++)
                {
                    f.Seek(FieldTypesBegin + i * 5 * 4 + Offset, SeekOrigin.Begin);
                    f.Read(Buffer, 0, 4);
                    Temp = BitConverter.ToInt32(Buffer, 0);

                    if (Temp < 0xFFFF)
                    {
                        i -= 1;
                        Offset += 4;
                        continue;
                    }

                    long nextAddress = Temp - AddressOffset;
                    if (nextAddress < 0 || nextAddress > f.Length)
                    {
                        i -= 1;
                        Offset += 4;
                        continue;
                    }

                    long savedPosition = f.Position;
                    string fieldName = ReadString(f, nextAddress);
                    if (!string.IsNullOrEmpty(fieldName) && !Names.Contains(fieldName))
                    {
                        Console.WriteLine($"Found field {fieldName} not found in initial list of names.");
                        loopTo += 1;
                    }
                    f.Position = savedPosition;

                    var tmp = new TypeEntry
                    {
                        Name = Temp
                    };
                    f.Read(Buffer, 0, 4);
                    Temp = BitConverter.ToInt32(Buffer, 0);
                    tmp.Offset = Temp;
                    f.Read(Buffer, 0, 4);
                    Temp = BitConverter.ToInt32(Buffer, 0);
                    tmp.Size = Temp;
                    f.Read(Buffer, 0, 4);
                    Temp = BitConverter.ToInt32(Buffer, 0);
                    tmp.Type = Temp;
                    f.Read(Buffer, 0, 4);
                    Temp = BitConverter.ToInt32(Buffer, 0);
                    tmp.Flags = Temp;
                    Info.Add(tmp);
                }

                Console.WriteLine(string.Format("{0} fields extracted.", Names.Count));
                WriteFileHeader(w1, versInfo);
                WriteFileHeader(w2, versInfo);

                string LastFieldType = "";
                string sName;
                string sField;
                int BasedOn = 0;
                string BasedOnName = "";
                var EndNum = new Dictionary<string, int>();
                int lastValidIndex = 0;

                // on first iteration just add the enum names
                for (int j = 0, loopTo1 = Info.Count - 1; j <= loopTo1; j++)
                {
                    long nextAddress = Info[j].Name - AddressOffset;
                    if (nextAddress < 0 || nextAddress > f.Length)
                    {
                        Console.WriteLine("Wrong address for update field name! " + nextAddress);
                        continue;
                    }
                    sName = ReadString(f, nextAddress);
                    if (!string.IsNullOrEmpty(sName))
                    {
                        sField = ToField(sName.Substring(0, sName.IndexOf("_")));
                        if (sName == "UINT_FIELD_BASESTAT0" ||
                            sName == "UINT_FIELD_BASESTAT1" ||
                            sName == "UINT_FIELD_BASESTAT2" ||
                            sName == "UINT_FIELD_BASESTAT3" ||
                            sName == "UINT_FIELD_BASESTAT4" ||
                            sName == "UINT_FIELD_BYTES_1")
                            sField = "Unit";
                        if (sName == "OBJECT_FIELD_CREATED_BY")
                            sField = "GameObject";
                        if ((LastFieldType ?? "") != (sField ?? ""))
                        {
                            if (!string.IsNullOrEmpty(LastFieldType))
                            {
                                EndNum.Add(LastFieldType, Info[lastValidIndex].Offset + 1);
                            }
                            LastFieldType = sField;
                        }
                        lastValidIndex = j;
                    }
                }

                sField = "";
                LastFieldType = "";

                lastValidIndex = 0;
                for (int j = 0, loopTo1 = Info.Count - 1; j <= loopTo1; j++)
                {
                    long nextAddress = Info[j].Name - AddressOffset;
                    if (nextAddress < 0 || nextAddress > f.Length)
                    {
                        Console.WriteLine("Wrong address for update field name! " + nextAddress);
                        w1.WriteLine("// An error occurred while reading field at this spot");
                        w2.WriteLine("// An error occurred while reading field at this spot");
                        continue;
                    }
                    sName = ReadString(f, nextAddress);
                    if (!string.IsNullOrEmpty(sName))
                    {
                        sField = ToField(sName.Substring(0, sName.IndexOf("_")));
                        if (sName == "UINT_FIELD_BASESTAT0" ||
                            sName == "UINT_FIELD_BASESTAT1" ||
                            sName == "UINT_FIELD_BASESTAT2" ||
                            sName == "UINT_FIELD_BASESTAT3" ||
                            sName == "UINT_FIELD_BASESTAT4" ||
                            sName == "UINT_FIELD_BYTES_1")
                            sField = "Unit";
                        if (sName == "OBJECT_FIELD_CREATED_BY")
                            sField = "GameObject";
                        if ((LastFieldType ?? "") != (sField ?? ""))
                        {
                            if (!string.IsNullOrEmpty(LastFieldType))
                            {
                                if (LastFieldType.ToLower() == "object")
                                {
                                    w1.WriteLine("{{ {0, -22}, {1, -50}, {2, -5}, {3, -3}, {4, -17}, {5} }},", ToTypeMask(LastFieldType), "\"" + LastFieldType.ToUpper() + "_END" + "\"", "0x" + (Info[lastValidIndex].Offset + Info[lastValidIndex].Size).ToString("X"), 0, ToType(0, true), ToFlags(0, versInfo.FilePrivatePart, true));
                                    w2.WriteLine("    {0,-48} = {1,-20}", LastFieldType.ToUpper() + "_END", "0x" + (Info[lastValidIndex].Offset + Info[lastValidIndex].Size).ToString("X"));
                                }
                                else
                                {
                                    int address = BasedOn + Info[lastValidIndex].Offset + Info[lastValidIndex].Size;
                                    string basedOnFieldName = BasedOnName.Substring(BasedOnName.IndexOf('.') + 1);
                                    if (basedOnFieldName != "OBJECT_END")
                                        address += EndNum["Object"];

                                    w1.WriteLine("{{ {0, -22}, {1, -50}, {2, -5}, {3, -3}, {4, -17}, {5} }},", ToTypeMask(LastFieldType), "\"" + LastFieldType.ToUpper() + "_END" + "\"", "0x" + (address).ToString("X"), 0, ToType(0, true), ToFlags(0, versInfo.FilePrivatePart, true));
                                    w2.WriteLine("    {0,-48} = {1,-20}// 0x{2:X3}", LastFieldType.ToUpper() + "_END", basedOnFieldName + " + 0x" + (Info[lastValidIndex].Offset + Info[lastValidIndex].Size).ToString("X"), BasedOn + Info[lastValidIndex].Offset + Info[lastValidIndex].Size);
                                }
                                w2.WriteLine("};");
                            }

                            w1.WriteLine("// enum E" + sField + "Fields");
                            w2.WriteLine("enum E" + sField + "Fields");
                            w2.WriteLine("{");

                            if (sField.ToLower() == "container")
                            {
                                BasedOn = EndNum["Item"];
                                BasedOnName = "EItemFields.ITEM_END";
                            }
                            else if (sField.ToLower() == "player")
                            {
                                BasedOn = EndNum["Unit"];
                                BasedOnName = "EUnitFields.UNIT_END";
                            }
                            else if (sField.ToLower() != "object")
                            {
                                BasedOn = EndNum["Object"];
                                BasedOnName = "EObjectFields.OBJECT_END";
                            }

                            LastFieldType = sField;
                        }

                        if (BasedOn > 0)
                        {
                            int address = BasedOn + Info[j].Offset;
                            string basedOnFieldName = BasedOnName.Substring(BasedOnName.IndexOf('.') + 1);
                            if (basedOnFieldName != "OBJECT_END")
                                address += EndNum["Object"];

                            w1.WriteLine("{{ {0, -22}, {1, -50}, {2, -5}, {3, -3}, {4, -17}, {5} }},", ToTypeMask(sField), "\"" + sName + "\"", "0x" + (address).ToString("X"), Info[j].Size, ToType(Info[j].Type, true), ToFlags(Info[j].Flags, versInfo.FilePrivatePart, true));
                            w2.WriteLine("    {0,-48} = {1,-20}// 0x{2:X3} - Size: {3} - Type: {4} - Flags: {5}", sName, basedOnFieldName + " + 0x" + (Info[j].Offset).ToString("X") + ",", BasedOn + Info[j].Offset, Info[j].Size, ToType(Info[j].Type, false), ToFlags(Info[j].Flags, versInfo.FilePrivatePart, false));
                        }
                        else
                        {
                            w1.WriteLine("{{ {0, -22}, {1, -50}, {2, -5}, {3, -3}, {4, -17}, {5} }},", ToTypeMask(sField), "\"" + sName + "\"", "0x" + (Info[j].Offset).ToString("X"), Info[j].Size, ToType(Info[j].Type, true), ToFlags(Info[j].Flags, versInfo.FilePrivatePart, true));
                            w2.WriteLine("    {0,-48} = {1,-20}// 0x{2:X3} - Size: {3} - Type: {4} - Flags: {5}", sName, "0x" + (Info[j].Offset).ToString("X") + ",", Info[j].Offset, Info[j].Size, ToType(Info[j].Type, false), ToFlags(Info[j].Flags, versInfo.FilePrivatePart, false));
                        }

                        lastValidIndex = j;
                    }
                }

                if (!string.IsNullOrEmpty(LastFieldType))
                {
                    int address = BasedOn + Info[Info.Count - 1].Offset + Info[Info.Count - 1].Size;
                    string basedOnFieldName = BasedOnName.Substring(BasedOnName.IndexOf('.') + 1);
                    if (basedOnFieldName != "OBJECT_END")
                        address += EndNum["Object"];
                    w1.WriteLine("{{ {0, -22}, {1, -50}, {2, -5}, {3, -3}, {4, -17}, {5} }},", ToTypeMask(sField), "\"" + LastFieldType.ToUpper() + "_END" + "\"", "0x" + (address).ToString("X"), 0, ToType(0, true), ToFlags(0, versInfo.FilePrivatePart, true));
                    w2.WriteLine("    {0,-48} = {1,-20}// 0x{2:X3}", LastFieldType.ToUpper() + "_END", basedOnFieldName + " + 0x" + (Info[Info.Count - 1].Offset + Info[Info.Count - 1].Size).ToString("X"), BasedOn + Info[Info.Count - 1].Offset + Info[Info.Count - 1].Size);
                }

                w1.Flush();
                w2.WriteLine("};");
                w2.Flush();
            }

            o1.Close();
            o2.Close();
            f.Close();
        }
        static void Main(string[] args)
        {
            ExtractUpdateFields();
        }
    }
}
