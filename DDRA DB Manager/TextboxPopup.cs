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
    public partial class TextboxPopup : Form
    {
        public TextboxPopup(string title, string text)
        {
            InitializeComponent();
            this.Text = title;
            textBox1.Text = text.Replace("\\n", Environment.NewLine); //Yeah I have no idea why it's like this
            //textBox1.Text = "aaaaaaaaaaa\r\naaaaaaaaaa";
        }

        private void TextboxPopup_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
