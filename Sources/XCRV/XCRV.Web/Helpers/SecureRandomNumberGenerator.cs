using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace XCRV.Web.Helpers
{
    public class SecureRandomNumberGenerator
    {
		public static int SecureRandomNo()
		{
			int randomvalue = 0;
			using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
			{
				byte[] val = new byte[6];
				crypto.GetBytes(val);
				randomvalue = BitConverter.ToInt32(val, 1);
			}
			return randomvalue;

		}
	}
}
