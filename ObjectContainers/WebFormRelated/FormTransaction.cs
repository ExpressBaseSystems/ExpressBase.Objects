using System.Collections.Generic;

namespace ExpressBase.Objects
{
    public class FormTransaction
    {
        public string CreatedBy { get; set; }

        public string CreatedById { get; set; }

        public string CreatedAt { get; set; }

        public string ActionType { get; set; }

        public bool MissingEntry { get; set; }

        public Dictionary<string, FormTransactionRow> Tables { get; set; }//Key = Table name

        public Dictionary<string, FormTransactionTable> GridTables { get; set; }//Key = Table name

        public FormTransaction()
        {
            this.Tables = new Dictionary<string, FormTransactionRow>();
            this.GridTables = new Dictionary<string, FormTransactionTable>();
        }
    }

    public class FormTransactionTable
    {
        public Dictionary<int, FormTransactionRow> Rows { get; set; }//Key = Row id

        public Dictionary<int, string> ColumnMeta { get; set; }

        public string Title { get; set; }

        public FormTransactionTable()
        {
            this.Rows = new Dictionary<int, FormTransactionRow>();
            this.ColumnMeta = new Dictionary<int, string>();
        }
    }

    public class FormTransactionRow
    {
        public Dictionary<string, FormTransactionEntry> Columns { get; set; }//Key = Column name

        public FormTransactionRow()
        {
            this.Columns = new Dictionary<string, FormTransactionEntry>();
        }

        public bool IsRowModified
        {
            get
            {
                foreach (KeyValuePair<string, FormTransactionEntry> col in this.Columns)
                {
                    if (col.Value.IsModified)
                        return true;
                }
                return false;
            }
        }
    }

    public class FormTransactionEntry
    {
        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string Title { get; set; }

        public bool IsModified { get; set; }
    }
}
