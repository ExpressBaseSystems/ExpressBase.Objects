using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExpressBase.Objects
{
    public class EbToolbox
    {
        public string AllControlls { get; set; }

        public string AllMetas { get; set; }

        public string html { get; set; }

        EbToolbox() { }

        public EbToolbox(BuilderType _builderType)
        {
            string _toolsHtml = string.Empty;

            this.AllMetas = "AllMetas = {";

            this.AllControlls = "var EbObjects = {};";

            var types = this.GetType().GetTypeInfo().Assembly.GetTypes();

            foreach (var tool in types)
            {
                if (tool.GetTypeInfo().IsSubclassOf(typeof(EbControl)))
                {
                    if (tool.GetTypeInfo().IsDefined(typeof(EnableInBuilder))
                         && tool.GetTypeInfo().GetCustomAttribute<EnableInBuilder>().BuilderTypes.Contains(_builderType))
                    {
                        if (!tool.GetTypeInfo().IsDefined(typeof(HideInToolBox)))
                            _toolsHtml += GetToolHtml(tool.Name.Substring(2));
                        this.GetJsObject(tool, _builderType);
                    }
                }
            }

            this.AllMetas += "}";

            this.AllControlls += "";

            this.html = _toolsHtml;
        }

        public string getHead()
        {
            return this.AllControlls + this.AllMetas;
        }

        public void GetJsObject(System.Type tool, BuilderType _builderType)
        {
            string _props = string.Empty;

            var me = Activator.CreateInstance(tool);

            var props = me.GetType().GetProperties();

            List<Meta> MetaCollection = new List<Meta>();

            if (tool.GetTypeInfo().IsSubclassOf(typeof(EbControlContainer)))
            {
                _props += @"
this.IsContainer = true,
this.Controls = new EbControlCollection();";
                _props += (me as EbControlContainer).getAdditionalProps();
            }

            foreach (var prop in props)
            {
                var propattrs = prop.GetCustomAttributes();

                if (prop.IsDefined(typeof(EnableInBuilder))
                             && prop.GetCustomAttribute<EnableInBuilder>().BuilderTypes.Contains(_builderType))
                {
                    _props += JsVarDecl(prop);

                    var meta = new Meta { name = prop.Name };

                    foreach (Attribute attr in propattrs)
                    {
                        if (attr is PropertyGroup)
                            meta.group = (attr as PropertyGroup).Name;

                        //set corresponding editor
                        else if (attr is PropertyEditor)
                        {
                            meta.editor = (attr as PropertyEditor).PropertyEditorType;
                            if (prop.PropertyType.GetTypeInfo().IsEnum)
                                meta.options = Enum.GetNames(prop.PropertyType);
                        }
                    }

                    //if prop is of enum type set DD editor
                    if (prop.PropertyType.GetTypeInfo().IsEnum)
                    {
                        meta.editor = PropertyEditorType.DropDown;
                        meta.options = Enum.GetNames(prop.PropertyType);
                    }

                    //if prop is of premitive type set corresponding editor
                    if (!prop.IsDefined(typeof(PropertyEditor)) && !prop.PropertyType.GetTypeInfo().IsEnum)
                        meta.editor = GetTypeOf(prop);

                    MetaCollection.Add(meta);
                }

            }

            this.AllMetas += @"
'@Name'  : @MetaCollection,"
.Replace("@Name", tool.Name)
.Replace("@MetaCollection", JsonConvert.SerializeObject(MetaCollection));

            this.AllControlls += @"
EbObjects.@NameObj = function @NameObj(id) {
    this.$type = '@Type';
    this.Id = id;
    this.Name = id;@Props
};"
.Replace("@Name", tool.Name)
.Replace("@Type", me.GetType().FullName)
.Replace("@Props", _props);

        }

        private static PropertyEditorType GetTypeOf(PropertyInfo prop)
        {
            var typeName = prop.PropertyType.Name;

            if (typeName.Contains("Int") || typeName.Contains("Decimal") ||
                    typeName.Contains("Double") || typeName.Contains("Single"))
                return PropertyEditorType.Number;

            else if (typeName == "String")
                return PropertyEditorType.Text;

            else if (typeName == "Boolean")
                return PropertyEditorType.Boolean;

            return PropertyEditorType.Text;
        }

        private static string JsVarDecl(PropertyInfo prop)
        {
            string s = @"
    this.{0} = {1};";

            if (prop.PropertyType == typeof(string))
            {
                if (prop.Name.EndsWith("Color"))
                    return string.Format(s, prop.Name, "'#FFFFFF'");
                else
                    return string.Format(s, prop.Name, "''");
            }
            else if (prop.PropertyType == typeof(int))
                return string.Format(s, prop.Name, "0");

            else if (prop.PropertyType == typeof(bool))
                return string.Format(s, prop.Name, "false");

            else if (prop.PropertyType.GetTypeInfo().IsEnum)
                return string.Format(s, prop.Name, "'--select--'");

            else
                return string.Format(s, prop.Name, "null");
        }

        private static string GetToolHtml(string tool_name)
        {
            return @"<div eb-type='@toolName' class='well well-sm'>
                            @toolName
                    </div>".Replace("@toolName", tool_name);
        }
    }

    public class Meta
    {
        public string name { get; set; }

        public string group { get; set; }

        public PropertyEditorType editor { get; set; }

        public string[] options { get; set; }
    }

    public class HideInToolBox : Attribute { }
}
