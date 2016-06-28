Permission Plugin
================

This is a plugin for [Nukem's MW3 server addon](http://www.itsmods.com/forum/Thread-Release-MW3-Server-Addon--7734.html). It allows server admins to have control over who can use commands. It has support for multiple groups, like admin, moderators and users. This plugin will work with any other plugin automatically.

The idea behind this central plugin is that admins don't have to setup xuids for every single plugin seperatly.
It's now possible to modify the chat messages, you can for example add a tag for admins or moderators.

Requirements
------------
- Nukem's server addon v1.190+
- MW3 Dedicated server v1.4.382+
- .NET Framework 3.0


Admin usage
-----------
1. To install the plugin, simply place aaaPermissionPlugin.dll into %ServerRoot%/plugins/. **Note: do not rename the .dll file, this will cause issues.** 
2. Now, you can start the server and let the config file generate. 
3. Open sv_config.ini and edit the settings under *[Permission]* to your likings. 

- You can add new groups by adding them after *Usergroups=* and adding 2 new lines, *[New Group Name]_xuids=...* and *[New Group Name]_commands=...* 
- Wildcards are supported in commands, but only at the end. This is mostly for usage with "God Plugin". If you want a group to be able to use !mapdome and !mapterminal from God Plugin, you should add !map*, and so forth.
- The SpecialChat config option can enable modifying of messages. SpecialChatGroups set which groups should be modified. You can add more groups by adding them to SpecialChatGroups and adding a new line: *[Group Name]_SpecialSay=[{0}] {1}: ^8{2}*
- *[Group Name]_SpecialSay* modifies what a message looks like. The following strings get replaced by: {0} = the user's group, {1} = the username, {2} = the message.
- Adding ALL after *Admin_commands* is now unsupported. You have to add all commands manually. 
- When logging is enabled, usage of commands is logged to addon/logs/PermissionPlugin.log. 0 = off, 1 = on, 2 = on but only for certain groups. (Add those to *Logging_groups*) *Logging_full* determines if only the command should be logged, or the arguments as well. 0 = command only, 1 = command + arguments.
- Make sure to keep commands lowercase and don't add any spaces.

Player usage
-----------
- !help -- shows the commands you are allowed to use.
- !gettype -- shows what usergroup you are part of. (IE. admin, moderator or user)
- !getxuid -- Shows your personal xuid.


Plugin developer usage
----------------------
- Just make sure you don't add any "isAdmin" checks yourself. So basically make sure that when this plugin isn't installed, everyone is able to use all commands.
- Make your commands start with '!'.

Compiling
---------
- Make sure the project is set to .NET framework 3.0.
- Add addon.dll from %ServerRoot%/dist/ to your project.