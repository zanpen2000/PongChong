using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceFileSystemWatcher.Models
{

    /*
 * 快速计算任意大小的文件的MD5，调用方法如下：
 * 
        string fp = @"d:\databases\Dremis_LandTax_fgj_1.ldf";
        string hexStr = FileHash.calc(fp);
        Console.WriteLine(hexStr);
        Console.ReadKey();
 */

    /// <summary>
    /// 给定文件, 根据策略计算并生成文件的Md5， 以16进制字符串的形式返回
    /// Author：张鹏
    /// Date：2013/5/31
    /// </summary>
    public class FileHash
    {
        /// <summary>
        /// 拆分份数
        /// </summary>
        static int BreakNumber = HashVariables.BreakNumber;

        /// <summary>
        /// 达到需要拆分的大小(字节),
        /// </summary>
        static public long EnoughToBreakup = HashVariables.EnoughToBreakup;

        /// <summary>
        /// 每份length
        /// </summary>
        static int EachLength = HashVariables.EachLength;

        static MD5 M = MD5.Create();
        static FileInfo Info;
        static List<byte[]> listBytes = new List<byte[]>();


        /// <summary>
        /// 以16进制字符串返回指定文件的md5
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static string Calc(string fileName)
        {
            try
            {
                return Get(fileName);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string Get(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

            Info = new FileInfo(filename);

            byte[] hashBytes;
            try
            {
                if (__needBreakup())
                {
                    long seag = Info.Length / BreakNumber;
                    if (fs.CanSeek)
                    {
                        for (int i = 0; i < listBytes.Count; i++)
                        {
                            fs.Seek(i * seag, SeekOrigin.Begin);
                            fs.Read(listBytes[i], 0, EachLength);

                            M.ComputeHash(listBytes[i]);
                        }
                        hashBytes = M.Hash;
                    }
                    else throw new IOException("文件内指针定位失败!");
                }
                else
                {
                    hashBytes = M.ComputeHash(fs);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                fs.Close();
            }
            return __byteToHexStr(hashBytes);
        }

        private static bool __needBreakup()
        {
            listBytes.Clear();
            if (Info.Length >= EnoughToBreakup)
            {
                //初始化字节数组
                for (int i = 0; i < BreakNumber; i++)
                {
                    listBytes.Add(new byte[EachLength]);
                }
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 字节数组转为16进制字符串
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        private static string __byteToHexStr(byte[] md5)
        {
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < md5.Length; i++)
            {
                sb.Append(md5[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

   
}
