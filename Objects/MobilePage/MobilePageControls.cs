using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileControls : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        [UIproperty]
        public string Label { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileTextBox : EbMobileControls
    {
        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout control' id='@id' eb-type='EbMobileTextBox' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <input type='text' class='eb_mob_textbox' />
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileNumericBox : EbMobileControls
    {
        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout control' id='@id' eb-type='EbMobileNumericBox' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <input type='text' class='eb_mob_textbox' />
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDateTime : EbMobileControls
    {
        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout control' id='@id' eb-type='EbMobileDateTime' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <input type='text' class='eb_mob_textbox' placeholder='YYYY-MM-DD'/>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileSimpleSelect : EbMobileControls
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbMobileSSOption> Options { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout control' id='@id' eb-type='EbMobileSimpleSelect' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <select class='eb_mob_select'>
                                    <option>--select--</option>
                                </select>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileSSOption : EbMobilePageBase
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public string EbSid { get; set; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileFileUpload : EbMobileControls
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public bool EnableCameraSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool EnableFileSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool MultiSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool EnableEdit { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout control' id='@id' eb-type='EbMobileFileUpload' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <button class='eb_mob_fupbtn filesbtn'> Files </button>
                               <button class='eb_mob_fupbtn camerabtn'> Camera </button>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
                this.Init = function(id)
                {
                    this.EnableCameraSelect = true;
                    this.EnableFileSelect= true;
                    this.MultiSelect= true;
                    this.EnableEdit= true;
                };";
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileBoolean : EbMobileControls
    {
        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout control' id='@id' eb-type='EbMobileBoolean' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <input type='checkbox' class='eb_mob_checkbox'/>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileTableLayout : EbMobileControls
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public int RowCount { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int ColumCount { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public List<EbMobileTableCell> CellCollection { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_tablelayout control' eb-type='EbMobileGrid' id='@id'>
                        <div class='eb_mob_tablelayout_inner'>
                            
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
                this.Init = function(id)
                {
                    this.RowCount = 2;
                    this.ColumCount= 2;
                };";
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileTableCell : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public int RowIndex { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int CellIndex { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public List<EbMobileDataColumn> ColumnCollection { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDataColumn : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public int ColumnIndex { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ColumnName { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public EbDbTypes Type { get; set; }
    }
}
