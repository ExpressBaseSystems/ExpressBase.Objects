using ExpressBase.Common;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    public class DataStruct4CalView
    {
        private string KeyColumnName { get; set; }

        private string ValueColumnName { get; set; }

        private string DateColumnName { get; set; }

        public DataStruct4CalViewCols Columns = new DataStruct4CalViewCols();

        private Dictionary<int, DataStruct4CalViewRow> _innerDict = new Dictionary<int, DataStruct4CalViewRow>();

        //keys
        //party1 or their id
        //party2
        //party3...
        public DataStruct4CalView(EbCalendarView C)
        {
            this.KeyColumnName = C.PrimaryKey.Name;
            this.DateColumnName = C.LinesColumns.FirstOrDefault(col => !col.IsCustomColumn && (col.Type == EbDbTypes.Date || col.Type == EbDbTypes.DateTime))?.Name;
            this.ValueColumnName = C.DataColumns.FirstOrDefault(col => col.bVisible)?.Name;
        }
        public void Add(EbDataRow row)
        {
            if (!_innerDict.ContainsKey(Convert.ToInt32(row[this.KeyColumnName])))
                _innerDict.Add(Convert.ToInt32(row[this.KeyColumnName]), new DataStruct4CalViewRow());

            _innerDict[Convert.ToInt32(row[this.KeyColumnName])]
                .Add(Columns.GetColumnKey(Convert.ToDateTime(row[this.DateColumnName])), Convert.ToInt64(row[this.ValueColumnName]));
        }

        public void GetFormatedTable(ref EbDataTable _formattedTable, RowColletion masterRows, ref Dictionary<int, List<object>> summary)
        {
            for (int i = 0; i < masterRows.Count; i++)//filling master data
            {
                _formattedTable.Rows.Add(_formattedTable.NewDataRow2());
                for (int j = 0; j < masterRows[0].Count; j++)
                {
                    _formattedTable.Rows[i][j] = masterRows[i][j];
                }
            }
            int summary_last_index = summary.Keys.Last();
            for (int i = 0; i < _formattedTable.Rows.Count; i++)//filling consolidated data
            {
                EbDataRow Row = _formattedTable.Rows[i];
                int _id = (int)Row[KeyColumnName];
                if (_innerDict.ContainsKey(_id))
                {
                    IEnumerable<int> keys = _innerDict[_id].GetKeys();
                    foreach (int key in keys)
                    {
                        long value = _innerDict[_id].GetValue(key);
                        Row[key] = value;
                        Row["Total"] = (long)(Row["Total"] ?? 0L) + value;
                        summary[key][0] = (long)summary[key][0] + value;
                        summary[summary_last_index][0] = (long)summary[summary_last_index][0] + value;
                    }
                }
            }
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
        public IEnumerable<int> GetKeys()
        {
            IEnumerable<int> keys = _innerDict.Select(x => x.Key);
            return keys;
        }
        public long GetValue(int index)
        {
            return _innerDict[index];
        }
        public void Add(int columnKey, long value)
        {
            if (!_innerDict.ContainsKey(columnKey))
                _innerDict.Add(columnKey, 0);
            _innerDict[columnKey] += value;
        }
    }
}
