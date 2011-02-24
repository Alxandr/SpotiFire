
using System.ServiceModel;
namespace SpotiFire.Server
{
    public interface ISpotifireClient
    {
        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void LoginComplete();

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void LoginFailed();

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void RequireLogin();

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void Ping();
    }
}
