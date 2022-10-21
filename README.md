# VarietyMattersStockpile

![Image](https://i.imgur.com/buuPQel.png)

Update of Cozarkians mod
https://steamcommunity.com/sharedfiles/filedetails/?id=2266068546

![Image](https://i.imgur.com/pufA0kM.png)

	
![Image](https://i.imgur.com/Z4GOv8H.png)

# Overview

Adds several new features to stockpile filters for added control over what to store, how much to store, and when to fill a stockpile. 

Created for use with https://steamcommunity.com/sharedfiles/filedetails/?id=2207657844]Variety Matters, you can now create a stockpile near your cook that will hold small stacks of different ingredients, increasing the diversity of ingredients used in meals. 

For vanilla uses, you can create a food shelf in your dining room that won't refill until the shelves are empty, saving hauling time and helping meals get eaten before they rot; or give your pawns' closets with only 1 of each type of clothing. 

# Mod Features

**Limit Duplicate Stacks:** Limit the number of stacks of the same item that can be stored in a stockpile. For example, with a duplicate limit of 2, pawns will never store more than 2 stacks of corn, no matter how large the stockpile. 

There is an option to treat items with a stack size of 1 as duplicates if they have the same def name, allowing for the creation of closets/armories with a variety of clothing/armor.

**Limit Stack Size:**: Set a limit to reduce stack sizes.The limit will only reduce stack sizes, not increase them. Useful to divide limited supplies of medicine among multiple stockpiles.

**Limit Refilling**: Set a limit for how empty a stockpile must be before pawns will start refilling. Move the slider all the way to the right for vanilla behavior. Move it to the left and pawns will stop refilling any stacks until a certain number of cells are empty. Once pawns start refilling, they will keep filling the stockpile until full.

Stockpiles will automatically start filling when created or enlarged. There is a toggle to start/stop refilling that appears when appropriate. If you toggle off a completely empty stockpile, it will disable the stockpile until manually restarted.

# Compatibility / Mod Interaction


**Project Rim Factory:** No known errors. Doesn't work with hoppers.

**LWM Deep Storage:**


- Duplicate Limit: Works to limit stacks up to 1 per cell. Trying to set a duplicate limit greater than 1 per cell will be treated as 1 per cell.
		
- Stack Size Limit: Updated to work properly.
		
- Refill Limit: Recommended to keep set to always fill. Moving the slider works, but will substantially reduce the usefulness of deep storage.


		

**Ogre Stack:** No known issues.
		
**Satisfied Storage (e.g. Storage Hysteresis):** Must be loaded before this mod. Satisfied Storage will not consider reduced stack sizes when determining whether to refill a cell.
	
**Jobs of Opportunity:** No known issues.
	
**Pick Up and Haul:** Not tested after update. Prior to update, had an amusing bug with the stack size feature, but other features worked fine.
	
**Stockpile Ranking:** No known errors. Duplicate limit feature will prevent lower-ranked items from being placed in stockpile if there are items of a higher rank that aren't in any stockpile.
	
**Stockpile Stack Limit:** Not compatible (feature was incorporated with modification).
	
**Other Storage Mods:** Not tested.
	
# Known Issues:


Pawns ignore the stack limit when placing stacks from a recently completed bill, requiring follow-up hauling jobs to remove the excess. Can be avoided by setting bills to drop on the floor or to deliver to a non-limited stockpile.
	
# Credits:

	This mod borrows heavily from the Storage Stack Limit and Storage Hysteresis mods.
	https://steamcommunity.com/sharedfiles/filedetails/?id=2015532615]Sellophane's Remade and Updated Stockpile Stack Limit
	https://steamcommunity.com/sharedfiles/filedetails/?id=1852323982]Darksider's Version 
	https://steamcommunity.com/sharedfiles/filedetails/?id=1651076103]Original Stack Limit
	https://steamcommunity.com/sharedfiles/filedetails/?id=726479594]RimWorld Search Agency
	https://steamcommunity.com/sharedfiles/filedetails/?id=2003354028]Satisfied Storage	https://steamcommunity.com/sharedfiles/filedetails/?id=784324350]Original Storage Hysteresis

![Image](https://i.imgur.com/PwoNOj4.png)



-  See if the the error persists if you just have this mod and its requirements active.
-  If not, try adding your other mods until it happens again.
-  Post your error-log using https://steamcommunity.com/workshop/filedetails/?id=818773962]HugsLib and command Ctrl+F12
-  For best support, please use the Discord-channel for error-reporting.
-  Do not report errors by making a discussion-thread, I get no notification of that.
-  If you have the solution for a problem, please post it to the GitHub repository.




