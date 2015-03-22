using System;
using Security;
using Foundation;
using UIKit;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace KeychainPassword
{
	public static class KeychainUtils
	{
		public static bool StoreKeychain ()
		{

			var secret = NSData.FromString (UIDevice.CurrentDevice.IdentifierForVendor.ToString(), NSStringEncoding.Unicode);

			var record = new SecRecord (SecKind.GenericPassword) {
				Service = NSBundle.MainBundle.BundleIdentifier,
				Account = "SecurityViewAccount",
				AccessControl = new SecAccessControl (SecAccessible.WhenPasscodeSetThisDeviceOnly, SecAccessControlCreateFlags.UserPresence),
				UseNoAuthenticationUI = true,
				ValueData = secret
			};

			var res = SecKeyChain.Add (record);

			if (res == SecStatusCode.Success) {
				return true;
			} else {
				return false;
			}

		}

		public static SecStatusCode UpdateKeychain ()
		{

			var secret = NSData.FromString (UIDevice.CurrentDevice.IdentifierForVendor.ToString(), NSStringEncoding.Unicode);

			var update = new SecRecord (SecKind.GenericPassword) {
				ValueData = secret
			};

			var query = new SecRecord (SecKind.GenericPassword) {
				Service = NSBundle.MainBundle.BundleIdentifier,
				Account = "SecurityViewAccount",
				AccessControl = new SecAccessControl (SecAccessible.WhenPasscodeSetThisDeviceOnly, SecAccessControlCreateFlags.UserPresence),
				UseNoAuthenticationUI = true,
				ValueData = secret,
				UseOperationPrompt = "Authenticate to Update"
			};

			return SecKeyChain.Update (query, update);
		}

		public static string RetrieveKeychain (string message)
		{

			if (IsiPhone4Or4S ()) {
				message = "";
			}

			var query = new SecRecord (SecKind.GenericPassword) {
				Service = NSBundle.MainBundle.BundleIdentifier,
				Account = "SecurityViewAccount",
				AccessControl = new SecAccessControl (SecAccessible.WhenPasscodeSetThisDeviceOnly, SecAccessControlCreateFlags.UserPresence),
				UseOperationPrompt = message,
			};


			SecStatusCode status;

			var res = SecKeyChain.QueryAsData (query, false, out status);

			if (res != null) {
				return NSString.FromData(res, NSStringEncoding.Unicode).ToString ();
			}

			return null;
		}

		static bool IsiPhone4Or4S() {
			var deviceModelName = DeviceHardware.Version;

			return (deviceModelName == "iPhone3,1" || deviceModelName == "iPhone3,2" || deviceModelName == "iPhone3,3" || deviceModelName == "iPhone4,1");
		}

		public static class DeviceHardware
		{
			public const string HardwareProperty = "hw.machine";



			// Changing the constant to "/usr/bin/libSystem.dylib" allows this P/Invoke to work on Mac OS X
			// Using "hw.model" as property gives Macintosh model, "hw.machine" kernel arch (ppc, ppc64, i386, x86_64)
			[DllImport(Constants.SystemLibrary)]
			internal static extern int sysctlbyname( [MarshalAs(UnmanagedType.LPStr)] string property, // name of the property
				IntPtr output, // output
				IntPtr oldLen, // IntPtr.Zero
				IntPtr newp, // IntPtr.Zero
				uint newlen // 0
			);

			public static string Version
			{
				get
				{
					// get the length of the string that will be returned
					var pLen = Marshal.AllocHGlobal(sizeof(int));
					sysctlbyname(DeviceHardware.HardwareProperty, IntPtr.Zero, pLen, IntPtr.Zero, 0);

					var length = Marshal.ReadInt32(pLen);

					// check to see if we got a length
					if (length == 0)
					{
						Marshal.FreeHGlobal(pLen);
						return "Unknown";
					}

					// get the hardware string
					var pStr = Marshal.AllocHGlobal(length);
					sysctlbyname(DeviceHardware.HardwareProperty, pStr, pLen, IntPtr.Zero, 0);

					// convert the native string into a C# string
					var hardwareStr = Marshal.PtrToStringAnsi(pStr);

					var ret = hardwareStr;

					// cleanup
					Marshal.FreeHGlobal(pLen);
					Marshal.FreeHGlobal(pStr);

					return ret;
				}
			}
		}
	}
}

