using System;
using UIKit;

namespace Redhotminute.Mvx.Plugin.Style.Touch {
	public static class FontAlignmentHelper {
		public static UITextAlignment ToNativeAlignment(this Font font) {

			switch (font.Alignment) {
				case TextAlignment.Left: return UITextAlignment.Left;break;
				case TextAlignment.Center: return UITextAlignment.Center; break;
				case TextAlignment.Right: return UITextAlignment.Right; break;
				case TextAlignment.Justified: return UITextAlignment.Justified; break;
			}
			return UITextAlignment.Left;
		}
	}
}
