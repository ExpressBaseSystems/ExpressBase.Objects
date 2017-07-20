using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace ExpressBase.Objects
{
    public enum TextTransform
    {
        Normal,
        LowerCase,
        UpperCase,
    }

    public enum TextMode
    {
        SingleLine,
        Email,
        Password,
        Color
    }

    [ProtoBuf.ProtoContract]
    public class EbTextBox : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        [EnableInBuilder(BuilderType.FormBuilder, BuilderType.FilterDialogBuilder)]
        [PropertyGroup("Behavior")]
        [PropertyEditor(PropertyEditorType.Number)]
        public int MaxLength { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [EnableInBuilder(BuilderType.FormBuilder)]
        [PropertyGroup("Behavior")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public TextTransform TextTransform { get; set; }

        [ProtoBuf.ProtoMember(3)]
        [EnableInBuilder(BuilderType.FormBuilder)]
        [PropertyGroup("Behavior")]
        public TextMode TextMode { get; set; }

        [ProtoBuf.ProtoMember(4)]
        [EnableInBuilder(BuilderType.FormBuilder)]
        [PropertyGroup("Behavior")]
        public string PlaceHolder { get; set; }

        [ProtoBuf.ProtoMember(5)]
        [EnableInBuilder(BuilderType.FormBuilder)]
        [PropertyGroup("Appearance")]
        public string Text { get; set; }

        [ProtoBuf.ProtoMember(6)]
        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.FormBuilder)]
        public bool AutoCompleteOff { get; set; }

        [ProtoBuf.ProtoMember(7)]
        [PropertyGroup("Behavior")]
        public string MaxDateExpression { get; set; }

        [ProtoBuf.ProtoMember(8)]
        [PropertyGroup("Behavior")]
        public string MinDateExpression { get; set; }

        //[ProtoBuf.ProtoMember(9)]
        //[Description("Identity")]
        //public override string Name { get; set; }

        //[ProtoBuf.ProtoMember(10)]
        //[Description("Identity")]
        //public override string Label { get; set; }

        public EbTextBox() { }

        public EbTextBox(object parent)
        {
            this.Parent = parent;
        }

        public override string GetHead()
        {
            return (((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) + @"".Replace("{0}", this.Name));
        }

        private string TextTransformString
        {
            get { return (((int)this.TextTransform > 0) ? "$('#{0}').keydown(function(event) { textTransform(this, {1}); }); $('#{0}').on('paste', function(event) { textTransform(this, {1}); });".Replace("{0}", this.Name).Replace("{1}", ((int)this.TextTransform).ToString()) : string.Empty); }
        }


        public override void SetData(object value)
        {
            this.Text = (value != null) ? value.ToString() : string.Empty;
        }

        public override object GetData()
        {
            return this.Text;
        }


        public static string test = JsObject(BuilderType.FormBuilder);

        public override string GetHtml()
        {
            return @"
<div class='Eb-ctrlContainer' style='@hiddenString'>
    <span id='@nameLbl' style='@lblBackColor @LblForeColor'> @label </span>
        <div  class='input-group' style='width: 100%;'>
            <span class='input-group-addon'><i class='fa fa-envelope' aria-hidden='true' class='input-group-addon'></i> @attachedLbl </span>
            <input type='@textMode'  id='@name' name='@name' autocomplete = '@autoComplete' data-toggle='tooltip' title='@toolTipText' @tabIndex @maxLength style='width:@width; height:@heightpx; @backColor @foreColor display:inline-block; @fontStyle @readOnlyString @required @placeHolder @text @tabIndex  />
        </div>
    <span class='helpText'> @helpText </span>
</div>"
.Replace("@name", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@width", "100%")
.Replace("@height", this.Height.ToString())
.Replace("@label", this.Label)
.Replace("@maxLength", (this.MaxLength > 0) ? string.Format("maxlength='{0}'", this.MaxLength) : string.Empty)
.Replace("@textMode", this.TextMode.ToString().ToLower())
.Replace("@hiddenString", this.HiddenString)
.Replace("@required", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString", this.ReadOnlyString)
.Replace("@toolTipText", this.ToolTipText)
.Replace("@helpText", this.HelpText)
.Replace("@placeHolder", "placeholder='" + this.PlaceHolder + "'")
.Replace("@text", "value='" + this.Text + "'")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@autoComplete", (this.AutoCompleteOff || this.TextMode.ToString().ToLower() == "password") ? "off" : "on")
.Replace("@backColor", "background-color:" + this.BackColor + ";")
.Replace("@foreColor", "color:" + this.ForeColor + ";")
.Replace("@lblBackColor", "background-color:" + this.LabelBackColor + ";")
.Replace("@LblForeColor", "color:" + this.LabelForeColor + ";" + test);

        }


        public static string JsObject(BuilderType _builderType)
        {
            string _props = string.Empty;
            var me = new EbTextBox();

            var props = me.GetType().GetProperties();

            List<Meta> MetaCollection = new List<Meta>();

            foreach (var prop in props)
            {
                var propattrs = prop.GetCustomAttributes();

                if (prop.IsDefined(typeof(EnableInBuilder))
                     && prop.GetCustomAttribute<EnableInBuilder>().BuilderTypes.Contains(_builderType))
                {
                    _props += JsVarDecl(prop);

                    var meta = new Meta { name = prop.Name };
                    //Type = (prop.PropertyType.GetTypeInfo().IsEnum) ?  "select" : (prop.PropertyType.Name).Contains("Int") ? "number" : prop.PropertyType.Name

                    foreach (Attribute attr in propattrs)
                    {
                        if (attr is PropertyGroup)
                            meta.group = (attr as PropertyGroup).Name;
                        else if (attr is PropertyEditor)
                            meta.editor = (attr as PropertyEditor).PropertyEditorType;
                    }


                    if (!prop.IsDefined(typeof(PropertyEditor)) && !prop.PropertyType.GetTypeInfo().IsEnum)
                        meta.editor = GetTypeOf(prop);

                    MetaCollection.Add(meta);
                }
            }

            return @"
var TextBoxObj = function (id) {
    this.$type = '@Type';
    this.Id = id;
    this.Name = id;@Props
    this.Meta=@meta
};"
.Replace("@Type", me.GetType().FullName)
.Replace("@Props", _props)
.Replace("@meta", JsonConvert.SerializeObject(MetaCollection));
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

        private static PropertyEditorType GetTypeOf(PropertyInfo prop)
        {
            var typeName = prop.PropertyType.Name;

            if (typeName.Contains("Int") || typeName.Contains("Decimal") ||
                    typeName.Contains("Double") || typeName.Contains("Single"))
                return PropertyEditorType.Number;

            else if (typeName == "String")
                return PropertyEditorType.Text;

            else if (typeName == "Boolean")
                return PropertyEditorType.boolean;

            return PropertyEditorType.Text;
        }
    }

    public enum BuilderType
    {
        DisplayBlockBuilder,
        FilterDialogBuilder,
        FormBuilder,
        ReportBuilder,
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class EnableInBuilder : Attribute
    {
        public BuilderType[] BuilderTypes { get; set; }

        public EnableInBuilder(params BuilderType[] types)
        {
            this.BuilderTypes = types;
        }
    }

    public enum PropertyEditorType
    {
        DropDown,
        Collection,
        Columns,
        Color,
        Number,
        Text,
        boolean
    }

    public class PropertyEditor : Attribute
    {
        public PropertyEditorType PropertyEditorType { get; set; }

        public PropertyEditor(PropertyEditorType type)
        {
            this.PropertyEditorType = type;
        }
    }

    public class PropertyGroup : Attribute
    {
        public string Name { get; set; }

        public PropertyGroup(string groupName)
        {
            this.Name = groupName;
        }
    }

    public class Meta
    {
        public string name { get; set; }

        public string group { get; set; }

        //public string Type { get; set; }

        public PropertyEditorType editor { get; set; }
    }
}
