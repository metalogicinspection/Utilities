using System;
using System.IO;
using SevenZip;

namespace Metalogic.DataUtil.SevenZip
{
    public class CompressHelper
    {
        static int dictionary = 1 << 23;

        // static Int32 posStateBits = 2;
        // static  Int32 litContextBits = 3; // for normal files
        // UInt32 litContextBits = 0; // for 32-bit data
        // static  Int32 litPosBits = 0;
        // UInt32 litPosBits = 2; // for 32-bit data
        // static   Int32 algorithm = 2;
        // static    Int32 numFastBytes = 128;

        static bool eos = false;

        static CoderPropID[] propIDs = 
				{
					CoderPropID.DictionarySize,
					CoderPropID.PosStateBits,
					CoderPropID.LitContextBits,
					CoderPropID.LitPosBits,
					CoderPropID.Algorithm,
					CoderPropID.NumFastBytes,
					CoderPropID.MatchFinder,
					CoderPropID.EndMarker
				};

        // these are the default properties, keeping it simple for now:
        static object[] properties = 
				{
					(Int32)(dictionary),
					(Int32)(2),
					(Int32)(3),
					(Int32)(0),
					(Int32)(2),
					(Int32)(128),
					"bt4",
					eos
				};

        public static byte[] Compress(byte[] inputBytes)
        {
            MemoryStream inStream = new MemoryStream(inputBytes);
            MemoryStream outStream = new MemoryStream();
            global::SevenZip.Compression.LZMA.Encoder encoder = new global::SevenZip.Compression.LZMA.Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            long fileSize = inStream.Length;
            for (int i = 0; i < 8; i++)
                outStream.WriteByte((Byte)(fileSize >> (8 * i)));
            encoder.Code(inStream, outStream, -1, -1, null);
            return outStream.ToArray();
        }

        public static Stream Compress(Stream inStream)
        {
            //MemoryStream inStream = new MemoryStream(inputBytes);
            MemoryStream outStream = new MemoryStream();
            global::SevenZip.Compression.LZMA.Encoder encoder = new global::SevenZip.Compression.LZMA.Encoder();
            try
            {
                encoder.SetCoderProperties(propIDs, properties);
                encoder.WriteCoderProperties(outStream);
            }
            catch (Exception ex)
            {
                string l = ex.Message;
            }
            long fileSize = inStream.Length;
            for (int i = 0; i < 8; i++)
                outStream.WriteByte((Byte)(fileSize >> (8 * i)));
            encoder.Code(inStream, outStream, -1, -1, null);
            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
        }

        public static byte[] Decompress(byte[] inputBytes)
        {
            MemoryStream newInStream = new MemoryStream(inputBytes);

            global::SevenZip.Compression.LZMA.Decoder decoder = new global::SevenZip.Compression.LZMA.Decoder();

            newInStream.Seek(0, 0);
            MemoryStream newOutStream = new MemoryStream();

            byte[] properties2 = new byte[5];
            if (newInStream.Read(properties2, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = newInStream.ReadByte();
                if (v < 0)
                    throw (new Exception("Can't Read 1"));
                outSize |= ((long)(byte)v) << (8 * i);
            }
            decoder.SetDecoderProperties(properties2);

            long compressedSize = newInStream.Length - newInStream.Position;
            decoder.Code(newInStream, newOutStream, compressedSize, outSize, null);

            byte[] b = newOutStream.ToArray();

            return b;
        }

        public static Stream Decompress(Stream inStream)
        {
            MemoryStream newInStream = new MemoryStream();
            int mSize;
            Byte[] mWriteData = new Byte[32768];
            while (true)
            {
                mSize = inStream.Read(mWriteData, 0, mWriteData.Length);
                if (mSize > 0)
                {
                    newInStream.Write(mWriteData, 0, mSize);
                }
                else
                {
                    break;
                }
            }
            inStream.Close();

            global::SevenZip.Compression.LZMA.Decoder decoder = new global::SevenZip.Compression.LZMA.Decoder();

            newInStream.Seek(0, SeekOrigin.Begin);
            MemoryStream newOutStream = new MemoryStream();

            byte[] properties2 = new byte[5];
            if (newInStream.Read(properties2, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = newInStream.ReadByte();
                if (v < 0)
                    throw (new Exception("Can't Read 1"));
                outSize |= ((long)(byte)v) << (8 * i);
                //outSize |= ((long)(byte)v) << (4 * i);
            }
            decoder.SetDecoderProperties(properties2);

            long compressedSize = newInStream.Length - newInStream.Position;
            decoder.Code(newInStream, newOutStream, compressedSize, outSize, null);
            newOutStream.Seek(0, SeekOrigin.Begin);
            return newOutStream;
        }
    }
}
