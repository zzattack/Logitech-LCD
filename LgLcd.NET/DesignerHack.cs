using System;
using System.ComponentModel;

namespace LgLcd {

	// hack to allow forms inheriting abstract FormApplets to be designered
	// http://www.pocketsilicon.com/post/Using-Visual-Studio-Whidbey-to-Design-Abstract-Forms.aspx

	internal class ConcreteWinFormsApplet : WinFormsApplet {
		public override event EventHandler UpdateLcdScreen;
		public override void OnDeviceArrival(DeviceType deviceType) {}
		public override void OnDeviceRemoval(DeviceType deviceType) {}
		public override void OnAppletEnabled() {}
		public override void OnAppletDisabled() {}
		public override void OnCloseConnection() {}
		public override void OnConfigure() {}
		public override string AppletName {
			get { return ""; }
		}
	}
	
	internal class ConcreteClassProvider : TypeDescriptionProvider {
		public ConcreteClassProvider() :
			base(TypeDescriptor.GetProvider(typeof(WinFormsApplet))) {
		}

		public override Type GetReflectionType(Type objectType, object instance) {
			if (objectType == typeof(WinFormsApplet))
				return typeof(ConcreteWinFormsApplet);
			return base.GetReflectionType(objectType, instance);
		}

		public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args) {
			if (objectType == typeof(WinFormsApplet)) {
				objectType = typeof(ConcreteWinFormsApplet);
			}
			return base.CreateInstance(provider, objectType, argTypes, args);
		}
	}
}