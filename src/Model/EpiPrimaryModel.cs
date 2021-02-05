// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2020 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using SyncroSim.Core;
using SyncroSim.StochasticTime;

namespace SyncroSim.Epi
{
    class EpiPrimaryModel : StochasticTimeTransformer
    {
        public override void Configure()
        {
            this.ConfigureRunControl();
        }

        private void ConfigureRunControl()
        {
            DataSheet ds = this.ResultScenario.GetDataSheet(Shared.DATASHEET_RUN_CONTROL_NAME);
            DataRow dr = ds.GetDataRow();

            if (dr == null)
            {
                DataTable dt = ds.GetData();
                dr = dt.NewRow();
                dt.Rows.Add(dr);
            }

            if (dr[Shared.DATASHEET_RUN_CONTROL_MIN_ITERATION_COLUMN_NAME] == DBNull.Value ||
                dr[Shared.DATASHEET_RUN_CONTROL_MAX_ITERATION_COLUMN_NAME] == DBNull.Value)
            {
                dr[Shared.DATASHEET_RUN_CONTROL_MIN_ITERATION_COLUMN_NAME] = 1;
                dr[Shared.DATASHEET_RUN_CONTROL_MAX_ITERATION_COLUMN_NAME] = 1;
            }

            if (dr[Shared.DATASHEET_RUN_CONTROL_START_DATE_COLUMN_NAME] == DBNull.Value ||
                dr[Shared.DATASHEET_RUN_CONTROL_END_DATE_COLUMN_NAME] == DBNull.Value)
            {
                dr[Shared.DATASHEET_RUN_CONTROL_START_DATE_COLUMN_NAME] = DateTime.Now;
                dr[Shared.DATASHEET_RUN_CONTROL_END_DATE_COLUMN_NAME] = DateTime.Now.AddDays(30);
            }

            DateTime Start = (DateTime)dr[Shared.DATASHEET_RUN_CONTROL_START_DATE_COLUMN_NAME];
            DateTime End = (DateTime)dr[Shared.DATASHEET_RUN_CONTROL_END_DATE_COLUMN_NAME];

            Start = new DateTime(Start.Year, Start.Month, Start.Day, 0, 0, 0);
            End = new DateTime(End.Year, End.Month, End.Day, 0, 0, 0);
            int TotalDays = (End - Start).Days + 1;

            if (TotalDays <= 0)
            {
                Shared.ThrowEpidemicException("The run control start date cannot be greater than the end date.");
            }

            dr[Shared.DATASHEET_RUN_CONTROL_MIN_TIMESTEP_COLUMN_NAME] = 1;
            dr[Shared.DATASHEET_RUN_CONTROL_MAX_TIMESTEP_COLUMN_NAME] = TotalDays;
        }
    }
}
