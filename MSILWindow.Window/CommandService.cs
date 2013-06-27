using System.ServiceModel;

namespace MSILWindow.Window
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class CommandService : ICommandService
    {
        WindowControl _uc;

        public CommandService(WindowControl uc)
        {
            _uc = uc;
        }

        public void Execute(string code)
        {
            _uc.Update(code);
        }
    }
}