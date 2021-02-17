// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2020 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Windows.Forms;

namespace SyncroSim.Epi
{
    public partial class ChooseDateForm : Form
    {
        public ChooseDateForm()
        {
            InitializeComponent();
        }

        public DateTime DateTime
        {
            get
            {
                return this.MainDateTimePicker.Value;
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
