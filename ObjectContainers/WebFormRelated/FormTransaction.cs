﻿using System.Collections.Generic;

namespace ExpressBase.Objects
{
    public class FormTransaction
    {
        public string CreatedBy { get; set; }

        public string CreatedById { get; set; }

        public string CreatedAt { get; set; }

        public string ActionType { get; set; }

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
        public Dictionary<int, FormTransactionRow> NewRows { get; set; }//Key = Row id

        public Dictionary<int, FormTransactionRow> EditedRows { get; set; }//Key = Row id

        public Dictionary<int, FormTransactionRow> DeletedRows { get; set; }//Key = Row id

        public List<FormTransactionMetaInfo> ColumnMeta { get; set; }

        public string Title { get; set; }

        public FormTransactionTable()
        {
            this.NewRows = new Dictionary<int, FormTransactionRow>();
            this.EditedRows = new Dictionary<int, FormTransactionRow>();
            this.DeletedRows = new Dictionary<int, FormTransactionRow>();
            this.ColumnMeta = new List<FormTransactionMetaInfo>();
        }
    }

    public class FormTransactionRow
    {
        public Dictionary<string, FormTransactionEntry> Columns { get; set; }//Key = Column name

        public FormTransactionRow()
        {
            this.Columns = new Dictionary<string, FormTransactionEntry>();
        }
    }

    public class FormTransactionEntry
    {
        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string Title { get; set; }

        public bool IsModified { get; set; }

        public bool IsNumeric { get; set; }
    }

    public class FormTransactionMetaInfo
    {
        public int Index { get; set; }

        public string Title { get; set; }

        public bool IsNumeric { get; set; }
    }
}
