using System;
using System.Collections.Generic;
using Addon;
//Pozzuh xuid = 0110000102fd9a03
namespace _PermissionPlugin
{
    public class _PermissionPlugin : CPlugin
    {
        public List<string> Admins;
        public List<string> UserGroups;

        public override void OnServerLoad()
        {
            ServerPrint("Permission plugin loaded. Author: Pozzuh. Version 1.1 Beta");

            initUserGroups();
            firstStart();
        }

        void firstStart()
        {
            if (GetServerCFG("Permission", "Usergroups", "x") == "x")
                SetServerCFG("Permission", "Usergroups", "Admin,Moderator,User");

            if (GetServerCFG("Permission", "Admin_xuids", "x") == "x")
                SetServerCFG("Permission", "Admin_xuids", "xuid1,xuid2,xuid3");

            if (GetServerCFG("Permission", "Admin_commands", "x") == "x")
                SetServerCFG("Permission", "Admin_commands", "*ALL*");

            if (GetServerCFG("Permission", "Moderator_xuids", "x") == "x")
                SetServerCFG("Permission", "Moderator_xuids", "xuid1,xuid2,xuid3");

            if (GetServerCFG("Permission", "Moderator_commands", "x") == "x")
                SetServerCFG("Permission", "Moderator_commands", "!help,!getxuid,!gettype");

            if (GetServerCFG("Permission", "User_commands", "x") == "x")
                SetServerCFG("Permission", "User_commands", "!help,!getxuid,!gettype");

            if (GetServerCFG("Permission", "User_xuids", "x") == "x")
                SetServerCFG("Permission", "User_xuids", "*EVERYONE*");
        }

        void initUserGroups()
        {
            UserGroups = new List<string>();

            string UserGroups_string = GetServerCFG("Permission", "Usergroups", "Admin,Moderator,User");

            foreach (string Group in UserGroups_string.Split(','))
                UserGroups.Add(Group);
        }

        List<string> getUsersInGroup(string groupname)
        {
            List<string> group = new List<string>();

            string group_string = GetServerCFG("Permission", groupname + "_xuids", " ");

            if (group_string != " ")
                foreach (string xuid in group_string.Split(','))
                    group.Add(xuid);
            else
                if (groupname != "User")
                    ServerPrint("No users set for group: " + groupname);

            return group;
        }

        List<string> getCommandsAllowedInGroup(string groupname)
        {
            List<string> group = new List<string>();

            string group_string = GetServerCFG("Permission", groupname + "_commands", " ");

            if (group_string != " ")
                foreach (string cmd in group_string.Split(','))
                    group.Add(cmd);
            else
                ServerPrint("No commands set for group: " + groupname);

            return group;
        }

        string getUserGroup(string xuid)
        {
            foreach (string group in UserGroups)
            {
                List<string> usersInGroup = new List<string>();
                usersInGroup = getUsersInGroup(group);

                if (usersInGroup.Contains(xuid))
                    return group;
            }

            return "User";
        }

        public override ChatType OnSay(string Message, ServerClient Client)
        {
            string userIsInGroup = getUserGroup(Client.XUID);
            string lowMsg = Message.ToLower();

            if (lowMsg.StartsWith("!getxuid")) //Can't be used in the permission plugin OH THE IRONY
            {
                TellClient(Client.ClientNum, "Your xuid is: \'" + Client.XUID + "\'.", true);
                return ChatType.ChatNone;
            }

            if (lowMsg.StartsWith("!gettype")) //Can't be used in the permission plugin OH THE IRONY
            {
                TellClient(Client.ClientNum, "Your user type is: \'" + userIsInGroup + "\'.", true);
                return ChatType.ChatNone;
            }

            if (lowMsg.StartsWith("!help") || lowMsg.StartsWith("!cmdlist"))
            {
                List<string> allowed_commands = getCommandsAllowedInGroup(userIsInGroup);
                string msg = "You can use the following commands^1:^7 ";

                for (int i = 0; i < allowed_commands.Count; i++)
                {
                    if (i == (allowed_commands.Count - 2))
                        msg += allowed_commands[i] + "^1 and^7 ";
                    else if (i == (allowed_commands.Count - 1))
                        msg += allowed_commands[i] + "^1.";
                    else
                        msg += allowed_commands[i] + "^1,^7 ";
                }

                TellClient(Client.ClientNum, msg, true);
                return ChatType.ChatNone;
            }

            if (userIsInGroup != "Admin")
            {
                if (!lowMsg.StartsWith("!"))
                    return ChatType.ChatAll;

                List<string> allowed_commands = getCommandsAllowedInGroup(userIsInGroup);

                if (allowed_commands.Contains(lowMsg.Split(' ')[0]))
                    return ChatType.ChatContinue;
                else
                {
                    TellClient(Client.ClientNum, "^1You aren't allowed to use that command!", true);
                    return ChatType.ChatNone;
                }
            }

            return ChatType.ChatContinue;
        }
    }
}