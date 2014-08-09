using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Demo
{
    

    internal class Class1
    {


        public static List<ListViewItem> galFileNameGroup = new List<ListViewItem>();

        public static void getAllDir(string path)
        {
            //文件夹栈，用于消除递归
            List<string> folders = new List<string>();

            if (Directory.Exists(path))
            {
                folders.Add(path);//栈初始化


                while (folders.Count > 0)
                {
                    string[] fileList;//文件列表

                    //检查该目录是否可以打开，不可以打开直接读取下一个文件夹
                    try
                    {
                        fileList = Directory.GetFileSystemEntries(path);
                    }
                    catch (System.Exception ex)
                    {
                        if (folders.Count > 0)//读取下一个文件夹
                        {
                            folders.RemoveAt(0);//移除目录
                            path = folders[0];
                        }

                        continue;
                    }

                    folders.RemoveAt(0);//移除目录

                    foreach (string file in fileList)
                    {
                        if (Directory.Exists(file))
                        {
                            //遇到文件夹就保存文件名以便日后展开
                            folders.Add(file);
                        }
                        else
                        {
                            //文件
                            string fileName = Path.GetFileName("@" + file);//获取文件名

                            //判断是否有扩展名
                            if (fileName.LastIndexOf('.') != -1)
                            {
                                string exname = fileName.Substring(fileName.LastIndexOf('.') + 1);//获得扩展名
                                Match exnameMatch = Regex.Match(exname, "(exe|iso|rar)");//正则匹配扩展名
                                if (exnameMatch.Success)
                                {
                                    ListViewItem lvi = new ListViewItem();
                                    ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();

                                    //添加第一列(游戏名称)
                                    lvi.Text = fileName.Substring(0, fileName.LastIndexOf('.'));
                                    lvi.Tag = fileName;

                                    //添加第二行(游戏路径)
                                    lvsi.Text = Path.GetFullPath(file);
                                    lvi.SubItems.Add(lvsi);

                                    //添加到表格中
                                    galFileNameGroup.Add(lvi);
                                }
                            }
                        }
                    }
                    if (folders.Count > 0)//读取下一个文件夹
                        path = folders[0];
                }
            }
        }
    }
}
