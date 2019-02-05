using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    public class EbWebForm : EbControlContainer
    {
        [Browsable(false)]
        public bool IsUpdate { get; set; }

        public bool IsRenderMode { get; set; }

        public EbWebForm() { }

        public override int TableRowId { get; set; }


        public static EbOperations Operations = WFOperations.Instance;

        public override string GetHead()
        {
            string head = string.Empty;

            foreach (EbControl c in this.Controls)
                head += c.GetHead();

            return head;
        }

        public override string GetHtml()
        {
            string html = "<form id='@ebsid@' isrendermode='@rmode@' ebsid='@ebsid@' class='formB-box form-buider-form ebcont-ctrl' eb-form='true' ui-inp eb-type='WebForm' @tabindex@>";

            foreach (EbControl c in this.Controls)
                html += c.GetHtml();

            html += "</form>";

            return html
                .Replace("@name@", this.Name)
                .Replace("@ebsid@", this.EbSid)
                .Replace("@rmode@", IsRenderMode.ToString())
                .Replace("@tabindex@", IsRenderMode ? string.Empty : " tabindex='1'");
        }

        public string GetControlNames()
        {
            List<string> _lst = new List<string>();

            //foreach (EbControl _c in this.FlattenedControls)
            //{
            //    if (!(_c is EbControlContainer))
            //        _lst.Add(_c.Name);
            //}

            return string.Join(",", _lst.ToArray());
        }

        public string GetSelectQuery(WebFormSchema _schema = null, Service _service = null)
        {
            string query = string.Empty;
            string queryExt = string.Empty;
            if (_schema == null)
                _schema = this.GetWebFormSchema();
            foreach (TableSchema _table in _schema.Tables)
            {
                string _cols = string.Empty;
                string _id = "id";

                if (_table.Columns.Count > 0)
                {
                    _cols = String.Join(", ", _table.Columns.Select(x => x.ColumnName));
                    if (_table.TableName != _schema.MasterTable)
                        _id = _schema.MasterTable + "_id";
                    else
                        _cols = "eb_auto_id," + _cols;
                    query += string.Format("SELECT id, {0} FROM {1} WHERE {2} = :id AND eb_del='F';", _cols, _table.TableName, _id);

                    foreach(ColumnSchema Col in _table.Columns)
                    {
                        if (Col.Control.GetType().Equals(typeof(EbPowerSelect)))
                        {
                            queryExt += (Col.Control as EbPowerSelect).GetSelectQuery(_service, Col.ColumnName, _table.ParentTable, _id);
                        }
                    }
                }
            }
            foreach(Object Ctrl in _schema.ExtendedControls)
            {
                queryExt += (Ctrl as EbFileUploader).GetSelectQuery();
            }
            return query + queryExt;
        }

        public string GetDeleteQuery(WebFormSchema _schema = null)
        {
            string query = string.Empty;
            if (_schema == null)
                _schema = this.GetWebFormSchema();
            foreach (TableSchema _table in _schema.Tables)
            {
                string _id = "id";
                if (_table.TableName != _schema.MasterTable)
                    _id = _schema.MasterTable + "_id";
                query += string.Format("UPDATE {0} SET eb_del='T',eb_lastmodified_by = :eb_modified_by, eb_lastmodified_at = NOW() WHERE {1} = :id AND eb_del='F';", _table.TableName, _id);
            }
            return query;
        }


        public WebFormSchema GetWebFormSchema()
        {
            WebFormSchema _formSchema = new WebFormSchema();
            _formSchema.FormName = this.Name;
            _formSchema.MasterTable = this.TableName.ToLower();
            //_formSchema.Tables = new List<TableSchema>();
            _formSchema = GetWebFormSchemaRec(_formSchema, this, this.TableName.ToLower());
            return _formSchema;
        }

        private WebFormSchema GetWebFormSchemaRec(WebFormSchema _schema, EbControlContainer _container, string _parentTable)
        {
            IEnumerable<EbControl> _flatControls = _container.Controls.Get1stLvlControls();
            TableSchema _table = _schema.Tables.FirstOrDefault(tbl => tbl.TableName == _container.TableName);
            if (_table == null)
            {
                _table = new TableSchema { TableName = _container.TableName.ToLower(), ParentTable = _parentTable };
                _schema.Tables.Add(_table);
                
                //List<ColumnSchema> _columns = new List<ColumnSchema>();
                //foreach (EbControl control in _flatControls)
                //{
                //    if (control is EbAutoId)
                //        _columns.Add(new ColumnSchema { ColumnName = "eb_auto_id", EbDbType = (int)EbDbTypes.String, Control = control });
                //    else
                //        _columns.Add(new ColumnSchema { ColumnName = control.Name, EbDbType = (int)control.EbDbType, Control = control });
                //}
                //if (_columns.Count > 0)
                //    _schema.Tables.Add(new TableSchema { TableName = _container.TableName.ToLower(), Columns = _columns, ParentTable = _parentTable });
            }
            foreach (EbControl control in _flatControls)
            {
                if (control is EbFileUploader)
                    _schema.ExtendedControls.Add(control);
                else if (control is EbAutoId)
                    _table.Columns.Add(new ColumnSchema { ColumnName = "eb_auto_id", EbDbType = (int)EbDbTypes.String, Control = control });
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
                        Container.TableName = _container.TableName;
                    else
                        __parentTbl = _container.TableName;
                    _schema = GetWebFormSchemaRec(_schema, Container, __parentTbl);
                }
            }
            return _schema;
        }

        public void AfterRedisGet(Service service)
        {
            EbFormHelper.AfterRedisGet(this, service);
        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            EbFormHelper.AfterRedisGet(this, Redis, client);
        }

        public override string DiscoverRelatedRefids()
        {
            return EbFormHelper.DiscoverRelatedRefids(this);
        }
    }

    public static class EbFormHelper
    {
        public static string DiscoverRelatedRefids(EbControlContainer _this)
        {
            string refids = string.Empty;
            for (int i = 0; i < _this.Controls.Count; i++)
            {
                if (_this.Controls[i] is EbUserControl)
                {
                    refids += _this.Controls[i].RefId + ",";
                }
                else
                {
                    PropertyInfo[] _props = _this.Controls[i].GetType().GetProperties();
                    foreach (PropertyInfo _prop in _props)
                    {
                        if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                            refids += _prop.GetValue(_this.Controls[i], null).ToString() + ",";
                    }
                }
            }
            return refids;
        }

        public static void AfterRedisGet(EbControlContainer _this, RedisClient Redis, IServiceClient client)
        {
            try
            {
                for (int i = 0; i < _this.Controls.Count; i++)
                {
                    if (_this.Controls[i] is EbUserControl)
                    {
                        EbUserControl _temp = Redis.Get<EbUserControl>(_this.Controls[i].RefId);
                        if (_temp == null)
                        {
                            var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = _this.Controls[i].RefId });
                            _temp = EbSerializers.Json_Deserialize(result.Data[0].Json);
                            Redis.Set<EbUserControl>(_this.Controls[i].RefId, _temp);
                        }
                        //_temp.RefId = _this.Controls[i].RefId;
                        (_this.Controls[i] as EbUserControl).Controls = _temp.Controls;
                        foreach (EbControl Control in (_this.Controls[i] as EbUserControl).Controls)
                        {
                            RenameControlsRec(Control, _this.Controls[i].Name);
                            //Control.ChildOf = "EbUserControl";
                            //Control.Name = _this.Controls[i].Name + "_" + Control.Name;
                        }
                        //_this.Controls[i] = _temp;
                        _this.Controls[i].AfterRedisGet(Redis, client);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION : FormAfterRedisGet " + e.Message);
            }
        }

        public static void AfterRedisGet(EbControlContainer _this, Service service)
        {
            try
            {
                for (int i = 0; i < _this.Controls.Count; i++)
                {
                    if (_this.Controls[i] is EbUserControl)
                    {
                        EbUserControl _temp = service.Redis.Get<EbUserControl>(_this.Controls[i].RefId);
                        if (_temp == null)
                        {
                            var result = service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = _this.Controls[i].RefId });
                            _temp = EbSerializers.Json_Deserialize(result.Data[0].Json);
                            service.Redis.Set<EbUserControl>(_this.Controls[i].RefId, _temp);
                        }
                        //_temp.RefId = _this.Controls[i].RefId;
                        (_this.Controls[i] as EbUserControl).Controls = _temp.Controls;
                        foreach (EbControl Control in (_this.Controls[i] as EbUserControl).Controls)
                        {
                            RenameControlsRec(Control, _this.Controls[i].Name);
                            //Control.ChildOf = "EbUserControl";
                            //Control.Name = _this.Controls[i].Name + "_" + Control.Name;
                        }
                        //_this.Controls[i] = _temp;
                        (_this.Controls[i] as EbUserControl).AfterRedisGet(service);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION : EbFormAfterRedisGet(service) " + e.Message);
            }
        }

        private static void RenameControlsRec(EbControl _control, string _ucName)
        {
            if (_control is EbControlContainer)
            {
                if (!(_control is EbUserControl))
                {
                    foreach (EbControl _ctrl in (_control as EbControlContainer).Controls)
                    {
                        RenameControlsRec(_ctrl, _ucName);
                    }
                }
            }
            else
            {
                _control.ChildOf = "EbUserControl";
                _control.Name = _ucName + "_" + _control.Name;
                _control.EbSid = _ucName + "_" + _control.EbSid;
            }
        }
    }
}
