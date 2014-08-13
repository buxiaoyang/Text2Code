using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;

namespace Text2Code
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.textBoxLineBegin.Text = "str.Append(\"";
            this.textBoxLineEnd.Text = "\");";
            this.textBoxReplace.Text = "\\,\\\\,\",\\\"";
            this.richTextBoxMain.AppendText("Welcome to use Text-To-Source tools.\n\n");
            this.richTextBoxMain.AppendText("This tool can easily format text file into the program code.\n\n");
            this.richTextBoxMain.AppendText("1, Click on the \"Select\" button, select the file you want to convert. (NOTE: only supports plain text files with UTF8 format).\n\n");
            this.richTextBoxMain.AppendText("2, Enter the begin string and end string of each line you want to append.\n\n");
            this.richTextBoxMain.AppendText("3, Enter the characters you want to be replaced in the line. (Note: separated by commas, for example enter \",\\\" can replace all \" to \\\", enter a,b,c,d can first replace all a to b and then replace all c to d.)\n\n");
            this.richTextBoxMain.AppendText("4, Click on the \"Process\" button to start conversion. After that you can copy the result to your code.\n\n");
            this.richTextBoxMain.AppendText("Any problem, please feel free to contact me at buxiaoyang@gamil.com. :)\n");
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            this.panelLineBegin.Width = this.panelParameter.Width / 3;
            this.panelLineReplace.Width = this.panelParameter.Width / 3;
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            string fileName = "";//File Name
            //打开文件对话框
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "txt files|*.txt|excel files|*.xls|html files|*.html|xml files|*.xml|all files|*.*";
            ofd.FilterIndex = 5;
            ofd.RestoreDirectory = true;
            ofd.ShowDialog();
            fileName = ofd.FileName;
            try
            {
                if (fileName != "" && System.IO.File.Exists(fileName))
                {
                    this.textBoxFilePath.Text = fileName;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Select File Failed.\n" + ex.Message);
            }
        }

        private void buttonProcess_Click(object sender, EventArgs e)
        {
            //Read file
            //this.richTextBoxMain.Text = "";
            string lineBegin = this.textBoxLineBegin.Text;
            string lineEnd = this.textBoxLineEnd.Text;
            string[] replace = this.textBoxReplace.Text.Split(',');
            int replaceCount = replace.Count();
            if (this.textBoxReplace.Text != "" && replaceCount > 0 && (replaceCount % 2) != 0)
            {
                MessageBox.Show("Replace Parameter isn't accepted.");
                this.textBoxReplace.Focus();
            }
            else
            { 
                try
                {
                    if (CheckIsTextFile(this.textBoxFilePath.Text))
                    {
                        using (StreamReader sr = File.OpenText(this.textBoxFilePath.Text))
                        {
                            this.richTextBoxMain.Text = "";
                            string s = "";
                            while ((s = sr.ReadLine()) != null)
                            {
                                this.richTextBoxMain.AppendText(processLine(s, replaceCount, replace, lineBegin, lineEnd));
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("The file you choose is not plain text file, please choose another file.");
                    }
                }
                catch (System.Exception ex)
                {
                    if(ex.Message == "Empty path name is not legal." && this.richTextBoxMain.Text != "")
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < richTextBoxMain.Lines.Count(); i++)
                        {
                            sb.Append(processLine(richTextBoxMain.Lines[i], replaceCount, replace, lineBegin, lineEnd));
                        }
                        this.richTextBoxMain.Text = sb.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Process Failed.\n" + ex.Message);
                    }
                }//end try
            }//end else
        }//end buttonProcess_Click

        public string processLine(string input, int replaceCount, string[] replace, string lineBegin, string lineEnd)
        {
            StringBuilder sb = new StringBuilder(input);
            if (this.textBoxReplace.Text != "")
            {
                for (int i = 0; i < replaceCount; i += 2)
                {
                    sb = sb.Replace(replace[i], replace[i + 1]);
                }
            }
            sb.Insert(0, lineBegin).Append(lineEnd + "\n");
            return sb.ToString();
        }

        /// <summary>
        /// Checks the file is textfile or not.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static bool CheckIsTextFile(string fileName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                bool isTextFile = true;
            
                int i = 0;
                int length = (int)fs.Length;
                byte data;
                while (i < length && isTextFile)
                {
                    data = (byte)fs.ReadByte();
                    isTextFile = (data != 0);
                    i++;
                }
                return isTextFile;
            }
            catch (Exception ex)
            {
                throw new Exception("Empty path name is not legal.");
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

    }//end class
}
