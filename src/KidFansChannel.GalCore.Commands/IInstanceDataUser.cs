namespace KidFansChannel.GalCore.Commands
{
	public interface IInstanceDataUser
	{
		bool DataReady { get; }

		void InitInstanceData();

		void FinalInstanceData();
	}
}
