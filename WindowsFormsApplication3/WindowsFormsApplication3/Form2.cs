using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace WindowsFormsApplication3
{
    public partial class Form2 : Form
    {
        List<string> imageLists = new List<string>();
        private string path = Application.StartupPath;
  
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        public Form2()
        {
            InitializeComponent();
            DeleteDir("C:\\给同学们\\结果");
            listView1.BackColor = Color.WhiteSmoke;
            bindListView();
            this.listView1.View = View.LargeIcon;
            this.listView1.LargeImageList = imageList1;
           
        }
        public static void DeleteDir(string aimPath)
        {
            try
            {
                //检查目标目录是否以目录分割字符结束如果不是则添加之  
                if (aimPath[aimPath.Length - 1] !=
                    Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                //得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组  
                //如果你指向Delete目标文件下面的文件而不包含目录请使用下面的方法  
                //string[]fileList=  Directory.GetFiles(aimPath);  
                string[] fileList = Directory.GetFileSystemEntries(aimPath);
                //遍历所有的文件和目录   
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个  
                    //目录就递归Delete该目录下面的文件   
                    if (Directory.Exists(file))
                    {
                        DeleteDir(aimPath + Path.GetFileName(file));
                    }
                    //否则直接Delete文件   
                    else
                    {
                       // File.Exists(aimPath + Path.GetFileName(file));
                        File.Delete(aimPath + Path.GetFileName(file));
                    }
                }
                //删除文件夹   
                //System.IO.Directory.Delete(aimPath, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void bindListView()
        {
            DirectoryInfo dir = new DirectoryInfo("C:\\给同学们\\结果");

            string[] files = new string[100];

            string ext = "";

            foreach (FileInfo d in dir.GetFiles())
            {
                ext = System.IO.Path.GetExtension(textBox2.Text.Trim() + d.Name);
                if (ext == ".bmp" || ext == ".jpeg") //在此只显示Jpg
                {
                    
                    
                    imageLists.Add("C:\\给同学们\\结果"+ "\\" + d.Name);
                    FileStream fileStream = new FileStream("C:\\给同学们\\结果" + "\\" + d.Name, FileMode.Open, FileAccess.Read);
                    int byteLength = (int)fileStream.Length;
                    byte[] fileBytes = new byte[byteLength];
                    fileStream.Read(fileBytes, 0, byteLength);
                    //文件流关闭,文件解除锁定
                    fileStream.Close();
                    imageList1.Images.Add(Image.FromStream(new MemoryStream(fileBytes)));
                }
            }
            for (int i = 0; i < imageLists.Count; i++)
            {

             
               
                //imageList1.Images.Add(System.Drawing.Image.FromFile(imageLists[i].ToString()));
                listView1.Items.Add(System.IO.Path.GetFileName(imageLists[i].ToString()), i);
                listView1.Items[i].ImageIndex = i;
                listView1.Items[i].Name = imageLists[i].ToString();
            }

        }
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
        // 窗体上鼠标按下时
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left & this.WindowState == FormWindowState.Normal)
            {
                // 移动窗体
                this.Capture = false;
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }       
      
        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("基于颜色直方图的图像检索\n\n吕碧茹 10122130102\n沈斌纯 10122130202", "程序信息");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            //file.ShowDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {

                this.textBox1.Text = file.FileName;
                string name=Path.GetFileNameWithoutExtension(file.FileName+".bmp");

                using (StreamWriter sw = new StreamWriter("C:\\Users\\RubyLyu\\Documents\\Visual Studio 2010\\图片检索\\图片检索" + "\\1.txt", false))
                {
                    sw.WriteLine(name);
                    sw.Close();//写入
                }
                
                this.pictureBox1.Image = Image.FromFile(file.FileName, false);
                DeleteDir("C:\\给同学们\\结果");
                listView1.BackColor = Color.WhiteSmoke;
                imageList1.Images.Clear();
                listView1.Items.Clear();
                imageLists.Clear();
                bindListView();
                this.listView1.View = View.LargeIcon;
                this.listView1.LargeImageList = imageList1;
                
               // System.Diagnostics.Process.Start(@"C:\\Users\\RubyLyu\\Documents\\Visual Studio 2010\\图片检索\\Debug\\图片检索.exe", "10\n");

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {

            DeleteDir("C:\\给同学们\\结果");
            listView1.BackColor = Color.WhiteSmoke;
            imageList1.Images.Clear();
            listView1.Items.Clear();
            imageLists.Clear();
            bindListView();
            this.listView1.View = View.LargeIcon;
            this.listView1.LargeImageList = imageList1;





            System.Diagnostics.Process ie = new System.Diagnostics.Process();
            ie.StartInfo.FileName = @"C:\\Users\\RubyLyu\\Documents\\Visual Studio 2010\\图片检索\\Debug\\图片检索.exe";
            ie.Start();
            ie.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            ie.WaitForExit();
            // process1.StartInfo.FileName = "调用程序的路径";
            ////该程序使用最小化运行
            //process1.StartInfo.WindowStyle=ProcessWindowStyle.Minimized;
            ////开始进程
            //process1.Start();
            ////设置等待关联进程退出的时间，并在该段时间结束前或该进程退出前，
            ////阻止当前线程执行。
            ////参数可指定等待时间 无参数就是无限等待
            //process1.WaitForExit();
            

            //System.Threading.Thread.Sleep(10000);

            MessageBox.Show("检索完成！","search");
            listView1.BackColor = Color.White;
            imageList1.Images.Clear();
            listView1.Items.Clear();
            imageLists.Clear();
            bindListView();
            this.listView1.View = View.LargeIcon;
            this.listView1.LargeImageList = imageList1;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.DialogResult = DialogResult.OK;
            
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string num = textBox2.Text.ToString();
            using (StreamWriter sw2 = new StreamWriter("C:\\Users\\RubyLyu\\Documents\\Visual Studio 2010\\图片检索\\图片检索" + "\\2.txt", false))
            {
                sw2.WriteLine(num);
                sw2.Close();//写入
            }
        }
    }
}
