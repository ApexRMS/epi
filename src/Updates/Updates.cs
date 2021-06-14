// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Core;
using System.Diagnostics;
using System.Globalization;

namespace SyncroSim.Epi
{
    class Updates : UpdateProvider
    {    
        public override void PerformUpdate(DataStore store, int currentSchemaVersion)
        {
            if (currentSchemaVersion < 1)
            {
                EpiUpdate_0001(store);           
            }

#if DEBUG
            //Verify that all expected indexes exist after the update because it is easy to forget to recreate them after 
            //adding a column to an existing table (which requires the table to be recreated if you want to preserve column order.)

            ASSERT_INDEX_EXISTS(store, "epi_DataSummary");
#endif
        }

#if DEBUG

        private static void ASSERT_INDEX_EXISTS(DataStore store, string tableName)
        {
            if (store.TableExists(tableName))
            {
                string IndexName = tableName + "_Index";
                string Query = string.Format(CultureInfo.InvariantCulture, "SELECT COUNT(name) FROM sqlite_master WHERE type = 'index' AND name = '{0}'", IndexName);
                Debug.Assert((long)store.ExecuteScalar(Query) == 1);
            }
        }

#endif

        /// <summary>
        /// EpiUpdate_0001
        /// 
        /// This update:
        /// 1. Adds a TransformerID field to the epi_DataSummary table
        /// 2. Adds an index to the epi_DataSummary table
        /// </summary>
        /// <param name="store"></param>
        private static void EpiUpdate_0001(DataStore store)
        {
            //TransformerID
            store.ExecuteNonQuery("ALTER TABLE epi_DataSummary RENAME TO TEMP_TABLE");

            store.ExecuteNonQuery(@"CREATE TABLE epi_DataSummary ( 
                DataSummaryID INTEGER PRIMARY KEY AUTOINCREMENT,
                ScenarioID    INTEGER,
                TransformerID INTEGER,
                Iteration     INTEGER,
                Timestep      DATE,
                Variable      INTEGER,
                Jurisdiction  INTEGER,
                AgeMin        INTEGER,
                AgeMax        INTEGER,
                Sex           INTEGER,
                Value         DOUBLE)");

            store.ExecuteNonQuery(
                @"INSERT INTO epi_DataSummary(
                ScenarioID, Iteration, Timestep, Variable, Jurisdiction, AgeMin, AgeMax, Sex, Value) 
                SELECT 
                ScenarioID, Iteration, Timestep, Variable, Jurisdiction, AgeMin, AgeMax, Sex, Value 
                FROM TEMP_TABLE");

            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");

            //Index
            CreateIndex(store, "epi_DataSummary", new string[] {
                "ScenarioID", "TransformerID", "Iteration", "Timestep", "Variable", "Jurisdiction" });
        }
    }
}
