using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XObfuscator.Methods.ControlFlow
{
    public static class ControlFlow
    {
        private static readonly Random random = new Random();

        // Main method to apply Control Flow Obfuscation
        public static void Obfuscate(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    // Skip methods without a body or very small methods
                    if (!method.HasBody || method.Body.Instructions.Count < 5) continue;

                    ObfuscateMethod(method);
                }
            }
        }

        // Obfuscates the control flow of a single method
        private static void ObfuscateMethod(MethodDef method)
        {
            var body = method.Body;
            var instructions = body.Instructions;
            var newInstructions = new List<Instruction>(instructions.Count);

            // Generate a random order for the instructions
            var shuffledBlocks = SplitInstructionsIntoBlocks(instructions);

            foreach (var block in shuffledBlocks)
            {
                // Add a random conditional branch at the start of each block
                var randomValue = random.Next(0, 2);
                newInstructions.Add(Instruction.Create(OpCodes.Ldc_I4, randomValue)); // Push random 0 or 1
                var branchTarget = block[0]; // The first instruction in the block
                var branchInstruction = Instruction.Create(OpCodes.Brfalse, branchTarget);
                newInstructions.Add(branchInstruction);

                // Add the block instructions
                newInstructions.AddRange(block);

                // Fix the branch target
                branchInstruction.Operand = block[0];
            }

            // Add a return or end instruction to ensure the method ends cleanly
            if (method.ReturnType.ElementType == ElementType.Void)
            {
                newInstructions.Add(Instruction.Create(OpCodes.Ret));
            }

            // Clear the old instructions and replace them with the new obfuscated instructions
            instructions.Clear();
            foreach (var instr in newInstructions)
            {
                instructions.Add(instr);
            }

            // Recalculate the max stack value
            body.SimplifyBranches();
            body.OptimizeBranches();
        }

        // Splits instructions into random blocks
        private static List<List<Instruction>> SplitInstructionsIntoBlocks(IList<Instruction> instructions)
        {
            var blocks = new List<List<Instruction>>();
            var currentBlock = new List<Instruction>();

            foreach (var instr in instructions)
            {
                currentBlock.Add(instr);

                // Randomly decide to start a new block
                if (random.Next(0, 3) == 0 && currentBlock.Count > 0)
                {
                    blocks.Add(new List<Instruction>(currentBlock));
                    currentBlock.Clear();
                }
            }

            // Add any remaining instructions as the last block
            if (currentBlock.Count > 0)
            {
                blocks.Add(currentBlock);
            }

            return blocks;
        }
    }
}
