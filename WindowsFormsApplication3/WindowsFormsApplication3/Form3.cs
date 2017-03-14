using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;

namespace WindowsFormsApplication3
{
    public partial class Form3 : Form
    {
        List<string> imageLists = new List<string>();
        private string path = Application.StartupPath;
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        int count=0;
        public Form3()
        {
             
            InitializeComponent();
            
            //imageList1.Images.Clear();
            //listView1.Items.Clear();
            //imageLists.Clear();
            ////刷新Listview
            bindListView();
            label1.Text = "当前图片库共有" + count.ToString()+ "张图片。";
            if (count < 8)
                panel1.BackColor = Color.Transparent;
            else
                panel1.BackColor = Color.WhiteSmoke;
            //this.listView1.View = View.LargeIcon;
            //this.listView1.LargeImageList = imageList1;
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
        private void bindListView()
        {
            string picPath = "C:\\给同学们\\图片库";

            DirectoryInfo dir = new DirectoryInfo(picPath);

            string[] files = new string[100];

            string ext = "";


            FlowLayoutPanel layout = new FlowLayoutPanel();
            layout.Dock = DockStyle.Fill;
            //foreach (var item in dir.GetFiles())
            //{
                
            //}
           

            foreach (FileInfo d in dir.GetFiles())
            {
                ext = System.IO.Path.GetExtension(picPath + d.Name);
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".bmp") //在此只显示Jpg
                {
                    imageLists.Add(picPath + "\\" + d.Name);
                    count++;
                    PictureBox pb = new PictureBox();
                    pb.Width = 100;
                    pb.Height = 100;
                    pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                    //pb.Image = item;
                    pb.ImageLocation = picPath + "\\" + d.Name;
                    layout.Controls.Add(pb);
                    layout.AutoScroll = true;

                }
            }
            
            //this.panel1.AutoScroll = true;
            //this.panel1.VerticalScroll.Visible = true;
            this.panel1.Controls.Add(layout);
            //this.panel2.Controls.Add(layout);

            //for (int i = 0; i < imageLists.Count; i++)
            //{
            //    imageList1.Images.Add(System.Drawing.Image.FromFile(imageLists[i].ToString()));
            //    listView1.Items.Add(System.IO.Path.GetFileName(imageLists[i].ToString()), i);
            //    listView1.Items[i].ImageIndex = i;
            //    listView1.Items[i].Name = imageLists[i].ToString();
            //}

        }
        private void button1_Click(object sender, EventArgs e)
        {

            imageList1.Images.Clear();
            this.Close();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("显示当前图片库。");
        }


        public PictureBoxSizeMode StretchImage { get; set; }

        private void button2_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("基于颜色直方图的图像检索\n\n吕碧茹 10122130102\n沈斌纯 10122130202", "程序信息");
        }
    }
}
