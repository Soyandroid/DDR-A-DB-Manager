using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;

namespace DDRA_DB_Manager
{
    public partial class ImportLoadingWindow : Form
    {
        List<XElement> oldDB;
        List<XElement> newDB;
        int i = 0;
        int numSongsToCheck;


        public ImportLoadingWindow(List<XElement> oldDB, List<XElement> newDB)
        {
            this.oldDB = oldDB;
            this.newDB = newDB;
            numSongsToCheck = newDB.Count();
            InitializeComponent();
        }

        private void ImportLoadingWindow_Load(object sender, EventArgs e)
        {
            //backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (i < numSongsToCheck)
            {
                bool matchFound = false;
                for (int j = 0; j < oldDB.Count(); j++)
                {
                    if ((int)oldDB[j].Element("mcode") == (int)newDB[i].Element("mcode"))
                    {
                        if (!XNode.DeepEquals(oldDB[j],newDB[i]))
                        {
                            //MessageBox.Show("Differences were found between the two elements.");
                            using (DualTextboxPopup compareEntriesPopup = new DualTextboxPopup(oldDB[j].ToString(), newDB[i].ToString()))
                            {
                                DialogResult result = compareEntriesPopup.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    oldDB[j] = newDB[i];
                                }
                            }
                        }
                        matchFound = true;
                        break;

                    }
                }
                if (!matchFound)
                {
                    //If they got here it's assumed that no mcodes were matches, so append it to the database.
                    oldDB.Add(newDB[i]);
                }

                //Thread.Sleep(10);

                double progress = (i / (double)numSongsToCheck)*100.0;
                backgroundWorker1.ReportProgress((int)progress+1);
                i++;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            checkingSongLabel.Text = "Checking song " + i.ToString() + " of " + numSongsToCheck.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }
    }
}
