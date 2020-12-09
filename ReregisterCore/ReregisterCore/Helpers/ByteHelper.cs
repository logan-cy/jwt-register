using ReregisterCore.Models;
using ReregisterCore.Viewmodels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ReregisterCore.Helpers
{
    public static class ByteHelper
    {
        public static byte[] ConvertUserToByteArray(UserViewmodel obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static UserViewmodel ConvertUserFromByteArray(byte[] arr)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                ms.Write(arr, 0, arr.Length);
                ms.Seek(0, SeekOrigin.Begin);
                var obj = bf.Deserialize(ms);
                return (UserViewmodel)obj;
            }
        }
    }
}
