using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum EbRadioValueType
    {
        Boolean,
        Integer,
        Text
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    public class EbRadioGroup : EbControl
    {
        public EbRadioGroup()
        {
            this.Options = new List<EbRadioOptionAbstract>();
            this.Options.Add(new EbRadioOption("Avegarge", "a"));
            this.Options.Add(new EbRadioOption("AboveAverage", "aa"));
            //this.Options.CollectionChanged += Options_CollectionChanged;
            this.ValueType = EbRadioValueType.Boolean;
        }

        private void Options_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (this.Options.Count == 3 && this.ValueType == EbRadioValueType.Boolean)
                    this.ValueType = EbRadioValueType.Integer;
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyGroup("Behavior")]
        public EbRadioValueType ValueType { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyGroup("Behavior")]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbRadioOptionAbstract> Options { get; set; }

        public override string GetDesignHtml()
        {
            //return @"<div class='btn-group' data-toggle='buttons'> <label class='btn btn-primary active'> <input type='radio' name='options' id='option1' autocomplete='off' checked> Radio 1 </label> <label class='btn btn-primary'> <input type='radio' name='options' id='option2' autocomplete='off'> Radio 2 </label> <label class='btn btn-primary'> <input type='radio' name='options' id='option3' autocomplete='off'> Radio 3 </label> </div>".RemoveCR().DoubleQuoted();
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {

            string html = @"
            <div class='Eb-ctrlContainer' Ctype='TableLayout'>
                <div>";

            foreach (EbControl ec in this.Options)
                html += ec.GetHtml();

            return html + "</div></div>";
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.Options.push(new EbObjects.EbRadioOption(id + '_Rd0'));
    this.Options.push(new EbObjects.EbRadioOption(id + '_Rd1'));
};";
        }
    }

    public class EbRadioOptionAbstract : EbControl
    {

    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    public class EbRadioOption : EbRadioOptionAbstract
    {
        new public string Label { get; set; }
        
        public string Value { get; set; }

        public EbRadioOption(string label, string val)
        {
            Label = label;
            Value = val;
        }

        public EbRadioOption(){ }

        public override string GetHtml()
        {
            return @"<input type ='radio' name='@name '> <span id='@nameLbl' style='@LabelBackColor @LabelForeColor '> @Label  </span>";
        }
    }
}
