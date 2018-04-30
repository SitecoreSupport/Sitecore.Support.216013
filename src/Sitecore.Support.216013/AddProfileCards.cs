namespace Sitecore.Support.Shell.Applications.ContentEditor.Pipelines.RenderContentEditorHeader
{
  using Sitecore;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditorHeader;
  public class AddProfileCards
  {
    public void Process(RenderContentEditorHeaderArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Item item = args.Item;
      Sitecore.Suport.Shell.Applications.ContentEditor.Pipelines.RenderContentEditorHeader.ProfileCardsControl control = new Sitecore.Suport.Shell.Applications.ContentEditor.Pipelines.RenderContentEditorHeader.ProfileCardsControl
      {
        ID = "ProfileCardsControl" + args.Item.ID.ToShortID(),
        Item = item
      };
      Context.ClientPage.AddControl(args.Parent, control);
    }
  }
}
