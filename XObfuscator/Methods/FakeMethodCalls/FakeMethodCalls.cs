using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XObfuscator.Methods.FakeMethodCalls
{
    public static class FakeMethodCalls
    {
        private static readonly Random random = new Random();

        // List of fake method names
        private static readonly List<string> FakeMethodNames = new List<string>
        {
            "InitializeCryptoEngine",
            "ValidateProtectionScheme",
            "LoadAntiDebugToolkit",
            "ExecuteRuntimeValidation",
            "StartObfuscationProcess",
            "InjectSecurityMarker",
            "VerifyIntegrityHash",
            "LaunchProtectionPayload",
            "RunVirtualMachine",
            "ActivateShieldingMechanism"
        };

        // Main method to apply Fake Method Calls obfuscation
        public static void Obfuscate(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                // Add fake methods to the type
                AddFakeMethods(type);

                foreach (var method in type.Methods)
                {
                    // Skip methods without a body or very small methods
                    if (!method.HasBody || method.Body.Instructions.Count < 5) continue;

                    InsertFakeMethodCalls(method, type);
                }
            }
        }

        // Adds fake methods to the type
        private static void AddFakeMethods(TypeDef type)
        {
            foreach (var fakeName in FakeMethodNames)
            {
                // Create a new public static void method
                var fakeMethod = new MethodDefUser(
                    fakeName,
                    MethodSig.CreateStatic(type.Module.CorLibTypes.Void),
                    MethodAttributes.Public | MethodAttributes.Static);

                // Add a basic method body with unreachable code
                var body = new CilBody();
                body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, "Fake method invoked")); // Load a string
                body.Instructions.Add(Instruction.Create(OpCodes.Pop)); // Pop it from the stack
                body.Instructions.Add(Instruction.Create(OpCodes.Ret)); // Return
                fakeMethod.Body = body;

                // Add the fake method to the type
                type.Methods.Add(fakeMethod);
            }
        }

        // Inserts fake method calls into unreachable code paths
        private static void InsertFakeMethodCalls(MethodDef method, TypeDef type)
        {
            var instructions = method.Body.Instructions;

            // Insert a fake method call wrapped in unreachable code
            var fakeMethod = type.Methods[random.Next(type.Methods.Count)];
            if (!FakeMethodNames.Contains(fakeMethod.Name)) return; // Ensure it's a fake method

            // Create a new block with unreachable fake calls
            var fakeBlock = GenerateUnreachableBlock(fakeMethod);

            // Insert the fake block at a random position
            int insertionIndex = random.Next(0, instructions.Count);
            foreach (var instr in fakeBlock)
            {
                instructions.Insert(insertionIndex, instr);
                insertionIndex++;
            }

            // Simplify and optimize the body
            method.Body.SimplifyBranches();
            method.Body.OptimizeBranches();
        }

        // Generates an unreachable block containing a fake method call
        private static List<Instruction> GenerateUnreachableBlock(MethodDef fakeMethod)
        {
            var nopInstruction = Instruction.Create(OpCodes.Nop);

            var block = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldc_I4_0), // Push 0 (false)
                Instruction.Create(OpCodes.Brtrue, nopInstruction), // If false, skip the block
                Instruction.Create(OpCodes.Call, fakeMethod), // Call the fake method
                nopInstruction // No-op after the fake call
            };

            return block;
        }
    }
}
