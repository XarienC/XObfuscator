# .NET Obfuscator

A powerful .NET Obfuscator designed to protect your applications from reverse-engineering by applying advanced obfuscation techniques while maintaining functionality.

## Example
![Obfuscator](https://i.imgur.com/Ty0pk7j.png)

## Features

- **Control Flow Obfuscation**: Rearranges code flow to confuse decompilers.
- **Renaming Protection**: Replaces meaningful names with random or thematic names.
- **Proxy Methods**: Redirects constants (integers, floats, strings) through proxy methods.
- **Anti-Decompiler Protections**: Includes Anti-DnSpy, Anti-De4dot, and Anti-Ildasm features.
- **Junk Code Insertion**: Adds meaningless code to inflate program size and complexity.
- **Watermark Embedding**: Adds a unique identifier to trace the program's origin.
- **Opaque Predicates**: Inserts always-true/false conditions to obscure logic.
- **Field Encapsulation**: Converts local variables into fields for added complexity.
- **Fake Attributes**: Injects misleading metadata to confuse reverse engineers.

## System Requirements

- **Operating System**: Windows 10 or later
- **.NET Runtime**: .NET 8.0 or higher
- **RAM**: 4GB minimum (8GB recommended)
- **Processor**: Dual-core processor or higher
- **Dependencies**: dnlib library included
- **Development Environment**: Visual Studio 2022 or compatible

## How to Use

1. Clone or download this repository.
2. Open the solution in Visual Studio 2022 or a compatible IDE.
3. Build the project to ensure all dependencies are installed.
4. Run the program and load your .NET application (DLL or EXE) into the obfuscator.
5. Select your desired obfuscation methods using the provided checkboxes.
6. Click the "Obfuscate" button to apply the obfuscation.
7. The obfuscated file will be saved in the same directory as the original file with `_XObfuscated` appended to the filename (or in a custom location if specified).

## Disclaimer

This tool is intended for legal purposes only. The use of this program to obfuscate malicious software or for any unlawful activity is strictly prohibited. Use at your own risk.

---
**Protect your applications with ease!**
