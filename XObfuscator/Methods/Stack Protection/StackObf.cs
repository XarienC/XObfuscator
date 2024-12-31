using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XObfuscator.Methods.StackProtection
{
    public static class StackObf
    {
        private static readonly Random random = new Random();

        public static void Obfuscate(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                // Skip global module types
                if (type.IsGlobalModuleType) continue;

                foreach (var method in type.Methods)
                {
                    // Skip methods without a body or constructor methods
                    if (!method.HasBody || method.IsConstructor) continue;

                    // Safely obfuscate the method
                    ObfuscateMethod(method);
                }
            }
        }

        private static void ObfuscateMethod(MethodDef method)
        {
            // Get the instructions collection
            var instructions = method.Body.Instructions;

            for (int i = 0; i < instructions.Count; i++)
            {
                var instr = instructions[i];

                // Process specific IL opcodes (e.g., ldc.i4, ldloc, etc.)
                if (instr.OpCode == OpCodes.Ldc_I4)
                {
                    // Replace constant integer loads with stack-based operations
                    int originalValue = (int)instr.Operand;

                    // Generate fake and real stack instructions
                    var fakeValue = random.Next(100, 200); // Generate a clearly fake value
                    var fakeOperations = new[]
                    {
                        Instruction.Create(OpCodes.Ldc_I4, fakeValue), // Load a fake value
                        Instruction.Create(OpCodes.Ldc_I4, originalValue), // Load the real value
                        Instruction.Create(OpCodes.Xor), // Obfuscate by XORing fake and real
                        Instruction.Create(OpCodes.Ldc_I4, fakeValue), // Load fake value again
                        Instruction.Create(OpCodes.Xor) // Reverse XOR to get the original value
                    };

                    // Replace the instruction with the fake operations
                    ReplaceInstruction(instructions, i, fakeOperations);
                    i += fakeOperations.Length - 1; // Skip over newly inserted instructions
                }
                else if (instr.OpCode == OpCodes.Stloc || instr.OpCode == OpCodes.Ldloc)
                {
                    // Add meaningless stack manipulations
                    var newInstrs = new[]
                    {
                        Instruction.Create(OpCodes.Dup), // Duplicate value
                        Instruction.Create(OpCodes.Pop), // Pop the duplicate
                        Instruction.Create(OpCodes.Nop)  // No operation (just filler)
                    };

                    // Insert the new instructions before the current one
                    InsertInstructions(instructions, i, newInstrs);
                    i += newInstrs.Length; // Skip over newly inserted instructions
                }
            }
        }

        private static void ReplaceInstruction(IList<Instruction> instructions, int index, Instruction[] newInstrs)
        {
            // Remove the original instruction
            instructions[index] = newInstrs[0];

            // Insert the remaining new instructions
            for (int i = 1; i < newInstrs.Length; i++)
            {
                instructions.Insert(index + i, newInstrs[i]);
            }
        }

        private static void InsertInstructions(IList<Instruction> instructions, int index, Instruction[] newInstrs)
        {
            // Insert new instructions at the specified index
            foreach (var instr in newInstrs)
            {
                instructions.Insert(index++, instr);
            }
        }
    }
}
