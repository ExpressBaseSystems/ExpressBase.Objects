using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

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
        [EnableInBuilder(BuilderType.FormBuilder)]
        [EnableInBuilder(BuilderType.FilterDialogBuilder)]
        [PropertyGroup("Behavior")]
        public int MaxLength { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [PropertyGroup("Behavior")]
        public TextTransform TextTransform { get; set; }

        [ProtoBuf.ProtoMember(3)]
        [PropertyGroup("Behavior")]
        public TextMode TextMode { get; set; }

        [ProtoBuf.ProtoMember(4)]
        [PropertyGroup("Behavior")]
        public string PlaceHolder { get; set; }

        [ProtoBuf.ProtoMember(5)]
        [PropertyGroup("Appearance")]
        public string Text { get; set; }

        [ProtoBuf.ProtoMember(6)]
        [PropertyGroup("Behavior")]
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
.Replace("@LblForeColor", "color:" + this.LabelForeColor + "; js string = " +JsObject);

        }

        public static string JsObject
        {
            get
            {
                var me = new EbTextBox();

                string s = string.Format(@"var TextBoxObj = function (id) {
                                this.$type = '{0}',
                                this.Id = id,
                                this.Name = id,", me.GetType().FullName);

                var props = me.GetType().GetProperties();

                foreach (var prop in props)
                {
                    s += JsVarDecl(prop);

                    //foreach (Attribute attr in prop.GetCustomAttributes())
                    //{
                    //    if (attr is EnableInBuilder)
                    //    {
                    //        if ((attr as EnableInBuilder).BuilderType == BuilderType.FormBuilder)
                    //        {

                    //        }
                    //    }
                    //}
                }
                System.Console.WriteLine(s);
                return s;
            }
        }

        private static string JsVarDecl(PropertyInfo prop)
        {
            string s = "this.{0} = {1},";

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
                return string.Format(s, prop.Name, "--select--");

            else 
                return string.Format(s, prop.Name, "null");
        }
    }

    public enum BuilderType
    {
        DisplayBlockBuilder,
        FilterDialogBuilder,
        FormBuilder,
        ReportBuilder,
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class EnableInBuilder : Attribute
    {
        public BuilderType BuilderType { get; set; }

        public EnableInBuilder(BuilderType type)
        {
            this.BuilderType = type;
        }
    }

    public enum PropertyEditorType
    {
        DropDown,
        Collection,
        Columns,
        Color
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
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
}
