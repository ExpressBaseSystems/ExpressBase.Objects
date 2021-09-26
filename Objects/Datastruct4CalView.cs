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

namespace ExpressBase.Objects.Objects
{
    public class DataStruct4CalView
    {
        private string KeyColumnName { get; set; }

        private string ValueColumnName { get; set; }

        private string DateColumnName { get; set; }

        public DataStruct4CalViewCols Columns = new DataStruct4CalViewCols();

        private Dictionary<int, DataStruct4CalViewRow> _innerDict = new Dictionary<int, DataStruct4CalViewRow>();//dict<partyid, dict<int column_index, long value>

        public List<ColumnCondition> ConditionalFormating { get; set; }

        public List<ObjectBasicInfo> ObjectLinks { get; set; }

        Globals globals = new Globals();

        public int InitialColumnsCount { get; set; }

        public AttendanceType CalendarType { get; set; }

        public bool ShowGrowthPercentage { get; set; }

        public DataStruct4CalView(EbCalendarView C)
        {
            this.KeyColumnName = C.PrimaryKey.Name;
            this.DateColumnName = C.LinesColumns.FirstOrDefault(col => !col.IsCustomColumn && (col.Type == EbDbTypes.Date || col.Type == EbDbTypes.DateTime))?.Name;
            this.ValueColumnName = C.DataColumns.FirstOrDefault(col => col.bVisible)?.Name;
            this.ConditionalFormating = C.DataColumns.FirstOrDefault(col => col.bVisible)?.ConditionalFormating;
            this.ObjectLinks = C.ObjectLinks;
            this.CalendarType = C.CalendarType;
            this.ShowGrowthPercentage = C.ShowGrowthPercentage;
        }

        public void Add(EbDataRow row)
        {
            if (!_innerDict.ContainsKey(Convert.ToInt32(row[this.KeyColumnName])))
                _innerDict.Add(Convert.ToInt32(row[this.KeyColumnName]), new DataStruct4CalViewRow());
            DateTime dt = Convert.ToDateTime(row[this.DateColumnName]);
            if (dt > DateTime.MinValue)
            {
                _innerDict[Convert.ToInt32(row[this.KeyColumnName])]
                    .Add(Columns.GetColumnKey(dt), Convert.ToInt64(row[this.ValueColumnName]));
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
            long value;
            long prev_value;
            int summary_last_index = summary.Keys.Last();
            for (int i = 0; i < _formattedTable.Rows.Count; i++)//filling consolidated data
            {
                Row = _formattedTable.Rows[i];
                int _id = (int)Row[KeyColumnName];
                if (_innerDict.ContainsKey(_id))
                {
                    foreach (int columnIndex in _innerDict[_id].GetKeys())
                    {
                        prev_value = 0;
                        value = _innerDict[_id].GetValue(columnIndex);
                        if (ShowGrowthPercentage && CalendarType != AttendanceType.DayWise)
                        {
                            if (columnIndex > this.InitialColumnsCount && Columns.GetColumnByIndex(columnIndex).StartDate > Columns.GetColumnByIndex(columnIndex - 1).StartDate)
                                prev_value = _innerDict[_id].GetValue(columnIndex - 1);
                        }

                        Row[columnIndex] = GetFormattedValue(value, Row, columnIndex, prev_value, _user_culture);
                        Row["Total"] = (Convert.ToDecimal(Row["Total"] ?? 0L) + Convert.ToDecimal(value)).ToString("N", _user_culture.NumberFormat);
                        summary[columnIndex][0] = (Convert.ToDecimal(summary[columnIndex][0]) + Convert.ToDecimal(value)).ToString("N", _user_culture.NumberFormat);
                        summary[summary_last_index][0] = (Convert.ToDecimal(summary[summary_last_index][0]) + Convert.ToDecimal(value)).ToString("N", _user_culture.NumberFormat); ;
                    }
                }
            }
        }

        private string GetFormattedValue(long value, EbDataRow row, int columnIndex, long prev_value, CultureInfo _user_culture)
        {
            string formattedVal = (Convert.ToDecimal(value) == 0) ? string.Empty : Convert.ToDecimal(value).ToString("N", _user_culture.NumberFormat);
            //string formattedVal = string.Empty;
            if (ConditionalFormating.Count > 0)
            {
                DoConditionalFormating(ref formattedVal, value, row);
            }
            else
            {
                if (this.ObjectLinks.Count == 1)
                {
                    formattedVal = "<a href ='#' class ='tablelink4calendar cal-data' idx='" + columnIndex + "'>" + formattedVal + "</a>";
                }
                else
                {
                    formattedVal = "<span clas ='cal-data'>" + formattedVal + "</span>";
                }

                if (ShowGrowthPercentage && CalendarType != AttendanceType.DayWise && prev_value > 0 && value > 0)
                {
                    long percent = ((value - prev_value) * 100) / value;
                    string color, direction;
                    if (percent < 0)
                    {
                        color = "red";
                        direction = "down";
                    }
                    else
                    {
                        color = "green";
                        direction = "up";
                    }
                    var val = (percent == 0) ? "&nbsp;" : Math.Abs(percent).ToString() + "%";
                    formattedVal += @"  <span class='per-cont " + color + "'>" +
                                            "<i class='fa fa-caret-" + direction + "'></i>" +
                                            "<div class='val-perc'>" + val + "</div>" +
                                        "</span>";

                    // formattedVal += "<span class='cal-percent-container'><span class ='cal-percent-caret-green'><i class='fa fa-caret-up'></i></span><span class ='cal-percent cal-percent-green'>" + Math.Abs(percent) + "%" + "</span></span>";
                }
            }
            return formattedVal;
        }

        public void DoConditionalFormating(ref string formattedVal, long value, EbDataRow row)
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

        public bool EvaluateExpression(EbDataRow _datarow, ref Globals globals, AdvancedCondition condition, long value)
        {
            foreach (FormulaPart formulaPart in condition.FormulaParts)
            {
                object __value = null;
                var __partType = _datarow.Table.Columns[formulaPart.FieldName].Type;
                if (__partType == EbDbTypes.Decimal || __partType == EbDbTypes.Int32)
                {
                    if (formulaPart.FieldName == this.ValueColumnName)
                    {
                        __value = value;
                    }
                    else
                        __value = (_datarow[formulaPart.FieldName] != DBNull.Value) ? _datarow[formulaPart.FieldName] : 0;
                }
                else
                    __value = _datarow[formulaPart.FieldName];

                globals[formulaPart.TableName].Add(formulaPart.FieldName, new NTV
                {
                    Name = formulaPart.FieldName,
                    Type = __partType,
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
        private Dictionary<int, long> _innerDict = new Dictionary<int, long>();

        public int GetCount()
        {
            return _innerDict.Count;
        }
        public Dictionary<int, long>.KeyCollection GetKeys()
        {
            return _innerDict.Keys;
        }
        public long GetValue(int index)
        {
            if (_innerDict.ContainsKey(index))
                return _innerDict[index];
            else return 0;
        }
        public void Add(int columnKey, long value)
        {
            if (!_innerDict.ContainsKey(columnKey))
                _innerDict.Add(columnKey, 0);
            _innerDict[columnKey] += value;
        }
    }
}
