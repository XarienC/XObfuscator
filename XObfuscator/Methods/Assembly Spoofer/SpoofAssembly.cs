using dnlib.DotNet.Emit;
using dnlib.DotNet;

namespace XObfuscator.Methods.AssemblySpoofer
{
    public static class SpoofAssembly
    {
        private static readonly Random random = new Random();

        // List of anime names for spoofing
        private static readonly string[] AnimeNames =
        {
            "Naruto", "Sasuke", "Sakura", "Goku", "Luffy", "Zoro", "Ichigo", "Levi", "Eren", "Mikasa",
            "Hinata", "Todoroki", "Bakugo", "Reigen", "Gojo", "Ayanokoji", "Rem", "Asuna", "Kirito", "Shinra"
        };

        public static void Obfuscate(ModuleDefMD module)
        {
            // Ensure the module and assembly are valid
            if (module?.Assembly == null)
                return;

            var assembly = module.Assembly;

            // List of attributes to spoof
            string[] spoofableAttributes = {
                "System.Reflection.AssemblyTitleAttribute",
                "System.Reflection.AssemblyDescriptionAttribute",
                "System.Reflection.AssemblyCompanyAttribute",
                "System.Reflection.AssemblyProductAttribute",
                "System.Reflection.AssemblyCopyrightAttribute",
                "System.Reflection.AssemblyTrademarkAttribute"
            };

            // Spoof each attribute
            foreach (string attributeName in spoofableAttributes)
            {
                SpoofAttribute(module, assembly, attributeName);
            }

            // Add fake version attributes
            AddFakeVersionAttributes(module, assembly);
        }

        private static void SpoofAttribute(ModuleDefMD module, AssemblyDef assembly, string attributeName)
        {
            // Find the existing attribute
            var attribute = assembly.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == attributeName);

            if (attribute != null)
            {
                // Replace the existing value with an anime name
                if (attribute.ConstructorArguments.Count > 0)
                {
                    attribute.ConstructorArguments[0] = new CAArgument(
                        module.CorLibTypes.String, GetRandomAnimeName());
                }
            }
            else
            {
                // Add a new attribute
                try
                {
                    var attrType = module.CorLibTypes.GetTypeRef("mscorlib", attributeName); // Updated here
                    var ctor = module.Import(attrType.ResolveTypeDefThrow().FindConstructors().FirstOrDefault());
                    var newAttribute = new CustomAttribute(ctor);
                    newAttribute.ConstructorArguments.Add(new CAArgument(
                        module.CorLibTypes.String, GetRandomAnimeName()));
                    assembly.CustomAttributes.Add(newAttribute);
                }
                catch
                {
                    //
                }
            }
        }

        private static void AddFakeVersionAttributes(ModuleDefMD module, AssemblyDef assembly)
        {
            string fakeVersion = $"{random.Next(1, 10)}.{random.Next(0, 10)}.{random.Next(0, 10)}.{random.Next(0, 100)}";

            // Add or replace AssemblyVersion
            AddOrReplaceAttribute(module, assembly, "System.Reflection.AssemblyVersionAttribute", fakeVersion);

            // Add or replace AssemblyFileVersion
            AddOrReplaceAttribute(module, assembly, "System.Reflection.AssemblyFileVersionAttribute", fakeVersion);
        }

        private static void AddOrReplaceAttribute(ModuleDefMD module, AssemblyDef assembly, string attributeName, string value)
        {
            var attribute = assembly.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == attributeName);

            if (attribute != null)
            {
                // Replace the existing value
                if (attribute.ConstructorArguments.Count > 0)
                {
                    attribute.ConstructorArguments[0] = new CAArgument(
                        module.CorLibTypes.String, value);
                }
            }
            else
            {
                // Add a new attribute
                try
                {
                    var attrType = module.CorLibTypes.GetTypeRef("mscorlib", attributeName); // Updated here
                    var ctor = module.Import(attrType.ResolveTypeDefThrow().FindConstructors().FirstOrDefault());
                    var newAttribute = new CustomAttribute(ctor);
                    newAttribute.ConstructorArguments.Add(new CAArgument(
                        module.CorLibTypes.String, value));
                    assembly.CustomAttributes.Add(newAttribute);
                }
                catch
                {
                    // Silently fail if the attribute type is invalid
                }
            }
        }

        private static string GetRandomAnimeName()
        {
            return AnimeNames[random.Next(AnimeNames.Length)];
        }
    }
}
