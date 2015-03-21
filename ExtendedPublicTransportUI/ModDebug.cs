using System;
using ColossalFramework.UI;

namespace EPTUI
{
    static class ModDebug
    {
        public static void Message(string message)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "[ITUI] " + message);
        }

        public static void Warning(string warning)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Warning, "[ITUI] " + warning);
        }
        
        public static void Error(string error)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, "[ITUI] " + error);
        }

        // http://www.skylinesmodding.com/t/how-to-use-colossalframework-ui/55/4
        public static void DumpAllSprites(string path)
        {
            System.Collections.Generic.List<UITextureAtlas.SpriteInfo> spritelist = UIView.GetAView().defaultAtlas.sprites;
            foreach (UITextureAtlas.SpriteInfo sprite in spritelist)
            {
                try
                {
                    byte[] pngbytes = sprite.texture.EncodeToPNG();
                    String filename = MakeValidFileName(sprite.name);
                    System.IO.File.WriteAllBytes(path + filename + ".png", pngbytes);
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }
            }
        }

        // http://stackoverflow.com/a/847251
        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
