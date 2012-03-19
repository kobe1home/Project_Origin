using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace Project_Origin_Server
{
    public static class Serializer<T>
    {
        public static byte[] SerializeObject(object pObjectToSerialize)
        {
            BinaryFormatter lFormatter = new BinaryFormatter();
            MemoryStream lStream = new MemoryStream();
            lFormatter.Serialize(lStream, pObjectToSerialize);
            byte[] lRet = new byte[lStream.Length];
            lStream.Position = 0;
            lStream.Read(lRet, 0, (int)lStream.Length);
            lStream.Close();
            //foreach (byte value in lRet)
            //{
            //    Console.Write((char)value);
            //}
            return lRet;
        }
        public static T DeserializeObject<T>(byte[] pData)
        {
            if (pData == null)
                return default(T);
            //foreach (byte value in pData)
            //{
            //    Console.Write((char)value);
            //}
            //Console.WriteLine();
            BinaryFormatter lFormatter = new BinaryFormatter();
            MemoryStream lStream = new MemoryStream(pData);
            object lRet = lFormatter.Deserialize(lStream);
            lStream.Close();
            return (T)lRet;
        }
    }

}
