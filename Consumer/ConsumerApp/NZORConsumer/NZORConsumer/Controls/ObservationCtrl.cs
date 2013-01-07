﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NZORConsumer.Controls
{
    public partial class ObservationCtrl : UserControl
    {
        public event ProcessingMessage Message;

        Model.ConsumerEntities _model = null;

        public ObservationCtrl()
        {
            InitializeComponent();
        }
        
        public void Initialise(Model.ConsumerEntities model)
        {
            _model = model;
        }

        private void taxonNameText_TextChanged(object sender, EventArgs e)
        {
            if (taxonNameText.Text.Length > 1)
            {
                NZORServiceMessage pm = null;
                AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
                acsc.AddRange(ConsumerClient.GetAutoCompleteList(_model.Harvests.First().ServiceUrl, taxonNameText.Text, 20, out pm));
                taxonNameText.AutoCompleteCustomSource = acsc;

                if (Message != null) Message(pm, false);
            }
        }
    }
}
