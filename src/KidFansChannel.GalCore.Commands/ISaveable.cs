using System.IO;

namespace KidFansChannel.GalCore.Commands
{
	public interface ISaveable
	{
		void Save(BinaryWriter bw);

		void Load(BinaryReader br);

		void ConfirmUpdate();
	}
}
