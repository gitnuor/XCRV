using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XCRV.Web.Helpers
{
    public static class LiquidFunctions
    {
        public static void RegisterViewModel(Type rootType)
        {
            rootType
                .Assembly
                .GetTypes()
                .Where(t => t.Namespace == rootType.Namespace)
                .ToList()
                .ForEach(RegisterSafeTypeWithAllProperties);
        }

        public static void RegisterSafeTypeWithAllProperties(Type type)
        {
            Template.RegisterSafeType(type,
                type
                    .GetProperties()
                    .Select(p => p.Name)
                    .ToArray());
        }

        public static string RenderViewModel(this Template template, object root)
        {
            return template.Render(
                Hash.FromAnonymousObject(root));
        }
    }
}
