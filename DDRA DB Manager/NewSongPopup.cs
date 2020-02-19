using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDRA_DB_Manager
{



    public partial class NewSongPopup : Form
    {

        private mixInfo[] mixList =
        {
            new mixInfo("1st Mix",          1000,   1),
            new mixInfo("2nd Mix",          2000,   2),
            new mixInfo("3rd Mix",          3000,   3),
            new mixInfo("4th Mix",          4000,   4),
            new mixInfo("5th Mix",          5000,   5),
            new mixInfo("6th Mix DDRMAX",   6000,   6),
            new mixInfo("7th Mix DDRMAX2",  7000,   7),
            new mixInfo("8th Mix EXTREME",  8000,   8),
            new mixInfo("SuperNOVA",        9000,   9),
            new mixInfo("SuperNOVA 2",      10000,  10),
            new mixInfo("X",                11000,  11),
            new mixInfo("X2",               12000,  12),
            new mixInfo("X3 vs 2nd",        13000,  13),
            new mixInfo("2013",             14000,  14),
            new mixInfo("2014",             15000,  15),
            new mixInfo("2015",             16000,  16),
            new mixInfo("A",                17000,  17),
            new mixInfo("======Other Games======",0,0),
            new mixInfo("KIDS",             18000,  1),
            //new mixInfo("USA",              2500,   2), //There are no unique songs in DDR USA
            new mixInfo("TRUE KiSS DESTiNATiON",2500,3),
            new mixInfo("DREAMS COME TRUE", 2600,   3),
            new mixInfo("Disney\'s RAVE / Disney\'s MIX", 2700, 3),
            new mixInfo("======CS Mixes======",0,   0),
            new mixInfo("おはスタ DanceDanceRevolution",3500, 3),
            new mixInfo("ときめきメモリアル2 Substories～Dancing Summer Vacation～",2500,2),
            new mixInfo("EXTRA MIX",        4500,   4),
            new mixInfo("Party Collection", 8500,   8),
            new mixInfo("Disney Channel EDITION", 18500,1),
            new mixInfo("DDR FESTIVAL / Extreme US",8750,8),
            new mixInfo("STRIKE / Extreme 2",8900,  8),
            new mixInfo("エアロビクスレボリューション", 18250, 1),
            new mixInfo("ダイエットチャンネル",18500, 1),
            new mixInfo("Disney Grooves",   6900,   6),
            new mixInfo("Hottest Party",    11500,  11),
            new mixInfo("Hottest Party 2",  11600,  11),
            new mixInfo("Hottest Party 3 / MUSIC FIT", 11700, 11),
            new mixInfo("Hottest Party 4 / DDR", 11800, 11),
            new mixInfo("Hottest Party 5 / DDR II", 12500, 12),
            new mixInfo("ULTRAMIX",         6500, 6),
            new mixInfo("ULTRAMIX 2",       6600, 6),
            new mixInfo("ULTRAMIX 3",       6700, 6),
            new mixInfo("ULTRAMIX 4",       6800, 6),
            new mixInfo("UNIVERSE",         12100, 12),
            new mixInfo("UNIVERSE 2",       12200, 12),
            new mixInfo("UNIVERSE 3",       12300, 12),
            new mixInfo("======No reserved IDs yet======",0,0),
            new mixInfo("In The Groove 1,2, or 3",0,0),
            new mixInfo("DDRMAX 3",0,0),
            new mixInfo("SuperNOVA 3",0,0)
        };


        public NewSongPopup()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(mixList);
            comboBox1.SelectedIndex = 0;
        }

        public int songID { get; set; }
        public int songMix { get; set; }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !checkBox1.Checked;
            numericUpDown1.Enabled = checkBox1.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonConfirm.Enabled = (mixList[comboBox1.SelectedIndex].mixSeriesID > 0);
            numericUpDown1.Value = mixList[comboBox1.SelectedIndex].mixStartingID;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            buttonConfirm.Enabled = (numericUpDown1.Value > 0);
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            songID = (int)numericUpDown1.Value;
            if (checkBox1.Checked)
                songMix = 1;
            else
                songMix = mixList[comboBox1.SelectedIndex].mixSeriesID;
            this.Close();
        }
    }
    //This would be cooler if it was an array
    class mixInfo
    {
        public string mixName { get; set; }
        public int mixStartingID { get; set; }
        public int mixSeriesID { get; set; }

        public mixInfo(string mName, int mStartingID, int mSeriesID)
        {
            mixName = mName;
            mixStartingID = mStartingID;
            mixSeriesID = mSeriesID;
        }

        //Needed to add items to dropdown list
        public override string ToString()
        {
            return mixName;
        }
    }
}
