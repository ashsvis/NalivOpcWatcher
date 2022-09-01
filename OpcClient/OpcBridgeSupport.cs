using System;

namespace OpcClient
{
    public sealed class OpcBridgeSupport : IDisposable
    {
        Type _opcBridgeServerType;
        readonly object _opcBridgeServerObject;

        public OpcBridgeSupport()
        {
            _opcBridgeServerType = Type.GetTypeFromProgID("OpcBridgeServer.Application");
            if (_opcBridgeServerType != null)
            {
                try
                {
                    _opcBridgeServerObject = Activator.CreateInstance(_opcBridgeServerType);
                    InitOpc();
                }
                catch
                {
                    _opcBridgeServerType = null;
                }
            }
        }

        public void Dispose()
        {
            if (_opcBridgeServerType == null) return;
            try
            {
                FinitOpc();
            }
            catch
            {
                _opcBridgeServerType = null;
            }
        }

        public void InitOpc()
        {
            if (_opcBridgeServerType != null)
            {
                _opcBridgeServerType.InvokeMember("InitOPC",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null, _opcBridgeServerObject, new object[] { });
            }
        }

        public void FinitOpc()
        {
            if (_opcBridgeServerType != null)
            {
                _opcBridgeServerType.InvokeMember("FinitOPC",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null, _opcBridgeServerObject, new object[] { });
            }
        }

        public string GetServers()
        {
            if (_opcBridgeServerType != null)
                return (string)_opcBridgeServerType.InvokeMember("GetServers",
                            System.Reflection.BindingFlags.InvokeMethod,
                            null, _opcBridgeServerObject, new object[] { });
            return String.Empty;
        }

        public string GetProps(string server)
        {
            if (_opcBridgeServerType != null)
                return (string)_opcBridgeServerType.InvokeMember("GetProps",
                        System.Reflection.BindingFlags.InvokeMethod,
                        null, _opcBridgeServerObject, new object[] { server });
            return String.Empty;
        }

        public void AddItem(string server, string group, string param)
        {
            if (_opcBridgeServerType != null)
            {
                _opcBridgeServerType.InvokeMember("AddItem",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null, _opcBridgeServerObject, new object[] { server, group, param });
            }
        }

        public string FetchItem(string server, string group, string param)
        {
            if (_opcBridgeServerType != null)
                return (string)_opcBridgeServerType.InvokeMember("FetchItem",
                        System.Reflection.BindingFlags.InvokeMethod,
                        null, _opcBridgeServerObject, new object[] { server, group, param });
            return String.Empty;
        }
    }
}
