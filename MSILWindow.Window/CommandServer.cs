using System;
using System.ServiceModel;

namespace MSILWindow.Window
{
    class CommandServer
    {
        private static readonly Uri ServiceUri = new Uri("net.pipe://localhost/Pipe");
        private const string PipeName = "UserControl1Service";
        private static WindowControl _uc;
        private static CommandService Service;
        private static ServiceHost _host;

        public void Start()
        {
            _host = new ServiceHost(Service, ServiceUri);
            _host.AddServiceEndpoint(typeof(ICommandService), new NetNamedPipeBinding(), PipeName);
            _host.Open();
        }

        public void Stop()
        {
            if ((_host == null) || (_host.State == CommunicationState.Closed))
                return;

            _host.Close();
            _host = null;
        }

        public CommandServer(WindowControl uc)
        {
            _uc = uc;
            Service = new CommandService(_uc);
        }
    }
}