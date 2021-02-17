// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2020 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Core;
using SyncroSim.Core.Forms;

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
        }

        public override void LoadDataFeed(DataFeed dataFeed)
        {
            base.LoadDataFeed(dataFeed);

            this.SetTextBoxBinding(
                this.TextBoxStartDate,
                Shared.DATASHEET_RUN_CONTROL_NAME,
                Shared.DATASHEET_RUN_CONTROL_START_DATE_COLUMN_NAME);

            this.SetTextBoxBinding(
                this.TextBoxEndDate,
                Shared.DATASHEET_RUN_CONTROL_NAME,
                Shared.DATASHEET_RUN_CONTROL_END_DATE_COLUMN_NAME);

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
    }
}
