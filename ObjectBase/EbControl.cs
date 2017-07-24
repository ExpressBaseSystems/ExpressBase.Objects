using ExpressBase.Objects.ObjectBase;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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

#if NET462
    [System.Serializable]
#endif
    public class EbControl : EbObject
    {
        [Browsable(false)]
        internal object Parent { get; set; }

        [ProtoBuf.ProtoMember(10)]
        [Description("Labels")]
        [System.ComponentModel.Category("Behavior")]
        public virtual string Label { get; set; }

        [ProtoBuf.ProtoMember(11)]
        [System.ComponentModel.Category("Behavior")]
        [Description("Labels")]
        public virtual string HelpText { get; set; }

        [ProtoBuf.ProtoMember(12)]
        [System.ComponentModel.Category("Behavior")]
        [Description("Labels")]
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
        [EnableInBuilder(BuilderType.FormBuilder)]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        [System.ComponentModel.Category("Accessibility")]
        public virtual string BackColor { get; set; }

        [ProtoBuf.ProtoMember(30)]
        [EnableInBuilder(BuilderType.FormBuilder)]
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
