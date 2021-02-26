// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2020 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Core;
using SyncroSim.Core.Forms;
using System.Windows.Forms;

namespace SyncroSim.Epi
{
    partial class RunControlDataFeedView : DataFeedView
    {
        public RunControlDataFeedView()
        {
            InitializeComponent();
        }

        protected override void InitializeView()
        {
            base.InitializeView();

            DataFeedView v = this.Session.CreateMultiRowDataFeedView(this.Scenario, this.ControllingScenario);
            this.PanelJurisdictions.Controls.Add(v);

            NativeMethods.SendMessage(this.TextBoxStartDate.Handle, NativeMethods.EM_SETCUEBANNER, 0, Shared.CUE_BANNER_DATE);
            NativeMethods.SendMessage(this.TextBoxEndDate.Handle, NativeMethods.EM_SETCUEBANNER, 0, Shared.CUE_BANNER_DATE);

            int h = this.TextBoxStartDate.Height;
            this.ButtonStartDate.Size = new System.Drawing.Size(h, h);
            this.ButtonEndDate.Size = new System.Drawing.Size(h, h);
        }

        public override void LoadDataFeed(DataFeed dataFeed)
        {
            base.LoadDataFeed(dataFeed);

            this.SetTextBoxBinding(
                this.TextBoxStartDate,
                Shared.DATASHEET_RUN_CONTROL_NAME,
                Shared.DATASHEET_RUN_CONTROL_MIN_TIMESTEP_COLUMN_NAME);

            this.SetTextBoxBinding(
                this.TextBoxEndDate,
                Shared.DATASHEET_RUN_CONTROL_NAME,
                Shared.DATASHEET_RUN_CONTROL_MAX_TIMESTEP_COLUMN_NAME);

            this.SetTextBoxBinding(
                this.TextBoxMaxIterations, 
                Shared.DATASHEET_RUN_CONTROL_NAME,
                Shared.DATASHEET_RUN_CONTROL_MAX_ITERATION_COLUMN_NAME);

            MultiRowDataFeedView v = (MultiRowDataFeedView)this.PanelJurisdictions.Controls[0];
            v.LoadDataFeed(dataFeed, Shared.DATASHEET_RUNTIME_JURISDICTION_NAME);
        }

        public override void EnableView(bool enable)
        {
            base.EnableView(enable);

            MultiRowDataFeedView v = (MultiRowDataFeedView)this.PanelJurisdictions.Controls[0];
            v.GridControl.IsReadOnly = (!enable);
        }

        protected override void OnBoundTextBoxValidated(TextBox textBox, string columnName, string newValue)
        {
            base.OnBoundTextBoxValidated(textBox, columnName, newValue);

            if (textBox == this.TextBoxMaxIterations)
            {
                DataSheet ds = this.DataFeed.GetDataSheet(Shared.DATASHEET_RUN_CONTROL_NAME);
                ds.SetSingleRowData(Shared.DATASHEET_RUN_CONTROL_MIN_ITERATION_COLUMN_NAME, 1);                
            }
        }

        private void ButtonStartDate_Click(object sender, System.EventArgs e)
        {
            ChooseDateForm f = new ChooseDateForm();

            if (f.ShowDialog(this) == DialogResult.OK)
            {
                DataSheet ds = this.DataFeed.GetDataSheet(Shared.DATASHEET_RUN_CONTROL_NAME);
                ds.SetSingleRowData(Shared.DATASHEET_RUN_CONTROL_MIN_TIMESTEP_COLUMN_NAME, f.DateTime);
                this.RefreshBoundControls();
            }
        }

        private void ButtonEndDate_Click(object sender, System.EventArgs e)
        {
            ChooseDateForm f = new ChooseDateForm();

            if (f.ShowDialog(this) == DialogResult.OK)
            {
                DataSheet ds = this.DataFeed.GetDataSheet(Shared.DATASHEET_RUN_CONTROL_NAME);
                ds.SetSingleRowData(Shared.DATASHEET_RUN_CONTROL_MAX_TIMESTEP_COLUMN_NAME, f.DateTime);
                this.RefreshBoundControls();
            }
        }
    }
}
