using dnlib.DotNet.Emit;
using dnlib.DotNet;

namespace XObfuscator.Methods.ProxyProtections
{
    public static class ManyProxy
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

            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];

                // Redirect integer constants
                if (instruction.IsLdcI4())
                {
                    var proxyMethod = CreateProxyMethod(module, (int)instruction.GetLdcI4Value());
                    instruction.OpCode = OpCodes.Call;
                    instruction.Operand = proxyMethod;
                }
                // Redirect float constants
                else if (instruction.OpCode == OpCodes.Ldc_R4)
                {
                    var proxyMethod = CreateProxyMethod(module, (float)instruction.Operand);
                    instruction.OpCode = OpCodes.Call;
                    instruction.Operand = proxyMethod;
                }
                // Redirect string constants
                else if (instruction.OpCode == OpCodes.Ldstr)
                {
                    var proxyMethod = CreateProxyMethod(module, (string)instruction.Operand);
                    instruction.OpCode = OpCodes.Call;
                    instruction.Operand = proxyMethod;
                }
            }

            // Simplify and optimize branches to prevent short branch issues
            method.Body.SimplifyBranches();
            method.Body.OptimizeBranches();
        }

        private static MethodDef CreateProxyMethod(ModuleDef module, int value)
        {
            // Create a new method for the integer proxy
            var methodImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
            var methodFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;

            var proxyMethod = new MethodDefUser(
                GenerateRandomName(),
                MethodSig.CreateStatic(module.CorLibTypes.Int32),
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
                    Instruction.Create(OpCodes.Ldc_I4, value), // Load the integer value
                    Instruction.Create(OpCodes.Ret)            // Return the value
                }
            };

            return proxyMethod;
        }

        private static MethodDef CreateProxyMethod(ModuleDef module, float value)
        {
            // Create a new method for the float proxy
            var methodImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
            var methodFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;

            var proxyMethod = new MethodDefUser(
                GenerateRandomName(),
                MethodSig.CreateStatic(module.CorLibTypes.Single),
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
                    Instruction.Create(OpCodes.Ldc_R4, value), // Load the float value
                    Instruction.Create(OpCodes.Ret)           // Return the value
                }
            };

            return proxyMethod;
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
                    Instruction.Create(OpCodes.Ret)          // Return the string
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
