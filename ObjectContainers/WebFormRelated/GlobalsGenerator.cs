using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects;
using System;
using System.Collections.Generic;

namespace ExpressBase.Objects.WebFormRelated
{
    public static class GlobalsGenerator
    {
        public static FormAsGlobal GetFormAsFlatGlobal(EbWebForm _this, WebformData _formdata)
        {
            Dictionary<string, string> grid = new Dictionary<string, string>();
            EbControl[] Allctrls = _this.Controls.FlattenAllEbControls();
            for (int i = 0; i < Allctrls.Length; i++)
            {
                if (Allctrls[i] is EbDataGrid)
                {
                    grid.Add((Allctrls[i] as EbDataGrid).TableName, (Allctrls[i] as EbDataGrid).Name);
                }
            }

            FormAsGlobal _globals = new FormAsGlobal { Name = _this.Name };
            ListNTV listNTV = new ListNTV();
            try
            {
                foreach (KeyValuePair<string, SingleTable> item in _formdata.MultipleTables)
                {
                    if (grid.ContainsKey(item.Key))
                    {
                        FormAsGlobal _grid = new FormAsGlobal { Name = grid[item.Key] };
                        for (int j = 0; j < item.Value.Count; j++)
                        {
                            ListNTV _gridline = new ListNTV();
                            foreach (SingleColumn col in item.Value[j].Columns)
                            {
                                if (col.Name != "id" && col.Name != "eb_row_num")
                                {
                                    NTV n = GetNtvFromFormData(_formdata, item.Key, j, col.Name);
                                    if (n != null)
                                        _gridline.Columns.Add(n);
                                }
                            }
                            _grid.Add(_gridline);
                        }
                        _globals.AddContainer(_grid);
                    }
                    else
                    {
                        foreach (SingleColumn col in item.Value[0].Columns)
                        {
                            if (!(col.Name == "id" && item.Key != _formdata.MasterTable) && item.Value.Count == 1)
                            {
                                NTV n = GetNtvFromFormData(_formdata, item.Key, 0, col.Name);
                                if (n != null)
                                    listNTV.Columns.Add(n);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in GetFormAsFlatGlobal. Message : " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            _globals.Add(listNTV);
            return _globals;
        }

        public static FG_WebForm GetCSharpFormGlobals(EbWebForm _this, WebformData _formdata)
        {
            FG_WebForm fG_WebForm = new FG_WebForm();
            GetCSharpFormGlobalsRec(fG_WebForm, _this, _formdata);
            return fG_WebForm;
        }

        private static void GetCSharpFormGlobalsRec(FG_WebForm fG_WebForm, EbControlContainer _container, WebformData _formdata)
        {
            SingleTable Table = _formdata.MultipleTables.ContainsKey(_container.TableName) ? _formdata.MultipleTables[_container.TableName] : new SingleTable();
            if (_container is EbDataGrid)
            {
                FG_DataGrid fG_DataGrid = new FG_DataGrid(_container as EbDataGrid, Table);
                fG_WebForm.DataGrids.Add(fG_DataGrid);
            }
            else if (_container is EbReview)
            {
                fG_WebForm.Review = new FG_Review(_container as EbReview, Table);
            }
            else
            {
                foreach (EbControl _control in _container.Controls)
                {
                    if (_control is EbControlContainer)
                    {
                        GetCSharpFormGlobalsRec(fG_WebForm, _control as EbControlContainer, _formdata);
                    }
                    else
                    {
                        object data = null;
                        if (_formdata.MultipleTables.ContainsKey(_container.TableName) && _formdata.MultipleTables[_container.TableName].Count > 0)
                            data = _formdata.MultipleTables[_container.TableName][0][_control.Name];
                        fG_WebForm.FlatCtrls.Controls.Add(new FG_Control(_control, data));
                    }
                }
            }
            
        }

        //get formdata as globals for c# script engine
        public static FormAsGlobal GetFormAsGlobal(EbWebForm _this, WebformData _formData, EbControlContainer _container = null, FormAsGlobal _globals = null)
        {
            if (_container == null)
                _container = _this;
            if (_globals == null)
                _globals = new FormAsGlobal { Name = _this.Name };

            ListNTV listNTV = new ListNTV();

            if (_formData.MultipleTables.ContainsKey(_container.TableName))
            {
                for (int i = 0; i < _formData.MultipleTables[_container.TableName].Count; i++)
                {
                    foreach (EbControl control in _container.Controls)
                    {
                        if (control is EbControlContainer)
                        {
                            FormAsGlobal g = new FormAsGlobal();
                            g.Name = (control as EbControlContainer).Name;
                            _globals.AddContainer(g);
                            g = GetFormAsGlobal(_this, _formData, control as EbControlContainer, g);
                        }
                        else
                        {
                            NTV n = GetNtvFromFormData(_formData, _container.TableName, i, control.Name);
                            if (n != null)
                                listNTV.Columns.Add(n);
                        }
                    }
                }
                _globals.Add(listNTV);
            }
            return _globals;
        }

        private static NTV GetNtvFromFormData(WebformData _formData, string _table, int _row, string _column)
        {
            NTV ntv = null;
            if (_formData.MultipleTables.ContainsKey(_table))
            {
                foreach (SingleColumn col in _formData.MultipleTables[_table][_row].Columns)
                {
                    if (col.Name.Equals(_column))
                    {
                        ntv = new NTV()
                        {
                            Name = _column,
                            Type = (EbDbTypes)col.Type,
                            Value = col.Value
                        };
                        break;
                    }
                }
            }
            return ntv;
        }

    }
}
