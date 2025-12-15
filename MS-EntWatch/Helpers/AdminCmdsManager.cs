using Sharp.Shared.Enums;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace MS_EntWatch.Helpers
{
    public static class AdminCmdsManager
    {
        static readonly List<HelpersAdmComm> AdmComms = [];
        public static bool InstallCommandCallback(string name, string permission, IClientManager.DelegateClientCommand callback)
        {
            foreach (var comm in AdmComms.ToList())
            {
                if (comm.GetName().Equals(name, StringComparison.OrdinalIgnoreCase)) return false;
            }

            AdmComms.Add(new(name, permission, callback));

            return true;
        }

        public static void UnRegCommands()
        {
            foreach (var comm in AdmComms.ToList())
            {
                comm.UnRegCommand();
            }
        }
    }

    class HelpersAdmComm
    {
        readonly string Name;
        readonly string Permission;
        readonly IClientManager.DelegateClientCommand Callback;

        public HelpersAdmComm(string name, string permission, IClientManager.DelegateClientCommand callback)
        {
            Name = name;
            Permission = permission;
            Callback = callback;
            EntWatch._clients!.InstallCommandCallback(Name, OnCommand);
        }

        public void UnRegCommand()
        {
            EntWatch._clients!.RemoveCommandCallback(Name, OnCommand);
        }

        public string GetName()
        {
            return Name; 
        }

        private ECommandAction OnCommand(IGameClient client, StringCommand command)
        {
            return OnAdminCommand(client, command, Permission, Callback);
        }
        
        private static ECommandAction OnAdminCommand(IGameClient client, StringCommand command, string permission, IClientManager.DelegateClientCommand callback)
        {
            if (callback is not null)
            {
                if (EW.CheckPermission(client, permission)) InvokeClientCallback(client, command, callback);
                else UI.ReplyToCommand(client, "EntWatch.NoPermission", command.ChatTrigger, "{red}");
            }
            return ECommandAction.Stopped;
        }

        private static void InvokeClientCallback(IGameClient client, StringCommand command, IClientManager.DelegateClientCommand callbacks)
        {
            foreach (var callback in callbacks.GetInvocationList())
            {
                try
                {
                    ((IClientManager.DelegateClientCommand)callback).Invoke(client, command);
                }
                catch (Exception e)
                {
                    UI.EWSysInfo("EntWatch.Info.Error", 15, $"{e.Message}");
                }
            }
        }
    }
}
