// epidemic: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2020 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Text;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using SyncroSim.Core;

namespace SyncroSim.Epi
{
    [ObfuscationAttribute(Exclude = true, ApplyToMembers = false)]
    internal class VariableDataSheet : DataSheet
    {
        private const string RESERVED_WORD_ERROR = "The Variable name '{0}' is reserved for internal use.  Please choose another name.";
        private const string IS_INTERNAL_FLAG_ERROR = "The Variable name '{0}' is incorrectly marked as internal.";

        public override string CreateFilterSpec()
        {
            return CreateUserVariableFilter();
        }

        public override void DeleteRows(IEnumerable<DataRow> rows)
        {
            List<DataRow> ToDelete = new List<DataRow>();

            foreach (DataRow dr in rows)
            {
                if (!NameHasValue(dr[Shared.DATASHEET_VARIABLE_NAME_COLUMN_NAME]))
                {
                    Debug.Assert(false);
                    continue;
                }

                bool FlagSet = IsInternalFlagSet(dr);

                if (FlagSet)
                {
                    continue;
                }

                ToDelete.Add(dr);
            }

            if (ToDelete.Count > 0)
            {
                base.DeleteRows(ToDelete);
            }
        }

        public override void Validate(object proposedValue, string columnName)
        {
            if (IsInternalName(Convert.ToString(proposedValue, CultureInfo.InvariantCulture)))
            {
                string m = string.Format(RESERVED_WORD_ERROR, proposedValue);
                throw new DataException(m);
            }

            base.Validate(proposedValue, columnName);
        }

        public override void Validate(DataTable proposedData, DataTransferMethod transferMethod)
        {
            foreach (DataRow dr in proposedData.Rows)
            {
                if (!NameHasValue(dr[Shared.DATASHEET_VARIABLE_NAME_COLUMN_NAME]))
                {
                    continue;
                }

                string Name = Convert.ToString(dr[Shared.DATASHEET_VARIABLE_NAME_COLUMN_NAME], CultureInfo.InvariantCulture);
                bool FlagSet = IsInternalFlagSet(dr);
                bool NameInternal = IsInternalName(Name);

                //If we do an import (or a paste) we will be asked to validate the union of the existing data
                //and the new data.  We need to make sure the incoming data doesn't contain a reserved word but
                //we don't want to fail if the reserved word is just there as part of the union.  The only way to
                //tell the difference is if the "IsInternal" flag is set.  Of course, the user could subvert this
                //with an import but it would require some effort and the internal name matching for a project data scoped
                //import makes sure that records with existing names are only ever updated - not deleted or duplicated.

                if (FlagSet && (!NameInternal))
                {
                    string m = string.Format(IS_INTERNAL_FLAG_ERROR, Name);
                    throw new DataException(m);
                }

                if ((!FlagSet) && NameInternal)
                {
                    string m = string.Format(RESERVED_WORD_ERROR, Name);
                    throw new DataException(m);
                }
            }

            base.Validate(proposedData, transferMethod);
        }

        private static bool NameHasValue(object proposedValue)
        {
            if ((proposedValue == null) || 
                string.IsNullOrWhiteSpace(Convert.ToString(proposedValue, CultureInfo.InvariantCulture)))
            {
                return false;
            }

            return true;
        }

        private static bool IsInternalFlagSet(DataRow dr)
        {
            if (dr[Shared.DATASHEET_VARIABLE_IS_INTERNAL_COLUMN_NAME] == DBNull.Value)
            {
                return false;
            }

            int value = (int)(long)dr[Shared.DATASHEET_VARIABLE_IS_INTERNAL_COLUMN_NAME];
            Debug.Assert(value == 0 || value == -1);

            if (value != Booleans.BoolToInt(true))
            {
                return false;
            }

            return true;
        }

        private static bool IsInternalName(object proposedValue)
        {
            if (!NameHasValue(proposedValue))
            {
                return false;
            }

            string name = Convert.ToString(proposedValue, CultureInfo.InvariantCulture);

            if (string.Compare(name, Shared.DATASHEET_VARIABLE_VALUE_DATA_CASES, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (string.Compare(name, Shared.DATASHEET_VARIABLE_VALUE_DATA_DEATHS, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (string.Compare(name, Shared.DATASHEET_VARIABLE_VALUE_DATA_POPULATION, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (string.Compare(name, Shared.DATASHEET_VARIABLE_VALUE_MODEL_CASES, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (string.Compare(name, Shared.DATASHEET_VARIABLE_VALUE_MODEL_DEATHS, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }

        private string CreateUserVariableFilter()
        {
            List<int> ids = GetInternalVariableIds();

            if (ids.Count == 0)
            {
                return null;
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, 
                    "{0} NOT IN ({1})", 
                    this.PrimaryKeyColumn.Name,
                    CreateIntegerFilterSpec(ids));
            }
        }

        private List<int> GetInternalVariableIds()
        {
            List<int> ids = new List<int>();

            foreach (DataRow dr in this.GetData().Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (dr[Shared.DATASHEET_VARIABLE_IS_INTERNAL_COLUMN_NAME] != DBNull.Value)
                    {
                        bool IsInternal = GetDataBool(dr[Shared.DATASHEET_VARIABLE_IS_INTERNAL_COLUMN_NAME]);

                        if (IsInternal)
                        {
                            ids.Add(Convert.ToInt32(dr[this.PrimaryKeyColumn.Name], CultureInfo.InvariantCulture));
                        }
                    }
                }
            }

            return ids;
        }

        private static string CreateIntegerFilterSpec(List<int> ids)
        {
            StringBuilder sb = new StringBuilder();

            foreach (int i in ids)
            {
                sb.Append(i.ToString(CultureInfo.InvariantCulture));
                sb.Append(",");
            }

            return sb.ToString().TrimEnd(',');
        }

        private static bool GetDataBool(object value)
        {
            if ((object.ReferenceEquals(value, DBNull.Value)))
            {
                return false;
            }
            else
            {
#if DEBUG
                int i = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                Debug.Assert(i == 0 || i == -1);

                if (i == 0)
                {
                    Debug.Assert(Convert.ToBoolean(value, CultureInfo.InvariantCulture) == false);
                }
                else
                {
                    Debug.Assert(Convert.ToBoolean(value, CultureInfo.InvariantCulture) == true);
                }
#endif
                return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            }
        }
    }
}