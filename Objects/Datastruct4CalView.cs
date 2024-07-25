using ExpressBase.Common;
using ExpressBase.Common.Singletons;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using ExpressBase.Security;
using System.Globalization;
using ExpressBase.CoreBase.Globals;

namespace ExpressBase.Objects.Objects
{
    public class DataStruct4CalView
    {
        private string PrimaryKeyColumnName { get; set; }

        private string ForeignKeyColumnName { get; set; }

        private List<DVBaseColumn> DataColumns { get; set; }

        private string DateColumnName { get; set; }

        public DataStruct4CalViewCols Columns = new DataStruct4CalViewCols();

        private Dictionary<int, DataStruct4CalViewRow> _innerDict = new Dictionary<int, DataStruct4CalViewRow>();//dict<partyid, dict<int column_index, long value>

        Dictionary<int, List<double>> Totals = new Dictionary<int, List<double>>();

        public List<ColumnCondition> ConditionalFormating { get; set; }

        public List<ObjectBasicInfo> ObjectLinks { get; set; }

        EbVisualizationGlobals globals = new EbVisualizationGlobals();

        public int InitialColumnsCount { get; set; }

        public AttendanceType CalendarType { get; set; }

        public bool ShowGrowthPercentage { get; set; }

        public DataStruct4CalView(EbCalendarView C)
        {
            this.PrimaryKeyColumnName = C.PrimaryKey.Name;
            this.ForeignKeyColumnName = C.ForeignKey.Name;
            this.DateColumnName = C.LinesColumns.FirstOrDefault(col => !col.IsCustomColumn && (col.Type == EbDbTypes.Date || col.Type == EbDbTypes.DateTime))?.Name;
            this.DataColumns = C.DataColumns.FindAll(col => col.bVisible);
            this.ConditionalFormating = C.DataColumns.FirstOrDefault(col => col.bVisible)?.ConditionalFormating;
            this.ObjectLinks = C.ObjectLinks;
            this.CalendarType = C.CalendarType;
            this.ShowGrowthPercentage = C.ShowGrowthPercentage;
        }

        public void Add(EbDataRow row)
        {
            //dict<partyid, dict<int column_index, Dict<string datacolname,long value>>
            if (!_innerDict.ContainsKey(Convert.ToInt32(row[this.ForeignKeyColumnName])))
                _innerDict.Add(Convert.ToInt32(row[this.ForeignKeyColumnName]), new DataStruct4CalViewRow());
            DateTime dt = Convert.ToDateTime(row[this.DateColumnName]);
            if (dt > DateTime.MinValue)
            {
                foreach (DVBaseColumn col in DataColumns)
                {
                    _innerDict[Convert.ToInt32(row[this.ForeignKeyColumnName])]
                        .Add(Columns.GetColumnKey(dt), col.Name, Convert.ToDouble(row[col.Name]));
                }
            }
        }

        public void GetFormatedTable(ref EbDataTable _formattedTable, RowColletion masterRows, ref Dictionary<int, List<object>> summary, User _user)
        {
            for (int i = 0; i < masterRows.Count; i++)//filling master data
            {
                _formattedTable.Rows.Add(_formattedTable.NewDataRow2());
                for (int j = 0; j < masterRows[0].Count; j++)
                {
                    _formattedTable.Rows[i][j] = masterRows[i][j];
                }
            }
            CultureInfo _user_culture = CultureHelper.GetSerializedCultureInfo(_user.Preference.Locale).GetCultureInfo();
            _user_culture.NumberFormat.NumberDecimalDigits = 0;
            EbDataRow Row;
            double value;
            double prev_value;
            int summary_last_index = summary.Keys.Last();
            string total;
            for (int i = 0; i < _formattedTable.Rows.Count; i++)//filling consolidated data
            {
                Row = _formattedTable.Rows[i];
                int _id = (int)Row[PrimaryKeyColumnName];
                if (_innerDict.ContainsKey(_id))
                {
                    if (!Totals.ContainsKey(_id))
                    {
                        Totals.Add(_id, new List<double>());
                    }
                    total = string.Empty;
                    foreach (int columnIndex in _innerDict[_id].GetKeys())
                    {
                        string val = string.Empty;
                        int j = 0;
                        foreach (DVBaseColumn col in DataColumns)
                        {
                            prev_value = 0;
                            value = _innerDict[_id].GetValue(columnIndex, col.Name);
                            if (ShowGrowthPercentage && CalendarType != AttendanceType.DayWise)
                            {
                                if (columnIndex > this.InitialColumnsCount && Columns.GetColumnByIndex(columnIndex).StartDate > Columns.GetColumnByIndex(columnIndex - 1).StartDate)
                                    prev_value = _innerDict[_id].GetValue(columnIndex - 1, col.Name);
                            }
                            val += GetFormattedValue(value, Row, columnIndex, prev_value, _user_culture, col);
                            try
                            {
                                if (Totals[_id].Count < j + 1)
                                    Totals[_id].Add(value);
                                else
                                    Totals[_id][j] = Totals[_id][j] + value;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message + "   " + _id);
                            }
                            summary[columnIndex][j] = (Convert.ToDecimal(summary[columnIndex][j], CultureInfo.InvariantCulture) + Convert.ToDecimal(value)).ToString("N", CultureInfo.InvariantCulture);
                            summary[summary_last_index][j] = (Convert.ToDecimal(summary[summary_last_index][j], CultureInfo.InvariantCulture) + Convert.ToDecimal(value)).ToString("N", CultureInfo.InvariantCulture); ;
                            j++;
                        }
                        Row[columnIndex] = val;
                    }
                    for (int k = 0; k < DataColumns.Count; k++)
                    {
                        if (Totals[_id].Count > k)
                            total += $"<div class='dataclass {DataColumns[k].Name}_class'>{Convert.ToDecimal(Totals[_id][k]).ToString("N", CultureInfo.InvariantCulture)}</div>";
                    }
                    Row["Total"] = total;
                }
            }

        }

        private string GetFormattedValue(double value, EbDataRow row, int columnIndex, double prev_value, CultureInfo _user_culture, DVBaseColumn col)
        {
            string Val = (value == 0) ? string.Empty : Convert.ToDecimal(value).ToString("N", CultureInfo.InvariantCulture);
            string formatteddata = string.Empty;
            if (ConditionalFormating.Count > 0)
            {
                DoConditionalFormating(ref formatteddata, value, row);
            }
            else
            {
                if (this.ObjectLinks.Count == 1)
                {
                    formatteddata = "<a href ='#' class ='tablelink4calendar cal-data' idx='" + columnIndex + "'>" + Val + "</a>";
                }
                else
                {
                    formatteddata = "<span clas ='cal-data'>" + Val + "</span>";
                }

                if (ShowGrowthPercentage && CalendarType != AttendanceType.DayWise && prev_value > 0 && value > 0)
                {
                    int percent = Convert.ToInt32(((value - prev_value) * 100) / value);
                    string color = "green", direction = "up";
                    if (percent < 0)
                    {
                        color = "red";
                        direction = "down";
                        percent *= -1;
                    };
                    formatteddata += @"  <span class='per-cont " + color + "'>" +
                                            "<i class='fa fa-caret-" + direction + "'></i>" +
                                            "<div class='val-perc'>" + percent + "% </div>" +
                                        "</span>";
                }
            }
            formatteddata = $"<div class='dataclass {col.Name}_class'>{formatteddata}</div>";
            return formatteddata;
        }

        public void DoConditionalFormating(ref string formattedVal, double value, EbDataRow row)
        {

            foreach (ColumnCondition cond in ConditionalFormating)
            {
                if (cond is AdvancedCondition)
                {
                    bool result = EvaluateExpression(row, ref globals, (cond as AdvancedCondition), value);
                    if ((cond as AdvancedCondition).RenderAS == AdvancedRenderType.Default)
                    {
                        if (result == (cond as AdvancedCondition).GetBoolValue())
                            formattedVal = "<div class='conditionformat' style='background-color:" + cond.BackGroundColor + ";color:" + cond.FontColor + ";'>" + value + "</div>";
                    }
                    else
                    {
                        if (result == (cond as AdvancedCondition).GetBoolValue())
                            formattedVal = "<i class='fa fa-check' aria-hidden='true'  style='color:green'></i>";
                        else
                            formattedVal = "<i class='fa fa-times' aria-hidden='true' style='color:red'></i>";
                    }
                }
                if (cond.CompareValues(value))
                {
                    formattedVal = "<div class='conditionformat' style='background-color:" + cond.BackGroundColor + ";color:" + cond.FontColor + ";'>" + value + "</div>";
                }
            }
        }

        public bool EvaluateExpression(EbDataRow _datarow, ref EbVisualizationGlobals globals, AdvancedCondition condition, double value)
        {
            foreach (FormulaPart formulaPart in condition.FormulaParts)
            {
                object __value = null;
                var __partType = _datarow.Table.Columns[formulaPart.FieldName].Type;
                if (__partType == EbDbTypes.Decimal || __partType == EbDbTypes.Int32)
                {
                    if (formulaPart.FieldName == this.DataColumns[0].Name)
                    {
                        __value = value;
                    }
                    else
                        __value = (_datarow[formulaPart.FieldName] != DBNull.Value) ? _datarow[formulaPart.FieldName] : 0;
                }
                else
                    __value = _datarow[formulaPart.FieldName];

                globals[formulaPart.TableName].Add(formulaPart.FieldName, new GNTV
                {
                    Name = formulaPart.FieldName,
                    Type = (GlobalDbType)__partType,
                    Value = __value
                });
            }
            return Convert.ToBoolean(condition.GetCodeAnalysisScript().RunAsync(globals).Result.ReturnValue);
        }

    }

    public class CalViewCol
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsWithin(DateTime dt)
        {
            return (dt >= this.StartDate && dt <= this.EndDate);
        }
    }

    public class DataStruct4CalViewCols
    {
        private Dictionary<CalViewCol, int> _innerDict = new Dictionary<CalViewCol, int>();

        public int GetColumnKey(DateTime dt)
        {
            foreach (CalViewCol col in _innerDict.Keys)
            {
                if (col.IsWithin(dt))
                    return _innerDict[col];
            }

            return -1;
        }

        public CalViewCol GetColumnByName(string name)
        {
            foreach (CalViewCol col in _innerDict.Keys)
            {
                if (col.Name == name)
                    return col;
            }

            return null;
        }

        public CalViewCol GetColumnByIndex(int index)
        {
            foreach (CalViewCol col in _innerDict.Keys)
            {
                if (_innerDict[col] == index)
                    return col;
            }

            return null;
        }

        public void Add(CalendarDynamicColumn _col)
        {
            if (_col != null)
                _innerDict.Add(new CalViewCol { Name = _col.Name, EndDate = _col.EndDT, StartDate = _col.StartDT }, _col.Data);
        }
    }

    public class DataStruct4CalViewRow
    {
        private Dictionary<int, Dictionary<string, double>> _innerDict = new Dictionary<int, Dictionary<string, double>>();

        public int GetCount()
        {
            return _innerDict.Count;
        }
        public Dictionary<int, Dictionary<string, double>>.KeyCollection GetKeys()
        {
            return _innerDict.Keys;
        }
        public double GetValue(int index, string name)
        {
            if (_innerDict.ContainsKey(index))
                return _innerDict[index][name];
            else return 0;
        }
        public void Add(int columnKey, string name, double value)
        {
            Dictionary<string, double> d = new Dictionary<string, double>();
            d.Add(name, 0);
            if (!_innerDict.ContainsKey(columnKey))
                _innerDict.Add(columnKey, d);
            if (!_innerDict[columnKey].ContainsKey(name))
                _innerDict[columnKey].Add(name, 0);
            _innerDict[columnKey][name] += value;
        }
    }
}
