using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class IotSignUtils
    {
        public static string sign(Dictionary<string, string> param,
            string deviceSecret, string signMethod)
        {
            string[] sortedKey = param.Keys.ToArray();
            Array.Sort(sortedKey);

            StringBuilder builder = new StringBuilder();
            foreach (var i in sortedKey)
            {
                builder.Append(i).Append(param[i]);
            }

            byte[] key = Encoding.UTF8.GetBytes(deviceSecret);
            byte[] signContent = Encoding.UTF8.GetBytes(builder.ToString());
            //这里是hmacmd5编码来加密
            var hmac = new HMACMD5(key);
            byte[] hashBytes = hmac.ComputeHash(signContent);
            StringBuilder signBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
                signBuilder.AppendFormat("{0:x2}", b);

            return signBuilder.ToString();
        }
    }
}
