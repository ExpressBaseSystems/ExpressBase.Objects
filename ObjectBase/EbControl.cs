using ExpressBase.Objects.Attributes;
using ExpressBase.Objects.ObjectBase;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(2000, typeof(EbControlContainer))]
    [ProtoBuf.ProtoInclude(2001, typeof(EbButton))]
    [ProtoBuf.ProtoInclude(2002, typeof(EbChart))]
    //[ProtoBuf.ProtoInclude(2003, typeof(EbDataGridViewColumn))]
    [ProtoBuf.ProtoInclude(2004, typeof(EbTextBox))]
    [ProtoBuf.ProtoInclude(2005, typeof(EbNumeric))]
    [ProtoBuf.ProtoInclude(2006, typeof(EbDate))]
    [ProtoBuf.ProtoInclude(2007, typeof(EbComboBox))]
    [ProtoBuf.ProtoInclude(2008, typeof(EbRadioGroup))]

    [JsonConverter(typeof(JsonSubtypes), "SubTypeType")]
    [JsonSubtypes.KnownSubType(typeof(EbNumeric), SubType.WithDecimalPlaces)]
    [JsonSubtypes.KnownSubType(typeof(EbTextBox), SubType.WithTextTransform)]
    [JsonSubtypes.KnownSubType(typeof(EbDate), SubType.WithEbDateType)]

    public class EbControl : EbObject
    {
        [Browsable(false)]
        internal object Parent { get; set; }

        //[EnableInBuilder(BuilderType.WebFormBuilder, BuilderType.FilterDialogBuilder)]
        //public override string Name { get; set; }

        [ProtoBuf.ProtoMember(10)]
        [Description("Labels")]
        [System.ComponentModel.Category("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        public virtual string Label { get; set; }

        [ProtoBuf.ProtoMember(11)]
        [System.ComponentModel.Category("Behavior")]
        [Description("Labels")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        public virtual string HelpText { get; set; }

        [ProtoBuf.ProtoMember(12)]
        [System.ComponentModel.Category("Behavior")]
        [Description("Labels")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        public virtual string ToolTipText { get; set; }

        [ProtoBuf.ProtoMember(13)]
        [Browsable(false)]
        public virtual int CellPositionRow { get; set; }

        [ProtoBuf.ProtoMember(14)]
        [Browsable(false)]
        public virtual int CellPositionColumn { get; set; }

        [ProtoBuf.ProtoMember(15)]
        [Browsable(false)]
        public virtual int Left { get; set; }

        [ProtoBuf.ProtoMember(16)]
        [Browsable(false)]
        public virtual int Top { get; set; }

        [ProtoBuf.ProtoMember(17)]
        [System.ComponentModel.Category("Layout")]
        public virtual int Height { get; set; }

        [ProtoBuf.ProtoMember(18)]
        [System.ComponentModel.Category("Layout")]
        public virtual int Width { get; set; }

        [ProtoBuf.ProtoMember(19)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool Required { get; set; }

        protected string RequiredString
        {
            get { return (this.Required ? "$('#{0}').focusout(function() { isRequired(this); }); $('#{0}Lbl').html( $('#{0}Lbl').text() + '<sup style=\"color: red\">*</sup>') ".Replace("{0}", this.Name) : string.Empty); }
        }

        [ProtoBuf.ProtoMember(20)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool Unique { get; set; }

        protected string UniqueString
        {
            get { return (this.Unique ? "$('#{0}').focusout(function() { isUnique(this); });".Replace("{0}", this.Name) : string.Empty); }
        }

        public static string AttachedLblAddingJS = @"
$('<div id=\'{0}AttaLbl\' class=\'attachedlabel atchdLblL\'>$</div>').insertBefore($('#{0}').parent()); $('#{0}').addClass('numinputL') 
$('#{0}AttaLbl').css({'padding':   ( $('#{0}').parent().height()/5 + 1) + 'px' });
$('#{0}AttaLbl').css({'font-size': ($('#{0}').css('font-size')) });
if( $('#{0}').css('font-size').replace('px','') < 10 )
    $('#{0}AttaLbl').css({'height':   ( $('#{0}').parent().height() - ( 10.5 - $('#{0}').css('font-size').replace('px','')) ) + 'px' });  
else
    $('#{0}AttaLbl').css({'height':   ( $('#{0}').parent().height()) + 'px' });  
";


        [ProtoBuf.ProtoMember(21)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool ReadOnly { get; set; }

        protected string ReadOnlyString
        {
            get { return (this.ReadOnly ? "background-color: #f0f0f0; border: solid 1px #bbb;' readonly" : "'"); }
        }

        [ProtoBuf.ProtoMember(22)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool Hidden { get; set; }

        protected string HiddenString
        {
            get { return (this.Hidden ? "visibility: hidden;" : string.Empty); }
        }

        [ProtoBuf.ProtoMember(23)]
        public virtual bool SkipPersist { get; set; }

        [ProtoBuf.ProtoMember(24)]
#if NET462
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
        public virtual string RequiredExpression { get; set; }

        [ProtoBuf.ProtoMember(25)]
        public virtual string UniqueExpression { get; set; }

        [ProtoBuf.ProtoMember(26)]
        public virtual string ReadOnlyExpression { get; set; }

        [ProtoBuf.ProtoMember(27)]
        public virtual string VisibleExpression { get; set; }

        [ProtoBuf.ProtoMember(28)]
        [System.ComponentModel.Category("Accessibility")]
        public virtual int TabIndex { get; set; }

        [ProtoBuf.ProtoMember(29)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        [System.ComponentModel.Category("Accessibility")]
        public virtual string BackColor { get; set; }

        [ProtoBuf.ProtoMember(30)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        [System.ComponentModel.Category("Accessibility")]
        public virtual string ForeColor { get; set; }

        [ProtoBuf.ProtoMember(31)]
        [System.ComponentModel.Category("Accessibility")]
        public virtual string LabelBackColor { get; set; }

        [ProtoBuf.ProtoMember(32)]
        [System.ComponentModel.Category("Accessibility")]
        public virtual string LabelForeColor { get; set; }

        public virtual string FontFamily { get; set; }

        public virtual float FontSize { get; set; }

        [ProtoBuf.ProtoMember(34)]
        public EbValidatorCollection Validators { get; set; }

        public EbControl() {
            this.Validators = new EbValidatorCollection();
        }

        public virtual string GetHead() { return string.Empty; }

        public virtual string GetHtml() { return string.Empty; }

        public override void Init4Redis(IRedisClient redisclient, IServiceClient serviceclient)
        {
            base.Redis = redisclient;
            base.ServiceStackClient = serviceclient;
        }

        public virtual void SetData(object value) { }

        public virtual object GetData() { return null; }

        public virtual string GetJsInitFunc() { return null; }

        public virtual string GetDesignHtml() { return "<div class='btn btn-default'> GetDesignHtml() not implemented </div>".RemoveCR().DoubleQuoted(); }

        protected string WrapWithDblQuotes(string input)
        {
            return "\"" + input + "\"";
        }

        public void GetJsObject(BuilderType _builderType, ref string MetaStr, ref string ControlsStr)
        {
            string _props = string.Empty;

            var props = this.GetType().GetAllProperties();

            List<Meta> MetaCollection = new List<Meta>();

            if (this is EbControlContainer)
                _props += @"this.IsContainer = true;";

            foreach (var prop in props)
            {
                var propattrs = prop.GetCustomAttributes();

                if (prop.IsDefined(typeof(EnableInBuilder))
                             && prop.GetCustomAttribute<EnableInBuilder>().BuilderTypes.Contains(_builderType))
                {
                    _props += JsVarDecl(prop);

                    var meta = new Meta { name = prop.Name};

                    foreach (Attribute attr in propattrs)
                    {
                        if (attr is PropertyGroup)
                            meta.group = (attr as PropertyGroup).Name;
                        else if (attr is HelpText)
                            meta.helpText =(attr as HelpText).value;

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

                    //if no helpText attribut is set, set - ""
                    if (!prop.IsDefined(typeof(HelpText)))
                        meta.helpText = "";

                    if (!prop.IsDefined(typeof(HideInPropertyGrid)))
                        MetaCollection.Add(meta);


                }

            }

            MetaStr += @"
'@Name'  : @MetaCollection,"
.Replace("@Name", this.GetType().Name)
.Replace("@MetaCollection", JsonConvert.SerializeObject(MetaCollection));

            ControlsStr += @"
EbObjects.@NameObj = function @NameObj(id) {
    this.$type = '@Type, ExpressBase.Objects';
    this.EbSid = id;
    @Props
    @InitFunc
    this.getHtml = function() { return  @html.replace(/@id/g, id); };
};"
.Replace("@Name", this.GetType().Name)
.Replace("@Type", this.GetType().FullName)
.Replace("@Props", _props)
.Replace("@InitFunc", this.GetJsInitFunc())
.Replace("@html", this.GetDesignHtml());

        }

        private string JsVarDecl(PropertyInfo prop)
        {
            string s = @"this.{0} = {1};";
            string _c = @"this.Controls = new EbControlCollection(JSON.parse('{0}'));";

            if (prop.PropertyType == typeof(string))
            {
                if (prop.Name.EndsWith("Color"))
                    return string.Format(s, prop.Name, "'#FFFFFF'");
                else
                    return string.Format(s, prop.Name, (prop.Name == "Name" || prop.Name == "EbSid") ? "id" : "''");
            }
            else if (prop.PropertyType == typeof(int))
                return string.Format(s, prop.Name, ((prop.Name == "Id") ? "id" : "0"));
            else if (prop.PropertyType == typeof(bool))
                return string.Format(s, prop.Name, "false");
            else if (prop.PropertyType.GetTypeInfo().IsEnum)
                return string.Format(s, prop.Name, "''");
            else
            {
                if (prop.Name == "Controls")
                    return string.Format(_c, JsonConvert.SerializeObject((this as EbControlContainer).Controls, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));

                return string.Format(s, prop.Name, "null");
            }
        }

        private PropertyEditorType GetTypeOf(PropertyInfo prop)
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

    }


    [ProtoBuf.ProtoContract]
    public class EbValidator
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public bool IsDisabled { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public string JScode { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public string FailureMSG { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class EbValidatorCollection : List<EbValidator>
    {

    }

    public enum SubType
    {
        WithDecimalPlaces,
        WithTextTransform,
        WithEbDateType
    }
}
