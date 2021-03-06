﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Apsiyon.Logger.Helper
{
    public class ObjectHelper
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null) return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }

        }

        public static object ByteArrayToObject(byte[] bytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(bytes, 0, bytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}