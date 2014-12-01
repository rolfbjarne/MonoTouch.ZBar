using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace ZBar
{
	public partial class ZBarSymbol
	{
		internal static ZBarSymbol CreateWithSymbol (IntPtr symbol)
		{
			var handle = objc_msgSend (Class.GetHandle (typeof (ZBarSymbol)), Selector.GetHandle ("alloc"));
			handle = objc_msgSend (handle, Selector.GetHandle ("initWithSymbol:"), symbol);
			var rv = new ZBarSymbol (handle);
			rv.DangerousRelease ();
			return rv;
		}

		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend (IntPtr ptr, IntPtr sel);   

		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend (IntPtr ptr, IntPtr sel, IntPtr symbol);
	}
}
