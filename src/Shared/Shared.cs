// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2020 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Globalization;

namespace SyncroSim.Epi
{
    static class Shared
    {
        //Common
        public const string ITERATION_COLUMN_NAME = "Iteration";
        public const string TIMESTEP_COLUMN_NAME = "Timestep";
        public const string DATE_COLUMN_NAME = "Date";
        public const string JURISDICTION_COLUMN_NAME = "Jurisdiction";
        public const string VALUE_COLUMN_NAME = "Value";
        public const string DATASHEET_NAME_COLUMN_NAME = "Name";

        //Run Control
        public const string DATASHEET_RUN_CONTROL_NAME = "epi_RunControl";
        public const string DATASHEET_RUN_CONTROL_MIN_TIMESTEP_COLUMN_NAME = "MinimumTimestep";
        public const string DATASHEET_RUN_CONTROL_MAX_TIMESTEP_COLUMN_NAME = "MaximumTimestep";
        public const string DATASHEET_RUN_CONTROL_MIN_ITERATION_COLUMN_NAME = "MinimumIteration";
        public const string DATASHEET_RUN_CONTROL_MAX_ITERATION_COLUMN_NAME = "MaximumIteration";
        public const string DATASHEET_RUN_CONTROL_MODEL_HISTORICAL_DEATHS_COLUMN_NAME = "ModelHistoricalDeaths";
        public const string DATASHEET_RUN_CONTROL_START_DATE_COLUMN_NAME = "StartDate";
        public const string DATASHEET_RUN_CONTROL_END_DATE_COLUMN_NAME = "EndDate";

        //Runtime Jurisdiction
        public const string DATASHEET_RUNTIME_JURISDICTION_NAME = "epi_RuntimeJurisdiction";

        //Variable
        public const string DATASHEET_VARIABLE_NAME = "epi_Variable";
        public const string DATASHEET_VARIABLE_NAME_COLUMN_NAME = "Name";
        public const string DATASHEET_VARIABLE_IS_INTERNAL_COLUMN_NAME = "IsInternal";

        public const string DATASHEET_VARIABLE_VALUE_DATA_CASES = "Cases";
        public const string DATASHEET_VARIABLE_VALUE_DATA_DEATHS = "Deaths";
        public const string DATASHEET_VARIABLE_VALUE_DATA_POPULATION = "Population";

        public static void ThrowEpidemicException(string message)
        {
            ThrowEpidemicException(message, new object[0]);
        }

        public static void ThrowEpidemicException(string message, params object[] args)
        {
            throw new EpidemicException(string.Format(CultureInfo.InvariantCulture, message, args));
        }
    }
}
