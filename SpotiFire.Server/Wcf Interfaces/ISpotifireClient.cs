
using System;
using System.ServiceModel;
namespace SpotiFire.Server
{
    public interface ISpotifireClient
    {
        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void Ping();

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void PlaybackStarted(Track track, Guid containerId, int index);
    }
}
