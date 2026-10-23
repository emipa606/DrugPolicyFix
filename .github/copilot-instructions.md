# GitHub Copilot Instructions for Drug Policy Fix (Continued) Mod

## Mod Overview and Purpose

**Mod Name:** Drug Policy Fix (Continued)

**Author:** Pelador, continued by others

The **Drug Policy Fix (Continued)** mod is an enhancement for RimWorld that automates the management of drug policies. Its main purpose is to automatically integrate new drugs into existing drug policies and sort the drug lists alphabetically, alleviating the need for manual adjustments and providing a streamlined experience.

## Key Features and Systems

- **Automated Drug Policy Adjustments**: Automatically updates drug policies whenever new drugs are added or existing ones are removed from the game, ensuring policies are always up-to-date.
- **Alphabetical Sorting**: Drug entries in policies are sorted alphabetically for ease of use, making it easier to locate and manage specific drugs.
- **Compatibility**: Integrates seamlessly with other mods that use the vanilla drug policy setup. It is compatible with multiplayer mode and works well with mods like Cherry Picker.
- **Localization**: Supports multiple languages, including a German translation.

## Coding Patterns and Conventions

- **Namespace and Type Organization**: The mod source is organized with specific types for handling different functionalities, such as `DrugPolicyDatabase_MakeNewDrugPolicy`, `DrugPolicyFix`, `DrugPolicyFixComponent`, and `DrugPolicySort`.
- **Method Naming**: Follow descriptive and camelCase naming for methods, e.g., `addNewDrugToPolicy`, `CorrectPolicies`, and `SortPolicies`.
- **Single Responsibility Principle**: Ensure that each class and method has a clear, single responsibility, which aligns with the specific tasks they perform.

## XML Integration

Though this mod primarily consists of C# files and does not include XML definitions, ensure that any future XML integrations align with RimWorld's standard XML schema. Pay attention to correct element definitions and use them to map new configurations if needed.

## Harmony Patching

The mod uses Harmony, a library for patching assemblies during runtime, which allows for modifications of the game's original code without altering it directly.

- **Key Patch File**: `DrugPolicyDatabase_MakeNewDrugPolicy.cs` includes a Harmony postfix method attached to the original `MakeNewDrugPolicy` method, ensuring that drug policies update automatically.
- **NonPublicFields**: Utilize this class for accessing private fields and methods where necessary.

## Suggestions for Copilot

1. **Automated Suggestions**: For adding new features or debugging, start by reviewing the existing C# files to understand the logical flow.
2. **XML Files**: Suggest structures for future XML integrations even though the current version lacks them.
3. **Harmony Enhancements**: Recommend enhancements to existing Harmony patches if needed, such as additional hooks or optimizations.
4. **Localization Support**: Identify areas to enhance multi-language support or add new translations.
5. **Error Handling**: Provide strategies for robust error handling and logging within the existing methods to improve reliability.

This file serves as a guide for maintaining and expanding the **Drug Policy Fix (Continued)** RimWorld mod using GitHub Copilot. Maintain consistency with existing patterns and practices while leveraging AI to enhance capabilities where applicable.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

