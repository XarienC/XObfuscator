using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace XObfuscator.Methods.AntiProtection
{
    // credits to nak0823

    public static class AntiDnSpy
    {
        public static void Obfuscate(ModuleDefMD module)
        {
            // Loop through all types in the module
            foreach (TypeDef type in module.Types)
            {
                // Loop through all methods in the type
                foreach (MethodDef method in type.Methods)
                {
                    // Skip methods without a body
                    if (method.Body == null) continue;

                    // Insert 33,333 NOP instructions at the start of the method body
                    for (int x = 0; x < 33333; x++)
                    {
                        method.Body.Instructions.Insert(0, new Instruction(OpCodes.Nop));
                    }

                    // Optimize and simplify the method body to ensure validity
                    method.Body.SimplifyBranches();
                    method.Body.OptimizeBranches();
                }
            }
        }
    }
}
