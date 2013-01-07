using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NZORConsumer
{
    public partial class XmlResultForm : Form
    {
        public XmlResultForm()
        {
            InitializeComponent();
        }

        public string XmlText
        {
            get
            {
                return textbox1.Text;
            }
            set
            {
                textbox1.Text = value;
                textbox1.Select(0, 0);
            }
        }

    }
}
