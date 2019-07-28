using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
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
    class EdLib
    {
        static public Color CFore = Color.White;
        static public Color CBack = Color.FromArgb(255, 30, 30, 30);
        static public string appName = "Tetra Project 数据";
        static public string path_AppData = Application.StartupPath + "\\TetraProjectEditorResources";
        static public FolderBrowserDialog dialog;
        static public OpenFileDialog dialogfile;
        static public string path_FileSelection;
        static public System.Xml.XmlNode settingsRoot;
        static public System.Xml.XmlDocument MainSettings;


        public static bool AskMsg(object content)
        {
            return MessageBox.Show(content.ToString(), appName, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK;
        }

        public static bool HasFile(string _path, bool type = false)
        {
            return type ? System.IO.Directory.Exists(_path) : System.IO.File.Exists(_path);
        }


        static public bool XmlOpen(string name, string root)
        {
            MainSettings = new System.Xml.XmlDocument();
            if (HasFile(path_AppData + "\\" + name + ".xml", false))
            {
                MainSettings.Load(path_AppData + "\\" + name + ".xml");
                settingsRoot = MainSettings.SelectSingleNode(root);
                return true;
            }
            return false;
        }

        static public void XmlSave(string name)
        {
            MainSettings.Save(path_AppData + "\\" + name + ".xml");
        }

        static public string Xml(string name)
        {
            return settingsRoot.SelectSingleNode(name).InnerText;
        }

        static public void SetXml(string name, string value)
        {
            if(!isText(value))
            {
                //AskMsg("XML错误: 无效值 " + name + " -> " + value);
                return;
            }

            settingsRoot.SelectSingleNode(name).InnerText = value;
        }


        public static Bitmap GetImageFromAseCel(Aseprite.ASEFile ase)
        {
            Aseprite.Cel cel = ase.frames[0].cels.FirstOrDefault().Value;
            if(cel == null)
            {
                foreach (var item in ase.frames)
                {
                    cel = ase.frames[0].cels.FirstOrDefault().Value;
                    if (cel != null)
                    {
                        break;
                    }
                }
            }
            int wide = ase.width;
            int height = ase.height;
            Bitmap image = new Bitmap(wide, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < wide; x++)
                {
                    var a = cel.GetPixel(x, y);
                    image.SetPixel(x, y, a);
                }
            }
            return image;
        }


        public static bool isText(string text)
        {
            if (text == null) return false;
            if (string.IsNullOrEmpty(text)) return false;
            return text.Trim() != "";
        }

        static public string InputBox(string title = "请输入内容", string text = "")
        {
            return Interaction.InputBox(title, "字符串", text, 100, 100);
        }


        public static ListViewItem TryGetListViewSelection(ListView list)
        {
            if (list.SelectedItems.Count > 0)
                return list.SelectedItems[0];
            return null;
        }


        static public void ShowFolderSelection(string text, string path ="")
        {
            dialog = new FolderBrowserDialog();
            dialog.SelectedPath = path;
            dialog.Description = text;
            path_FileSelection = "";
        }
        static public bool GetFolderSelection(Form yourForm)
        {
            if (yourForm.DialogResult == DialogResult.OK)
            {
                path_FileSelection = dialog.SelectedPath;
                dialog.Dispose();
                return true;
            }
            dialog.Dispose();
            return false;
        }

        public static void SetBrowser(ListView currentList, int width)
        {
            currentList.View = View.Details;
            currentList.GridLines = false;
            currentList.LabelEdit = false;
            currentList.Scrollable = true;
            currentList.HeaderStyle = ColumnHeaderStyle.Clickable;
            currentList.FullRowSelect = true;
            currentList.MultiSelect = true;
            currentList.Columns.Add("", currentList.Width, HorizontalAlignment.Left);
            currentList.HeaderStyle = ColumnHeaderStyle.None;
            currentList.Scrollable = true;
            currentList.Columns[0].Width = width;
            currentList.Width = width;

        }

        static public void ShowFileSelection(string text)
        {
            dialogfile = new OpenFileDialog();
            dialogfile.Filter = "可执行文件|*.exe";
            dialogfile.FileName = "";
            dialogfile.Title = text;
            path_FileSelection = "";
        }
        static public bool GetFileSelection()
        {
            if (dialogfile.ShowDialog() == DialogResult.OK)
            {
                path_FileSelection = dialogfile.FileName;
                dialogfile.Dispose();
                return true;
            }
            dialogfile.Dispose();
            return false;
        }


    }
}
