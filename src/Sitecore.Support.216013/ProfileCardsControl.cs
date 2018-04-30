namespace Sitecore.Suport.Shell.Applications.ContentEditor.Pipelines.RenderContentEditorHeader
{
  using Sitecore;
  using Sitecore.Analytics.Data;
  using Sitecore.Analytics.Pipelines.GetItemPersonalizationVisibility;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Pipelines;
  using Sitecore.Resources;
  using Sitecore.Text;
  using Sitecore.Web.UI;
  using Sitecore.Web.UI.HtmlControls;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Runtime.CompilerServices;
  using System.Runtime.InteropServices;
  using System.Text;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using Telerik.Web.UI;

  public class ProfileCardsControl : System.Web.UI.WebControls.WebControl
  {
    private static void InitializeTooltip(RadToolTipManager tooltip)
    {
      Assert.ArgumentNotNull(tooltip, "tooltip");
      tooltip.AnimationDuration = 200;
      tooltip.EnableShadow = true;
      tooltip.HideDelay = 0x3e8;
      tooltip.Width = new Unit(340);
      tooltip.Height = new Unit(0xd6);
      tooltip.ContentScrolling = ToolTipScrolling.None;
      tooltip.RelativeTo = ToolTipRelativeDisplay.Element;
      tooltip.Animation = ToolTipAnimation.Slide;
      tooltip.Position = ToolTipPosition.BottomCenter;
      tooltip.Skin = "Telerik";
      tooltip.MouseTrailing = false;
      tooltip.HideEvent = ToolTipHideEvent.LeaveTargetAndToolTip;
      tooltip.WebServiceSettings.Path = "/sitecore/shell/Applications/Analytics/Personalization/ToolTip/RenderToolTipService.asmx";
      tooltip.WebServiceSettings.Method = "OnTooltipUpdate";
      tooltip.ShowEvent = ToolTipShowEvent.FromCode;
      tooltip.OffsetY = 0;
      tooltip.AutoCloseDelay = 0x2710;
      tooltip.ShowCallout = true;
    }

    protected override void OnInit(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      this.Page.Header.Controls.Add(new LiteralControl("\r\n    <script type='text/JavaScript' src='/sitecore/shell/Applications/Analytics/Personalization/Carousel/jquery.jcarousel.min.js'></script>\r\n    <link href='/sitecore/shell/Applications/Analytics/Personalization/Carousel/skin.css' rel='stylesheet' />\r\n    <script type='text/JavaScript' src='/sitecore/shell/Applications/Analytics/Personalization/Tooltip.js'></script>"));
      HtmlTextWriter output = new HtmlTextWriter(new StringWriter());
      this.RenderProfileCards(this.Item, output);
      RadToolTipManager child = new RadToolTipManager
      {
        Skin = "Metro",
        ID = "ToolTipManager" + ((this.Item != null) ? this.Item.ID.ToShortID().ToString() : string.Empty),
        CssClass = "scRadTooltipManager"
      };
      this.Controls.Add(child);
      InitializeTooltip(child);
      this.Controls.Add(new LiteralControl(output.InnerWriter.ToString()));
      base.OnLoad(e);
    }

    private void RenderEditorHeaderSeparator(HtmlTextWriter output, string customClassName)
    {
      Assert.ArgumentNotNull(output, "output");
      Assert.ArgumentNotNull(customClassName, "customClassName");
      string str = string.Empty;
      if (!string.IsNullOrEmpty(customClassName))
      {
        str = " " + customClassName;
      }
      output.Write("<div class=\"scEditorHeaderSeperator\"><span class=\"scEditorHeaderSeperatorLine{0}\"></span></div>", str);
    }

    private bool RenderPersonalizationPanel(Sitecore.Data.Items.Item item)
    {
      if (CorePipelineFactory.GetPipeline("getItemPersonalizationVisibility", string.Empty) == null)
      {
        return true;
      }
      GetItemPersonalizationVisibilityArgs args = new GetItemPersonalizationVisibilityArgs(true, item);
      CorePipeline.Run("getItemPersonalizationVisibility", args);
      return args.Visible;
    }

    private void RenderProfileCardIcon(Sitecore.Data.Items.Item contextItem, Sitecore.Data.Items.Item profileItem, Sitecore.Data.Items.Item presetItem, HtmlTextWriter output)
    {
      Assert.ArgumentNotNull(contextItem, "contextItem");
      Assert.ArgumentNotNull(profileItem, "profileItem");
      Assert.ArgumentNotNull(output, "output");
      string url = (presetItem != null) ? ProfileUtil.UI.GetPresetThumbnail(presetItem) : ProfileUtil.UI.GetProfileThumbnail(profileItem);
      ImageBuilder builder = new ImageBuilder();
      UrlString str2 = new UrlString(url);
      builder.Src = str2.ToString();
      builder.Class = "scEditorHeaderProfileCardIcon";
      string uniqueID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("profileIcon");
      string str4 = (presetItem == null) ? (profileItem.ID.ToShortID() + "|" + profileItem.Language) : (presetItem.ID.ToShortID() + "|" + presetItem.Language);
      if (contextItem.Appearance.ReadOnly || !contextItem.Access.CanWrite())
      {
        output.Write("<a id=\"{1}\" href=\"#\" class=\"scEditorHeaderProfileCardIcon\" style=\"background-image:url('{0}'); background-repeat:no-repeat; background-position:center;\" onmouseover=\"showToolTipWithTimeout('{1}', '{2}', null, 500);\" onmouseout=\"cancelRadTooltip();\">", str2, uniqueID, str4);
        output.Write("</a>");
      }
      else
      {
        string personalizeProfileCommand = ProfileUtil.UI.GetPersonalizeProfileCommand(contextItem, profileItem);
        output.Write("<a id=\"{2}\" href=\"#\" class=\"scEditorHeaderProfileCardIcon\" onclick=\"javascript:return scForm.invoke('{1}')\" style=\"background-image:url('{0}'); background-repeat:no-repeat; background-position:center;\" onmouseover=\"showToolTipWithTimeout('{2}', '{3}', null, 500);\" onmouseout=\"cancelRadTooltip();\">", new object[] { str2, personalizeProfileCommand, uniqueID, str4 });
        output.Write("</a>");
      }
    }

    private void RenderProfileCardIcons(Sitecore.Data.Items.Item item, HtmlTextWriter output, out bool hasCardsConfigured)
    {
      hasCardsConfigured = false;
      Assert.ArgumentNotNull(output, "output");
      if (item != null)
      {
        TrackingField field;
        IEnumerable<ContentProfile> profiles = ProfileUtil.GetProfiles(item, out field);
        if (field != null)
        {
          int num = 0;
          foreach (ContentProfile profile in profiles)
          {
            if (profile != null)
            {
              Sitecore.Data.Items.Item profileItem = profile.GetProfileItem();
              if (profileItem != null)
              {
                if ((profile.Presets == null) || (profile.Presets.Count == 0))
                {
                  if (ProfileUtil.HasPresetData(profileItem, field))
                  {
                    this.RenderEditorHeaderSeparator(output, (num == 0) ? "scEditorHeaderSeperatorFirstLine" : string.Empty);
                    this.RenderProfileCardIcon(item, profileItem, null, output);
                    num++;
                  }
                }
                else
                {
                  int num2 = 0;
                  foreach (KeyValuePair<string, float> pair in profile.Presets)
                  {
                    Sitecore.Data.Items.Item presetItem = profile.GetPresetItem(pair.Key);
                    if (presetItem != null)
                    {
                      if (num2 == 0)
                      {
                        this.RenderEditorHeaderSeparator(output, (num == 0) ? "scEditorHeaderSeperatorFirstLine" : string.Empty);
                      }
                      this.RenderProfileCardIcon(item, profileItem, presetItem, output);
                      num++;
                      num2++;
                    }
                  }
                }
              }
            }
          }
          hasCardsConfigured = num > 0;
        }
      }
    }

    private void RenderProfileCards(Sitecore.Data.Items.Item item, HtmlTextWriter output)
    {
      if ((item != null) && this.RenderPersonalizationPanel(item))
      {
        bool flag;
        Assert.ArgumentNotNull(output, "output");
        string str = Translate.Text("Edit the profile cards associated with this item.");
        ImageBuilder builder = new ImageBuilder();
        builder.Src = new UrlString(Images.GetThemedImageSource("Office/32x32/photo_portrait.png", ImageDimension.id32x32)).ToString();
        builder.Class = "scEditorHeaderCustomizeProfilesIcon";
        builder.Alt = str;
        if (!item.Appearance.ReadOnly && item.Access.CanWrite())
        {
          output.Write("<a href=\"#\" class=\"scEditorHeaderCustomizeProfilesIcon\" onclick=\"javascript:return scForm.invoke('item:personalize')\" title=\"" + str + "\">");
          output.Write(builder.ToString());
          output.Write("</a>");
        }
        StringBuilder sb = new StringBuilder();
        HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb));
        this.RenderProfileCardIcons(item, writer, out flag);
        writer.Flush();
        if (flag)
        {
          if (!UIUtil.IsIE())
          {
            output.Write("<span class=\"scEditorHeaderProfileCards\">");
          }
          output.Write(sb.ToString());
          if (!UIUtil.IsIE())
          {
            output.Write("</span>");
          }
        }
      }
    }

    public Sitecore.Data.Items.Item Item { get; set; }
  }
}
