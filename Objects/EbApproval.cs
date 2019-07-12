using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Helpers;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    public enum KuSApproverRole
    {
        NHG_President = 0,
        ADS_Committee = 1,
        CDS_Committee = 2
    }

    [EnableInBuilder(BuilderType.WebForm)]
    public class EbApproval : EbControlUI
    {
        public EbApproval()
        {
            FormStages = new List<EbFormStage>();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            //this.EbDbType = this.EbDbType;
        }

        [EnableInBuilder(BuilderType.WebForm)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("200")]
        [PropertyGroup("Appearance")]
        public override int Height { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("true")]
        [PropertyGroup("Behavior")]
        [Alias("Serial numbered")]
        public bool IsShowSerialNumber { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Data")]
        [HelpText("Name Of database-table Which you want to store Data collected using this Form")]
        [InputMask("[a-z][a-z0-9]*(_[a-z0-9]+)*")]
        public virtual string TableName { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Behavior")]
        [ListType(typeof(EbFormStage))]
        [PropertyPriority(99)]
        public List<EbFormStage> FormStages { get; set; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-stack-exchange'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }


        public override string GetBareHtml()
        {
            int SlNo = 1;
            string html = @"
<div id='cont_@ebsid@' class='fs-grid-cont'>
    <table id='tbl_@ebsid@' class='table table-bordered fs-tbl' style='height:@_height@px;'>
        <thead>
            <tr>
            <th class='slno' style='width:50px'><span class='grid-col-title'>SL No</span></th>
            <th class='grid-col-title'><span class='grid-col-title'>Stage</span></th>
            <th class='grid-col-title'><span class='grid-col-title'>Approver Role</span></th>
            <th style='width:100px;'><span class='grid-col-title'> Status</span></th>
            <th class='grid-col-title'><span class='grid-col-title'>Reviewed by/At</span></th>
            <th class='grid-col-title'><span class='grid-col-title'>Remarks</span></th>
            ".Replace("@_height@", this.Height.ToString());
        //foreach (EbFormStage FormStage in FormStages)
        //{
        //    if (!FormStage.Hidden)
        //        html += string.Concat("<th style='width: @Width@; @bg@' @type@ title='", FormStage.Title, "'><span class='grid-col-title'>", FormStage.Title, "</span>@req@</th>")
        //            .Replace("@req@", (FormStage.Required ? "<sup style='color: red'>*</sup>" : string.Empty))
        //            .Replace("@Width@", (FormStage.Width <= 0) ? "auto" : FormStage.Width.ToString() + "%")
        //            .Replace("@type@", "type = '" + FormStage.ObjType + "'")
        //            .Replace("@bg@", FormStage.IsDisable ? "background-color:#fafafa; color:#555" : string.Empty);
        //}

        html += @"
            </tr>
        </thead>";

        html += @"
        <tbody>";
        foreach (EbFormStage FormStage in FormStages)
        {
            html += string.Concat(@"
            <tr name='", FormStage.Name , "'role='", FormStage.ApproverRole.ToString(), "' style ='@bg@'>",
                "<td class='row-no-td'>", SlNo++, "</td>",
                "<td col='stage'><span class='fstd-div'>", FormStage.Name, "</span></td>",
                "<td><span class='fstd-div'>", FormStage.ApproverRole.ToString().Replace("_", " "), "</span></td>",
                @"<td col='status' class='fs-ctrl-td'><div class='fstd-div'>", @"
                    <select class='selectpicker'>
                        <option value='2'>Hold</option>
                        <option value='1'>Accepted</option>
                        <option value='0'>Rejected</option>
                    </select></div>
                </td>
                <td class='fs-ctrl-td'>
                    <div class='fstd-div'>
                        <div class='fs-user-cont'>
                            <div class='fs-dp'></div>
                            <div class='fs-udtls-cont'>
                                <span class='fs-uname'>John Snow</span>
                                <span class='fs-time'>10:10 am</span>
                            </div>
                        </div>
                    </div>
                </td>",
                "<td col='remarks' class='fs-ctrl-td'><div class='fstd-div'>", "<textarea class='fs-textarea'></textarea>", "</div></td>",
            "</tr>");
        }

        html += @"
        </tbody>
    </table>
    <button class='btn btn-success fs-submit'>Submit</button>
</div>";

            return html;
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }


    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbFormStage
    {
        public EbFormStage() { }
        public string ObjType { get { return this.GetType().Name.Substring(2, this.GetType().Name.Length - 2); } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        public KuSApproverRole ApproverRole { get; set; }


    }
}
