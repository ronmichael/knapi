using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapi
{
    public class Tools
    {

        /// <summary>
        /// Base64 encode a user name and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Base64Encode(string username, string password)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(username + ":" + password);
            return Convert.ToBase64String(bytes);
        }


    }
}
