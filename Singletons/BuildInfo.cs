using ExpressBase.Common.Extensions;
using System;
using System.Reflection;

namespace ExpressBase.Objects.Singletons
{
    public static class BuildInfo
    {
        public static readonly string Md5Version = Assembly.GetAssembly(typeof(BuildInfo)).GetName().Version.ToString().ToMD5Hash();
    }
}
