using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ServiceStack;
using System.Collections.Generic;
using System.Linq;

namespace ExpressBase.Objects.WebFormRelated
{
    static class SchemaHelper
    {
        internal static WebFormSchema GetWebFormSchema(EbWebForm _this)
        {
            WebFormSchema _formSchema = new WebFormSchema();
            _formSchema.FormName = _this.Name;
            _formSchema.MasterTable = _this.TableName.ToLower();
            _formSchema = GetWebFormSchemaRec(_this, _formSchema, _this, _this.TableName.ToLower());
            _this.FormSchema = _formSchema;
            return _formSchema;
        }

        private static WebFormSchema GetWebFormSchemaRec(EbWebForm _this, WebFormSchema _schema, EbControlContainer _container, string _parentTable)
        {
            IEnumerable<EbControl> _flatControls = _container.Controls.Get1stLvlControls();
            string curTbl = _container.TableName.ToLower();
            TableSchema _table = _schema.Tables.FirstOrDefault(tbl => tbl.TableName == curTbl);
            if (_table == null)
            {
                if (_container is EbApproval)
                    _table = new TableSchema { TableName = curTbl, ParentTable = _parentTable, TableType = WebFormTableTypes.Approval, Title = _container.Label, ContainerName = _container.Name };
                else if (_container is EbReview)
                {
                    _table = new TableSchema { TableName = curTbl, ParentTable = _parentTable, TableType = WebFormTableTypes.Review, Title = _container.Label, ContainerName = _container.Name };
                    _schema.ExtendedControls.Add(_container);
                }
                else if (_container is EbDataGrid)
                    _table = new TableSchema { TableName = curTbl, ParentTable = _parentTable, TableType = WebFormTableTypes.Grid, Title = _container.Label, ContainerName = _container.Name, IsDynamic = _container.IsDynamicTabChild, DescOdr = !(_container as EbDataGrid).AscendingOrder };
                else
                    _table = new TableSchema { TableName = curTbl, ParentTable = _parentTable, TableType = WebFormTableTypes.Normal, ContainerName = _container.Name };
                _schema.Tables.Add(_table);
            }
            foreach (EbControl control in _flatControls)
            {
                if (control is EbFileUploader)
                    _schema.ExtendedControls.Add(control);
                else if (control is EbProvisionUser)
                {
                    (control as EbProvisionUser).VirtualTable = curTbl;
                    int idx = _schema.ExtendedControls.FindIndex(e => e is EbProvisionLocation);
                    if (idx >= 0)
                        (control as EbProvisionUser).AddLocConstraint = true;
                    _schema.ExtendedControls.Add(control);
                }
                else if (control is EbProvisionLocation)
                {
                    (control as EbProvisionLocation).VirtualTable = curTbl;
                    foreach (object temp in _schema.ExtendedControls.FindAll(e => e is EbProvisionUser))
                        (temp as EbProvisionUser).AddLocConstraint = true;
                    _schema.ExtendedControls.Add(control);
                }
                else if (control is EbDGUserControlColumn)
                {
                    foreach (EbControl _ctrl in (control as EbDGUserControlColumn).Columns)
                    {
                        _table.Columns.Add(new ColumnSchema { ColumnName = _ctrl.Name, EbDbType = (int)_ctrl.EbDbType, Control = _ctrl });
                    }
                }
                else if (control is EbSysLocation && !control.IsDisable)
                {
                    _this.IsLocEditable = true;
                    _table.Columns.Add(new ColumnSchema { ColumnName = control.Name, EbDbType = (int)control.EbDbType, Control = control });
                }
                else
                    _table.Columns.Add(new ColumnSchema { ColumnName = control.Name, EbDbType = (int)control.EbDbType, Control = control });
            }

            foreach (EbControl _control in _container.Controls)
            {
                if (_control is EbControlContainer)
                {
                    EbControlContainer Container = _control as EbControlContainer;
                    string __parentTbl = _parentTable;
                    if (Container.TableName.IsNullOrEmpty())
                        Container.TableName = curTbl;
                    else
                        __parentTbl = curTbl;
                    _schema = GetWebFormSchemaRec(_this, _schema, Container, __parentTbl);
                }
            }
            return _schema;
        }
    }
}
