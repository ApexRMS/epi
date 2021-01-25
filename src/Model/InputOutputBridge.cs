// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2020 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using SyncroSim.Core;

namespace SyncroSim.Epi
{
    class InputOutputBridge : Transformer
    {
        public override void Transform()
        {
            int MinTimestep = 0;
            int MaxTimestep = 0;
            DateTime StartDate = this.GetStartDateTime();
            DateTime EndDate = this.GetEndDateTime();
            DataTable dt1 = this.Scenario.GetDataSheet("epi_DataSummaryInput").GetData();
            DataTable dt2 = this.Scenario.GetDataSheet("epi_DataSummaryOutput").GetData();

            this.GetMinMaxTimestep(out MinTimestep, out MaxTimestep);

            using (DataStore store = this.Library.CreateDataStore())
            {
                foreach (DataRow drsrc in dt1.Rows)
                {
                    foreach (DataColumn c in dt1.Columns)
                    {
                        if (drsrc[c.ColumnName] == DBNull.Value)
                        {
                            Shared.ThrowEpidemicException(
                                "The value for '{0}' is required.",
                                c.ColumnName);
                        }
                    }

                    int? ts = null;

                    if (!this.TimestepFromDateTime(drsrc, StartDate, MinTimestep, MaxTimestep, out ts))
                    {
                        Shared.ThrowEpidemicException(
                            "Cannot convert date to timestep: {0}",
                            drsrc["Timestep"]);
                    }

                    DateTime dt = Convert.ToDateTime(drsrc["Timestep"]); 

                    string q = string.Format(
                        @"INSERT INTO epi_DataSummaryOutput(
                          ScenarioID,Iteration,Timestep,Date,Variable,Jurisdiction,AgeMin,AgeMax,Sex,Value) 
                          VALUES({0},{1},{2},'{3}',{4},{5},{6},{7},{8},{9})",
                            this.ResultScenario.Id,
                            drsrc["Iteration"],
                            ts,
                            dt.ToString("yyyy-MM-dd"),
                            drsrc["Variable"],
                            drsrc["Jurisdiction"],
                            drsrc["AgeMax"],
                            drsrc["AgeMin"],
                            drsrc["Sex"],
                            drsrc["Value"]);

                    store.ExecuteNonQuery(q);
                }
            }
        }

        private DateTime GetStartDateTime()
        {
            DataRow dr = this.ResultScenario.GetDataSheet(Shared.DATASHEET_RUN_CONTROL_NAME).GetDataRow();
            DateTime Start = (DateTime)dr[Shared.DATASHEET_RUN_CONTROL_START_DATE_COLUMN_NAME];

            return new DateTime(Start.Year, Start.Month, Start.Day, 0, 0, 0);
        }

        private DateTime GetEndDateTime()
        {
            DataRow dr = this.ResultScenario.GetDataSheet(Shared.DATASHEET_RUN_CONTROL_NAME).GetDataRow();
            DateTime Start = (DateTime)dr[Shared.DATASHEET_RUN_CONTROL_END_DATE_COLUMN_NAME];

            return new DateTime(Start.Year, Start.Month, Start.Day, 0, 0, 0);
        }

        private DateTime DateTimeFromTimestep(int timestep, DateTime dts, DateTime dte)
        {
            DateTime dt = new DateTime(dts.Year, dts.Month, dts.Day, 0, 0, 0);
            dt = dt.AddDays(timestep - 1);

            if (dt < dts)
            {
                Shared.ThrowEpidemicException(
                    "The timestep '{0}' ({1}) is less than the minimum date in run control.",
                    timestep, dt.ToString("yyyy-MM-dd"));
            }
            else if (dt > dte)
            {
                Shared.ThrowEpidemicException(
                    "The timestep '{0}'  ({1}) is greater than the maximum date in run control.",
                    timestep, dt.ToString("yyyy-MM-dd"));
            }

            return dt;
        }

        private bool TimestepFromDateTime(
            DataRow dr, 
            DateTime startDate, 
            int minTimestep, 
            int maxTimestep, 
            out int? timestep)
        {
            if (dr[Shared.TIMESTEP_COLUMN_NAME] == DBNull.Value)
            {
                timestep = null;
                return true;
            }

            DateTime d = (DateTime)dr[Shared.TIMESTEP_COLUMN_NAME];
            d = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
            int v = (d - startDate).Days + 1;

            if (v < minTimestep)
            {
                timestep = null;
                return false;
            }
            else if (v > maxTimestep)
            {
                timestep = null;
                return false;
            }

            timestep = v;
            return true;
        }

        private void GetMinMaxTimestep(out int minTimestep, out int maxTimestep)
        {
            DataRow dr = this.ResultScenario.GetDataSheet(Shared.DATASHEET_RUN_CONTROL_NAME).GetDataRow();

            minTimestep = Convert.ToInt32(dr[Shared.DATASHEET_RUN_CONTROL_MIN_TIMESTEP_COLUMN_NAME]);
            maxTimestep = Convert.ToInt32(dr[Shared.DATASHEET_RUN_CONTROL_MAX_TIMESTEP_COLUMN_NAME]);
        }
    }
}
