using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Addon;

namespace PermissionPlugin
{
    public class PermissionPlugin : CPlugin
    {
        public List<string> UserGroups;
        public List<string> LogGroups;
        public List<string> SpecialChatGroups;
        public List<string> CommandList; //List with all commands present in Config (regardless of access type)

        bool specialChat = false;

        StreamWriter log;
        int logType;

        public override void OnServerLoad()
        {
            ServerPrint("Permission plugin loaded. Author: Pozzuh. Version 1.4");

            initLog();
            firstStart(); 
            initUserGroups();
            initLogGroups();
            initCommandList();
            initSpecialChatGroups();
        }

        void firstStart()
        {
            if (GetServerCFG("Permission", "Usergroups", "-1") == "-1")
                SetServerCFG("Permission", "Usergroups", "Admin,Moderator,User");

            if (GetServerCFG("Permission", "Admin_xuids", "-1") == "-1")
                SetServerCFG("Permission", "Admin_xuids", "xuid1,xuid2,xuid3");

            if (GetServerCFG("Permission", "Admin_commands", "-1") == "-1")
                SetServerCFG("Permission", "Admin_commands", "!help,!getxuid,!gettype");

            if (GetServerCFG("Permission", "Moderator_xuids", "-1") == "-1")
                SetServerCFG("Permission", "Moderator_xuids", "xuid1,xuid2,xuid3");

            if (GetServerCFG("Permission", "Moderator_commands", "-1") == "-1")
                SetServerCFG("Permission", "Moderator_commands", "!help,!getxuid,!gettype");

            if (GetServerCFG("Permission", "User_commands", "-1") == "-1")
                SetServerCFG("Permission", "User_commands", "!help,!getxuid,!gettype");

            if (GetServerCFG("Permission", "User_xuids", "-1") == "-1")
                SetServerCFG("Permission", "User_xuids", "*EVERYONE*");

            if (GetServerCFG("Permission", "Logging", "-1") == "-1")
                SetServerCFG("Permission", "Logging", "0"); // 0 = off, 1 = all, 2 = groups

            if (GetServerCFG("Permission", "Logging_groups", "-1") == "-1")
                SetServerCFG("Permission", "Logging_groups", "Admin,Moderator");

            if (GetServerCFG("Permission", "SpecialChat", "-1") == "-1")
                SetServerCFG("Permission", "SpecialChat", "0");

            if (GetServerCFG("Permission", "SpecialChatGroups", "-1") == "-1")
                SetServerCFG("Permission", "SpecialChatGroups", "Admin,Moderator");

            if (GetServerCFG("Permission", "Admin_SpecialSay", "-1") == "-1")
                SetServerCFG("Permission", "Admin_SpecialSay", "[{0}] ^8{1}^7: {2}");

            if (GetServerCFG("Permission", "Moderator_SpecialSay", "-1") == "-1")
                SetServerCFG("Permission", "Moderator_SpecialSay", "[{0}] ^8{1}^7: {2}");
        }

        void initUserGroups()
        {
            UserGroups = new List<string>();

            string UserGroups_string = GetServerCFG("Permission", "Usergroups", "Admin,Moderator,User");

            foreach (string Group in UserGroups_string.Split(','))
                UserGroups.Add(Group);
        }

        void initSpecialChatGroups()
        {
            SpecialChatGroups = new List<string>();

            string sSpecialEnabled = GetServerCFG("Permission", "SpecialChat", "0");
            int iSpecialEnabled;
            Int32.TryParse(sSpecialEnabled, out iSpecialEnabled);
            specialChat = Convert.ToBoolean(iSpecialEnabled);

            if (!specialChat)
                return;

            string sGroups = GetServerCFG("Permission", "SpecialChatGroups", " ");

            if (sGroups != " ")
                foreach (string group in sGroups.Split(','))
                    SpecialChatGroups.Add(group);
 
        }

        string getSpecialChatGroupString(string group) //{0} = group, {1} = name, {2} = message
        {
            return GetServerCFG("Permission", group + "_SpecialSay", "{1}: ^8{2}");
        }

        bool groupHasSpecialSay(string group)
        {
            return SpecialChatGroups.Contains(group);
        }
        void initLogGroups()
        {
            LogGroups = new List<string>();

            string LogGroups_string = GetServerCFG("Permission", "Logging_groups", "Admin,Moderator");

            foreach (string Group in LogGroups_string.Split(','))
                LogGroups.Add(Group);
        }

        void initLog()
        {
            string sLogType = GetServerCFG("Permission", "Logging", "0");
            Int32.TryParse(sLogType, out logType);

            if (logType == 0)
                return;

            if (!File.Exists(@"addon/logs/PermissionPlugin.log"))
                log = new StreamWriter(@"addon/logs/PermissionPlugin.log");
            else
                log = File.AppendText(@"addon/logs/PermissionPlugin.log");
            
            Log("Server startup.");
        }

        void initCommandList()
        {
            CommandList = new List<string>();

            foreach (string userGroup in UserGroups)
            {
                CommandList.AddRange(getCommandsAllowedInGroup(userGroup));
            }
        }

        void Log(string msg)
        {
            if (logType == 0)
                return;

            if(log.BaseStream == null)
                log = File.AppendText(@"addon/logs/PermissionPlugin.log");

            using (log)
            {
                log.WriteLine("{0} - {1}: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), msg);
            }
        }

        void logCommand(string xuid, string name, string cmd, bool wasAllowed, string userIsInGroup)
        {
            if (logType == 0)
                return;

            if (logType == 2)
            {
               // if (!LogGroups.Exists(element => element == userIsInGroup))
               //     return;
                if(!LogGroups.Contains(userIsInGroup))
                    return;
            }

            var s = new StringBuilder();
            s.Append(xuid);
            s.Append(" - ");
            s.Append(name);
            s.Append(" - ");
            s.Append(userIsInGroup);
            s.Append(" - ");
            s.Append(cmd);

            if (!wasAllowed)
            {
                s.Append(" - ");
                s.Append("TRIED TO USE, BUT WAS NOT ALLOWED.");
            }

            Log(s.ToString());
        }

        bool checkCommandExist(string cmd)
        {
            if (!CommandList.Contains(cmd))
            {
                List<string> list = CommandList.FindAll(s => s.EndsWith("*"));
                foreach (string s in list)
                {
                    if(cmd.StartsWith(s.Split('*')[0]))
                        return true;
                }
                return false;
            }
            return true;
        }

        bool canUseCommand(string cmd, string group)
        {
            List<string> allowedCommands = getCommandsAllowedInGroup(group);
            if (!allowedCommands.Contains(cmd))
            {
                List<string> list = allowedCommands.FindAll(s => s.EndsWith("*"));
                foreach (string s in list)
                {
                    if (cmd.StartsWith(s.Split('*')[0]))
                        return true;
                }

                return false;
            }
            return true;
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

        public override ChatType OnSay(string Message, ServerClient Client, bool teamChat)
        {
            string userIsInGroup = getUserGroup(Client.XUID);
            string lowMsg = Message.ToLower();
            List<string> allowed_commands = getCommandsAllowedInGroup(userIsInGroup);

            if (!lowMsg.StartsWith("!"))
            {
                if(specialChat && !teamChat && groupHasSpecialSay(userIsInGroup))
                {
                    string formatString = getSpecialChatGroupString(userIsInGroup);
                    ServerSay(string.Format(formatString, getUserGroup(Client.XUID), Client.Name, Message), true);
                    return ChatType.ChatNone;
                }
                
                return ChatType.ChatContinue;
            }

            if (lowMsg.StartsWith("!getxuid")) //Can't be used in the permission plugin OH THE IRONY (or something?)
            {
                logCommand(Client.XUID, Client.Name, "!getxuid", true, userIsInGroup);
                TellClient(Client.ClientNum, "Your xuid is: \'" + Client.XUID + "\'.", true);
                return ChatType.ChatNone;
            }

            if (lowMsg.StartsWith("!gettype"))
            {
                logCommand(Client.XUID, Client.Name, "!gettype", true, userIsInGroup);
                TellClient(Client.ClientNum, "Your user type is: \'" + userIsInGroup + "\'.", true);
                return ChatType.ChatNone;
            }

            if (lowMsg.StartsWith("!help") || lowMsg.StartsWith("!cmdlist"))
            {
                logCommand(Client.XUID, Client.Name, "!help", true, userIsInGroup);
                
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

            if (!checkCommandExist(lowMsg.Split(' ')[0]))
            {
                TellClient(Client.ClientNum, "^1That command doesn't exist.", true);
                return ChatType.ChatNone;
            }

            //if (allowed_commands.Contains(lowMsg.Split(' ')[0]))
            if(canUseCommand(lowMsg.Split(' ')[0], userIsInGroup))
            {
                logCommand(Client.XUID, Client.Name, lowMsg.Split(' ')[0], true, userIsInGroup);
                return ChatType.ChatContinue;
            }
            else
            {
                logCommand(Client.XUID, Client.Name, lowMsg.Split(' ')[0], false, userIsInGroup);
                TellClient(Client.ClientNum, "^1You aren't allowed to use that command!", true);
                return ChatType.ChatNone;
            }
        }
    }
}