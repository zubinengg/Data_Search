using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace CDR_Search
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Read specified text file & return # lines
        # region BGworker1
        public int ReadTheFile(string fileName)
        {
            int numLines = 0;

            using (StreamReader sr = new StreamReader(fileName))
            {
                string nextLine;
                while ((nextLine = sr.ReadLine()) != null)
                {
                    numLines++;
                    if (numLines % 100000 == 0)
                    {
                        label2.Invoke((MethodInvoker)delegate { label2.Text = "Counted Lines :" + numLines.ToString(); });
                    }
                }
            }

            return numLines;
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {


            // Filename to process was passed to RunWorkerAsync(), so it's available
            // here in DoWorkEventArgs object.
            string sFileToRead = (string)e.Argument;
            e.Result = ReadTheFile(sFileToRead);
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                int numLines = (int)e.Result;
                label2.Text = "Number of lines Found :" + numLines.ToString() + " You can Now enter search Key";
                this.textBox2.ReadOnly = false;
                this.textBox4.Text = numLines.ToString();
            }
        }
        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select Database file";
            dlg.Filter = "Text File (*.txt)|*.txt|CSV File (*.csv)|*.csv";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = dlg.FileName;
                this.textBox4.Text = "";
                this.textBox5.Text = "";
                this.textBox6.Text = "";
                // Set up background worker object & hook up handlers
                BackgroundWorker bgWorker;
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

                // Launch background thread to do the work of reading the file.  This will
                // trigger BackgroundWorker.DoWork().  Note that we pass the filename to
                // process as a parameter.
                bgWorker.RunWorkerAsync(dlg.FileName);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox2.Text != "" && this.textBox1.Text != "")
            {
                this.button2.Enabled = true;
            }
            else
            {
                this.button2.Enabled = false;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.button2.Enabled = false;
            this.textBox2.ReadOnly = true;
            this.textBox4.ReadOnly = true;
            this.textBox5.ReadOnly = true;
            this.textBox6.ReadOnly = true;
            this.textBox3.ReadOnly = true;
        }
        # region Finder
        public int ReadTheFile_find(string fileName)
        {
            int numLines = 0;
            int found = 0;

            textBox2.Invoke((MethodInvoker)delegate { this.textBox2.ReadOnly = true; });
            button2.Invoke((MethodInvoker)delegate { this.button2.Enabled = false; });
            textBox3.Invoke((MethodInvoker)delegate { this.textBox3.Text = ""; });


            using (StreamReader sr = new StreamReader(fileName))
            {
                string nextLine;
                string key = this.textBox2.Text;
                while ((nextLine = sr.ReadLine()) != null)
                {
                    numLines++;
                    
                    if (nextLine.Contains(key))
                    {
                        found += 1;
                        //label2.Invoke((MethodInvoker)delegate { label2.Text = "Counted Lines :" + numLines.ToString(); });
                        textBox3.Invoke((MethodInvoker)delegate { this.textBox3.AppendText((found).ToString() + ". " + nextLine + Environment.NewLine); });

                    }
                    if (numLines % 100000 == 0)
                    {
                        double divder = Convert.ToDouble(int.Parse(this.textBox4.Text.ToString()));                        
                        progressBar1.Invoke((MethodInvoker)delegate { this.progressBar1.Value = Convert.ToInt16(Convert.ToDouble(numLines) / divder * 100); });
                        textBox5.Invoke((MethodInvoker)delegate { this.textBox5.Text = (numLines.ToString()); });
                        textBox6.Invoke((MethodInvoker)delegate { this.textBox6.Text = (found.ToString()); });

                    }
                    if (found>100)
                    {
                        break;
                    }

                }
            }

            return numLines;
        }

        void bgWorker_DoWork1_find(object sender, DoWorkEventArgs e)
        {


            // Filename to process was passed to RunWorkerAsync(), so it's available
            // here in DoWorkEventArgs object.

            string sFileToRead = (string)e.Argument;
            e.Result = ReadTheFile_find(sFileToRead);
        }

        void bgWorker_RunWorkerCompleted_find(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                int numLines = (int)e.Result;
                label2.Text = "Number of lines Found :" + numLines.ToString();
                this.textBox2.ReadOnly = false;
                //this.textBox4.Text = numLines.ToString();
            }
        }
        #endregion
        private void button2_Click(object sender, EventArgs e)
        {
            string file_name = this.textBox1.Text.ToString();
            long lineCount = 0;


            int count = 0;
            int found = 0;
            BackgroundWorker bgWorker;
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork1_find);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted_find);

            // Launch background thread to do the work of reading the file.  This will
            // trigger BackgroundWorker.DoWork().  Note that we pass the filename to
            // process as a parameter.
            bgWorker.RunWorkerAsync(file_name);
            foreach (string line in File.ReadLines(file_name))
            {
                count += 1;
                if (count > 10)
                {
                    this.textBox3.AppendText(lineCount.ToString() + ". " + line + Environment.NewLine);
                    break;
                }
                if (line.Contains(this.textBox2.Text))
                {
                    found += 1;
                    this.textBox3.AppendText(count.ToString() + ". " + line + Environment.NewLine);
                }
                if (found > 100)
                {
                    break;
                }
                //this.textBox3.Text += count.ToString()+". " +    line + "\n";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
