
using System;
using System.ServiceModel;
namespace SpotiFire.Server
{
    public interface ISpotifireClient
    {
        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void Ping();

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void SongStarted(Track track, Guid containerId, int index);

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void PlaybackStarted();

        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void PlaybackEnded();
    }
}
