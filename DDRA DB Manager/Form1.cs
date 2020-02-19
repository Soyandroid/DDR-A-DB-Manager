using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Collections;
using System.Diagnostics; 

namespace DDRA_DB_Manager
{


    public partial class Form1 : Form
    {
        private List<XElement> songdb;
        private crappyDebugLogLog DebugLog;
        private string programLocation;
        private string databaseLocation;

        private Color[] colorList =
        {
            Color.White, //None
            Color.GreenYellow, //1st
            Color.LightPink, //2nd
            Color.LightYellow, //3rd
            Color.Plum, //4th
            Color.LightBlue, //5th
            Color.Orange, //6th MAX
            Color.LightSalmon, //7th MAX2
            Color.PaleGreen, //EXTREME
            Color.LightCoral, //SuperNOVA
            Color.LightBlue, //SuperNOVA2
            Color.Orange, //X
            Color.LightGreen, //X2
            Color.LightBlue, //X3 vs 2nd
            Color.SkyBlue, //2013
            Color.PowderBlue, //2014
            Color.Cyan, //2015
            Color.DeepSkyBlue, //Ace
            Color.Gold, //A20
            Color.White //Nothing (Again)
        };

        //These lists are only used for CSV import and export.
        //TODO: public readonly IList<string> ITitles = new List<string> {"German", "Spanish", "Corrects", "Wrongs" }.AsReadOnly();
        //Or IReadOnlyList<string>
        private static readonly string[] mixNames =
        {
            "Invalid",
            "DanceDanceRevolution 1st Mix",
            "DanceDanceRevolution 2nd Mix",
            "DanceDanceRevolution 3rd Mix",
            "DanceDanceRevolution 4th Mix",
            "DanceDanceRevolution 5th Mix",
            "DDRMAX -DanceDanceRevolution 6thMIX-",
            "DDRMAX2 -DanceDanceRevolution 7thMIX-",
            "DanceDanceRevolution EXTREME",
            "DanceDanceRevolution SuperNOVA",
            "DanceDanceRevolution SuperNOVA2",
            "DanceDanceRevolution X",
            "DanceDanceRevolution X2",
            "DanceDanceRevolution X3 vs 2nd MIX",
            "DanceDanceRevolution (2013)",
            "DanceDanceRevolution (2014)",
            "DanceDanceRevolution (2015)",
            "DanceDanceRevolution A",
            "DanceDanceRevolution A20"
        };
        private static string[] genreNames =
        {
            "Tokimeki_Idol",
            "How_To_Play",
            "Popular_Songs",
            "Touhou",
            "Hibabita",
            "?",
            "Variety",
            "New Songs",
            "?",
            "Anime_Game",
            "Japanese_Pops",
            "?"
        };
        private static string[] bemaniNames =
        {
            "DanceRush",
            "Nostalgia",
            "Museca",
            "BeatStream",
            "FutureTomTom",
            "SDVX",
            "DanceEvolution",
            "ReflecBeat",
            "jubeat",
            "Gitadora",
            "Popn",
            "IIDX"
        };

        public Form1()
        {
            InitializeComponent();
            programLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            DebugLog = new crappyDebugLogLog();
        }

        private void openDatabase(bool firstRun = false)
        {
            string fileName = null;

            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Title = "Select a musicdb.xml or startup.arc...";
                openFileDialog1.Filter = "All valid files|*.xml;*.arc|DDR A musicdb.xml (*.xml)|*.xml|DDR A startup.arc (*.arc)|*.arc";
                //Why is this 1 indexed?
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog1.FileName;
                }
            }

            if (fileName != null)
            {
                //Do something with the file, for example read text from it
                //string text = File.ReadAllText(fileName);
                if (Path.GetExtension(fileName) == ".arc")
                {
                    string fileDest = Path.Combine(programLocation, Path.GetFileName(fileName));
                    //Check if we aren't trying to open the startup.arc already in the program dir...
                    if (fileName != fileDest)
                    {
                        DebugLog.WriteLine("Opened file was a .arc, copying to program dir and extracting.");
                        if (File.Exists(fileDest))
                        {
                            File.Delete(fileDest);
                            DebugLog.WriteLine("Deleted already existing " + fileDest);
                        }
                        //if (fileName != fileDest)
                        File.Copy(fileName, fileDest);
                    }
                    DebugLog.WriteLine("Going to extract .arc");
                    ProcessStartInfo processStartInfo;
                    processStartInfo = new ProcessStartInfo
                    {
                        FileName = @"arcutil.exe",
                        Arguments = "x " + Path.GetFileName(fileDest),
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };
                    var process = Process.Start(processStartInfo);
                    //DebugLog.Write(process.StandardOutput.ReadToEnd());
                    process.WaitForExit();
                    fileName = Path.Combine(programLocation, "data/gamedata/musicdb.xml");
                }
                databaseLocation = fileName;
                DebugLog.WriteLine("Opening " + fileName);
                XDocument TempXml = XDocument.Load(fileName);
                songdb = TempXml.Element("mdb").Elements().ToList();
                DebugLog.WriteLine("Read database XML into songDB array.");
                //DebugLog.WriteLine(songdb.ElementAt(0).InnerXML());
                listBox1.Items.Clear();
                foreach (XElement item in songdb)
                {
                    listBox1.Items.Add(item.Element("title").Value.ToString());
                }
                label_NumSongs.Text = "Number of songs: " + listBox1.Items.Count.ToString();
                listBox1.Enabled = true;

                /*using (XmlReader reader = XmlReader.Create(fileName))
                {

                }*/
                listBox1.SelectedIndex = 0;
            }
            else if (firstRun) //If first run and they pressed cancel
            {
                Application.Exit();
                return;
            }
            //Dumb hack because calling openDatabase() right after InitializeComponent() leaves the window hidden
            if (firstRun)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //If there is nothing selected
            if (listBox1.SelectedIndex == -1)
            {
                return;
            }
            XElement songItem = songdb.ElementAt(listBox1.SelectedIndex);
            textboxTitle.Text = songItem.Element("title").Value;
            textboxArtist.Text = songItem.Element("artist").Value;
            textboxInternalName.Text = songItem.Element("basename").Value;

            if (songItem.Element("title_yomi") == null)
                textboxTitleYomigana.Text = "";
            else
                textboxTitleYomigana.Text = (string)songItem.Element("title_yomi");

            dropdownSeries.SelectedIndex = (int)songItem.Element("series")-1;
            numericSongID.Value = (int)songItem.Element("mcode");

            if (songItem.Element("bgstage") == null)
                dropdownStage.SelectedIndex = 0;
            else
                dropdownStage.SelectedIndex = (int)songItem.Element("bgstage");

            if (songItem.Element("lamp") == null)
                dropdownLamp.SelectedIndex = 0;
            else
                dropdownLamp.SelectedIndex = (int)songItem.Element("lamp");

            if (songItem.Element("region") == null)
                combobox_Region.SelectedIndex = 0;
            else
                combobox_Region.SelectedIndex = (int)songItem.Element("region");

            if (songItem.Element("eventno") == null)
                numericEventNumber.Value = 0;
            else
                numericEventNumber.Value = (int)songItem.Element("eventno");

            if (songItem.Element("limited") == null)
                numericUnlockNumber.Value = 0;
            else
                numericUnlockNumber.Value = (int)songItem.Element("limited");
            if (songItem.Element("limited_cha") == null)
                numericLimited_cha.Value = 0;
            else
                numericLimited_cha.Value = (int)songItem.Element("limited_cha");

            //Movie
            if (songItem.Element("movie") == null)
            {
                dropdownVideoStyle.SelectedIndex = 0;
                textBoxMovieOverride.Enabled = false;
                numericBGAOffset.Enabled = false;
            }
            else
            {
                dropdownVideoStyle.SelectedIndex = (int)songItem.Element("movie");
                textBoxMovieOverride.Enabled = true;
                numericBGAOffset.Enabled = true;
            }

            if (songItem.Element("movieoffset") == null)
                numericBGAOffset.Value = 0;
            else
                numericBGAOffset.Value = (int)songItem.Element("movieoffset");

            if (songItem.Element("movieoverride") == null)
                textBoxMovieOverride.Text = "";
            else
                textBoxMovieOverride.Text = (string)songItem.Element("movieoverride");

            //Announcer
            if (songItem.Element("voice") == null)
            {
                dropdownAnnouncer.SelectedIndex = 0;
            }
            else
            {
                dropdownAnnouncer.SelectedIndex = (int)songItem.Element("voice");
            }


            numericBPMmax.Value = (int)songItem.Element("bpmmax");
            if (songItem.Element("bpmmin") == null)
                numericBPMmin.Value = numericBPMmax.Value;
            else
                numericBPMmin.Value = (int)songItem.Element("bpmmin");

            int[] difficulties = songItem.Element("diffLv").Value.Split(' ').Select(int.Parse).ToArray();
            SingleDiff_Beginner.Value = difficulties[0];
            SingleDiff_Light.Value = difficulties[1];
            SingleDiff_Standard.Value = difficulties[2];
            SingleDiff_Heavy.Value = difficulties[3];
            SingleDiff_Challenge.Value = difficulties[4];

            DoubleDiff_Beginner.Value = difficulties[5];
            DoubleDiff_Light.Value = difficulties[6];
            DoubleDiff_Standard.Value = difficulties[7];
            DoubleDiff_Heavy.Value = difficulties[8];
            DoubleDiff_Challenge.Value = difficulties[9];

            if (songItem.Element("bemaniflag") != null)
            {
                textbox_binaryString.Text = Convert.ToString(Int32.Parse(songItem.Element("bemaniflag").Value), 2).PadLeft(12, '0');
                textbox_bemaniflag.Text = songItem.Element("bemaniflag").Value;
                for (int i = 0; i < BemaniflagCheckBox.Items.Count; i++)
                {
                    BemaniflagCheckBox.SetItemCheckState(i, (textbox_binaryString.Text[i] == '1') ? CheckState.Checked : CheckState.Unchecked);
                }
            }
            else
            {
                textbox_binaryString.Text = "";
                textbox_bemaniflag.Text = "";
                foreach (int i in BemaniflagCheckBox.CheckedIndices)
                {
                    BemaniflagCheckBox.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
            if (songItem.Element("genreflag") != null)
            {
                textbox_binaryGenre.Text = Convert.ToString(Int32.Parse(songItem.Element("genreflag").Value), 2).PadLeft(12, '0');
                textbox_genreflag.Text = songItem.Element("genreflag").Value;
                for (int i = 0; i < GenreflagCheckBox.Items.Count; i++)
                {
                    GenreflagCheckBox.SetItemCheckState(i, (textbox_binaryGenre.Text[i] == '1') ? CheckState.Checked : CheckState.Unchecked);
                }
            }
            else
            {
                textbox_binaryGenre.Text = "";
                textbox_genreflag.Text = "";
                foreach (int i in GenreflagCheckBox.CheckedIndices)
                {
                    GenreflagCheckBox.SetItemCheckState(i, CheckState.Unchecked);
                }
            }


        }

        private void redrawListBox()
        {

        }

        private void openDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openDatabase();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String binaryString = "";
            for (int i = 0; i < BemaniflagCheckBox.Items.Count; i++)
            {
                binaryString += (BemaniflagCheckBox.GetItemChecked(i) ? "1" : "0");
            }
            textbox_binaryString.Text = binaryString;
            textbox_bemaniflag.Text = Convert.ToInt32(binaryString, 2).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the song \'" + listBox1.Items[listBox1.SelectedIndex].ToString() + "?. This cannot be undone!", "Warning",MessageBoxButtons.YesNo);
            if(dialogResult == DialogResult.Yes)
            {
                //Since something has to be selected...
                if (listBox1.SelectedIndex == 0)
                {
                    listBox1.SelectedIndex += 1;
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex - 1);
                    songdb.RemoveAt(listBox1.SelectedIndex - 1);
                }
                else
                {
                    listBox1.SelectedIndex -= 1;
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex + 1);
                    songdb.RemoveAt(listBox1.SelectedIndex + 1);
                }
                label_NumSongs.Text = "Number of songs: " + listBox1.Items.Count.ToString();
            }
        }

        private void numericBPMmin_ValueChanged(object sender, EventArgs e)
        {
            if (numericBPMmin.Value > numericBPMmax.Value)
            {
                numericBPMmax.Value = numericBPMmin.Value;
            }
        }

        private void numericBPMmax_ValueChanged(object sender, EventArgs e)
        {
            if (numericBPMmax.Value < numericBPMmin.Value)
            {
                numericBPMmin.Value = numericBPMmax.Value;
            }
        }

        private void colorListAccordingToMixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorListAccordingToMixToolStripMenuItem.Checked)
                listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            else
                listBox1.DrawMode = DrawMode.Normal;
            /*for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.Items[i].
                (int)songdb.ElementAt(i).Element("series")
            }*/
        }

        /// <summary>
        /// Handles the DrawItem event of the listBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {

            e.DrawBackground();
            Graphics g = e.Graphics;

            // draw the background color you want
            // mine is set to olive, change it to whatever you want

            if (e.Index < 0)
                g.FillRectangle(new SolidBrush(colorList[0]), e.Bounds);
            else if (e.State.HasFlag(DrawItemState.Selected))
            {
                g.FillRectangle(new SolidBrush(Color.Black), e.Bounds);
                g.DrawString(listBox1.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            else
            {
                Color c = colorList[(int)songdb.ElementAt(e.Index).Element("series")];
                g.FillRectangle(new SolidBrush(c), e.Bounds);

                // draw the text of the list item, not doing this will only show
                // the background color
                // you will need to get the text of item to display
                g.DrawString(listBox1.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));
                //g.DrawString(e.State.ToString(), e.Font, new SolidBrush(e.ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));

            }
            e.DrawFocusRectangle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new NewSongPopup();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                bool foundID = false;
                int i = 0;
                int newID = form.songID;
                //You know what fuck it I don't know how to do this
                //Garbage code
                while (!foundID)
                {
                    //Check before grabbing the mcode of the next element
                    int mcode = (int)songdb[i].Element("mcode");

                    if (i >= songdb.Count())
                    {
                        newID = (int)songdb[i].Element("mcode") + 1;
                        foundID = true;
                    }
                    int nextMcode =  (int)songdb[i+1].Element("mcode");

                    //We found an element with a higher ID, now we just have to find a gap...
                    if (nextMcode > newID)
                    {
                        i++;
                        foundID = true;
                    }
                    else
                    {
                        i++;
                    }
                    /*//Search for gaps
                    if (nextMcode - mcode > 1)
                    {
                        //Keep going until we reach an element higher than the new ID
                        if (mcode < newID)
                        {
                            i++;
                        }
                        else if (nextMcode != newID)
                        {
                            foundID = true;
                        }

                    }
                    else if ((int)songdb[i].Element("mcode") - (int)songdb[i - 1].Element("mcode") > 1)
                    {
                        newID = (int)songdb[i - 1].Element("mcode") + 1;
                        foundID = true;
                    }
                    else
                    {
                        
                        i++;
                    }*/
                }
                DebugLog.WriteLine("Found empty ID: " + newID.ToString());
                DebugLog.WriteLine("Previous element: (ID: " + (string)songdb[i].Element("mcode") + ") " + (string)songdb[i].Element("title"));
                
                XElement newSongItem = new XElement("music",
                    new XElement("mcode", new XAttribute("__type", "u32"), newID.ToString()),
                    new XElement("basename", ""),
                    new XElement("title", "Unnamed Song"),
                    new XElement("artist", ""),
                    new XElement("bpmmax", new XAttribute("__type", "u16"), "0"),
                    new XElement("series", new XAttribute("__type", "u8"), form.songMix.ToString()),
                    new XElement("diffLv", new XAttribute("__type", "u8"), new XAttribute("__count", "10"), "0 0 0 0 0 0 0 0 0 0")
                    );
                songdb.Insert(i, newSongItem);
                listBox1.Items.Insert(i, "Unnamed Song");
                listBox1.SelectedIndex = i;
                label_NumSongs.Text = "Number of Songs: " + songdb.Count().ToString();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Written by ARC.\nSpecial Thanks: The rest of the DDR A Omnimix & Megamix team!");
        }

        private void textboxTitle_Leave(object sender, EventArgs e)
        {
            if (checkBoxYomigana.Checked)
            {
                var forbiddenChars = @"@,.;'():一!！～~☆#$%^&*-_+=}{][|><`".ToCharArray();
                var cleanedString = new string(textboxTitle.Text.Where(c => !forbiddenChars.Contains(c)).ToArray());
                if (!textboxTitle.Text.Equals(cleanedString))
                {
                    textboxTitleYomigana.Text = cleanedString;
                }
                else
                {
                    textboxTitleYomigana.Text = "";
                }
            }
        }

        private void showDebugLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// If the element exists, remove it. Otherwise do nothing.
        /// </summary>
        /// <param name="element">The element to remove.</param>
        /// <returns>Returns true if an element was removed, or false if there was nothing to remove.</returns>
        private bool removeIfExists(XElement element)
        {
            if (element != null)
            {
                element.Remove();
                return true;
            }
            return false;
        }

        /// <summary>
        /// If element doesn't exist, create it with the specified type and value
        /// </summary>
        /// <param name="songItem">The songItem</param>
        /// <param name="name">The name of the element being accessed</param>
        /// <param name="value">The value to assign to the element</param>
        /// <param name="type">The type attribute of the element, if it doesn't already exist.</param>
        private void addAndOrAssign(XElement songItem, string name, string value, string type = null)
        {
            if (songItem.Element(name) == null)
            {
                if (type != null)
                {
                    songItem.Add(new XElement(name, new XAttribute("__type", type), value));
                }
                else
                {
                    songItem.Add(new XElement(name, value));
                }
            }
            else
            {
                songItem.Element(name).Value = value;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            //Welcome to programming Hell
            XElement songItem = songdb.ElementAt(listBox1.SelectedIndex);
            XElement songItemBackup = new XElement(songItem);
            songItem.Element("title").Value = textboxTitle.Text;
            songItem.Element("artist").Value = textboxArtist.Text;
            songItem.Element("basename").Value = textboxInternalName.Text;
            if (textboxTitleYomigana.Text != "")
            {
                addAndOrAssign(songItem, "title_yomi", textboxTitleYomigana.Text);
            }
            else
            {
                removeIfExists(songItem.Element("title_yomi"));
            }
            songItem.Element("series").Value = (dropdownSeries.SelectedIndex + 1).ToString();
            songItem.Element("mcode").Value = numericSongID.Value.ToString();
            if (dropdownStage.SelectedIndex != 0)
            {
                addAndOrAssign(songItem, "bgstage", dropdownStage.SelectedIndex.ToString(), "u16");
            }
            else
            {
                removeIfExists(songItem.Element("bgstage"));
            }
            if (dropdownLamp.SelectedIndex != 0)
            {
                addAndOrAssign(songItem, "lamp",dropdownLamp.SelectedIndex.ToString(),"u16");
            }
            else
            {
                removeIfExists(songItem.Element("lamp"));
            }
            if (combobox_Region.SelectedIndex != 0)
            {
                addAndOrAssign(songItem, "region", combobox_Region.SelectedIndex.ToString(), "u8");
            }
            else
            {
                removeIfExists(songItem.Element("region"));
            }
            //Using > 0 instead of != because what's consistency
            if (numericEventNumber.Value > 0)
            {
                addAndOrAssign(songItem, "eventno", numericEventNumber.Value.ToString(), "u8");
            }
            else
            {
                removeIfExists(songItem.Element("eventno"));
            }

            if (numericUnlockNumber.Value > 0)
            {
                addAndOrAssign(songItem, "limited", numericUnlockNumber.Value.ToString(), "u8");
            }
            else
            {
                removeIfExists(songItem.Element("limited"));
            }
            if (numericLimited_cha.Value > 0)
            {
                addAndOrAssign(songItem, "limited_cha", numericLimited_cha.Value.ToString(), "u8");
            }
            else
            {
                removeIfExists(songItem.Element("limited_cha"));
            }

            if (dropdownVideoStyle.SelectedIndex > 0)
            {
                addAndOrAssign(songItem, "movie", dropdownVideoStyle.SelectedIndex.ToString(), "u8");
            }
            else
            {
                removeIfExists(songItem.Element("movie"));
            }

            if (numericBGAOffset.Value > 0)
            {
                addAndOrAssign(songItem, "movieoffset", numericBGAOffset.Value.ToString(), "u32");
            }
            else
            {
                removeIfExists(songItem.Element("movieoffset"));
            }
            if (textBoxMovieOverride.Text != "")
            {
                songItem.Element("movieoverride").Value = textBoxMovieOverride.Text;
            }
            else
            {
                removeIfExists(songItem.Element("movieoverride"));
            }

            if (dropdownAnnouncer.SelectedIndex > 0)
            {
                addAndOrAssign(songItem, "voice", dropdownAnnouncer.SelectedIndex.ToString(), "u8");
            }
            else
            {
                removeIfExists(songItem.Element("voice"));
            }

            if (numericBPMmin.Value != numericBPMmax.Value)
            {
                addAndOrAssign(songItem, "bpmmin", numericBPMmin.Value.ToString(), "u16");
                songItem.Element("bpmmax").Value = numericBPMmax.Value.ToString();
            }
            else
            {
                removeIfExists(songItem.Element("bpmmin"));
                songItem.Element("bpmmax").Value = numericBPMmax.Value.ToString();
            }
            //Haha yes
            songItem.Element("diffLv").Value =
                SingleDiff_Beginner.Value.ToString() + " "
                + SingleDiff_Light.Value.ToString() + " "
                + SingleDiff_Standard.Value.ToString() + " "
                + SingleDiff_Heavy.Value.ToString() + " "
                + SingleDiff_Challenge.Value.ToString() + " "
                + DoubleDiff_Beginner.Value.ToString() + " "
                + DoubleDiff_Light.Value.ToString() + " "
                + DoubleDiff_Standard.Value.ToString() + " "
                + DoubleDiff_Heavy.Value.ToString() + " "
                + DoubleDiff_Challenge.Value.ToString();

            if (textbox_genreflag.Text != "0" && textbox_genreflag.Text != "")
            {
                addAndOrAssign(songItem, "genreflag", textbox_genreflag.Text, "u32");
            }
            else
            {
                removeIfExists(songItem.Element("genreflag"));
            }

            if (textbox_bemaniflag.Text != "0" && textbox_bemaniflag.Text != "")
            {
                addAndOrAssign(songItem, "bemaniflag", textbox_bemaniflag.Text, "u32");
            }
            else
            {
                removeIfExists(songItem.Element("bemaniflag"));
            }
            //Because WinForms is stupid, this will set the index to -1, so jump to previous item before renaming
            string title = textboxTitle.Text;
            if (listBox1.SelectedIndex > 0)
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
                listBox1.Items[listBox1.SelectedIndex + 1] = title;
                listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
            }
            else //If they've selected the first song in the list, we have to jump to the next item instead of previous
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                listBox1.Items[listBox1.SelectedIndex - 1] = title;
                listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
            }

            /*TextboxPopup textboxPopup = new TextboxPopup("Debugging...", songItemBackup.ToString() + "\\nNew item:\\n" + songItem.ToString());
            textboxPopup.ShowDialog();*/

        }

        private void dropdownVideoStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Movie
            if (dropdownVideoStyle.SelectedIndex == 0)
            {
                textBoxMovieOverride.Enabled = false;
                numericBGAOffset.Enabled = false;
            }
            else
            {
                textBoxMovieOverride.Enabled = true;
                numericBGAOffset.Enabled = true;
            }
        }

        private void debugGetXMLOfCurrentSongToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void textbox_genreflag_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            String binaryString = "";
            for (int i = 0; i < GenreflagCheckBox.Items.Count; i++)
            {
                binaryString += (GenreflagCheckBox.GetItemChecked(i) ? "1" : "0");
            }
            textbox_binaryGenre.Text = binaryString;
            textbox_genreflag.Text = Convert.ToInt32(binaryString, 2).ToString();

        }

        private void saveAndCompileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkForErrorsBeforeSavingToolStripMenuItem.Checked)
            {
                if (checkForDuplicates() != null)
                {
                    if (MessageBox.Show("Duplicate elements were found. Are you sure you want to save?\nYou can see the duplicated songs in Functions -> Check for duplicates.", "Duplicates found", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }
            }
            //Ex. s -> 2000-08-17T16:32:32
            string currentTime = DateTime.Now.ToString("s").Replace(':','_');
            if (!Directory.Exists("backups"))
                Directory.CreateDirectory("backups");
            string fileDest = Path.Combine(programLocation,"backups/", Path.GetFileNameWithoutExtension(databaseLocation)+currentTime+".xml");
            File.Copy(databaseLocation, fileDest);
            DebugLog.WriteLine("backed up " + Path.GetFileName(databaseLocation) + " to backups/" + Path.GetFileNameWithoutExtension(databaseLocation)+currentTime+".xml");
            if (!Directory.Exists("data"))
            {
                MessageBox.Show("You don't have an extracted data folder in this manager's directory yet, so pick the startup.arc you want to put a new musicdb.xml into.\n\nWarning: If you open a musicdb.xml instead of a startup.arc the next time you start this program, it will use the existing data folder when repacking.");
                string arcFileName;
                using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
                {
                    //openFileDialog1.InitialDirectory = "c:\\";
                    openFileDialog1.Filter = "DDR A Archive (*.arc)|*.arc";
                    //Why is this 1 indexed?
                    openFileDialog1.FilterIndex = 1;
                    openFileDialog1.RestoreDirectory = true;

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        arcFileName = openFileDialog1.FileName;

                        DebugLog.WriteLine("Copying temporary startup.arc.");
                        fileDest = Path.Combine(programLocation, Path.GetFileName("startup.arc"));
                        File.Copy(arcFileName, fileDest);
                        DebugLog.WriteLine("Going to extract .arc");
                        ProcessStartInfo processStartInfo;
                        processStartInfo = new ProcessStartInfo
                        {
                            FileName = @"arcutil.exe",
                            Arguments = "x startup.arc",
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        };
                        var process = Process.Start(processStartInfo);
                        //DebugLog.Write(process.StandardOutput.ReadToEnd());
                        process.WaitForExit();

                    }
                    else
                    {
                        return;
                    }
                }
                
            }
            XDocument newSongDB = new XDocument(new XComment("Generated by DB Manager on " + currentTime), new XElement("mdb", songdb));
            File.Delete("data/gamedata/musicdb.xml");
            newSongDB.Save(Path.Combine(programLocation, "data/gamedata/musicdb.xml"));
            DebugLog.WriteLine("Wrote the modified songDB to disk.");
            DebugLog.WriteLine("Going to pack .arc");
            ProcessStartInfo processStartInfo2;
            processStartInfo2 = new ProcessStartInfo
            {
                FileName = @"arcutil.exe",
                Arguments = "p startup.arc data",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var process2 = Process.Start(processStartInfo2);
            DebugLog.Write(process2.StandardOutput.ReadToEnd());
            process2.WaitForExit();

            //Copy built file to the new location, since arcutils just places it in the program dir... Yes It's shitty
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "startup.arc|startup.arc";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "startup.arc";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String sourcePath = Path.Combine(programLocation, Path.GetFileName("startup.arc"));
                string destPath = saveFileDialog1.FileName;
                if (sourcePath != destPath)
                {
                    File.Copy(sourcePath,destPath, true);
                }
                MessageBox.Show("Saved to " + saveFileDialog1.FileName + ". The musicdb.xml before modification has been backed up to backups/" + Path.GetFileNameWithoutExtension(databaseLocation) + currentTime + ".xml");
                /*if ((myStream = saveFileDialog1.OpenFile()) != null)
                 {
                     // Code to write the stream goes here.
                     myStream.Close();
                 }*/
                //MessageBox.Show("The startup.arc has been built. Copy it to data/arc\\nThe musicdb.xml before modification has been backed up to backups/" + Path.GetFileNameWithoutExtension(databaseLocation)+currentTime+".xml");
            }
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("arcutil.exe"))
            {
                MessageBox.Show("I need arcutils to work! Please place arcutil.exe in the same directory.");
            }
            if (File.Exists("gamemdx.dll"))
            {
                MessageBox.Show("Don't place me in the root of your DDR A data, you'll break arcutils. Place me in an empty folder instead.");
            }
            openDatabase(true);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int index = listBox1.FindString(textBox1.Text, -1);
            if (index != -1)
            {
                // Select the found item:
                listBox1.SetSelected(index, true);
            }
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> csv = new List<string>();
            for (int i = 0; i < songdb.Count(); i++)
            {
                string lineCSV = "";
                XElement songItem = songdb[i];
                lineCSV += songItem.Element("title").Value + ",";
                lineCSV += songItem.Element("artist").Value + ",";
                lineCSV += mixNames[(int)songItem.Element("series")] + ",";
                if (songItem.Element("bpmmin") == null)
                {
                    lineCSV += songItem.Element("bpmmax").Value + ",";
                }
                else
                {
                    lineCSV += songItem.Element("bpmmin").Value + "-" + songItem.Element("bpmmax").Value + ",";

                }

                if (songItem.Element("genreflag") != null)
                {
                    string binaryString = Convert.ToString(Int32.Parse(songItem.Element("genreflag").Value), 2).PadLeft(11, '0');
                    for (int j = 0; j < binaryString.Length; j++)
                    {
                        if (binaryString[j] == '1')
                        {
                            lineCSV += genreNames[j] + " ";
                        }
                    }
                    lineCSV += ",";
                }
                else
                {
                    lineCSV += ",";
                }
                if (songItem.Element("bemaniflag") != null)
                {
                    string binaryString = Convert.ToString(Int32.Parse(songItem.Element("bemaniflag").Value), 2).PadLeft(12, '0');
                    for (int j = 0; j < binaryString.Length; j++)
                    {
                        if (binaryString[j] == '1')
                        {
                            lineCSV += bemaniNames[j] + " ";
                        }
                    }
                    lineCSV += ",";
                }
                else
                {
                    lineCSV += ",";
                }
                lineCSV += songItem.Element("basename").Value + ",";
                lineCSV += songItem.Element("diffLv").Value;

                csv.Add(lineCSV);
            }
            //TODO: Change to a file save dialog.
            System.IO.File.WriteAllLines(Path.Combine(programLocation, "masterlist.csv"), csv.ToArray());
            MessageBox.Show("The CSV has been exported to the folder where this program is located.\nThe csv is in the format of title,artist,series,bpm (min-max),genre,other appearances,internal name,difficulty.\nID, lamp, stage, movie have not been recorded.");
        }

        private void unlockAllSongsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void showIDsInListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showIDsInListToolStripMenuItem.Checked)
            {
                for (int i = 0; i < songdb.Count(); i++)
                    listBox1.Items[i] = songdb[i].Element("mcode").Value + " - " + songdb[i].Element("title").Value;
            }
            else
            {
                for (int i = 0; i < songdb.Count(); i++)
                    listBox1.Items[i] = songdb[i].Element("title").Value;
            }
        }

        private void importFromCSVUnfinishedToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void compareWithAnotherMusicdbUnfinishedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = null;
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "DDR A MusicDB (*.xml)|*.xml";
                //Why is this 1 indexed?
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog1.FileName;
                }
            }
            if (fileName != null)
            {
                DebugLog.WriteLine("Opening " + fileName);
                XDocument TempXml = XDocument.Load(fileName);
                List<XElement> newDB = TempXml.Element("mdb").Elements().ToList();
                DebugLog.WriteLine("Read new database XML into songDB array.");
                ImportLoadingWindow importLoadingWindow = new ImportLoadingWindow(songdb, newDB);
                importLoadingWindow.ShowDialog();
                /*songdb = songdb.Concat(newDB).ToList();
                foreach (XElement item in newDB)
                {
                    listBox1.Items.Add(item.Element("title").Value.ToString());
                }*/
                listBox1.SelectedIndex = -1;
                listBox1.Items.Clear();
                foreach (var songEntry in songdb)
                {
                    listBox1.Items.Add(songEntry.Element("title").Value.ToString());
                }
                listBox1.SelectedIndex = 0;
                label_NumSongs.Text = "Number of songs: " + listBox1.Items.Count.ToString();

            }
        }

        private void unlockAllSongsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will remove all unlock flags from the song database. Are you sure?", "Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                int flagCount = 0;
                for (int i = 0; i < songdb.Count(); i++)
                {
                    if (removeIfExists(songdb[i].Element("limited")))
                        flagCount++;
                }
                MessageBox.Show("Removed " + flagCount.ToString() + " unlock flags.");
            }
        }

        private void showDebugLogToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            TextboxPopup textboxPopup = new TextboxPopup("Debug Log", DebugLog.getLog());
            textboxPopup.ShowDialog();
        }

        private void debugGetXMLOfCurrentSongToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            XElement songItem = songdb[listBox1.SelectedIndex];
            TextboxPopup textboxPopup = new TextboxPopup(songItem.Element("title").Value, songItem.ToString());
            textboxPopup.ShowDialog();
        }

        
        
        private string checkForDuplicates()
        {
            //Song ID and song title
            Dictionary<int, string> d = new Dictionary<int, string>();

            int dupeCount = 0;
            string dupeList = "The following IDs have duplicates:\\n";
            foreach (var songItem in songdb)
            {
                int itemID = (int)songItem.Element("mcode");
                string itemName = (string)songItem.Element("title");
                if (d.ContainsKey(itemID))
                {
                    dupeCount++;
                    dupeList += itemName + " and " + d[itemID] + " (ID: " + itemID.ToString() + ")\\n";
                }
                else
                    d.Add(itemID, itemName);
            }

            //Song internal name and song title
            Dictionary<string, string> d2 = new Dictionary<string, string>();
            
            string dupeNameList = "The following internal names have duplicates:\\n";
            foreach (var songItem in songdb)
            {
                string baseName = (string)songItem.Element("basename");
                string itemName = (string)songItem.Element("title");
                if (d2.ContainsKey(baseName))
                {
                    dupeCount++;
                    dupeNameList += itemName + " and " + d2[baseName] + " (basename: " + baseName.ToString() + ")\\n";
                }
                else
                    d2.Add(baseName, itemName);
            }
            //Because I can't return false
            if (dupeCount == 0)
                return null;

            return dupeList + "\\n\\n"+dupeNameList;
        }

        private void checkForDuplicatesNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dupeList = checkForDuplicates();
            if (dupeList == null)
                MessageBox.Show("There were no duplicates found.");
            else
            {
                TextboxPopup tp = new TextboxPopup("Duplicate IDs and internal names", dupeList);
                tp.ShowDialog();
            }
            
        }

        //This is just shitty copypasted code from the regular save function
        private void justSaveXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkForErrorsBeforeSavingToolStripMenuItem.Checked)
            {
                if (checkForDuplicates() != null)
                {
                    if (MessageBox.Show("Duplicate elements were found. Are you sure you want to save?\nYou can see the duplicated songs in Functions -> Check for duplicates.", "Duplicates found", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }
            }
            string currentTime = DateTime.Now.ToString("s").Replace(':', '_');
            if (!Directory.Exists("backups"))
                Directory.CreateDirectory("backups");
            string fileDest = Path.Combine(programLocation, "backups/", Path.GetFileNameWithoutExtension(databaseLocation) + currentTime + ".xml");
            File.Copy(databaseLocation, fileDest);
            DebugLog.WriteLine("backed up " + Path.GetFileName(databaseLocation) + " to backups/" + Path.GetFileNameWithoutExtension(databaseLocation) + currentTime + ".xml");

            XDocument newSongDB = new XDocument(new XComment("Generated by DB Manager on " + currentTime), new XElement("mdb", songdb));
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = "musicdb.xml";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                newSongDB.Save(saveFileDialog1.FileName);
                DebugLog.WriteLine("Wrote the modified songDB to disk.");
            }
        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxYomigana_CheckedChanged(object sender, EventArgs e)
        {
            textboxTitleYomigana.Enabled = !checkBoxYomigana.Checked;
        }

        private void debugGenerate1000DuplicateEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will add 1000 dupe entries to the song database for the purpose of testing memory allocation. Are you sure?", "Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                const int startingID = 15000;
                int insertLocation = 0;
                for (int i = 0; i < songdb.Count(); i++)
                {
                    //Hardcoded cuz I'm dumb and it's for debugging anyway
                    if ((int)songdb[i].Element("mcode") > 24576)
                    {
                        DebugLog.WriteLine("Found starting location to write songs. " + songdb[i].Element("mcode").Value);
                        insertLocation = i + 1;
                        break;
                    }
                }
                for (int i = 0; i< 1500; i++)
                {
                    XElement newSongItem = new XElement("music",
                        new XElement("mcode", new XAttribute("__type", "u32"), (startingID+i).ToString()),
                        new XElement("basename", "maxx"),
                        new XElement("title", "Unnamed Song"),
                        new XElement("artist", "No Artist"),
                        new XElement("bpmmax", new XAttribute("__type", "u16"), "0"),
                        new XElement("series", new XAttribute("__type", "u8"), "17"),
                        new XElement("diffLv", new XAttribute("__type", "u8"), new XAttribute("__count", "10"), "5 9 12 15 0 0 9 12 15 0")
                        );
                    songdb.Insert(insertLocation+i, newSongItem);
                    listBox1.Items.Insert(insertLocation+i, "Unnamed Song");

                }
                //listBox1.SelectedIndex = i;
                label_NumSongs.Text = "Number of Songs: " + songdb.Count().ToString();
            }
        }

        private void gapsInSonglistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            string dupeList = "Gaps: ";
            bool inGap = false;
            for (int i = 0; i < songdb.Count()-1; i++)
            {

                int songID = (int)songdb[i].Element("mcode");
                int nextSongID = (int)songdb[i+1].Element("mcode");
                if (inGap)
                {
                    if (songID+1 == nextSongID)
                    {
                        inGap = false;
                        dupeList += songID.ToString() + "\\n";
                    }
                }
                else
                {
                    if (songID + 1 != nextSongID)
                    {
                        inGap = true;
                        dupeList += songID.ToString() + "-";

                    }

                }
            }
            TextboxPopup tp = new TextboxPopup("Gaps", dupeList);
            tp.ShowDialog();

        }

        private void songsInEachMixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] groupCounts = new int[19];
            //mixNames
            foreach (var songItem in songdb)
            {
                groupCounts[(int)songItem.Element("series")]++;
            }
            string outStr = "";
            for (int i = 0; i < mixNames.Length; i++)
            {
                outStr += mixNames[i] + ": " + groupCounts[i] + "\\n";
            }
            TextboxPopup tp = new TextboxPopup("Num songs", outStr);
            tp.ShowDialog();

        }

        private void doNotAttemptEpicModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("You were warned.", "Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {

                listBox1.SelectedIndex = -1;
                listBox1.Items.Clear();
                songdb = new List<XElement>();
                DebugLog.WriteLine("Reset database...");
                //DebugLog.WriteLine(songdb.ElementAt(0).InnerXML());


                for (int i = 0; i < 900; i++)
                {
                    XElement newSongItem = new XElement("music",
                        new XElement("mcode", new XAttribute("__type", "u32"), (i+1).ToString()),
                        new XElement("basename", "maxx"),
                        new XElement("title", "MAX 300"),
                        new XElement("artist", "Ω"),
                        new XElement("bpmmax", new XAttribute("__type", "u16"), "300"),
                        new XElement("genreflag", new XAttribute("__type", "u32"), "4095"),
                        new XElement("bemaniflag", new XAttribute("__type", "u32"), "4095"),
                        new XElement("series", new XAttribute("__type", "u8"), Math.Floor((double)i/50+1).ToString()),
                        new XElement("diffLv", new XAttribute("__type", "u8"), new XAttribute("__count", "10"), "5 9 12 15 0 0 9 12 15 0")
                        );
                    songdb.Insert(i, newSongItem);
                    listBox1.Items.Insert(i, "MAX300");

                }
                listBox1.SelectedIndex = 0;
                label_NumSongs.Text = "Number of Songs: " + songdb.Count().ToString();
            }
        }

        private void unlockAllChallengeChartsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("This will remove all challenge unlock flags from the song database. Are you sure?", "Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                int flagCount = 0;
                for (int i = 0; i < songdb.Count(); i++)
                {
                    if (removeIfExists(songdb[i].Element("limited_cha")))
                        flagCount++;
                }
                MessageBox.Show("Removed " + flagCount.ToString() + " unlock flags.");
            }
        }

        private void exportFlagsToStepManiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*String binaryString = "";
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                binaryString += (checkedListBox2.GetItemChecked(i) ? "1" : "0");
            }
            textbox_binaryGenre.Text = binaryString;
            textbox_genreflag.Text = Convert.ToInt32(binaryString, 2).ToString();*/

            //An array of lists containing songs
            //Each genre is its own list inside this array
            List<XElement>[] bemaniGroups = new List<XElement>[bemaniNames.Length];
            //TODO: Convert this to linq somehow
            for (int i = 0; i < bemaniGroups.Length; i++)
               bemaniGroups[i] = new List<XElement>();
            List<XElement>[] genreGroups = new List<XElement>[genreNames.Length];
            for (int i = 0; i < genreGroups.Length; i++)
                genreGroups[i] = new List<XElement>();

            foreach (var songItem in songdb)
            {
                if (songItem.Element("bemaniflag") != null)
                {
                    string binaryString = Convert.ToString(int.Parse(songItem.Element("bemaniflag").Value), 2).PadLeft(12, '0');
                    for (int i = 0; i < bemaniNames.Length; i++)
                    {
                        //Add to the corresponding list, so if i=0 then it's the DanceRush flag, so add to the list indexed at 0
                        if (binaryString[i] == '1')
                            bemaniGroups[i].Add(songItem);
                    }
                }
                if (songItem.Element("genreflag") != null)
                {
                    string binaryString = Convert.ToString(Int32.Parse(songItem.Element("genreflag").Value), 2).PadLeft(12, '0');
                    for (int i = 0; i < genreNames.Length; i++)
                    {
                        if (binaryString[i] == '1')
                            genreGroups[i].Add(songItem);
                    }
                }
            }

            //TODO: Change to a file save dialog.
            List<string> bemaniSortTxt = new List<string>();
            for (int i = 0; i<bemaniGroups.Length; i++)
            {
                //Add the group name, ex. ---DanceRush
                bemaniSortTxt.Add("---"+bemaniNames[i]);
                foreach (var songItem in bemaniGroups[i])
                    bemaniSortTxt.Add(songItem.Element("title").Value);
            }

            List<string> genreSortTxt = new List<string>();
            for (int i = 0; i < genreGroups.Length; i++)
            {
                //Don't add empty groups (The genres have these, for some reason)
                if (genreGroups[i].Any())
                {
                    //Add the group name, ex. ---DanceRush
                    genreSortTxt.Add("---" + genreNames[i]);
                    foreach (var songItem in genreGroups[i])
                        genreSortTxt.Add(songItem.Element("title").Value);
                }
            }

            System.IO.File.WriteAllLines(Path.Combine(programLocation, "SongManager Bemani.txt"), bemaniSortTxt.ToArray());
            System.IO.File.WriteAllLines(Path.Combine(programLocation, "SongManager Genre.txt"), genreSortTxt.ToArray());
            MessageBox.Show("The preferred sort has been exported to the folder where this program is located.");




        }
    }
    public static class XElementExtension
    {
        public static string InnerXML(this XElement el)
        {
            var reader = el.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }
    }

    /*public class ListboxCustomItem
    {
        private string title;
        public ListboxCustomItem(XElement element)
        {
            title = element.Element("title").Value;
        }

        public override string ToString()
        {
            return title;
        }
    }*/

    public class crappyDebugLogLog
    {
        private string log;

        public crappyDebugLogLog()
        {
            log = "";
        }
        public void WriteLine(string msg)
        {
            log += msg + "\\n";
        }
        public void Write(string msg)
        {
            log += msg;
        }
        public string getLog()
        {
            return log;
        }
    }
}
