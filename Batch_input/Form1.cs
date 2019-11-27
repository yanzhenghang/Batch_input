using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;
//using System.Text;
//using System.Text;
using System.Threading;
using System.IO;


namespace Batch_input
{

    //public struct POINT
    //{
    //    public int x;
    //    public int y;
    //}
    //public static class APIMethod
    //{
    //    [DllImport("user32.dll")]
    //    static extern IntPtr WindowFromPoint(POINT Point);

    //    [DllImport("user32.dll")]
    //    static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

    //    [DllImport("user32.dll")]
    //    static extern bool GetCursorPos(out POINT lpPoint);

    //    [DllImport("user32.dll")]
    //    static extern bool SetWindowText(IntPtr hWnd, string lpString);

    //    public static POINT GetCursorPos()
    //    {
    //        POINT p;
    //        if (GetCursorPos(out p))
    //        {
    //            return p;
    //        }
    //        throw new Exception();
    //    }

    //    public static IntPtr WindowFromPoint()
    //    {
    //        POINT p = GetCursorPos();
    //        return WindowFromPoint(p);
    //    }
    //}


    public partial class Form1 : Form
    {
        const int WM_SETTEXT = 0x000C;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_CLOSE = 0x0010;
        const int WM_CHAR = 0x0102;

        StringBuilder window_name = new StringBuilder(256);
        StringBuilder window_class_name = new StringBuilder(256);
        IList<FileInfo> lst;
        string PATH_SELECTED = "C:\\"; //默认，防止报错
        POINTAPI point = new POINTAPI();
        bool Checked_ramdom = false;
        bool Checked_moren = false;


        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll ")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string FrmText);


        [StructLayout(LayoutKind.Sequential)]//定义与API相兼容结构体，实际上是一种内存转换  
        public struct POINTAPI
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]//获取鼠标坐标  
        public static extern int GetCursorPos(
            ref POINTAPI lpPoint
        );

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]//指定坐标处窗体句柄  
        public static extern int WindowFromPoint(
            int xPoint,
            int yPoint
        );

        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(
            int hWnd,
            StringBuilder lpString,
            int nMaxCount
        );

        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        public static extern int GetClassName(
            int hWnd,
            StringBuilder lpString,
            int nMaxCont
        );



        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private static void GetDirectorys(string strPath, ref List<string> lstDirect)
        {
            DirectoryInfo diFliles = new DirectoryInfo(strPath);
            DirectoryInfo[] diArr = diFliles.GetDirectories();
            //DirectorySecurity directorySecurity = null;
            foreach (DirectoryInfo di in diArr)
            {
                try
                {
                    //directorySecurity = new DirectorySecurity(di.FullName, AccessControlSections.Access);
                    //if (!directorySecurity.AreAccessRulesProtected)
                    //{
                    lstDirect.Add(di.FullName);
                    GetDirectorys(di.FullName, ref lstDirect);
                    //}
                }
                catch
                {
                    continue;
                }
            }
        }
        /// <summary>
        /// 遍历当前目录及子目录
        /// </summary>
        //<param name="strPath">文件路径</param>
        /// <returns>所有文件</returns>
        private static IList<FileInfo> GetFiles(string strPath)
        {
            List<FileInfo> lstFiles = new List<FileInfo>();
            
            List<string> lstDirect = new List<string>();
            lstDirect.Add(strPath);
            DirectoryInfo diFliles = null;
            GetDirectorys(strPath, ref lstDirect);
            foreach (string str in lstDirect)
            {
                try
                {
                    diFliles = new DirectoryInfo(str);

                    List<FileInfo> lstFiles_eachDir = new List<FileInfo>();
                    List<FileInfo> lstFiles_eachDir_clean = new List<FileInfo>();
                    List<FileInfo> lstFiles_eachDir_out = new List<FileInfo>();
                    lstFiles_eachDir.AddRange(diFliles.GetFiles());
                    
                    for (int i = 0; i < lstFiles_eachDir.Count; i++)
                    {
                        
                        String tmp = lstFiles_eachDir[i].Name.ToLower();
                        
                        int count = 0;
                        if (tmp.Contains("clean"))
                        {
                            lstFiles_eachDir_clean.Add(lstFiles_eachDir[i]);
                            count++;
                        }
                        else if (tmp.Contains("set"))
                        {
                            String last_item = (i == 0 ? tmp : lstFiles_eachDir[i - 1].Name.ToLower());
                            if (last_item.Contains("set"))
                            {
                                int val = tmp.IndexOf('_');
                                int val_last = last_item.IndexOf('_');
                                if (val > 0)
                                {

                                    if (val_last > 0)
                                    {
                                        if (tmp[val - 1] != last_item[val_last - 1])
                                        {
                                            if (lstFiles_eachDir_clean.Count > 0)
                                            {
                                                lstFiles_eachDir_out.Add(lstFiles_eachDir_clean[0]);
                                                lstFiles_eachDir_clean.RemoveAt(0);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (lstFiles_eachDir_clean.Count > 0)
                                        {
                                            lstFiles_eachDir_out.Add(lstFiles_eachDir_clean[0]);
                                            lstFiles_eachDir_clean.RemoveAt(0);
                                        }

                                    }
                                }
                            }

                           
                            lstFiles_eachDir_out.Add(lstFiles_eachDir[i]);


                        }                        

                    }

                    lstFiles_eachDir_out.AddRange(lstFiles_eachDir_clean);
                    //lstFiles_eachDir.RemoveAt(i);
                    lstFiles.AddRange(lstFiles_eachDir_out);

                    //lstFiles.AddRange(diFliles.GetFiles());
                }
                catch
                {
                    continue;
                }
            }
            return lstFiles;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog P_File_Folder = new FolderBrowserDialog();

            if (P_File_Folder.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = P_File_Folder.SelectedPath;
                //MessageBox.Show(P_File_Folder.SelectedPath);//选定目录后打印路径信息           
            }

            string str = P_File_Folder.SelectedPath.ToString();//@"E:\";

            if (!str.EndsWith("\\"))
            {
                str += "\\";
            }
            PATH_SELECTED = string.Copy(str);
            //IList<FileInfo> lst = GetFiles(str);
            richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾  
            richTextBox1.ScrollToCaret();//滚动到控件光标处   
            richTextBox1.AppendText("正在查找该路径下的文本文件，请等待…\r\n");
            lst = GetFiles(str);

            for (int i = 0; i < lst.Count; i++)
            {
                string tmp0 = lst[i].ToString();

                string tmp1 = tmp0.Substring(tmp0.Length - 3, 3);
                //tmp0.Remove(0,tmp0.Length-3);
                int tmp_value = string.Compare("txt", tmp1, true);
                if (tmp_value != 0)
                {
                    lst.Remove(lst[i]);
                    i = i - 1;
                }
                else
                {
                    richTextBox1.Focus(); //让文本框获取焦点 
                    richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾  
                    richTextBox1.ScrollToCaret();//滚动到控件光标处   
                    richTextBox1.AppendText(lst[i].ToString() + "\r\n".ToString());//添加内容  
                }

            }
            richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾  
            richTextBox1.ScrollToCaret();//滚动到控件光标处   
            richTextBox1.AppendText("找到 " + lst.Count.ToString() + " 个文本文件！" + "\r\n".ToString());



            //MessageBox.Show(lst[1].ToString());


            //if (!Directory.Exists(str))
            //{
            //    try
            //    {
            //        Directory.CreateDirectory(str);
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message);
            //        Console.ReadKey();
            //        return;
            //    }
            //}


            //if (File.Exists(str + "*.txt"))
            //{

            //File.Delete(str + "test.txt");
            //}


            //FileInfo file = new FileInfo(str + "test.txt");
            //if (!file.Directory.Exists)
            //{
            //    Directory.CreateDirectory(file.DirectoryName);
            //}
            //using (StreamWriter outFileWriter = new StreamWriter(str + "test.txt", false, Encoding.UTF8))
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach (FileInfo item in lst)
            //    {
            //        sb.Append("\"");
            //        sb.Append(item.FullName);
            //        sb.Append("\"");
            //        sb.Append(",");
            //        sb.Append("\r\n");
            //    }
            //    sb.Remove(sb.Length - 2, 2);
            //    outFileWriter.WriteLine(sb.ToString());
            //    outFileWriter.Flush();
            //    outFileWriter.Close();
            //}
            //Console.WriteLine("END");
            //Console.ReadKey();




            //this.richTextBox1.Focus();
            //for (int i = 65; i < 91; i++)
            //{
            //    char MyChar = (char)i;
            //    SendKeys.Send(MyChar.ToString());
            //    System.Threading.Thread.Sleep(100);//当前线程挂起指定的时间
            //    SendKeys.Flush();//将信息从基础缓冲区移动到其目标，或消除缓冲区，或同时执行两种操作。                
            //}

        }

        public struct FilesInPath
        {
            public int Start;
            public int Len;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<FilesInPath> groups = new List<FilesInPath>();
            FilesInPath element = new FilesInPath();
            for (int i = 0; i < lst.Count; i++)
            {
                int j = 1;
                string tmp = lst[i].Directory.ToString();
                while (i + j < lst.Count)
                {                    
                    string tmp1 = lst[i + j].DirectoryName.ToString();                    
                    if (string.Compare(tmp, tmp1) == 0)
                    {
                        j = j + 1;
                    }
                    else
                    {
                        break;                       
                    }
                }
                element.Start = i;
                element.Len = j;
                groups.Add(element);
                i = i + j - 1;
                //groups.Add();
            }
            if (Checked_ramdom)
            {
                //List<int> list = new List<int>();
                //list.Sort();// 升序排序
                //list.Reverse();// 反转顺序
                //Collections.shuffle(groups);
                groups =  groups.OrderBy(x => Guid.NewGuid()).ToList();
            }  
            



            //FilesInPath FF = new FilesInPath();

            //    int [] shunxu_table = new int [lst.Count];
            //for (int i = 0; i < lst.Count; i++)
            //{ shunxu_table[i] = i; }

            //if (Checked_ramdom)
            //{
            //    shunxu_table = shunxu_table.OrderBy(x => Guid.NewGuid()).ToArray();

            //}            

            IntPtr hwnd_i = FindWindow(window_class_name.ToString(), window_name.ToString());//
            IntPtr htextbox = FindWindowEx(hwnd_i, IntPtr.Zero, "EDIT", null);
            //IntPtr htextbox = FindWindowEx(hwnd_i, IntPtr.Zero, "EDIT", null);
            IntPtr htextbox2 = FindWindowEx(hwnd_i, htextbox, "EDIT", null);//填上次获得的句柄，可以得到下一个的句柄。
            //SendMessage(htextbox, WM_SETTEXT, IntPtr.Zero, "hello YZH 0x0D".ToString());
            //Thread.Sleep(1000);

            //string str = File.ReadAllText(@"c:\temp\ascii.txt");

            //SendKeys.Send(i.ToString());
            //System.Threading.Thread.Sleep(100);
            //SendKeys.Flush();
            //Thread.Sleep(1000);

            Thread.Sleep(2000);
            richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾  
            richTextBox1.ScrollToCaret();//滚动到控件光标处   
            richTextBox1.AppendText("请在2s内将光标移动到需要输入的起始地方！\r\n");

            //lst.Count()
            int cnt = 0;
            for (int i = 0; i < groups.Count; i++)
            {
                richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾  
                richTextBox1.ScrollToCaret();//滚动到控件光标处   
                richTextBox1.AppendText("\r\n"+lst[groups[i].Start].DirectoryName+"\r\n");
                //string tmp = lst[i].ToString();
                //tmp = PATH_SELECTED + tmp;
                for (int j = 0;j< groups[i].Len;j++)
                {
                    int loc = groups[i].Start + j;
                    string tmp = lst[loc].FullName;
                    if (File.Exists(tmp))
                    {
                        FileStream fs = new FileStream(tmp, FileMode.Open, FileAccess.Read);
                        StreamReader sr = new StreamReader(fs);
                        String str_read = "";
                        //String str_read = sr.ReadToEnd();
                        String eachLine = sr.ReadLine();
                        while (eachLine != null)
                        {
                            if (!eachLine.Trim().StartsWith("#"))
                            {
                                str_read += eachLine;
                                str_read += "\r\n";
                            }                         
                            eachLine = sr.ReadLine();
                        }                        
                        str_read = str_read + "\r\n";
                        Clipboard.Clear();
                        Clipboard.SetText(str_read, TextDataFormat.UnicodeText);
                        //SendMessage(htextbox, WM_SETTEXT, IntPtr.Zero, "zhangyifan2");//str_read
                        Thread.Sleep(500);
                        SendKeys.SendWait("+{INSERT}");
                        cnt = cnt + 1;
                        //SendKeys.SendWait("^v");


                        richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾  
                        richTextBox1.ScrollToCaret();//滚动到控件光标处   
                        //richTextBox1.AppendText("第 " + (i+j+1).ToString() + " 个文本完成拷贝粘贴！\r\n" + lst[loc].Name.ToString() + "\r\n");
                        richTextBox1.AppendText("第 " + cnt.ToString() + " 个文本完成拷贝粘贴！\r\n" + lst[loc].Name.ToString() + "\r\n");

                        //string file_line;
                        //while ((file_line = sr.ReadLine()) != null)
                        //{


                        //richTextBox1.Text = tmp_str + System.Environment.NewLine;
                        //SendKeys.Send("\r");
                        //SendKeys.Send("\n");
                        //System.Threading.Thread.Sleep(100);
                        //SendKeys.Flush();


                        //SendMessage(htextbox, WM_SETTEXT, IntPtr.Zero, tmp_str);
                        //file_line.ToString().
                        //SendKeys.SendWait(file_line.ToString());
                        //SendKeys.SendWait("{ENTER}");
                        //SendMessage(htextbox, WM_CHAR, IntPtr.Zero, "0x13");
                        Thread.Sleep(800);
                        //SendKeys.Flush();
                        //}
                        //sr.WriteLine(this.textBox3.Text.Trim() + "+" + this.textBox4.Text);//开始写入值
                        sr.Close();
                        fs.Close();
                    }
                }

                
                
                


            }
            Clipboard.Clear();


            // 判断文件是否存在，不存在则创建，否则读取值显示到窗体
            //if (!File.Exists("F:\\TestTxt.txt"))
            //{
            //    //FileStream fs1 = new FileStream("F:\\TestTxt.txt", FileMode.Create, FileAccess.Write);//创建写入文件 
            //    //StreamWriter sw = new StreamWriter(fs1);
            //    //sw.WriteLine(this.textBox3.Text.Trim() + "+" + this.textBox4.Text);//开始写入值
            //    //sw.Close();
            //    //fs1.Close();
            //}
            //else
            //{
            //    FileStream fs = new FileStream("F:\\TestTxt.txt", FileMode.Open, FileAccess.Read);
            //    StreamReader sr = new StreamReader(fs);
            //    string file_line;
            //    while( (file_line= sr.ReadLine() ) != null)
            //    {

            //    }        


            //    //sr.WriteLine(this.textBox3.Text.Trim() + "+" + this.textBox4.Text);//开始写入值
            //    sr.Close();
            //    fs.Close();
            //}



            //POINTAPI point = new POINTAPI();//必须用与之相兼容的结构体，类也可以  

            //for (int i = 100; i > 0; i--)
            //{
            //    GetCursorPos(ref point);//获取当前鼠标坐标 
            //    int hwnd_i = WindowFromPoint(point.X, point.Y);//获取指定坐标处窗口的句柄 
            //    StringBuilder name = new StringBuilder(256);
            //    GetWindowText(hwnd_i, name, 256);
            //    //this.richTextBox1.Text = name.ToString();
            //    MessageBox.Show(name.ToString());


            //    IntPtr hwnd = FindWindow(null, name.ToString());//新建文本文档.txt - 记事本
            //    IntPtr htextbox = FindWindowEx(hwnd, IntPtr.Zero, "EDIT", null);
            //    //IntPtr htextbox2 = FindWindowEx(hwnd, htextbox, "EDIT", null);//填上次获得的句柄，可以得到下一个的句柄。
            //    SendMessage(htextbox, WM_SETTEXT, IntPtr.Zero, "hello YZH 0x0D".ToString());
            //    Thread.Sleep(1000);

            //    string str = File.ReadAllText(@"c:\temp\ascii.txt");

            //    SendKeys.Send(i.ToString());
            //    System.Threading.Thread.Sleep(100);
            //    SendKeys.Flush();
            //    Thread.Sleep(1000);
            //}



            //POINTAPI point = new POINTAPI();//必须用与之相兼容的结构体，类也可以  
            ////add some wait time  
            //for (int i = 5; i>0;i--)
            //{
            //    Thread.Sleep(8000);
            //    GetCursorPos(ref point);//获取当前鼠标坐标  

            //    int hwnd = WindowFromPoint(point.X, point.Y);//获取指定坐标处窗口的句柄  
            //    StringBuilder name = new StringBuilder(256);
            //    GetWindowText(hwnd, name, 256);
            //    MessageBox.Show(name.ToString());
            //    GetClassName(hwnd, name, 256);
            //    MessageBox.Show(name.ToString());

            //    //SendMessage();


            //}


            //POINT pointer1 =  APIMethod.GetCursorPos();

            // SendKeys.Send(APIMethod.GetCursorPos().ToString());

            //SendKeys.Send(APIMethod.WindowFromPoint().ToString());

            //this.Text = "读取txt";
            //this.BackColor = Color.FromArgb(255, 255, 0);

            ////数字输入            
            //this.richTextBox1.Focus(); //焦点请求
            //for (int i = 0; i < 10; i++)
            //{
            //    SendKeys.Send(i.ToString());
            //    System.Threading.Thread.Sleep(100);
            //    SendKeys.Flush();
            //}

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {


            POINTAPI point = new POINTAPI();//必须用与之相兼容的结构体，类也可以  

            for (int i = 1; i > 0; i--)
            {
                Thread.Sleep(6000);
                GetCursorPos(ref point);//获取当前鼠标坐标 
                int hwnd_i = WindowFromPoint(point.X, point.Y);//获取指定坐标处窗口的句柄 
                //StringBuilder window_name = new StringBuilder(256);
                GetWindowText(hwnd_i, window_name, 256);
                GetClassName(hwnd_i, window_class_name, 256);
                //this.richTextBox1.Text = name.ToString();
                textBox2.Text = window_name.ToString();
                //MessageBox.Show(name.ToString());


                //IntPtr hwnd = FindWindow(null, name.ToString());//新建文本文档.txt - 记事本
                //IntPtr htextbox = FindWindowEx(hwnd, IntPtr.Zero, "EDIT", null);
                //IntPtr htextbox2 = FindWindowEx(hwnd, htextbox, "EDIT", null);//填上次获得的句柄，可以得到下一个的句柄。
                //SendMessage(htextbox, WM_SETTEXT, IntPtr.Zero, "hello YZH 0x0D".ToString());
                //Thread.Sleep(1000);

                //string str = File.ReadAllText(@"c:\temp\ascii.txt");

                //SendKeys.Send(i.ToString());
                //System.Threading.Thread.Sleep(100);
                //SendKeys.Flush();
                //Thread.Sleep(1000);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            { Checked_moren = true; }
            else { Checked_moren = false; }
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            { Checked_ramdom = true; }
            else { Checked_ramdom = false; }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}



//public partial class Form1 : Form
//{
//    MouseHook mh;
//    private void Form1_Load(object sender, EventArgs e)
//    {            //安装鼠标钩子
//        mh = new MouseHook();
//        mh.SetHook();
//        mh.MouseMoveEvent += mh_MouseMoveEvent;
//    }
//    void mh_MouseMoveEvent(object sender, MouseEventArgs e)
//    {   //当前鼠标位置
//        int x = e.Location.X;
//        int y = e.Location.Y;
//        lb_p.Text = string.Format("（{0}，{1}）", x, y);
//        int hwnd = Win32Api.WindowFromPoint(x, y);//获取指定坐标处窗口的句柄
//        lb_h.Text = hwnd.ToString();
//    }
//    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
//    {
//        mh.UnHook();
//    }
//}

