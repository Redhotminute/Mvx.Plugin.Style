using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Platform.UI;
using Redhotminute.Mvx.Plugin.Style.Models;

namespace Redhotminute.Mvx.Plugin.Style.Plugin
{
	public abstract class AssetPlugin : IAssetPlugin
	{
		RedhotminuteStyleConfiguration _configuration;

		public AssetPlugin() {
			_configuration = new RedhotminuteStyleConfiguration() { FontSizeFactor = 1.0f, LineHeightFactor = 1.0f};
			FontSizeFactor = _configuration.FontSizeFactor.Value;
			LineHeightFactor = _configuration.LineHeightFactor;
		}

		public void Setup(RedhotminuteStyleConfiguration configuration) 
        {
			if (configuration != null) 
            {
				_configuration = configuration;
			}
			if (_configuration.FontSizeFactor.HasValue) 
            {
				FontSizeFactor = _configuration.FontSizeFactor.Value;
			}
			LineHeightFactor = _configuration.LineHeightFactor;
		}


		public static float FontSizeFactor { get; set; }

		public static float? LineHeightFactor { get; set; }

		public static float GetPlatformFontSize(float fontSize) =>
			fontSize * FontSizeFactor;

		private Dictionary<string,IBaseFont> _fonts;
		private Dictionary<string,IBaseFont> Fonts 
        {
			get
            {
				if (_fonts == null)
                {
					_fonts = new Dictionary<string, IBaseFont>();
				}
				return _fonts;
			}
		}

        private Dictionary<string,List<FontTag>> _fontsTagged;
        private Dictionary<string,List<FontTag>> FontsTagged 
        {
			get 
            {
				if (_fontsTagged == null) 
                {
                    _fontsTagged = new Dictionary<string,List<FontTag>>();
				}
				return _fontsTagged;
			}
		}

		private Dictionary<string, MvxColor> _colors;
		private Dictionary<string, MvxColor> Colors 
        {
			get
            {
				if (_colors == null) 
                {
					_colors = new Dictionary<string, MvxColor>();
				}
				return _colors;
			}
		}

        public abstract void ConvertFontFileNameForPlatform(ref IBaseFont font);

        public virtual bool CanAddFont(IBaseFont font)
        {
            if (string.IsNullOrEmpty(font.Name))
            {
                throw new Exception("Added font should have a reference name");
            }

            if (string.IsNullOrEmpty(font.FontFilename))
            {
                throw new Exception("Added font should have a filename");
            }

            return true;
        }

		#region IAssetPlugin implementation

        private bool GetColorFromFontName(ref string fontColor,ref string fontName,string fontAndColor)
		{
			if (fontAndColor.Contains(":"))
			{
				var elements = fontAndColor.Split(':');
                if (elements.Length > 1)
				{
                    fontName = elements[0];
				}

                if(!string.IsNullOrWhiteSpace(elements[1])){
                    fontColor = elements[1];
                    return true;
                }    
            }else
            {
                fontName = fontAndColor;
            }

            return false;
		}

		public IBaseFont GetFontByName(string fontAndColorId)
		{
            string fontColor = string.Empty;
            string fontName = string.Empty;
            bool foundColor = GetColorFromFontName(ref fontColor,ref fontName,fontAndColorId);

            string combinedFontId = foundColor ? fontAndColorId : fontName;

            //if a color is set, it's a unique font
            Fonts.TryGetValue(combinedFontId, out IBaseFont font);

            //if the font is not found, but has a modified color, store it
            if (font == null && foundColor)
            {
                Fonts.TryGetValue(fontName, out IBaseFont fontWithoutColor);
                if (fontWithoutColor != null)
                {
                    if (fontWithoutColor is Font)
                    {
                        font = Font.CopyFont<Font, Font>((Font)fontWithoutColor, combinedFontId);
                        font.Color = GetColor(fontColor);
                    }
                    else if (fontWithoutColor is BaseFont)
                    {
                        font = BaseFont.CopyFont<BaseFont, BaseFont>((BaseFont)fontWithoutColor, combinedFontId);
                        font.Color = GetColor(fontColor);
					}

                    AddFont(font);
                }
            }

			return font;
		}

        public IBaseFont GetFontByTag(string originalFontName, string tag) =>
            GetFontByTagWithTag(originalFontName, tag, out FontTag nothing);

        public IBaseFont GetFontByTagWithTag(string originalFontName,string tag, out FontTag originalTag) 
        {
            string fontName = string.Empty;
            originalTag = null;

            if (FontsTagged.TryGetValue(originalFontName, out List<FontTag> fontTag))
            {
                originalTag = fontTag.FirstOrDefault(c => c.Tag.Equals(tag));
                if(originalTag!= null)
                {
                    fontName = originalTag.OriginalFontName;
                }
            }

			if (string.IsNullOrEmpty(fontName)) 
            {
				return null;
			}

			var font= GetFontByName(fontName);
			return font;
		}

		public IAssetPlugin AddFont(IBaseFont font,List<FontTag> fontTags) 
        {
            if (!CanAddFont(font))
            {
                return this;
            }

			//convert the filename so the platform would understand this
			ConvertFontFileNameForPlatform(ref font);
			Fonts.Add(font.Name, font);

			//for each tag, add a font
			if (fontTags != null && fontTags.Count > 0) 
            {
				if (FontsTagged != null) 
                {
					if (!FontsTagged.ContainsKey(font.Name)) 
                    {
                        FontsTagged[font.Name] = new List<FontTag>();
					}
                    FontsTagged[font.Name].AddRange(fontTags);
				}
			}
			return this;
		}

		public IAssetPlugin AddFont(IBaseFont font,FontTag fontTag) 
        {
            List<FontTag> tags = new List<FontTag>
            {
                fontTag
            };
            return AddFont(font, tags);
		}

		public IAssetPlugin AddFont(IBaseFont font) =>
			AddFont(font, new List<FontTag>());

		public virtual IAssetPlugin ClearFonts() 
        {
			_fonts = null;
			_fontsTagged = null;
			return this;
		}

		public virtual IAssetPlugin ClearColors() 
        {
			_colors = null;
			return this;
		}

		public IAssetPlugin AddColor(MvxColor color, string id) 
        {
			Colors.Add(id, color);
			return this;
		}

		public MvxColor GetColor(string colorId) 
        {
            Colors.TryGetValue(colorId, out MvxColor color);
            return color;
		}

		#endregion
	}
}

