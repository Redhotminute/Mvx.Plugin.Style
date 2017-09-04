using System;
using MvvmCross.Binding.Parse.Binding;

namespace Redhotminute.Mvx.Plugin.Style.Binding {

	public interface IMvxFontBindingParser
		: IMvxBindingParser {
		string DefaultConverterName { get; set; }
		string DefaultAssetPluginName { get; set; }
	}
}
