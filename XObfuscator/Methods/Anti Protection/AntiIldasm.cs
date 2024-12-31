using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XObfuscator.Methods.AntiProtection
{

    // credits to nak0823

    public static class AntiIldasm
    {
        public static void Obfuscate(ModuleDefMD module)
        {
            // Loop through all modules in the assembly
            foreach (ModuleDefMD mod in module.Assembly.Modules)
            {
                // Create a reference to the SuppressIldasmAttribute
                TypeRef attrRef = mod.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute");

                // Create a reference to the constructor of SuppressIldasmAttribute
                var ctorRef = new MemberRefUser(mod, ".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void), attrRef);

                // Create a custom attribute using the constructor reference
                var customAttribute = new CustomAttribute(ctorRef);

                // Add the custom attribute to the module's CustomAttributes collection
                mod.CustomAttributes.Add(customAttribute);
            }
        }
    }
}
