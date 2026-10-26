using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace KidFansChannel.CloseTo
{
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[DebuggerNonUserCode]
	internal class Resource
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("KidFansChannel.CloseTo.Resource", typeof(Resource).Assembly);
					resourceMan = resourceManager;
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string ClearListStr
		{
			get
			{
				return ResourceManager.GetString("ClearListStr", resourceCulture);
			}
		}

		internal static Icon CloseToIcon
		{
			get
			{
				object obj = ResourceManager.GetObject("CloseToIcon", resourceCulture);
				return (Icon)obj;
			}
		}

		internal static string CloseToStr
		{
			get
			{
				return ResourceManager.GetString("CloseToStr", resourceCulture);
			}
		}

		internal static string CommentStr
		{
			get
			{
				return ResourceManager.GetString("CommentStr", resourceCulture);
			}
		}

		internal static string ConfigStr
		{
			get
			{
				return ResourceManager.GetString("ConfigStr", resourceCulture);
			}
		}

		internal static string DateStr
		{
			get
			{
				return ResourceManager.GetString("DateStr", resourceCulture);
			}
		}

		internal static string ExitStr
		{
			get
			{
				return ResourceManager.GetString("ExitStr", resourceCulture);
			}
		}

		internal static string LevelStr
		{
			get
			{
				return ResourceManager.GetString("LevelStr", resourceCulture);
			}
		}

		internal static string LoadStr
		{
			get
			{
				return ResourceManager.GetString("LoadStr", resourceCulture);
			}
		}

		internal static string MenuItemStr
		{
			get
			{
				return ResourceManager.GetString("MenuItemStr", resourceCulture);
			}
		}

		internal static string OpenGhostStr
		{
			get
			{
				return ResourceManager.GetString("OpenGhostStr", resourceCulture);
			}
		}

		internal static string OpenMaiStr
		{
			get
			{
				return ResourceManager.GetString("OpenMaiStr", resourceCulture);
			}
		}

		internal static string RoomBgStr
		{
			get
			{
				return ResourceManager.GetString("RoomBgStr", resourceCulture);
			}
		}

		internal static string SaveFileStr
		{
			get
			{
				return ResourceManager.GetString("SaveFileStr", resourceCulture);
			}
		}

		internal static string SaveStateStr
		{
			get
			{
				return ResourceManager.GetString("SaveStateStr", resourceCulture);
			}
		}

		internal static string SaveStr
		{
			get
			{
				return ResourceManager.GetString("SaveStr", resourceCulture);
			}
		}

		internal static byte[] script_lang_dat
		{
			get
			{
				object obj = ResourceManager.GetObject("script_lang_dat", resourceCulture);
				return (byte[])obj;
			}
		}

		internal static string StateStr
		{
			get
			{
				return ResourceManager.GetString("StateStr", resourceCulture);
			}
		}

		internal static byte[] Transparent
		{
			get
			{
				object obj = ResourceManager.GetObject("Transparent", resourceCulture);
				return (byte[])obj;
			}
		}

		internal Resource()
		{
		}
	}
}
