using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileTableView : EbMobileDashBoardControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyGroup(PGConstants.DATA)]
        public string DataSourceRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
        [HelpText("sql query to get data from offline database")]
        [PropertyGroup(PGConstants.DATA)]
        public EbScript OfflineQuery { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.DATA)]
        public string BindingTable { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileTableView' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml'>
                               <table>
                                    <tr><th></th><th></th><th></th></tr>
                                    <tr><th></th><th></th><th></th></tr>
                                    <tr><th></th><th></th><th></th></tr>
                                </table>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }
}
