using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Text;
using ExpressBase.Common.Extensions;
using Newtonsoft.Json;

namespace ExpressBase.Objects.ApiBuilderRelated
{
    public class ListOrdered: List<EbApiWrapper>
    {
        public ListOrdered()
        {
            this.Sort((x, y) => x.RouteIndex.CompareTo(y.RouteIndex));
        }
    }

    public abstract class EbApiWrapper : EbObject
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public virtual int RouteIndex { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [UIproperty]
        [MetaOnly]
        public string Label { set; get; }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbApi: EbApiWrapper
    {
        public override int RouteIndex { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public ListOrdered Resources { set; get; }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSqlReader : EbApiWrapper
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyGroup("Data Settings")]
        public string Refid { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='apiPrcItem dropped' eb-type='SqlReader' id='@id'><div tabindex='1' class='drpbox' onclick='$(this).focus();'> @Label </div></div>".RemoveCR().DoubleQuoted(); ;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSqlFunc : EbApiWrapper
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iSqlFunction)]
        [PropertyGroup("Data Settings")]
        public string Refid { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='apiPrcItem dropped' eb-type='SqlFunc' id='@id'><div tabindex='1' class='drpbox' onclick='$(this).focus();'> @Label </div></div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSqlWriter : EbApiWrapper
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataWriter)]
        [PropertyGroup("Data Settings")]
        public string Refid { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='apiPrcItem dropped' eb-type='SqlWriter' id='@id'><div tabindex='1' class='drpbox' onclick='$(this).focus();'> @Label </div></div>".RemoveCR().DoubleQuoted();
        }
    }
}
