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

    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    public class EbRadioGroup : EbControl
    {
        public EbRadioGroup()
        {
            this.Options = new ObservableCollection<EbRadioOption>();
            this.Options.CollectionChanged += Options_CollectionChanged;
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

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public EbRadioValueType ValueType { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [System.ComponentModel.Category("Behavior")]
        public ObservableCollection<EbRadioOption> Options { get; set; }

        private string RadioCode
        {
            get
            {
                string rs = @"<div id='@namecontainer' class='Eb-ctrlContainer' style='position:absolute; left:@leftpx; top:@toppx; @hiddenString'>
                                <input id='@namehidden' type ='hidden' name='Ebradio'>
                                <span id='@nameLbl' style='@lblBackColor @LblForeColor'>@label</span>
                                <div data-toggle='tooltip' title='@toolTipText'>";
                if (this.Options.Count == 2)
                {
                    rs += @"
                        <input id='@name' type = 'checkbox' data-toggle = 'toggle' data-on='@OnValue' data-off='@OffValue'>
                        <script>$('#@name').change(function() {
                            if($(this).prop('checked')===true)
                                $('#@namehidden').val( '@OnValue' );
                            else
                                $('#@namehidden').val( '@OffValue' );
                        });</script>"
                    .Replace("@OnValue", this.Options[0].Label).Replace("@OffValue", this.Options[1].Label);
                }
                else
                {
                    rs += "<div class='btn-group' data-toggle='buttons'>";
                    for (int i = 0; i < this.Options.Count; i++)
                    {
                        rs += @"
                            <label id='r@idx' class='btn btn-primary'>
                                <input type ='radio' name='options' autocomplete='off'>
                                @option
                            </label>".Replace("@option", this.Options[i].Label).Replace("@idx",i.ToString());
                    }
                    rs += @"</div>  <script>
                                        $('#@namecontainer label').on('click', function () {
                                            $('#@namehidden').val( $(this).text().trim() );
                                        })
                                    </script>";
                }
                return rs + "</div><span class='helpText'> @helpText </span></div>";
            }
        }

        public override string GetHead()
        {
            return this.RequiredString + @"
$('#@idcontainer [type=checkbox]').bootstrapToggle();
$('#@idcontainer [type=radio]').on('click', function () {
    $(this).button('toggle');
})




".Replace("@id", this.Name );
        }

        public override string GetHtml()
        {
            return @"
            @RadioCode
"
.Replace("@RadioCode", this.RadioCode)
.Replace("@name", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@width", 30.ToString())//this.Width.ToString())
.Replace("@height", 20.ToString())// this.Height.ToString())
.Replace("@label", this.Label)
.Replace("@hiddenString", this.HiddenString)
.Replace("@toolTipText", this.ToolTipText)
.Replace("@helpText", this.HelpText)
//.Replace("@backColor", "background-color:" + this.BackColorSerialized + ";")
//.Replace("@foreColor", "color:" + this.ForeColorSerialized + ";")
//.Replace("@lblBackColor", "background-color:" + this.LabelBackColorSerialized + ";")
//.Replace("@LblForeColor", "color:" + this.LabelForeColorSerialized + ";")
;
        }
    }

    [ProtoContract]
#if NET462
    [Serializable]
#endif
    public class EbRadioOption
    {
        [ProtoMember(1)]
        public string Label { get; set; }

        [ProtoMember(2)]
        public string Value { get; set; }
    }
}
