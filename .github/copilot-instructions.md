# GitHub Copilot Instruction File for RimWorld Modding Project

## Mod Overview and Purpose

**Variety Stockpile Mod**

The Variety Stockpile mod expands on the existing storage and hauling capabilities within RimWorld. It introduces a greater variety of stockpile zones with custom settings, allowing for more efficient and organized storage. This mod will enable players to have greater control over their inventory management, ensuring that items are stored correctly and efficiently, which can profoundly impact gameplay and colony success.

## Key Features and Systems

- **Custom Building Storage Notifications**: Two main classes (`Building_Storage_Notify_LostThing` and `Building_Storage_Notify_ReceivedThing`) are utilized to handle events when items are lost or received in storage buildings, enhancing feedback and inventory management.

- **AI Utility Enhancements**: The `HaulAIUtility_HaulToCellStorageJob` class provides static methods to improve the AI's ability to haul items to the most appropriate storage cell, optimizing pawns' hauling tasks.

- **Storage Settings Management**: With classes like `StorageSettings_ExposeData`, `StorageSettingsClipboard_Copy`, and `StorageSettingsClipboard_PasteInto`, the mod facilitates saving, copying, and pasting storage settings across different stockpile zones.

- **Stockpile Zone Management**: The classes `Zone_Stockpile_AddCell`, `Zone_Stockpile_Notify_LostThing`, `Zone_Stockpile_Notify_ReceivedThing`, and `Zone_Stockpile_Notify_RemoveCell` manage the cells within stockpile zones, ensuring dynamic and responsive stockpile management.

- **Utility Enhancements**: Enhancements such as `StoreUtility_NoStorageBlockersIn` and `StoreUtility_TryFindBestBetterStoreCellFor` help find optimal storage options without obstructions.

## Coding Patterns and Conventions

- **Class Structure**: Each class is designed to handle a specific aspect of storage and hauling in RimWorld, often adhering to SRP (Single Responsibility Principle) where possible.

- **Static Utility Methods**: Common functionality such as hauling logic is encapsulated within static classes, allowing for easy access and reusability across various components of the mod.

- **Event Handling**: Specialized notification classes (e.g., `Zone_Stockpile_Notify_LostThing`) effectively encapsulate event-driven interactions within the mod.

- **Modular Design**: The mod's design ensures that each component (e.g., notification, hauling job) can be developed or replaced independently without affecting other parts of the system.

## XML Integration

While the C# files represent the core logic, proper XML integration is crucial for seamless modding in RimWorld. XML files typically define configurations, item definitions, and game behaviors. It's important to ensure that any custom data structures or behaviors interfaced via C# are properly mapped and defined in XML to work with the RimWorld engine.

## Harmony Patching

Harmony is used for runtime method patching, allowing modification of the game's default behavior:

- Patches should be placed in appropriately named classes to indicate their purpose (e.g., `StoreUtility_TryFindBestBetterStoreCellFor` might use Harmony to adjust default storage cell determination logic).

- Both Prefix and Postfix methods might be utilized to inject logic before or after the game's original methods.

- Carefully manage Transpilers if method body modifications are necessary, maintaining compatibility with other mods.

## Suggestions for Copilot

- Use contextual hints and patterns derived from the existing codebase to assist in writing new classes or methods aligned with the mod's design principles.

- When creating new functionality, follow the project's coding conventions, such as class per responsibility, static utility classes for shared logic, and encapsulated event handling.

- Prompt Copilot to suggest more efficient ways to deal with common patterns like event handling, list management, and condition checks within the RimWorld modding context.

- Leverage Copilot for generating boilerplate code for repetitive tasks like getter/setter methods, basic method structures, and heavily documented parts of the code to maintain consistency.

- In complex systems requiring integration between C# and XML, use Copilot to suggest common paradigms or troubleshoot potential errors in API use based on recognizable error messages or patterns. 

This file serves as a guideline for developers using GitHub Copilot to expand or maintain the Variety Stockpile Mod for RimWorld. Ensure to have the relevant dependencies installed and set up the Harmony library to execute patches correctly.
