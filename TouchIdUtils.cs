using System;
using LocalAuthentication;
using Foundation;
using UIKit;

namespace KeychainPassword
{
	public class TouchIdUtils
	{
		public static bool SetupTouchIdAuthentication ()
		{

			if (!UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				return false;
			}

			if (!KeychainUtils.StoreKeychain ()) {
				return KeychainUtils.UpdateKeychain () == Security.SecStatusCode.Success;
			}

			return true;
		}

		public static bool IsPhoneSecured ()
		{

			if (!UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				return false;
			}

			return (IsBiometricSupportedAndPresent () || IsPasscodePresent ());
		}

		static bool IsBiometricSupportedAndPresent ()
		{
			var context = new LAContext ();
			NSError error;
			return context.CanEvaluatePolicy (LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error);
		}

		static bool IsPasscodePresent ()
		{
			var context = new LAContext ();
			NSError error;
			var result = context.CanEvaluatePolicy (LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error);

			switch ((LAStatus)((long)error.Code)) {
			case LAStatus.PasscodeNotSet:
				result = false;
				break;
			case LAStatus.TouchIDNotAvailable:
			case LAStatus.TouchIDNotEnrolled:
				result = true;
				break;

			}

			return result;
			
		}

		public static bool RequestAuthentication (string message)
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				return false;
			}

			return KeychainUtils.RetrieveKeychain (message) != null;
		}
	}
}

