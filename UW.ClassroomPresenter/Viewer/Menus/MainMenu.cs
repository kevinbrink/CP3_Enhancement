// $Id: MainMenu.cs 1471 2007-10-06 23:59:36Z linnell $

using System;
using System.Windows.Forms;

using UW.ClassroomPresenter.Decks;
using UW.ClassroomPresenter.Model;

namespace UW.ClassroomPresenter.Viewer.Menus {
    /// <summary>
    /// The main menu shown at the top of the Viewer form.
    /// </summary>
    public class ViewerMainMenu : MainMenu {
        public ViewerMainMenu(ControlEventQueue dispatcher, PresenterModel model, DeckMarshalService marshal, FileMenu.CloseFormDelegate cfd) {
            this.MenuItems.Add(new FileMenu(dispatcher, model, marshal, cfd));
            this.MenuItems.Add(new EditMenu(dispatcher, model));
            this.MenuItems.Add(new ViewMenu(dispatcher, model));
            //this.MenuItems.Add(new ConnectMenu(dispatcher, model));
            this.MenuItems.Add(new ToolsMenu(model));
            this.MenuItems.Add(new DecksMenu(dispatcher, model));
            this.MenuItems.Add(new StudentMenu(dispatcher, model));
            this.MenuItems.Add(new HelpMenu());
        }
    }
}
