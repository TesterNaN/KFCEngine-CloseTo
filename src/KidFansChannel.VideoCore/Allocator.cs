using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using DirectShowLib;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.VideoCore
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public class Allocator : IVMRSurfaceAllocator9, IVMRImagePresenter9, IDisposable
	{
		private const int E_FAIL = -2147467259;

		private const int DxMagicNumber = -759872593;

		private Device device;

		private AdapterInformation adapterInfo;

		private Hashtable textures;

		private Hashtable surfaces;

		private IntPtr[] unmanagedSurfaces;

		private Texture privateTexture;

		private Surface privateSurface;

		private IVMRSurfaceAllocatorNotify9 vmrSurfaceAllocatorNotify;

		public EventHandler Render;

		private IntPtr privateTexturePtr;

		private Size textureSize;

		private Size videoSize;

		public Texture Texture
		{
			get
			{
				if (privateTexture != null)
				{
					return privateTexture;
				}
				if (privateTexturePtr != IntPtr.Zero)
				{
					return textures[privateTexturePtr] as Texture;
				}
				return null;
			}
		}

		public Size TextureSize
		{
			get
			{
				return textureSize;
			}
		}

		public Size VideoSize
		{
			get
			{
				return videoSize;
			}
		}

		public Allocator(Device dev)
		{
			adapterInfo = Manager.Adapters[dev.CreationParameters.AdapterOrdinal];
			device = dev;
		}

		~Allocator()
		{
			Dispose();
		}

		private void DeleteSurfaces()
		{
			lock (this)
			{
				if (privateTexture != null)
				{
					privateTexture.Dispose();
					privateTexture = null;
				}
				if (privateSurface != null)
				{
					privateSurface.Dispose();
					privateSurface = null;
				}
				if (textures != null)
				{
					foreach (Texture value in textures.Values)
					{
						value.Dispose();
					}
					textures = null;
				}
				if (surfaces == null)
				{
					return;
				}
				foreach (Surface value2 in surfaces.Values)
				{
					value2.Dispose();
				}
				surfaces = null;
			}
		}

		public int InitializeDevice(IntPtr dwUserID, ref VMR9AllocationInfo lpAllocInfo, ref int lpNumBuffers)
		{
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			int num = 1;
			int num2 = 1;
			if (vmrSurfaceAllocatorNotify == null)
			{
				return -2147467259;
			}
			int num3 = 0;
			try
			{
				IntPtr objectByValue = device.GetObjectByValue(-759872593);
				IntPtr adapterMonitor = Manager.GetAdapterMonitor(adapterInfo.Adapter);
				num3 = vmrSurfaceAllocatorNotify.SetD3DDevice(objectByValue, adapterMonitor);
				DsError.ThrowExceptionForHR(num3);
				videoSize = new Size(lpAllocInfo.dwWidth, lpAllocInfo.dwHeight);
				if (device.DeviceCaps.TextureCaps.SupportsPower2)
				{
					while (num < lpAllocInfo.dwWidth)
					{
						num <<= 1;
					}
					while (num2 < lpAllocInfo.dwHeight)
					{
						num2 <<= 1;
					}
					lpAllocInfo.dwWidth = num;
					lpAllocInfo.dwHeight = num2;
				}
				textureSize = new Size(lpAllocInfo.dwWidth, lpAllocInfo.dwHeight);
				lpAllocInfo.dwFlags = (VMR9SurfaceAllocationFlags)((int)lpAllocInfo.dwFlags | 4);
				DeleteSurfaces();
				unmanagedSurfaces = new IntPtr[lpNumBuffers];
				num3 = vmrSurfaceAllocatorNotify.AllocateSurfaceHelper(ref lpAllocInfo, ref lpNumBuffers, unmanagedSurfaces);
				if (num3 < 0)
				{
					DeleteSurfaces();
					if (lpAllocInfo.Format > 808464432)
					{
						privateTexture = new Texture(device, lpAllocInfo.dwWidth, lpAllocInfo.dwHeight, 1, Usage.RenderTarget, adapterInfo.CurrentDisplayMode.Format, Pool.Default);
						privateSurface = privateTexture.GetSurfaceLevel(0);
					}
					lpAllocInfo.dwFlags = (VMR9SurfaceAllocationFlags)((int)lpAllocInfo.dwFlags & -5);
					lpAllocInfo.dwFlags = (VMR9SurfaceAllocationFlags)((int)lpAllocInfo.dwFlags | 8);
					unmanagedSurfaces = new IntPtr[lpNumBuffers];
					num3 = vmrSurfaceAllocatorNotify.AllocateSurfaceHelper(ref lpAllocInfo, ref lpNumBuffers, unmanagedSurfaces);
					if (num3 < 0)
					{
						return num3;
					}
				}
				else
				{
					surfaces = new Hashtable(unmanagedSurfaces.Length);
					textures = new Hashtable(unmanagedSurfaces.Length);
					for (int i = 0; i < lpNumBuffers; i++)
					{
						Surface surface = new Surface(unmanagedSurfaces[i]);
						Texture value = (Texture)surface.GetContainer(new Guid("85C31227-3DE5-4f00-9B3A-F11AC38C18B5"));
						surfaces.Add(unmanagedSurfaces[i], surface);
						textures.Add(unmanagedSurfaces[i], value);
					}
				}
				return 0;
			}
			catch (DirectXException ex)
			{
				return ex.ErrorCode;
			}
			catch
			{
				return -2147467259;
			}
		}

		public int TerminateDevice(IntPtr dwID)
		{
			DeleteSurfaces();
			return 0;
		}

		public int GetSurface(IntPtr dwUserID, int SurfaceIndex, int SurfaceFlags, out IntPtr lplpSurface)
		{
			lplpSurface = IntPtr.Zero;
			if (SurfaceIndex > unmanagedSurfaces.Length)
			{
				return -2147467259;
			}
			lock (this)
			{
				lplpSurface = unmanagedSurfaces[SurfaceIndex];
				Marshal.AddRef(lplpSurface);
				return 0;
			}
		}

		public int AdviseNotify(IVMRSurfaceAllocatorNotify9 lpIVMRSurfAllocNotify)
		{
			lock (this)
			{
				vmrSurfaceAllocatorNotify = lpIVMRSurfAllocNotify;
				IntPtr objectByValue = device.GetObjectByValue(-759872593);
				IntPtr adapterMonitor = Manager.GetAdapterMonitor(adapterInfo.Adapter);
				return vmrSurfaceAllocatorNotify.SetD3DDevice(objectByValue, adapterMonitor);
			}
		}

		public int StartPresenting(IntPtr dwUserID)
		{
			lock (this)
			{
				if (device == null)
				{
					return -2147467259;
				}
				return 0;
			}
		}

		public int StopPresenting(IntPtr dwUserID)
		{
			return 0;
		}

		public int PresentImage(IntPtr dwUserID, ref VMR9PresentationInfo lpPresInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			lock (this)
			{
				try
				{
					if (NeedToHandleDisplayChange())
					{
						IntPtr objectByValue = device.GetObjectByValue(-759872593);
						IntPtr adapterMonitor = Manager.GetAdapterMonitor(adapterInfo.Adapter);
						vmrSurfaceAllocatorNotify.ChangeD3DDevice(objectByValue, adapterMonitor);
					}
					return PresentHelper(lpPresInfo);
				}
				catch (DirectXException ex)
				{
					return ex.ErrorCode;
				}
				catch
				{
					return -2147467259;
				}
			}
		}

		private int PresentHelper(VMR9PresentationInfo lpPresInfo)
		{
			try
			{
				if (privateTexture != null)
				{
					Marshal.AddRef(lpPresInfo.lpSurf);
					using (Surface surface = new Surface(lpPresInfo.lpSurf))
					{
						device.StretchRectangle(surface, new Rectangle(0, 0, surface.Description.Width, surface.Description.Height), privateSurface, new Rectangle(0, 0, privateSurface.Description.Width, privateSurface.Description.Height), TextureFilter.Linear);
					}
				}
				else
				{
					if (!textures.ContainsKey(lpPresInfo.lpSurf))
					{
						return -2147467259;
					}
					privateTexturePtr = lpPresInfo.lpSurf;
				}
				if (Render != null)
				{
					Render(this, EventArgs.Empty);
				}
				return 0;
			}
			catch (DirectXException ex)
			{
				return ex.ErrorCode;
			}
			catch
			{
				return -2147467259;
			}
		}

		private bool NeedToHandleDisplayChange()
		{
			if (vmrSurfaceAllocatorNotify == null)
			{
				return false;
			}
			IntPtr adapterMonitor = Manager.GetAdapterMonitor(device.CreationParameters.AdapterOrdinal);
			IntPtr adapterMonitor2 = Manager.GetAdapterMonitor(adapterInfo.Adapter);
			return adapterMonitor != adapterMonitor2;
		}

		public void Dispose()
		{
			DeleteSurfaces();
		}
	}
}
