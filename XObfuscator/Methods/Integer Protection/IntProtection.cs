using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XObfuscator.Methods.IntegerProtection
{
    public static class IntProtection
    {
        private static readonly Random random = new Random();

        public static void Obfuscate(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                // Skip global types
                if (type.IsGlobalModuleType) continue;

                foreach (var method in type.Methods)
                {
                    // Skip methods without a body or constructors
                    if (!method.HasBody || method.IsConstructor) continue;

                    var instructions = method.Body.Instructions;
                    var targets = new Dictionary<Instruction, Instruction>(); // Maps old to new instructions

                    for (int i = 0; i < instructions.Count; i++)
                    {
                        var instr = instructions[i];

                        // Check for ldc.i4 instructions
                        if (IsLdcI4(instr.OpCode))
                        {
                            // Get the integer value from the opcode or operand
                            int originalValue = GetLdcI4Value(instr);

                            // Generate obfuscated instructions
                            var newInstructions = GenerateObfuscatedInteger(originalValue);

                            // Map the old instruction to the first new instruction
                            targets[instr] = newInstructions[0];

                            // Replace the current instruction
                            instructions.RemoveAt(i);
                            foreach (var newInstr in newInstructions)
                            {
                                instructions.Insert(i++, newInstr);
                            }
                        }
                    }

                    // Update branch targets
                    foreach (var instr in instructions)
                    {
                        if (instr.Operand is Instruction target && targets.ContainsKey(target))
                        {
                            instr.Operand = targets[target]; // Update the branch target
                        }
                        else if (instr.Operand is IList<Instruction> targetList)
                        {
                            for (int j = 0; j < targetList.Count; j++)
                            {
                                if (targets.ContainsKey(targetList[j]))
                                {
                                    targetList[j] = targets[targetList[j]]; // Update multi-target branches
                                }
                            }
                        }
                    }

                    // Update exception handlers
                    foreach (var handler in method.Body.ExceptionHandlers)
                    {
                        if (handler.TryStart != null && targets.ContainsKey(handler.TryStart))
                            handler.TryStart = targets[handler.TryStart];
                        if (handler.TryEnd != null && targets.ContainsKey(handler.TryEnd))
                            handler.TryEnd = targets[handler.TryEnd];
                        if (handler.HandlerStart != null && targets.ContainsKey(handler.HandlerStart))
                            handler.HandlerStart = targets[handler.HandlerStart];
                        if (handler.HandlerEnd != null && targets.ContainsKey(handler.HandlerEnd))
                            handler.HandlerEnd = targets[handler.HandlerEnd];
                    }

                    // Simplify and optimize the method body
                    method.Body.SimplifyBranches();
                    method.Body.OptimizeBranches();
                }
            }
        }

        // Checks if the opcode is an ldc.i4 opcode
        private static bool IsLdcI4(OpCode opCode)
        {
            return opCode == OpCodes.Ldc_I4 ||
                   opCode == OpCodes.Ldc_I4_S ||
                   opCode == OpCodes.Ldc_I4_0 ||
                   opCode == OpCodes.Ldc_I4_1 ||
                   opCode == OpCodes.Ldc_I4_2 ||
                   opCode == OpCodes.Ldc_I4_3 ||
                   opCode == OpCodes.Ldc_I4_4 ||
                   opCode == OpCodes.Ldc_I4_5 ||
                   opCode == OpCodes.Ldc_I4_6 ||
                   opCode == OpCodes.Ldc_I4_7 ||
                   opCode == OpCodes.Ldc_I4_8 ||
                   opCode == OpCodes.Ldc_I4_M1;
        }

        // Extracts the integer value from an ldc.i4 instruction
        private static int GetLdcI4Value(Instruction instr)
        {
            // Handle shorthand opcodes
            if (instr.OpCode == OpCodes.Ldc_I4_0) return 0;
            if (instr.OpCode == OpCodes.Ldc_I4_1) return 1;
            if (instr.OpCode == OpCodes.Ldc_I4_2) return 2;
            if (instr.OpCode == OpCodes.Ldc_I4_3) return 3;
            if (instr.OpCode == OpCodes.Ldc_I4_4) return 4;
            if (instr.OpCode == OpCodes.Ldc_I4_5) return 5;
            if (instr.OpCode == OpCodes.Ldc_I4_6) return 6;
            if (instr.OpCode == OpCodes.Ldc_I4_7) return 7;
            if (instr.OpCode == OpCodes.Ldc_I4_8) return 8;
            if (instr.OpCode == OpCodes.Ldc_I4_M1) return -1;

            // Handle ldc.i4.s (short integers)
            if (instr.OpCode == OpCodes.Ldc_I4_S && instr.Operand is sbyte sbyteValue)
            {
                return sbyteValue; // Convert sbyte to int
            }

            // Handle regular ldc.i4 or ldc.i4.s with an operand
            if (instr.Operand is int intValue)
            {
                return intValue;
            }

            throw new InvalidOperationException($"Unsupported ldc.i4 opcode or operand: {instr.OpCode}");
        }

        // Generates obfuscated expressions for integers
        private static List<Instruction> GenerateObfuscatedInteger(int value)
        {
            var instructions = new List<Instruction>();

            if (value == 0)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0)); // Special case for 0
                return instructions;
            }

            // Handle negative values
            bool isNegative = value < 0;
            if (isNegative)
            {
                value = -value; // Work with positive value first
            }

            // Choose a random obfuscation pattern
            int pattern = random.Next(0, 3); // 0 = addition/subtraction, 1 = multiplication/division, 2 = nested arithmetic

            switch (pattern)
            {
                case 0: // Addition and Subtraction
                    int a = random.Next(1, value); // Random part 1
                    int b = value - a;             // Random part 2
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4, a));  // Load 'a'
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4, b));  // Load 'b'
                    instructions.Add(Instruction.Create(OpCodes.Add));        // a + b
                    break;

                case 1: // Multiplication and Division
                    if (value > 1) // Ensure value is greater than 1 for multiplication/division
                    {
                        int multiplier = random.Next(2, Math.Min(value, 10));  // Random multiplier (avoid 0 and 1)
                        int quotient = value / multiplier;    // Calculate quotient
                        int remainder = value % multiplier;   // Calculate remainder
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4, quotient));  // Load quotient
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4, multiplier)); // Load multiplier
                        instructions.Add(Instruction.Create(OpCodes.Mul));               // quotient * multiplier
                        if (remainder != 0)
                        {
                            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, remainder)); // Load remainder
                            instructions.Add(Instruction.Create(OpCodes.Add));              // + remainder
                        }
                    }
                    else
                    {
                        // Fallback to simpler addition/subtraction for small values
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4_1)); // Load 1
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4, value - 1)); // Load (value - 1)
                        instructions.Add(Instruction.Create(OpCodes.Add)); // 1 + (value - 1)
                    }
                    break;

                case 2: // Nested Arithmetic
                    if (value > 2) // Ensure value is large enough for nested arithmetic
                    {
                        int x = random.Next(1, value / 2 + 1); // Part 1
                        int y = random.Next(1, value - x);     // Part 2
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4, x));  // Load 'x'
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4, y));  // Load 'y'
                        instructions.Add(Instruction.Create(OpCodes.Add));        // x + y
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4, value - (x + y))); // Remaining value
                        instructions.Add(Instruction.Create(OpCodes.Add));        // (x + y) + remaining
                    }
                    else
                    {
                        // Fallback to simpler addition/subtraction for small values
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4_1)); // Load 1
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4, value - 1)); // Load (value - 1)
                        instructions.Add(Instruction.Create(OpCodes.Add)); // 1 + (value - 1)
                    }
                    break;
            }

            // If the original value was negative, negate the result
            if (isNegative)
            {
                instructions.Add(Instruction.Create(OpCodes.Neg)); // Negate the result
            }

            return instructions;
        }
    }
}
