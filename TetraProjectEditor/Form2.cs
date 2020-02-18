using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//本程序由 Tamill (1298878474@qq.com) 制作
//代码未经允许请勿擅自修改或使用
//如有问题或建议 欢迎联系QQ: 1298878474


namespace TetraProjectEditor
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            AutoScale(this);
        }
        public List<string> descs = new List<string>();
        public List<string> paths = new List<string>();
        public List<string> names = new List<string>();
        public List<string> ids = new List<string>();
        public int indexOfLastSelection;

        private void Form2_Load(object sender, EventArgs e)
        {
            //this.ForeColor = EdLib.CFore; this.BackColor = EdLib.CBack;
            //listView1.ForeColor = EdLib.CFore; listView1.BackColor = EdLib.CBack;
            //richTextBox1.ForeColor = EdLib.CFore; richTextBox1.BackColor = EdLib.CBack;
            //button1.ForeColor = EdLib.CFore; button1.BackColor = EdLib.CBack;
            //button2.ForeColor = EdLib.CFore; button2.BackColor = EdLib.CBack;
            //button3.ForeColor = EdLib.CFore; button3.BackColor = EdLib.CBack;
            //button4.ForeColor = EdLib.CFore; button4.BackColor = EdLib.CBack;
            //button5.ForeColor = EdLib.CFore; button5.BackColor = EdLib.CBack;
            this.Text = Form1.appChineseName;
            this.Icon = Form1.main.Icon;
            EdLib.SetBrowser(listView1, listView1.Width);
            reloadList();
            listView1.Items.Add("☀官方资源包");
            descs.Add("该资源包为官方原版内容资源包，包含所有该游戏的图标、模型、音乐、卡牌设计等数据，适合新手学习使用。");
            names.Add("Builtin");
            ids.Add("");
            var pos = EdLib.HasFile(Form1.GetPath("{game}"), true) ? Form1.GetPath("{game}") : Form1.GetPath("{localgame}");
            paths.Add(pos + "\\TetraProject_Data\\StreamingAssets\\Packages\\Builtin");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //EdLib.AskMsg(pos + "\\TetraProject_Data\\StreamingAssets\\Packages\\Builtin");
        }


        private void LoadFiles(string path)
        {
            bool IsCurrentPackageSteamWorkshop = false;
            path = Form1.GetPath(path);
            if (!EdLib.CanBeEdited(path))
            {
                IsCurrentPackageSteamWorkshop = true;
            }

            string[] folders = System.IO.Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                string oneInfo = System.IO.Path.GetFileNameWithoutExtension(folder);
                string mainInfo = oneInfo;
                string descInfo = "";
                string IdInfo = "";
                if (mainInfo == "5e2bb7ebdd3c13006a48498d")
                    continue;
                if (EdLib.HasFile(path + "\\" + mainInfo + "\\PackageInfo.json", false))
                {
                    using (System.IO.StreamReader file = System.IO.File.OpenText(path + "\\" + mainInfo + "\\PackageInfo.json"))
                    {
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            Newtonsoft.Json.Linq.JObject o = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.Linq.JToken.ReadFrom(reader);


                            var a = o["displayName"];
                            var b =o["description"];
                            var c = o["id"];
                            if (a != null)
                                oneInfo = (IsCurrentPackageSteamWorkshop ? "☁在线资源包 " : "◉自制资源包 ") + a.ToString();
                            if (b != null)
                                descInfo = "" + b.ToString();
                            if (c != null)
                                IdInfo = o["id"].ToString();

                        }
                    }
                }
                listView1.Items.Add(oneInfo);
                names.Add(oneInfo);
                descs.Add(descInfo);
                ids.Add(IdInfo);
                paths.Add(path + "\\" + mainInfo);
            }
        }

        private void reloadList()
        {
            listView1.Clear();
            descs.Clear();
            paths.Clear();
            EdLib.SetBrowser(listView1, 160);
            if (EdLib.HasFile(Form1.path_UserPackage, true))
            {
                
                LoadFiles("{packages}");
                LoadFiles("{localpackages}");

            }
        }

        private void ListView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            var text = EdLib.TryGetListViewSelection(listView1);
            if (text != null)
            {
                indexOfLastSelection = text.Index;
                richTextBox1.Text = descs[indexOfLastSelection];
                workshopId = ids[indexOfLastSelection];
                
                
                if (EdLib.HasFile(paths[indexOfLastSelection] + "\\icon.png"))
                {
                    Image image = System.Drawing.Image.FromFile(paths[indexOfLastSelection] + "\\icon.png");
                    pictureBox1.Image = image;
                }
                else if(EdLib.HasFile(EdLib.path_AppData + "\\icon.png"))
                {
                    Image image = System.Drawing.Image.FromFile(EdLib.path_AppData + "\\icon.png");
                    pictureBox1.Image = image;
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1.returnTaskIDForOtherWindow = 1024;
            this.Close();
        }

        public string workshopId = "";
        private void button1_Click(object sender, EventArgs e)
        {
             //System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=" + workshopId);
           
            
        }


        public void AutoScale(Form frm)
        {
            frm.Tag = frm.Width.ToString() + "," + frm.Height.ToString();
            frm.SizeChanged += new EventHandler(frmScreen_SizeChanged);
        }

        private void frmScreen_SizeChanged(object sender, EventArgs e)
        {
            string[] tmp = ((Form)sender).Tag.ToString().Split(',');
            float width = (float)((Form)sender).Width / (float)Convert.ToInt16(tmp[0]);
            float heigth = (float)((Form)sender).Height / (float)Convert.ToInt16(tmp[1]);

            ((Form)sender).Tag = ((Form)sender).Width.ToString() + "," + ((Form)sender).Height;

            foreach (Control control in ((Form)sender).Controls)
            {
                control.Scale(new SizeF(width, heigth));

            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            
            Form1.main.CreateNewPackage();
            indexOfLastSelection = -1;
            this.Close();

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if(indexOfLastSelection >= 0 && indexOfLastSelection < listView1.Items.Count)
            {
                if (!EdLib.CanBeEdited(paths[indexOfLastSelection]))
                {
                    EdLib.AskMsg(Form1.loadOnlinePackageWarning);
                }
                else
                {
                    if (EdLib.HasFile(paths[indexOfLastSelection] + "\\PackageInfo.json"))
                    {
                        string json = System.IO.File.ReadAllText(paths[indexOfLastSelection] + "\\PackageInfo.json");
                        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                        jsonObj["description"] = richTextBox1.Text;
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                        System.IO.File.WriteAllText(paths[indexOfLastSelection] + "\\PackageInfo.json", output);
                        descs[indexOfLastSelection] = richTextBox1.Text;

                    }
                }


            }


        }

        private void Button5_Click(object sender, EventArgs e)
        {

            if (indexOfLastSelection >= 0 && indexOfLastSelection < listView1.Items.Count)
            {
                if (!EdLib.CanBeEdited(paths[indexOfLastSelection]))
                {
                    EdLib.AskMsg(Form1.loadOnlinePackageWarning);
                }
                else
                {
                    if (EdLib.HasFile(paths[indexOfLastSelection], true))
                    {
                        if (EdLib.AskMsg("确认要移除该资源包吗? 该操作将无法挽回，确定吗?"))
                        {
                            try {
                                System.IO.Directory.Delete(paths[indexOfLastSelection]);
                                indexOfLastSelection = -2;
                                //FileSystem.DeleteDirectory(paths[indexOfLastSelection], UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                                reloadList();
                            }
                            catch(Exception E)
                            {
                                EdLib.AskMsg("由于文件夹被占用或无法正常处理(被防病毒软件阻止)，因此删除操作已取消。");
                            }
                            
                        }
                    }
                }
            }


        }
    }
}
