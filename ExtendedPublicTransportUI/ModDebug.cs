namespace EPTUI
{
    static class ModDebug
    {
        public static void Message(string message)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "[EPT] " + message);
        }

        public static void Warning(string warning)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Warning, "[EPT] " + warning);
        }
        
        public static void Error(string error)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, "[EPT] " + error);
        }
    }
}
