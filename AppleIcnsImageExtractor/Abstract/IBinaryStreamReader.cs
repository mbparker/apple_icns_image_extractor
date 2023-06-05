using MediaWidget.Core.Models;

namespace MediaWidget.Core.Abstract
{
    public interface IBinaryStreamReader
    {
        bool Eof { get; }

        Stream Stream { get; set; }

        string ReadAnsiString(int byteCount, Endianess endianess = Endianess.LittleEndian);

        bool ReadBool(Endianess endianess = Endianess.LittleEndian);

        byte ReadByte();

        byte[] ReadBytes(int count, Endianess endianess = Endianess.LittleEndian);

        short ReadInt16(Endianess endianess = Endianess.LittleEndian);

        int ReadInt32(Endianess endianess = Endianess.LittleEndian);

        long ReadInt64(Endianess endianess = Endianess.LittleEndian);

        float ReadSingle(Endianess endianess = Endianess.LittleEndian);

        ushort ReadUInt16(Endianess endianess = Endianess.LittleEndian);

        uint ReadUInt24(Endianess endianess = Endianess.LittleEndian);

        uint ReadUInt32(Endianess endianess = Endianess.LittleEndian);

        ulong ReadUInt64(Endianess endianess = Endianess.LittleEndian);

        string ReadUnicodeString(int byteCount, Endianess endianess = Endianess.LittleEndian);
    }
}