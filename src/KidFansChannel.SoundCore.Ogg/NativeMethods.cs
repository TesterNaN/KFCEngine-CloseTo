using System.Runtime.InteropServices;

namespace KidFansChannel.SoundCore.Ogg
{
	internal class NativeMethods
	{
		public const int ifod_err_open_failed = 1;

		public const int ifod_err_malloc_failed = 2;

		public const int ifod_err_read_failed = 3;

		public const int ifod_err_not_vorbis_data = 4;

		public const int ifod_err_vorbis_version_mismatch = 5;

		public const int ifod_err_invalid_vorbis_header = 6;

		public const int ifod_err_internal_fault = 7;

		public const int ifod_err_unspecified_error = 8;

		private NativeMethods()
		{
		}

		[DllImport("PlayOgg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "oggOpenFile")]
		public unsafe static extern int OpenFile(string fileName, void** vf_out);

		[DllImport("PlayOgg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "oggOpenStream")]
		public unsafe static extern int OpenStream(byte[] stream, int sizeOfStream, void** vf_out);

		[DllImport("PlayOgg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "oggDecodePacket")]
		public unsafe static extern int DecodePacket(void* vf_ptr, void* buf_out, int buf_byte_size, int bits_per_sample, int* channels_cnt, int* sampling_rate, int* err_ov_hole_cnt, int* err_ov_ebadlink_cnt);

		[DllImport("PlayOgg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "oggFinal")]
		public unsafe static extern int Final(void* vf_ptr);

		[DllImport("PlayOgg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "oggSeekTime")]
		public unsafe static extern int SeekTime(void* vf_ptr, double time);

		[DllImport("PlayOgg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "oggTellTime")]
		public unsafe static extern double TellTime(void* vf_ptr);

		[DllImport("PlayOgg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "oggLengthTime")]
		public unsafe static extern double LengthTime(void* vf_ptr);
	}
}
