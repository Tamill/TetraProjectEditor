using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using KeyboardHooker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Xml;
using unvell.ReoGrid;
using unvell.ReoGrid.CellTypes;
//本程序由 Tamill (1298878474@qq.com) 制作
//代码未经允许请勿擅自修改或使用
//如有问题或建议 欢迎联系QQ: 1298878474


namespace TetraProjectEditor
{
    public partial class Form1 : Form
    {

        public TextEditor mainEditor;
       ElementHost editorHost;
        static public string path_GameApp;
        static public string path_UserPackage;
        static public string path_CurrentPackage;
        public string currentPackageName;
        public string currentPackageDesc;
        static public string appChineseName = "原石计划卡牌数据浏览器";
        static public string loadOnlinePackageWarning = "当前资源包属于STEAM创意工坊资源包或官方资源包，不可修改。";
        public ReoGridControl ReoMain;
        Cell Editing = null;
        public static Form1 main;

        public Form1()
        {
            main = this;
            InitializeComponent();
            AutoScale(this);
            editorHost = elementHost1;
           // editorHost.Height = editorPanel.Size.Height;
           // editorHost.Width = editorPanel.Size.Width;
            mainEditor = new TextEditor();
            editorHost.Child = mainEditor;
            //editorPanel.Controls.Add(editorHost);
        }

        public bool IsCSVLoader = false;
        private void Form1_Shown(object sender, EventArgs e)
        {
            LoadUserInfo();
            this.Text = appChineseName + "    " + currentPackageName;
            
            if (!EdLib.HasFile(path_GameApp, true))
            {
                StartFirstUsingSettings();
            }
            reoGridControl2.SheetTabWidth = 600;
            IsCSVLoader = true;
            loadAll();
            
            //pictureBox3.Controls.Add(label5);
            //label5.Location = new Point(120, 270);
            label5.BackColor = System.Drawing.Color.Transparent;
            label5.FlatStyle = FlatStyle.Flat;
            label5.Font = new Font("黑体", 10, FontStyle.Regular);
            //pictureBox3.Controls.Add(pictureBox1);

            //pictureBox1.Location = new Point(120, 150);
            pictureBox1.BackColor = System.Drawing.Color.Transparent;
            //pictureBox2.Controls.Add(pictureBox3);
            //pictureBox3.Location = new Point(0, 0);
            //pictureBox3.BackColor = System.Drawing.Color.Transparent;
            //pictureBox1.Hide();
            pictureBox1.Controls.Add(label4);
            //label4.Location = new Point(10, 100);
            label4.BackColor = System.Drawing.Color.Transparent;
            label4.FlatStyle = FlatStyle.Flat;
            label4.TextAlign = ContentAlignment.TopCenter;
            label4.Font = new Font("黑体", 12, FontStyle.Regular);
            
            //pictureBox3.Controls.Add(label6);
            label6.Text = "1";
            //label6.Location = new Point(97, 150);
            label6.BackColor = System.Drawing.Color.Transparent;
            label6.FlatStyle = FlatStyle.Flat;
            label6.Font = new Font("黑体", 18, FontStyle.Regular);
            label6.Height = 20;
            //pictureBox3.Controls.Add(label7);
            label7.Text = "1";
            //label7.Location = new Point(100, 180);
            label7.BackColor = System.Drawing.Color.Transparent;
            label7.FlatStyle = FlatStyle.Flat;
            label7.Font = new Font("黑体", 12, FontStyle.Regular);
            label7.ForeColor = Color.DarkGray;
            //pictureBox3.Controls.Add(label8);
            label8.Text = "1";
            //label8.Location = new Point(100, 200);
            label8.BackColor = System.Drawing.Color.Transparent;
            label8.FlatStyle = FlatStyle.Flat;
            label8.Font = new Font("黑体", 12, FontStyle.Regular);
            label8.ForeColor = Color.DarkRed;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;


        }

        string lastPicture = "";
        string lastPictureBG = "";
        string lastPictureBG2 = "";
        string lastASE = "";
        string lastMainEdtiorText = "";
        private void Item_SelectionRangeChanged(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        {
            

            if (e == null) return;
            var cellLine = e.Range.StartPos.Row;

            if(Editing != null)
            {
                string local;
                if (Editing.Data == null)
                    local = "";
                else
                    local = Editing.Data.ToString();

                if (local != lastMainEdtiorText)
                 mainEditor.Text = local;
            }

            if (Editing != null  && !IsCurrentPackageSteamWorkshop)
            {
                reoGridControl2.CurrentWorksheet[Editing.Position] = GetClear(mainEditor.Text);
                reoGridControl2.CurrentWorksheet.Cells[Editing.Position].Style.TextWrap = TextWrapMode.WordBreak;
                reoGridControl2.CurrentWorksheet.AutoFitRowHeight(Editing.Position.Row, false);
                reoGridControl2.CurrentWorksheet.SetRowsHeight(Editing.Position.Row, 1, 20);
                mainEditor.Text = "";
                Editing = null;
            }
            var nameSheet = reoGridControl2.CurrentWorksheet.Name;

            for (int i = 0; i < 25; i++)
            {
                if (reoGridControl2.CurrentWorksheet[0, i] == null) continue;
                var name = reoGridControl2.CurrentWorksheet[0, i].ToString().ToLower();
                var current = reoGridControl2.CurrentWorksheet[cellLine, i];

                if (cellLine == 0)
                    return;

                if (current == null)
                {
                    if ((nameSheet == "Card" && name == "code") ||( nameSheet == "Character" && name == "fieldcode"))
                    {
                        Editing = reoGridControl2.CurrentWorksheet.Cells[cellLine, i];
                    }
                   
                    if (nameSheet == "Card")
                    {
                        switch (name)
                        {
                            case "id":
                                if (EdLib.HasFile(EdLib.path_AppData + "\\icon.png") && lastPicture != EdLib.path_AppData + "\\icon.png")
                                {
                                    Image image = System.Drawing.Image.FromFile(EdLib.path_AppData + "\\icon.png");
                                    pictureBox1.Image = image;
                                    lastPicture = EdLib.path_AppData + "\\icon.png";
                                }
                                break;
                            case "displayname":
                                label4.Text = "无名称";
                                break;
                            case "description":
                                label5.Text = "无描述";
                                break;
                            case "energyreq":
                                label6.Text = "0";
                                break;
                            case "range":
                                label7.Text = "0";
                                break;
                            case "code":
                                mainEditor.Text = "";
                                break;
                            case "spreadradius":
                                label8.Text = "0";
                                break;
                            case "backgroundid":
                                var pos2 = Form1.GetPath("{game}\\TetraProject_Data\\StreamingAssets\\Packages\\Builtin\\CardBackground\\5\\Card.png");
                                if (EdLib.HasFile(pos2, false) && lastPictureBG != pos2)
                                {
                                    lastPictureBG = pos2;
                                    Image image = System.Drawing.Image.FromFile(pos2);
                                    pictureBox3.Image = image;

                                }
                                break;
                        }
                    }
                    if (nameSheet == "Character")
                    {
                        switch (name)
                        {
                            case "fieldcode":
                                mainEditor.Text = "";
                                break;
                            case "id":
                                if (EdLib.HasFile(EdLib.path_AppData + "\\icon.png") && lastASE != EdLib.path_AppData + "\\icon.png")
                                {
                                    Image image = System.Drawing.Image.FromFile(EdLib.path_AppData + "\\icon.png");
                                    pictureBox4.Image = image;
                                    lastASE = EdLib.path_AppData + "\\icon.png";
                                }
                                break;
                        }
                    }

                    continue;

                }
                if (nameSheet == "Card")
                {
                    switch (name)
                    {
                        case "id":
                            var pos = Form1.GetPath(Form1.path_CurrentPackage) + "\\CardArt\\" + current.ToString().Trim() + ".png";
                            if (EdLib.HasFile(pos, false) && lastPicture != pos)
                            {
                                lastPicture = pos;
                                Image image = System.Drawing.Image.FromFile(pos);
                                pictureBox1.Image = image;
                            }
                            else if (EdLib.HasFile(EdLib.path_AppData + "\\icon.png") && lastPicture != EdLib.path_AppData + "\\icon.png")
                            {
                                Image image = System.Drawing.Image.FromFile(EdLib.path_AppData + "\\icon.png");
                                pictureBox1.Image = image;
                                lastPicture = EdLib.path_AppData + "\\icon.png";

                            }
                            break;
                        case "displayname":
                            label4.Text = current.ToString().Trim();
                            break;
                        case "description":
                            label5.Text = current.ToString().Trim();
                            break;
                        case "energyreq":
                            label6.Text = current.ToString().Trim();
                            break;
                        case "range":
                            label7.Text = current.ToString().Trim();
                            break;
                        case "code":

                            Editing = reoGridControl2.CurrentWorksheet.Cells[cellLine, i];
                            OptimizeVision(current.ToString());
                            lastMainEdtiorText = Editing.Data.ToString();
                            break;
                        case "spreadradius":
                            label8.Text = current.ToString().Trim();
                            break;
                        case "backgroundid":
                            var pos2 = Form1.GetPath("{game}\\TetraProject_Data\\StreamingAssets\\Packages\\Builtin\\CardBackground\\" + current.ToString().Trim() + "\\Card.png");
                            if (EdLib.HasFile(pos2, false) && lastPictureBG != pos2)
                            {
                                lastPictureBG = pos2;
                                Image image = System.Drawing.Image.FromFile(pos2);
                                //pictureBox2.Image = ZoomImage(new Bitmap(image), -25,-25);
                                pictureBox3.Image = image;
                               // pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
                                //pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;

                            }
                            var pos3 = Form1.GetPath("{game}\\TetraProject_Data\\StreamingAssets\\Packages\\Builtin\\CardBackground\\" + current.ToString().Trim() + "\\CardGlow.png");
                            if (EdLib.HasFile(pos3, false) && lastPictureBG2 != pos3)
                            {
                                lastPictureBG2 = pos3;
                                Image image = System.Drawing.Image.FromFile(pos3);
                                //pictureBox2.Image = ZoomImage(new Bitmap(image), -25,-25);
                                //pictureBox2.Image = image;
                                //pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
                                //pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

                            }
                            break;
                    }
                    
                }
                if (nameSheet == "Character")
                {

                    switch (name)
                    {

                        case "id":
                            var pos = Form1.GetPath(Form1.path_CurrentPackage) + "\\CharacterModel\\" + current.ToString().Trim() + ".ase";
                            
                            if (EdLib.HasFile(pos, false) && lastASE != pos)
                            {
                                lastASE = pos;
                                var bytes = System.IO.File.ReadAllBytes(pos);
                                var ase = Aseprite.ASEParser.Parse(bytes);
                                if (ase == null) break;
                                var image = EdLib.GetImageFromAseCel(ase);
                                if (image != null)
                                {
                                    pictureBox4.Image = image;
                                }
                                //pictureBox4.BackgroundImageLayout = ImageLayout.Stretch;
                                //pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;

                            }
                            break;
                        case "fieldcode":

                            Editing = reoGridControl2.CurrentWorksheet.Cells[cellLine, i];

                            OptimizeVision(current.ToString());
                            lastMainEdtiorText = Editing.Data.ToString();

                            break;
                        default:
                            break;
                    }
                }
            }

            //Text = e.Cell.Data.ToString();
        }




        public void LoadCharacterSheet()
        {

            var sheet = reoGridControl2.CurrentWorksheet;

        }

        public void LoadCardSheet()
        {
            var sheet = reoGridControl2.CurrentWorksheet;
            sheet.SetColumnsWidth(0, 1, 100);
         
            for (int i = 0; i < sheet.ColumnCount; i++)
            {
   
                if (sheet[0, i] == null) continue;
                
                var name = sheet[0, i].ToString().ToLower();
               
                if (name == "displayname")
                {
                    sheet.AutoFitColumnWidth(i, true);
                }
               
                if (name == "id")
                {
                    for (int d = 1; d < sheet.RowCount; d++)
                    {
                        if (sheet[d, i] == null) continue;
                        sheet.Cells[d, i].Style.HAlign = ReoGridHorAlign.Left;
                        sheet.AutoFitRowHeight(d, false);
                        sheet.SetRowsHeight(d, 1, 20);

                    }
                }
              
                if (name == "code" || name == "description")
                {
                    sheet.SetColumnsWidth(i, sheet.RowCount, 250);

                    for (int d = 1; d < sheet.RowCount; d++)
                    {
                        if (sheet[d, i] == null) continue;
                        sheet.Cells[d, i].Style.VAlign = ReoGridVerAlign.Top;
                        sheet.Cells[d, i].Style.TextWrap = TextWrapMode.WordBreak;
                        sheet.AutoFitRowHeight(d, false);
                        sheet.SetRowsHeight(d, 1, 20);

                    }
                    //sheet.AutoFitColumnWidth(i, true);
                }

            }
            return;
            for (int i = 0; i < sheet.RowCount; i++)
            {
                object save = sheet[i, 0];
                if (save != null)
                {

                    var pos = Form1.GetPath(Form1.path_CurrentPackage) + "\\CardArt\\" + save.ToString().Trim() + ".png";
                    if (EdLib.HasFile(pos, false))
                    {
                        Image image = Image.FromFile(pos);
                        var imgCell = new ImageCell(image, ImageCellViewMode.Zoom);
                        imgCell.ViewMode = ImageCellViewMode.Zoom;
                        sheet[i, 0] = imgCell;

                        //sheet.SetRowsHeight(i, 1, 50);

                    }
                }
            }

        }



        public bool IsCurrentPackageSteamWorkshop = false;
        public void loadAll()
        {
            var pos2 = GetPath("{game}\\TetraProject_Data\\StreamingAssets\\Packages\\Builtin\\Database\\CardCommand.csv");

            if (EdLib.HasFile(pos2))
            {
                
                if (AutoCompleteData == null)
                {
                    AutoCompleteData = new List<ICompletionData>();
                    var obj = new ReoGridControl();
                    obj.Load(pos2, unvell.ReoGrid.IO.FileFormat.CSV);
                    for (int i = 1; i < obj.CurrentWorksheet.RowCount; i++)
                    {
                        if (obj.CurrentWorksheet[i, 0] != null)
                        {
                            if (obj.CurrentWorksheet[i, 1] != null)
                            {
                                AutoCompleteData.Add(new MyCompletionData(obj.CurrentWorksheet[i, 0].ToString(), obj.CurrentWorksheet[i, 1].ToString()));
                            }
                        }
                    }
                    obj.Dispose();

                }



            }


            if (EdLib.HasFile(path_CurrentPackage, true))
            {
                //sheet.SetColumnsWidth(0, 1, 50);
                IsCurrentPackageSteamWorkshop = false;
                var pos = GetPath(path_CurrentPackage) + "\\Database\\";
                if(path_CurrentPackage.Contains("steamapps"))
                {
                    IsCurrentPackageSteamWorkshop = true;
                }
                //return;
                try
                {
                    if(IsCSVLoader)
                    {
                        LoadCSV(pos);
                    }
                    else
                    {
                        if(EdLib.HasFile(pos + "database.xlsx"))
                        {
                            ReoMain.Load(pos + "database.xlsx", unvell.ReoGrid.IO.FileFormat.Excel2007);
                        }
                        else
                        {
                            ReoMain.Load(pos + "database.xls", unvell.ReoGrid.IO.FileFormat.Excel2007);
                        }
                       
                    }
                    
                }
                catch (Exception e)
                {
                    if(e.Message.Contains("ZipFile"))
                    {
                        //直接读取--- csv
                        LoadCSV(pos);
                    }
                    else
                    {
                        EdLib.AskMsg("读取资料库时发生严重异常，错误信息：\n" + e.ToString() + "\n" + e.Message);

                        return;
                    }

                }
              
                ReoMain.Readonly = IsCurrentPackageSteamWorkshop;

                foreach (var item in reoGridControl2.Worksheets)
                {
                    

                    item.NameTextColor = EdLib.CFore;
                    item.NameBackColor = EdLib.CBack;
                    item.SelectionRangeChanged += Item_SelectionRangeChanged;
                    if (item.Name == "Card")
                        ReoMain.CurrentWorksheet = item;
                }
                
                LoadSheet();
                return;
                
            }

        }


        void LoadCSV(string pos)
        {

            ReoMain.Worksheets.Clear();
            string[] folders = System.IO.Directory.GetFiles(pos);
            for (int i = 0; i < folders.Length; i++)
            {
                string folder = folders[i];
                string oneInfo = System.IO.Path.GetFileName(folder);
                string ex = System.IO.Path.GetExtension(folder);
                string name = System.IO.Path.GetFileNameWithoutExtension(folder);

                if (ex == ".csv")
                {
                    var worksheet = ReoMain.CreateWorksheet(name);
                    ReoMain.Worksheets.Add(worksheet);
                    try
                    {
                        LoadCsv(File.OpenRead(pos + oneInfo), worksheet);
                    }
                    catch (Exception d)
                    {
                        EdLib.AskMsg("读取资料库时发生严重异常，错误信息：\n" + d.ToString() + "\n" + d.Message);
                    }
                    //EdLib.AskMsg("Worksheets.Count = " + ReoMain.Worksheets.Count);


                }
            }

        }


        void LoadCsv(Stream stream, Worksheet sheet)
        {
            unvell.ReoGrid.IO.CSVFormat.Read(stream, sheet, RangePosition.EntireRange, Encoding.Default, 256, true);
        }
        public void LoadSheet()
        {
            pictureBox1.Hide();
            //pictureBox2.Hide();
            pictureBox3.Hide();
            pictureBox4.Hide();
            label4.Hide();
            label5.Hide();
            label6.Hide();
            label7.Hide();
            label8.Hide();
            Editing = null;
            mainEditor.Text = "";
            switch (reoGridControl2.CurrentWorksheet.Name)
            {
                case "Card":
                    pictureBox1.Show();
                    //pictureBox2.Show();
                    pictureBox3.Show();
                    label4.Show();
                    label5.Show();
                    label6.Show();
                    label7.Show();
                    label8.Show();
                    LoadCardSheet();
                    break;
                case "Character":
                    pictureBox4.Show();
                    LoadCharacterSheet();
                    break;
                default:

                    break;
            }

        }



        int lastSelIndex = 0;
        public string GetCodeCorrectly(string code)
        {
            //code = @"evt.UseCard:{IncreaseMaxHp:1};evt.DecreaseHp:{SerialRun:{CreaserIncreaseNext:ApplyDamage,$user.maxHp*10%}};Equipment:";
            StringBuilder builder = new StringBuilder();
            bool includingStr = false;
            char lastWord = ' ';
            string lastLine = "";
            int __index = 0;
            char nextWord = ' ';
            int _index = 0;
            bool lastChangedNextLine = false;
            int IfIndex = 0;
            foreach (char word in code)
            {
                if (word == '\n')
                    continue;
                if (word == ' ')
                    continue;
                builder.Append(word);

            }



            code = builder.ToString();
            builder.Clear();
            foreach (char word in code)
            {
                __index++;
                if (__index > 0 && __index < code.Length)
                    nextWord = code[__index];

                if (word == ' ' || word == '	')
                {
                    continue;
                }

                if ((lastWord == 'I' || lastWord == 'i') && word == 'f')
                {
                    if (__index > 0 && __index + 1 < code.Length)
                    {
                        if(code[__index + 1] == '{')
                            IfIndex++;
                    }


                   
                }


                if (word == '}')
                {
                    _index--;
                    if (_index < 0)
                        _index = 0;


                    if (IfIndex > 0)
                    {
                        IfIndex--;
                        if (IfIndex == 1)
                            IfIndex = 0;
                    }

                }
                if (lastChangedNextLine && IfIndex == 0)
                {
                    for (int i = 0; i < _index; i++)
                    {
                        builder.Append("	");
                        
                    }
                    lastSelIndex += 4;
                }
                lastChangedNextLine = false;




                if (word == '\n')
                    continue;

                if (nextWord == '}')
                    lastChangedNextLine = true;


                builder.Append(word);
                if (word == ';')
                    lastChangedNextLine = true;

                if (word == ',')
                    builder.Append(" ");

                if (word == '{')
                {
                    _index++;
                    if (IfIndex > 0)
                    {
                        IfIndex++;
                    }
                    else
                    {
                        lastChangedNextLine = true;
                    }

                    

                }

                if (word == '}' && nextWord != ';')
                {

                    if (IfIndex > 0)
                    {
  
                    }
                    else

                    {
                        lastChangedNextLine = true;
                    }
                }
                lastWord = word;
                if (lastChangedNextLine && IfIndex == 0)
                {
                    builder.Append("\r\n");
   
                }
            }
            return builder.ToString();

        }









        public void ChangeKeyColor(string key, Color color, RichTextBox rbox)
        {
            var text = rbox.Text;
            var similarIndex = 0;
            if (key == "") return;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == key[0])
                {
                    for (int c = 0; c < key.Length && i + c < text.Length; c++)
                    {
                        if (text[i + c] == key[c])
                        {
                            similarIndex++;

                        }
                    }
                    if (similarIndex >= key.Length)
                    {
                        rbox.SelectionStart = i;
                        rbox.SelectionLength = key.Length;
                        rbox.SelectionColor = color;
                    }
                    similarIndex = 0;
                }
            }
        }

        public void ChangeKeyColorToEnd(string key, Color color, RichTextBox rbox)
        {
            var text = rbox.Text;
            var similarIndex = 0;
            var length = 1;
            var start = 0;
            var savedStart = 0;
            bool findEnd = false;
            if (key == "") return;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == key[0])
                {
                    for (int c = 0; c < key.Length && i + c < text.Length; c++)
                    {
                        if (text[i + c] == key[c])
                        {
                            similarIndex++;
                        }
                        if (text[i + c] == key[0])
                        {
                            start = i;
                        }
                    }
                    if (similarIndex >= key.Length)
                    {
                        findEnd = true;
                        savedStart = start;
                    }
                    similarIndex = 0;
                }
                if (findEnd && (text[i] == '}' || text[i] == ';' || text[i] == ',' || text[i] == '=' || text[i] == '*' || text[i] == '+' || text[i] == '-' || text[i] == '/'))
                {
                    length = i - savedStart;
                    findEnd = false;
                    rbox.SelectionStart = savedStart;
                    rbox.SelectionLength = length;
                    rbox.SelectionColor = color;

                }



            }
        }


        public void OptimizeVision(string text)
        {
            mainEditor.Text = GetCodeCorrectly(text);
        }

        public void OptimizeColor()
        {
            return;
        }

















        public static string GetPath(string path)
        {
            path = path.Replace("{user}", path_CurrentPackage);
            path = path.Replace("{game}", path_GameApp);
            path = path.Replace("{packages}", path_UserPackage);
            path = path.Replace("{editor}", EdLib.path_AppData);
            path = path.Replace("{localgame}", Application.StartupPath.Replace("workshop\\content\\1017410\\1815462892", "common\\Tetra Project\\TetraProject.exe"));


            //path = path.Replace("{localpackages}", "G:\\SteamGames\\steamapps\\workshop\\content\\1017410");

            path = path.Replace("{localpackages}", Application.StartupPath.Replace("1017410\\1815462892", "1017410"));

            path = path.Replace("\\TetraProject.exe", "");
            return path.Trim();
        }


        private void LoadUserInfo()
        {
            EdLib.XmlOpen("user", "AppConfig");
            path_GameApp = EdLib.Xml("Game");
            path_UserPackage = EdLib.Xml("Package");
            path_CurrentPackage = EdLib.Xml("UserPackage");
            currentPackageName = EdLib.Xml("PackageName");
        }

        private void SaveUserInfo()
        {
            EdLib.XmlOpen("user", "AppConfig");
            EdLib.SetXml("Game", GetPath(path_GameApp));
            EdLib.SetXml("Package", GetPath(path_UserPackage));
            EdLib.SetXml("UserPackage", GetPath(path_CurrentPackage));
            EdLib.SetXml("PackageName", currentPackageName);
            EdLib.XmlSave("User");
        }

        public void AutoScale(Form frm)
        {

        }


        /*
                if (control == pictureBox1 || control == pictureBox2 || control == pictureBox3 || control == pictureBox4 || control == label4 || control == label5 || control == label6 || control == label7 || control == label8)
                {
     
                    continue;
                }
         */



        private void StartFirstUsingSettings()
        {

            if (!EdLib.HasFile(path_UserPackage, true))
            {
                path_UserPackage = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\TetraProject\\Packages";
                if (!EdLib.HasFile(path_UserPackage, true))
                {
                    EdLib.AskMsg("未能自动找到您的 Packages 路径，请您手动设置。");
                }
            }
            EdLib.path_FileSelection = GetPath("{game}");
            var pos = GetPath("{localgame}");
            if (EdLib.HasFile(pos, true) && !pos.Contains("Debug"))
            {
                path_GameApp = GetPath(pos);
                path_CurrentPackage = PackageSelection();
                SaveUserInfo();
                loadAll();
                return;
            }
            else
            {
                EdLib.AskMsg(GetPath( "无法自动获取游戏目录，请您手动选择一些文件 TetraProject.exe， \n错误信息:{localgame}"));
            }


            StartFirstUsingSettingsFlag:

            EdLib.ShowFileSelection("找到您的游戏 TetraProject.exe");
            if (EdLib.GetFileSelection())
            {
                var path = System.IO.Path.GetFileName(EdLib.path_FileSelection);
                if (path  != "TetraProject.exe")
                {
                    if(EdLib.AskMsg("不是 TetraProject.exe，是否重新开始选择?。"))
                    {
                        goto StartFirstUsingSettingsFlag;
                    }
                    
                }
                else
                {
                    path_GameApp = EdLib.path_FileSelection;
                }
            }
            


            path_CurrentPackage = PackageSelection();
            SaveUserInfo();
            loadAll();
        }

        void textEditor_TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }


        public CompletionWindow completionWindow;
        public IList<ICompletionData> AutoCompleteData;
        void textEditor_TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                completionWindow = new CompletionWindow(mainEditor.TextArea);
                var k = completionWindow.CompletionList.CompletionData;
                completionWindow.Width = 500;
                foreach (var item in AutoCompleteData)
                {
                    k.Add(item);
                }
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
                
            }
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (AutoCompleteData == null) return;
            var isKey1Down = e.KeyValue == (int)Keys.Q && (int)Control.ModifierKeys == (int)Keys.Control;
            var isKey2Down = e.KeyValue == (int)Keys.T && (int)Control.ModifierKeys == (int)Keys.Control;
            if (isKey2Down)
            {
                completionWindow = new CompletionWindow(mainEditor.TextArea);
                var k = completionWindow.CompletionList.CompletionData;
                completionWindow.Width = 500;
                foreach (var item in AutoCompleteData)
                {
                    k.Add(item);
                }
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };

            }
            if(isKey1Down)
            {
                if(reoGridControl3.Visible)
                {
                    回到资源包ToolStripMenuItem_Click_1(this, null);
                }
                else
                {
                    打开CardCommandToolStripMenuItem_Click_1(this, null);
                }
            }
        }

        public class MyCompletionData : ICompletionData
        {
            public MyCompletionData(string text, string describe)
            {
                this.Text = text;
                this.Describe = describe;
            }

            public System.Windows.Media.ImageSource Image
            {
                get { return null; }
            }

            public string Text { get; private set; }
            public string Describe { get; private set; }
            // Use this property if you want to show a fancy UIElement in the list.
            public object Content
            {
                get { return this.Text; }
            }

            public object Description
            {
                get { return Describe; }
            }

            public double Priority => 0.0f;

            public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }
        }


        public void CreateNewPackage()
        {
            string infoName = EdLib.InputBox("请输入资源包文件夹名称 （英文）").Trim();
            string infoName2 = EdLib.InputBox("请输入MOD名称").Trim();
            string infoIntro = EdLib.InputBox("请输入MOD简介").Trim();
            if (infoName.Trim() == "" || infoName2.Trim() == "") return;
            EdLib.AskMsg("准备按程序生成默认资源，这将需要一些时间，请耐心等待。");
            var pos = Form1.GetPath(Form1.path_UserPackage) + "\\" + infoName;
            System.IO.Directory.CreateDirectory(pos);
            System.IO.Directory.CreateDirectory(pos + "\\DataBase");
            System.IO.Directory.CreateDirectory(pos + "\\CardArt");
            System.IO.Directory.CreateDirectory(pos + "\\CharacterModel");
            System.IO.Directory.CreateDirectory(pos + "\\Effect");
            System.IO.Directory.CreateDirectory(pos + "\\Emoji");
            System.IO.Directory.CreateDirectory(pos + "\\EnvironmentModel");
            System.IO.Directory.CreateDirectory(pos + "\\Illustration");
            System.IO.Directory.CreateDirectory(pos + "\\MinimapModel");
            System.IO.Directory.CreateDirectory(pos + "\\Music");
            System.IO.Directory.CreateDirectory(pos + "\\Sound");
            System.IO.Directory.CreateDirectory(pos + "\\StageMap");
            System.IO.File.Copy(Form1.GetPath(Form1.path_GameApp) + "\\TetraProject_Data\\StreamingAssets\\Packages\\Builtin\\Database\\Database.xls", pos + "\\DataBase\\DataBase.xls");
            System.IO.File.Copy(EdLib.path_AppData + "\\icon.png", pos+ "\\icon.png");
            if (!EdLib.HasFile(pos + "\\PackageInfo.json", false))
            {
                System.IO.File.WriteAllText(pos + "\\PackageInfo.json", @"{
    ""id"": """ + infoName + @""",
    ""displayName"": """ + infoName2 + @""",
    ""description"": """ + infoIntro + @""",
    ""publishedTags"": [
        ""Card""
    ]
}
               ");
            }
            path_CurrentPackage = pos;
            loadAll();
            foreach (var item in ReoMain.Worksheets)
            {
                for (int i = 1; i < item.RowCount - 1; i++)
                {
                    for (int d = 0; d < item.ColumnCount; d++)
                    {
                        if(item[i,d] != null )
                        {
                            item[i, d] = null;
                        }
                    }
                }

            }
            SaveFile();
            EdLib.AskMsg("资源包创建完毕");
            System.Diagnostics.Process.Start(pos);
        }


        public string PackageSelection()
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
            if(frm.indexOfLastSelection == -1)
            {
                return path_CurrentPackage;
            }

            currentPackageName = frm.indexOfLastSelection < frm.names.Count ? frm.names[frm.indexOfLastSelection] : "";
            return frm.indexOfLastSelection < frm.paths.Count ? frm.paths[frm.indexOfLastSelection] : "";
     
        }

        private void 更换资源包ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            path_CurrentPackage = PackageSelection();
            
            this.Text = appChineseName + "    " + currentPackageName;
            SaveUserInfo();
            loadAll();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            reoGridControl3.Hide();
            ReoMain = this.reoGridControl2;
            this.ForeColor = EdLib.CFore; this.BackColor = EdLib.CBack;
            menuStrip1.ForeColor = EdLib.CFore; menuStrip1.BackColor = EdLib.CBack;
            IHighlightingDefinition customHighlighting;
            var path = EdLib.path_AppData + "\\EditorHighlighting.xshd";
            using (StreamReader s = new StreamReader(path))
            {
                if (s == null)
                    throw new InvalidOperationException("加载编辑器数据失败。");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                    HighlightingLoader.Load(reader, HighlightingManager.Instance);

                }
            }
            mainEditor.ShowLineNumbers = true;
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".csscript" }, customHighlighting);
            mainEditor.SyntaxHighlighting = customHighlighting;
            mainEditor.FontSize = 15f;
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (System.Windows.Media.Brush)converter.ConvertFromString(ColorTranslator.ToHtml(Color.FromArgb(30, 30, 30)));
            var brush2 = (System.Windows.Media.Brush)converter.ConvertFromString(ColorTranslator.ToHtml(Color.FromArgb(218, 218, 218)));
            mainEditor.Background = brush;
            mainEditor.Foreground = brush2;
            mainEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            k_hook = new KeyboardHook();
            k_hook.KeyDownEvent += new KeyEventHandler(hook_KeyDown);//钩住键按下
            k_hook.Start();//安装键盘钩子
        }
        KeyboardHook k_hook;

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            OptimizeColor();
        }


        public static string GetClear(string code)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char word in code)
            {
                if (word == '\n' || word == '\r')
                    continue;
                if (word == ' ' || word == '	')
                    continue;
                builder.Append(word);

            }
            code = builder.ToString();
            builder.Clear();
            return code;
        }

        private void 选择游戏程序ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            StartFirstUsingSettings();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            reoGridControl2.Worksheets.Clear();
        }



        public void SaveFile()
        {
            if (EdLib.HasFile(path_CurrentPackage, true))
            {
                if (IsCurrentPackageSteamWorkshop)
                {
                    EdLib.AskMsg(loadOnlinePackageWarning);
                }
                else
                {
                    ReoMain.Save(GetPath(path_CurrentPackage) + "\\Database\\database.xlsx", unvell.ReoGrid.IO.FileFormat.Excel2007);
                    foreach (var item in ReoMain.Worksheets)
                    {
                        var destPos = GetPath(path_CurrentPackage) + "\\Database\\" + item.Name + ".csv";
                        item.ExportAsCSV(destPos);
                        File.WriteAllText(destPos, File.ReadAllText(destPos, Encoding.Default), Encoding.UTF8);
                    }
                }

            }


        }

        private void 保存数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFile();
        }


        private void reoGridControl2_CurrentWorksheetChanged(object sender, EventArgs e)
        {
          
            LoadSheet();
        }

        private void 打开资源包文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(EdLib.HasFile(path_CurrentPackage, true))
            {
                if(IsCurrentPackageSteamWorkshop)
                {
                    EdLib.AskMsg(loadOnlinePackageWarning);
                }
                else
                {
                    System.Diagnostics.Process.Start(path_CurrentPackage);
                }
                
            }
        }

        private void 关于我们ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
             EdLib.AskMsg(@"程序：Tamill
联系：1298878474@qq.com
适用游戏： Tetra Project （原石计划） Steam 版
如有bug 欢迎加群:951022336 汇报。
如加入 mod 制作队伍，欢迎加群:652279837。 

©2016-2019 Yiyin Tang Studio. All rights reserved.
Game Development: Alive Game Studio
");
        }

        private void 启动游戏ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("steam://rungameid/1017410");
        
        }

        private void SplitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SplitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SplitContainer1_Panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void SplitContainer1_Panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void PictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ElementHost1_ChildChanged(object sender, ChildChangedEventArgs e)
        {

        }

   
        private void 回到资源包ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            reoGridControl3.Hide();
        }

        private void 打开CardCommandToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
           
            var pos2 = GetPath("{game}\\TetraProject_Data\\StreamingAssets\\Packages\\Builtin\\Database\\CardCommand.csv");

            if (EdLib.HasFile(pos2))
            {
               reoGridControl3.Load(pos2, unvell.ReoGrid.IO.FileFormat.CSV);
                var sheet = reoGridControl3.CurrentWorksheet;
                sheet.SetColumnsWidth(0, 2, 200);
                reoGridControl3.Show();
            }
        }

        private void reoGridControl3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void 官方教程档案ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdLib.AskMsg("敬请期待!");
        }

        private void 以CSV方式重载入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsCSVLoader = false;
            loadAll();
            IsCSVLoader = true;
        }

        private void 常见异常ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pos2 = EdLib.path_AppData + "\\help.xlsx";

            if (EdLib.HasFile(pos2))
            {
                reoGridControl3.Load(pos2, unvell.ReoGrid.IO.FileFormat.Excel2007);
                var sheet = reoGridControl3.CurrentWorksheet;
                reoGridControl3.Show();
            }
        }

    }
}

//本程序由 Tamill (1298878474@qq.com) 制作
//代码未经允许请勿擅自修改或使用
//如有问题或建议 欢迎联系QQ: 1298878474