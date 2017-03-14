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
    public partial class Form1 : Form
    {
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        public Form1()
        {
            InitializeComponent();
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
        public void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                //检查目标目录是否以目录分割字符  
                //结束如果不是则添加之   
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                //判断目标目录是否存在如果不存在则新建之  
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);
                //得到源目录的文件列表,该里面是包含  
                //文件以及目录路径的一个数组    
                //如果你指向copy目标文件下面的文件    
                //而不包含目录请使用下面的方法    
                //string[]fileList=  Directory.GetFiles(srcPath);  
                string[] fileList =
                    Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录    
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个  
                    //目录就递归Copy该目录下面的文件    
                    if (Directory.Exists(file))
                        CopyDir(
                            file,
                            aimPath + Path.GetFileName(file));
                    //否则直接Copy文件   
                    else
                        File.Copy(
                            file,
                            aimPath + Path.GetFileName(file),
                            true);
                }
                MessageBox.Show("导入成功！","导入");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }  
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                textBox1.Text = foldPath;
               //MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确定导入吗？", "导入", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //导入操作
                CopyDir(textBox1.Text, "C:\\给同学们\\图片库");
            }
            else
            {
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           // DialogResult result = MessageBox.Show("显示图片库");
            this.Visible = false;
            Form3 newForm = new Form3();
            newForm.ShowDialog();
            this.Visible = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("加载");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            DialogResult result = MessageBox.Show("确定清空吗？", "清空", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //清空操作
                DeleteDir("C:\\给同学们\\图片库");
                MessageBox.Show("清空完毕", "清空");
            }
            else
            {
                return;
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
            DialogResult result = MessageBox.Show("基于颜色直方图的图像检索\n\n吕碧茹 10122130102\n沈斌纯 10122130202","程序信息");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Form2 newForm = new Form2();
                
            if (newForm.ShowDialog() == DialogResult.OK)
            {
                this.Visible = true;
            }
        }

    }
}
