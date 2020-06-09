using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInPropertyGrid]
    public class EbFormNotification
    {
        public EbFormNotification() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript SendOnlyIf { get; set; }
    }

    [Alias("System")]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbFnSystem : EbFormNotification
    {
        public EbFnSystem() { }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [OnChangeExec(@"
if (this.NotifyBy === 0) this.NotifyBy = 1;
pg.HideProperty('Users');
pg.HideProperty('Roles');
pg.HideProperty('UserGroup');
if(this.NotifyBy === 1)
    pg.ShowProperty('Users');
else if(this.NotifyBy === 2)
    pg.ShowProperty('Roles');
else if(this.NotifyBy === 3)
    pg.ShowProperty('UserGroup');
")]
        public EbFnSys_NotifyBy NotifyBy { get; set; }
        
        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]//required ScriptEditorSQ
        public EbScript Users { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown, true)]
        public List<Int32> Roles { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropDataSourceJsFn("return ebcontext.UserGroups")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int UserGroup { get; set; }

        [PropertyGroup("Data")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Message { get; set; }
    }

    [Alias("Email")]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbFnEmail : EbFormNotification
    {
        public EbFnEmail() { }
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iEmailBuilder)]
        [EnableInBuilder(BuilderType.WebForm)]
        public string RefId { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public string Test2 { get; set; }
    }

    [Alias("Sms")]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbFnSms : EbFormNotification
    {
        public EbFnSms() { }

        [EnableInBuilder(BuilderType.WebForm)]
        public string Test3 { get; set; }
    }

    public enum EbFnSys_NotifyBy
    {
        Users = 1,
        Roles = 2,
        UserGroup = 3
    }
}
