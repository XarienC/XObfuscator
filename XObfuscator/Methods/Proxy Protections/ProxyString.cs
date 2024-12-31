using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XObfuscator.Methods.ProxyProtections
{
    public static class ProxyString
    {
        public static void Obfuscate(ModuleDefMD module)
        {
            // Loop through all types in the module
            foreach (var type in module.GetTypes())
            {
                // Skip the global module type
                if (type.IsGlobalModuleType) continue;

                // Loop through all methods in the type
                foreach (var method in type.Methods)
                {
                    // Skip methods without a body
                    if (!method.HasBody) continue;

                    // Process each instruction in the method
                    ProcessMethod(module, method);
                }
            }
        }

        private static void ProcessMethod(ModuleDef module, MethodDef method)
        {
            var instructions = method.Body.Instructions;

            foreach (var instruction in instructions)
            {
                // Check if the instruction loads a string constant
                if (instruction.OpCode == OpCodes.Ldstr)
                {
                    // Create a proxy method for the string
                    var proxyMethod = CreateProxyMethod(module, instruction.Operand.ToString());

                    // Replace the instruction with a call to the proxy method
                    instruction.OpCode = OpCodes.Call;
                    instruction.Operand = proxyMethod;
                }
            }
        }

        private static MethodDef CreateProxyMethod(ModuleDef module, string value)
        {
            // Create a new method for the string proxy
            var methodImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
            var methodFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;

            var proxyMethod = new MethodDefUser(
                GenerateRandomName(),
                MethodSig.CreateStatic(module.CorLibTypes.String),
                methodImplFlags,
                methodFlags
            );

            // Add the method to the global type
            module.GlobalType.Methods.Add(proxyMethod);

            // Create the method body
            proxyMethod.Body = new CilBody
            {
                Instructions =
                {
                    Instruction.Create(OpCodes.Ldstr, value), // Load the string constant
                    Instruction.Create(OpCodes.Ret)           // Return the string
                }
            };

            return proxyMethod;
        }

        private static string GenerateRandomName()
        {
            // Generate a random name for the proxy method
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new System.Random();
            return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
