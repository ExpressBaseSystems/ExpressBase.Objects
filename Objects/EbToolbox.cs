using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public class EbToolbox
    {
        public string GetToolboxHtml(BuilderType _builderType)
        {
            string _toolsHtml = string.Empty;

            var types = this.GetType().GetTypeInfo().Assembly.GetTypes();

            foreach (var tool in types)
            {
                if (tool.GetTypeInfo().IsSubclassOf(typeof(EbControl)))
                {
                    if (tool.GetTypeInfo().IsDefined(typeof(EnableInBuilder))
                         && tool.GetTypeInfo().GetCustomAttribute<EnableInBuilder>().BuilderTypes.Contains(_builderType))
                    {
                        _toolsHtml += GetToolHtml(tool.Name.Substring(2));
                    }
                }
            }

            return _toolsHtml;
        }

        private static string GetToolHtml(string tool_name)
        {
            return @"<div class='well well-sm'>
                            @toolName
                    </div>".Replace("@toolName", tool_name);
        }
    }
}
