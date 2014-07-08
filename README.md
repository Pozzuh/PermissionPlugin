Permission Plugin
================

This is a plugin for [Nukem's MW3 server addon](http://www.itsmods.com/forum/Thread-Release-MW3-Server-Addon--7734.html). It allows server admins to have control over who can use commands. It has support for multiple groups, like admin, moderators and users. This plugin will work with any other plugin automatically.

The idea behind this central plugin is that admins don't have to setup xuids for every single plugin seperatly.


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

- You can add new groups by adding them after *Usergroups=* and adding 2 new lines, *<New Group Name>_xuids=...* and *<New Group Name>_commands=...* 
- Adding ALL after *Admin_commands* is now unsupported. You have to add all commands manually. 
- Logging means logging of command usage. 0 = off, 1 = on, 2 = on but only for certain groups. (Add those to *Loging_groups*)
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