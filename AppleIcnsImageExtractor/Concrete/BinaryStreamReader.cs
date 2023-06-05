using MediaWidget.Core.Abstract;
using MediaWidget.Core.Models;

namespace MediaWidget.Core.Concrete
{
    using System;
    using System.IO;
    using System.Text;

    public class BinaryStreamReader : IBinaryStreamReader
    {
        private BinaryReader reader;

        private Stream stream;

        public bool Eof => Stream?.Position == Stream?.Length;

        public Stream Stream
        {
            get => stream;

            set
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }

                stream = value;

                if (stream != null)
                {
                    reader = new BinaryReader(stream, Encoding.ASCII, true);
                }
            }
        }

        public string ReadAnsiString(int byteCount, Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(byteCount, endianess);

            if (data != null)
            {
                return Encoding.ASCII.GetString(data, 0, data.Length);
            }

            return string.Empty;
        }

        public bool ReadBool(Endianess endianess = Endianess.LittleEndian)
        {
            return ReadInt32(endianess) != 0;
        }

        public byte ReadByte()
        {
            var data = CheckedRead(1);

            if (data != null)
            {
                return data[0];
            }

            return 0;
        }

        public byte[] ReadBytes(int count, Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(count, endianess);

            return data ?? new byte[0];
        }

        public short ReadInt16(Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(sizeof(short), endianess);

            if (data != null)
            {
                return BitConverter.ToInt16(data, 0);
            }

            return 0;
        }

        public int ReadInt32(Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(sizeof(int), endianess);

            if (data != null)
            {
                return BitConverter.ToInt32(data, 0);
            }

            return 0;
        }

        public long ReadInt64(Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(sizeof(long), endianess);

            if (data != null)
            {
                return BitConverter.ToInt64(data, 0);
            }

            return 0;
        }

        public float ReadSingle(Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(sizeof(float), endianess);

            if (data != null)
            {
                return BitConverter.ToSingle(data, 0);
            }

            return 0;
        }

        public ushort ReadUInt16(Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(sizeof(ushort), endianess);

            if (data != null)
            {
                return BitConverter.ToUInt16(data, 0);
            }

            return 0;
        }

        public uint ReadUInt24(Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(3, endianess);

            if (data != null)
            {
                var fullData = new byte[4];
                Array.Copy(data, 0, fullData, 1, 3);
                return BitConverter.ToUInt32(fullData, 0);
            }

            return 0;
        }

        public uint ReadUInt32(Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(sizeof(uint), endianess);

            if (data != null)
            {
                return BitConverter.ToUInt32(data, 0);
            }

            return 0;
        }

        public ulong ReadUInt64(Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(sizeof(ulong), endianess);

            if (data != null)
            {
                return BitConverter.ToUInt64(data, 0);
            }

            return 0;
        }

        public string ReadUnicodeString(int byteCount, Endianess endianess = Endianess.LittleEndian)
        {
            var data = CheckedRead(byteCount, endianess);

            if (data != null)
            {
                return Encoding.Unicode.GetString(data, 0, data.Length);
            }

            return string.Empty;
        }

        private byte[] CheckedRead(int byteCount, Endianess endianess = Endianess.LittleEndian)
        {
            if (Eof)
            {
                return null;
            }

            var buff = reader.ReadBytes(byteCount);

            if (endianess == Endianess.BigEndian)
            {
                Array.Reverse(buff);
            }

            return buff;
        }
    }
}