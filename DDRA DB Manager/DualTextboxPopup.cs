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
    /// <summary>
    /// A popup with two textboxes. Choosing the original (left) will return DialogResult.Cancel
    /// and choosing the new one (right) will return DialogResult.OK.
    /// </summary>
    public partial class DualTextboxPopup : Form
    {
        //XElement oldElement, newElement;
        public DualTextboxPopup(string oldElement, string newElement)
        {
            /*this.oldElement = oldElement;
            this.newElement = newElement;*/
            InitializeComponent();
            textBox1.Text = oldElement.Replace("\\n", Environment.NewLine); //Yeah I have no idea why it's like this
            textBox2.Text = newElement.Replace("\\n", Environment.NewLine);
        }

        private void DualTextboxPopup_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
