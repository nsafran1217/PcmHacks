using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace PcmHacking
{
    public class KeyAlgorithm
    {
        /// <summary>
        /// Gets the unlock key from the given key file.
        /// </summary>
        /// <remarks>
        /// keys_XX.bin files 65535 UInt16 arrays of keys, in order of seed 0000->FFFF
        /// To add an algorithm add the binary file as a resource, and add it to the switch statement here
        /// Each additional algo adds 128kb to the application
        /// </remarks>
        public static UInt16 GetKey(int algo, UInt16 seed)
        {
            switch (algo)
            {
                case 0x00:
                case 0x01:
                case 0x0F: // Black Box
                case 0x14: // P04
                case 0x28: // P01, P59
                case 0x36: // E54 LB7 Duramax
                    string keydata = string.Format("keys_{0}", algo.ToString("X2"));
                    global::System.Resources.ResourceManager rm = new global::System.Resources.ResourceManager("PcmHacking.Properties.Resources", typeof(PcmHacking.Properties.Resources).Assembly);
                    byte [] keys = (byte []) rm.GetObject(keydata, Properties.Resources.Culture);
                    return (UInt16)((keys[seed*2] << 8) + keys[(seed*2)+1]);
            }

            return 0x0000; // invalid or not available algo
        }
    }
}
