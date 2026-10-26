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
					IGraphBuilder val = graphBuilder;
					double result = default(double);
					int num = ((IMediaPosition)((val is IMediaPosition) ? val : null)).get_CurrentPosition(out result);
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
					IGraphBuilder val = graphBuilder;
					double result = default(double);
					int num = ((IMediaPosition)((val is IMediaPosition) ? val : null)).get_CurrentPosition(out result);
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
			int num = 0;
			graphBuilder = (IGraphBuilder)new FilterGraph();
			vmr9 = (IBaseFilter)new VideoMixingRenderer9();
			IVMRFilterConfig9 val = (IVMRFilterConfig9)vmr9;
			num = val.SetRenderingMode((VMR9Mode)4);
			DsError.ThrowExceptionForHR(num);
			num = val.SetNumberOfStreams(1);
			DsError.ThrowExceptionForHR(num);
			IVMRSurfaceAllocatorNotify9 val2 = (IVMRSurfaceAllocatorNotify9)vmr9;
			this.allocator = new Allocator(_device);
			Allocator allocator = this.allocator;
			allocator.Render = (EventHandler)Delegate.Combine(allocator.Render, new EventHandler(OnRender));
			num = val2.AdviseSurfaceAllocator(IntPtr.Zero, (IVMRSurfaceAllocator9)(object)this.allocator);
			DsError.ThrowExceptionForHR(num);
			num = this.allocator.AdviseNotify(val2);
			DsError.ThrowExceptionForHR(num);
			IVMRMixerControl9 val3 = (IVMRMixerControl9)vmr9;
			num = val3.SetMixingPrefs((VMR9MixerPrefs)4117);
			DsError.ThrowExceptionForHR(num);
			IGraphBuilder val4 = graphBuilder;
			num = ((IMediaEventEx)((val4 is IMediaEventEx) ? val4 : null)).SetNotifyWindow(_hwnd, 16507, IntPtr.Zero);
			DsError.ThrowExceptionForHR(num);
			num = graphBuilder.AddFilter(vmr9, "Video Mixing Renderer 9");
			DsError.ThrowExceptionForHR(num);
			num = graphBuilder.RenderFile(fileName, (string)null);
			if (num != 0 && MessageBox.Show("Error while playing video.\nFile may be corruptted or required decoding codec not installed.\nPlease check " + fileName + " or install latest windows media player.\nClick Cancel to ignore the video.", "Error", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
			{
				Stop();
				return;
			}
			DsError.ThrowExceptionForHR(num);
			IGraphBuilder val5 = graphBuilder;
			num = ((IMediaControl)((val5 is IMediaControl) ? val5 : null)).Run();
			DsError.ThrowExceptionForHR(num);
			_isPlaying = true;
		}

		public virtual void Stop()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			if (graphBuilder != null)
			{
				IGraphBuilder val = graphBuilder;
				num = ((IMediaEventEx)((val is IMediaEventEx) ? val : null)).SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
				IGraphBuilder val2 = graphBuilder;
				num = ((IMediaControl)((val2 is IMediaControl) ? val2 : null)).StopWhenReady();
				FilterState val5 = default(FilterState);
				do
				{
					IGraphBuilder val3 = graphBuilder;
					((IMediaControl)((val3 is IMediaControl) ? val3 : null)).Stop();
					IGraphBuilder val4 = graphBuilder;
					((IMediaControl)((val4 is IMediaControl) ? val4 : null)).GetState(0, out val5);
				}
				while ((int)val5 != 0);
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
				List<IBaseFilter> list = new List<IBaseFilter>();
				IEnumFilters val6 = default(IEnumFilters);
				num = graphBuilder.EnumFilters(out val6);
				DsError.ThrowExceptionForHR(num);
				IBaseFilter[] array = new IBaseFilter[1];
				while (val6.Next(array.Length, array, IntPtr.Zero) == 0)
				{
					list.Add(array[0]);
				}
				foreach (IBaseFilter item in list)
				{
					num = graphBuilder.RemoveFilter(item);
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
			IMediaEventEx val = (IMediaEventEx)graphBuilder;
			if (val == null)
			{
				return;
			}
			EventCode val2 = default(EventCode);
			IntPtr intPtr = default(IntPtr);
			IntPtr intPtr2 = default(IntPtr);
			while (val.GetEvent(out val2, out intPtr, out intPtr2, 0) == 0)
			{
				val.FreeEventParams(val2, intPtr, intPtr2);
				if ((int)val2 == 1 && Stopping != null)
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
