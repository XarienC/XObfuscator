using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

namespace XObfuscator.Methods.Opaque_Predicates
{
    public static class OpaquePredicates
    {
        private static readonly Random random = new Random();

        // Apply Opaque Predicates to the module
        public static void Apply(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                // Skip global module types and system types
                if (type.IsGlobalModuleType) continue;

                foreach (var method in type.Methods)
                {
                    // Skip methods without a body or very small methods
                    if (!method.HasBody || method.Body.Instructions.Count < 5) continue;

                    // Insert layered opaque predicates into the method
                    InsertLayeredOpaquePredicates(method);
                }
            }
        }

        // Inserts layered opaque predicates into a method
        private static void InsertLayeredOpaquePredicates(MethodDef method)
        {
            var instructions = method.Body.Instructions;

            // Randomly determine the number of layers
            int layers = random.Next(2, 5); // Add 2-4 layers of predicates

            for (int i = 0; i < layers; i++)
            {
                // Choose a random position to insert the opaque predicate
                int insertIndex = random.Next(0, instructions.Count);

                // Generate an opaque predicate with obfuscated variables
                var predicateInstructions = GenerateOpaquePredicateWithObfuscatedVariable(method);

                // Insert the opaque predicate into the method
                foreach (var instr in predicateInstructions)
                {
                    instructions.Insert(insertIndex, instr);
                    insertIndex++;
                }
            }

            // Simplify and optimize the body
            method.Body.SimplifyBranches();
            method.Body.OptimizeBranches();
        }

        // Generates an opaque predicate with obfuscated variables
        private static List<Instruction> GenerateOpaquePredicateWithObfuscatedVariable(MethodDef method)
        {
            var instructions = new List<Instruction>();

            // Create a local variable with a random name
            string variableName = GenerateRandomString(8);
            var localVariable = new Local(method.Module.CorLibTypes.Int32);
            method.Body.Variables.Add(localVariable);

            // Assign a random value to the variable
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, random.Next(1, 100))); // Load random int
            instructions.Add(Instruction.Create(OpCodes.Stloc, localVariable));      // Store in local variable

            // Generate a random opaque predicate pattern
            instructions.AddRange(GenerateRandomOpaquePredicate(localVariable));

            return instructions;
        }

        // Generates a random opaque predicate pattern
        private static List<Instruction> GenerateRandomOpaquePredicate(Local localVariable)
        {
            var instructions = new List<Instruction>();

            int patternChoice = random.Next(3); // Choose one of three patterns
            switch (patternChoice)
            {
                case 0:
                    // Pattern 1: Always true
                    var nopInstruction1 = Instruction.Create(OpCodes.Nop);
                    instructions.Add(Instruction.Create(OpCodes.Ldloc, localVariable)); // Load variable
                    instructions.Add(Instruction.Create(OpCodes.Dup));                 // Duplicate value
                    instructions.Add(Instruction.Create(OpCodes.Mul));                 // Square it
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4, 0));           // Load 0
                    instructions.Add(Instruction.Create(OpCodes.Sub));                 // Subtract
                    instructions.Add(Instruction.Create(OpCodes.Brfalse_S, nopInstruction1)); // Always true
                    instructions.Add(nopInstruction1);                                 // Add valid branch target
                    break;

                case 1:
                    // Pattern 2: Always false
                    var nopInstruction2 = Instruction.Create(OpCodes.Nop);
                    instructions.Add(Instruction.Create(OpCodes.Ldloc, localVariable)); // Load variable
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4_1));             // Load 1
                    instructions.Add(Instruction.Create(OpCodes.And));                  // Perform AND operation
                    instructions.Add(Instruction.Create(OpCodes.Brtrue_S, nopInstruction2)); // Always false
                    instructions.Add(nopInstruction2);                                  // Add valid branch target
                    break;

                case 2:
                    // Pattern 3: Complex
                    var nopInstruction3 = Instruction.Create(OpCodes.Nop);
                    instructions.Add(Instruction.Create(OpCodes.Ldloc, localVariable)); // Load variable
                    instructions.Add(Instruction.Create(OpCodes.Dup));                 // Duplicate value
                    instructions.Add(Instruction.Create(OpCodes.Xor));                 // XOR
                    instructions.Add(Instruction.Create(OpCodes.Brfalse_S, nopInstruction3)); // Complex
                    instructions.Add(nopInstruction3);                                 // Add valid branch target
                    break;
            }

            // Optional junk code after the predicate
            instructions.Add(Instruction.Create(OpCodes.Ldstr, GenerateRandomString(10))); // Load random string
            instructions.Add(Instruction.Create(OpCodes.Pop));                            // Pop it off the stack

            return instructions;
        }

        // Generates a random string for obfuscated variable names and junk instructions
        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[random.Next(chars.Length)];
            }
            return new string(buffer);
        }
    }
}
