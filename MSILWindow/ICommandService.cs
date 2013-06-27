using System.ServiceModel;

namespace MSILWindow
{
    [ServiceContract(Namespace = "http://example.com/RoslynCodeExecution")]
    interface ICommandService
    {
        [OperationContract]
        string Execute(string code);
    }
}