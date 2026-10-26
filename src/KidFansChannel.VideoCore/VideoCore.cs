using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DirectShowLib;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.VideoCore
{
	public class VideoCore
	{
		public const int WM_GRAPHNOTIFY = 16507;

		private IGraphBuilder graphBuilder;

		private IBaseFilter vmr9;

		private Allocator allocator;

		private Device _device;

		private IntPtr _hwnd;

		private bool _isPlaying;

		public Texture Texture
		{
			get
			{
				return allocator.Texture;
			}
		}

		public SizeF UV
		{
			get
			{
				return new SizeF((float)allocator.VideoSize.Width / (float)allocator.TextureSize.Width, (float)allocator.VideoSize.Height / (float)allocator.TextureSize.Height);
			}
		}

		public bool IsPlaying
		{
			get
			{
				return _isPlaying;
			}
		}

		public double Length
		{
			get
			{
				if (graphBuilder != null)
				{
					IGraphBuilder positionInterface = graphBuilder;
					double result = default(double);
					int playPosition = ((IMediaPosition)((positionInterface is IMediaPosition) ? positionInterface : null)).get_CurrentPosition(out result);
					return result;
				}
				return 0.0;
			}
		}

		public double Position
		{
			get
			{
				if (graphBuilder != null)
				{
					IGraphBuilder positionInterface = graphBuilder;
					double result = default(double);
					int playPosition = ((IMediaPosition)((positionInterface is IMediaPosition) ? positionInterface : null)).get_CurrentPosition(out result);
					return result;
				}
				return 0.0;
			}
		}

		public event EventHandler Stopping;

		public event EventHandler Stopped;

		public void Initial(Device device, IntPtr hwnd)
		{
			_device = device;
			_hwnd = hwnd;
		}

		public void Final()
		{
			Stop();
		}

		public virtual void Play(string fileName)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected Obj, but got Unknown
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected Obj, but got Unknown
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected Obj, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected Obj, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected Obj, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected Obj, but got Unknown
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Expected Obj, but got Unknown
			if (_isPlaying)
			{
				Stop();
			}
			int hr = 0;
			graphBuilder = (IGraphBuilder)new FilterGraph();
			vmr9 = (IBaseFilter)new VideoMixingRenderer9();
			IVMRFilterConfig9 vmrConfig = (IVMRFilterConfig9)vmr9;
			hr = vmrConfig.SetRenderingMode((VMR9Mode)4);
			DsError.ThrowExceptionForHR(hr);
			hr = vmrConfig.SetNumberOfStreams(1);
			DsError.ThrowExceptionForHR(hr);
			IVMRSurfaceAllocatorNotify9 allocatorNotify = (IVMRSurfaceAllocatorNotify9)vmr9;
			this.allocator = new Allocator(_device);
			Allocator allocator = this.allocator;
			allocator.Render = (EventHandler)Delegate.Combine(allocator.Render, new EventHandler(OnRender));
			hr = allocatorNotify.AdviseSurfaceAllocator(IntPtr.Zero, (IVMRSurfaceAllocator9)(object)this.allocator);
			DsError.ThrowExceptionForHR(hr);
			hr = this.allocator.AdviseNotify(allocatorNotify);
			DsError.ThrowExceptionForHR(hr);
			IVMRMixerControl9 mixerControl = (IVMRMixerControl9)vmr9;
			hr = mixerControl.SetMixingPrefs((VMR9MixerPrefs)4117);
			DsError.ThrowExceptionForHR(hr);
			IGraphBuilder eventInterface = graphBuilder;
			hr = ((IMediaEventEx)((eventInterface is IMediaEventEx) ? eventInterface : null)).SetNotifyWindow(_hwnd, 16507, IntPtr.Zero);
			DsError.ThrowExceptionForHR(hr);
			hr = graphBuilder.AddFilter(vmr9, "Video Mixing Renderer 9");
			DsError.ThrowExceptionForHR(hr);
			hr = graphBuilder.RenderFile(fileName, (string)null);
			if (hr != 0 && MessageBox.Show("Error while playing video.\nFile may be corruptted or required decoding codec not installed.\nPlease check " + fileName + " or install latest windows media player.\nClick Cancel to ignore the video.", "Error", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
			{
				Stop();
				return;
			}
			DsError.ThrowExceptionForHR(hr);
			IGraphBuilder controlInterface = graphBuilder;
			hr = ((IMediaControl)((controlInterface is IMediaControl) ? controlInterface : null)).Run();
			DsError.ThrowExceptionForHR(hr);
			_isPlaying = true;
		}

		public virtual void Stop()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			int hr = 0;
			if (graphBuilder != null)
			{
				IGraphBuilder eventInterface = graphBuilder;
				hr = ((IMediaEventEx)((eventInterface is IMediaEventEx) ? eventInterface : null)).SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
				IGraphBuilder controlInterface = graphBuilder;
				hr = ((IMediaControl)((controlInterface is IMediaControl) ? controlInterface : null)).StopWhenReady();
				FilterState filterState = default(FilterState);
				do
				{
					IGraphBuilder controlForStop = graphBuilder;
					((IMediaControl)((controlForStop is IMediaControl) ? controlForStop : null)).Stop();
					IGraphBuilder controlForState = graphBuilder;
					((IMediaControl)((controlForState is IMediaControl) ? controlForState : null)).GetState(0, out filterState);
				}
				while ((int)filterState != 0);
			}
			if (allocator != null)
			{
				allocator.Dispose();
				allocator = null;
			}
			if (vmr9 != null)
			{
				Marshal.ReleaseComObject(vmr9);
				vmr9 = null;
			}
			if (graphBuilder != null)
			{
				List<IBaseFilter> filters = new List<IBaseFilter>();
				IEnumFilters filterEnum = default(IEnumFilters);
				hr = graphBuilder.EnumFilters(out filterEnum);
				DsError.ThrowExceptionForHR(hr);
				IBaseFilter[] filterArray = new IBaseFilter[1];
				while (filterEnum.Next(filterArray.Length, filterArray, IntPtr.Zero) == 0)
				{
					filters.Add(filterArray[0]);
				}
				foreach (IBaseFilter item in filters)
				{
					hr = graphBuilder.RemoveFilter(item);
					while (Marshal.ReleaseComObject(item) > 0)
					{
					}
				}
				Marshal.ReleaseComObject(graphBuilder);
				graphBuilder = null;
			}
			_isPlaying = false;
			if (Stopped != null)
			{
				Stopped(this, EventArgs.Empty);
			}
		}

		public void HandleGraphicEvent()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected Obj, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			IMediaEventEx eventInterface = (IMediaEventEx)graphBuilder;
			if (eventInterface == null)
			{
				return;
			}
			EventCode eventCode = default(EventCode);
			IntPtr intPtr = default(IntPtr);
			IntPtr intPtr2 = default(IntPtr);
			while (eventInterface.GetEvent(out eventCode, out intPtr, out intPtr2, 0) == 0)
			{
				eventInterface.FreeEventParams(eventCode, intPtr, intPtr2);
				if ((int)eventCode == 1 && Stopping != null)
				{
					Stopping(graphBuilder, EventArgs.Empty);
				}
				if (graphBuilder == null)
				{
					break;
				}
			}
		}

		protected virtual void OnRender(object sender, EventArgs e)
		{
		}
	}
}
