using System.ServiceModel;

namespace MSILWindow.Window
{
    [ServiceContract(Namespace = "http://example.com/RoslynCodeExecution")]
    interface ICommandService
    {
        [OperationContract]
        void Execute(string code);
    }
}