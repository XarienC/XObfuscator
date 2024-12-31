using dnlib.DotNet;
using System;

namespace XObfuscator.Methods.FakeAttributes
{
    public static class FakeAttributes
    {
        // A list of realistic fake attribute names to add
        private static readonly List<string> FakeAttributeNames = new List<string>
        {
            "ZYXDNGuarder",
            "HVMRuntm.dll",
            "DebuggerHidden",
            "System.Runtime.CompilerServices.CompilationRelaxationsAttribute",
            "System.Runtime.CompilerServices.RuntimeCompatibilityAttribute",
            "AntiTamperAttribute",
            "DynamicProxyAttribute",
            "MemoryProtection",
            "XenoCode.Virtualizer",
            "ObfuscatedByGoliath",
            "VMProtect",
            "ConfuseGuard",
            "CodeSecure",
            "ProtectorShield",
            "AssemblyMarker"
        };

        // Main method to apply Fake Attributes
        public static void Obfuscate(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                // Skip global module type
                if (type.IsGlobalModuleType) continue;

                // Add multiple fake attributes to the type
                AddFakeAttributes(type);
            }
        }

        // Adds fake attributes to the given type
        private static void AddFakeAttributes(TypeDef type)
        {
            var module = type.Module;

            foreach (var fakeName in FakeAttributeNames)
            {
                // Create a fake TypeRef for the attribute
                var fakeAttributeType = new TypeRefUser(
                    module,
                    fakeName,
                    fakeName,
                    module.CorLibTypes.AssemblyRef);

                // Create a fake constructor for the attribute
                var fakeCtor = new MemberRefUser(module, ".ctor",
                    MethodSig.CreateInstance(module.CorLibTypes.Void),
                    fakeAttributeType);

                // Create the custom attribute instance
                var fakeAttribute = new CustomAttribute(fakeCtor);

                // Add the fake attribute to the type
                type.CustomAttributes.Add(fakeAttribute);
            }
        }
    }
}
