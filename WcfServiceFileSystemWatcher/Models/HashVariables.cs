using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfServiceFileSystemWatcher.Models
{
    public class HashVariables
    {
        /// <summary>
        /// 拆分份数
        /// </summary>
        public static int BreakNumber = 5;

        /// <summary>
        /// 达到需要拆分的大小(字节),
        /// </summary>
        public static long EnoughToBreakup = 4096 * 5;

        /// <summary>
        /// 每个切片的长度
        /// </summary>
        public static int EachLength = 4096;
    }
}
