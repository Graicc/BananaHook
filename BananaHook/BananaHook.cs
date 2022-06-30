using BepInEx;

namespace BananaHook
{
    /* That's me! */
    [BepInPlugin("net.rusjj.gtlib.bananahook", "BananaHook Lib", "1.3.1")]

    public class BananaHook : BaseUnityPlugin
    {
        private static BananaHook m_hInstance;
        internal static void Log(string msg) => m_hInstance.Logger.LogMessage(msg);

        void Awake()
        {
            m_hInstance = this;
            Patch.Apply();
            new HookAndPatch.EventListener();
        }
    }
}