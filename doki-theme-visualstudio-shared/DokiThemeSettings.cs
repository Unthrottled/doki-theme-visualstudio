﻿using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace doki_theme_visualstudio {
  public class SettingsService {
    private static SettingsService? _instance;

    public static SettingsService Instance =>
      _instance ?? throw new Exception("Expected settings to be initialized!");

    public static void Init(Package package) {
      _instance ??= new SettingsService(package);
    }

    public static bool IsInitialized() => _instance != null;

    private readonly Package _package;

    private SettingsService(Package package) {
      _package = package;
    }

    public event EventHandler<SettingsService>? SettingsChanged;

    public void ShitChangedYo() {
      SettingsChanged?.Invoke(this, this);
    }

    public bool DrawSticker {
      get {
        var page = (DokiThemeSettings)_package.GetDialogPage(typeof(DokiThemeSettings));
        return page.DrawSticker;
      }
    }

    public bool DrawWallpaper {
      get {
        var page = (DokiThemeSettings)_package.GetDialogPage(typeof(DokiThemeSettings));
        return page.DrawWallpaper;
      }
    }

    public string CustomStickerImageAbsolutePath {
      get {
        var page = (DokiThemeSettings)_package.GetDialogPage(typeof(DokiThemeSettings));
        return page.CustomStickerImageAbsolutePath;
      }
    }

    public string CustomWallpaperImageAbsolutePath {
      get {
        var page = (DokiThemeSettings)_package.GetDialogPage(typeof(DokiThemeSettings));
        return page.CustomWallpaperImageAbsolutePath;
      }
    }

    public double WallpaperOpacity {
      get {
        var page = (DokiThemeSettings)_package.GetDialogPage(typeof(DokiThemeSettings));
        return page.WallpaperOpacity;
      }
    }
  }


  class DokiThemeSettings : DialogPage {
    bool _drawSticker = true;

    [DescriptionAttribute("Draw the cute sticker in the bottom right hand corner of your editor?")]
    public bool DrawSticker {
      get { return _drawSticker; }
      set { _drawSticker = value; }
    }

    bool _drawWallpaper = true;

    [DescriptionAttribute("Draw the beautiful wallpaper in the background of your editor?")]
    public bool DrawWallpaper {
      get { return _drawWallpaper; }
      set { _drawWallpaper = value; }
    }

    [DescriptionAttribute("Use custom image for wallpaper, to use default image clear the value from this setting")]
    [EditorAttribute(typeof(BrowseFile), typeof(UITypeEditor))]
    public string CustomWallpaperImageAbsolutePath { get; set; }

    [DescriptionAttribute("Use custom image for sticker, to use default image clear the value from this setting")]
    [EditorAttribute(typeof(BrowseFile), typeof(UITypeEditor))]
    public string CustomStickerImageAbsolutePath { get; set; }


    private double _wallpaperOpacity = -1.0;

    [DescriptionAttribute("Customize the wallpaper opacity, set to -1.0 to use default value for theme")]
    [EditorAttribute(typeof(BrowseFile), typeof(UITypeEditor))]
    public double WallpaperOpacity {
      get { return _wallpaperOpacity; }
      set { _wallpaperOpacity = value; }
    }

    protected override void OnApply(PageApplyEventArgs e) {
      base.OnApply(e);
      SettingsService.Instance.ShitChangedYo();
    }
  }

  internal class BrowseFile : UITypeEditor {
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
      return UITypeEditorEditStyle.Modal;
    }

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
      IWindowsFormsEditorService edSvc =
        (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
      if (edSvc != null) {
        OpenFileDialog open = new OpenFileDialog();
        open.FileName = Path.GetFileName((string)value);

        try {
          open.InitialDirectory = Path.GetDirectoryName((string)value);
        } catch (Exception) {
        }

        if (open.ShowDialog() == DialogResult.OK) {
          return open.FileName;
        }
      }

      return value;
    }

    public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
      return false;
    }
  }
}
