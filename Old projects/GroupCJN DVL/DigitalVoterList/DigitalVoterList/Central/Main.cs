// -----------------------------------------------------------------------
// <copyright file="Main.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Central.Central
{
    using System.Windows.Forms;

    using global::Central.Central.Controllers;
    using global::Central.Central.Models;
    using global::Central.Central.Views;

    using View = global::Central.Central.Views.View;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Main
    {
        private View view;

        private ServerSetupController setup;

        public Main()
        {
            var setupView = new ServerSetupWindow();
            setupView.Show();
            var setupModel = new ServerSetup();

            this.setup = new ServerSetupController(setupView, setupModel);
            this.setup.Connected += this.ShowApplication;

            Application.Run();
        }

        private void ShowApplication()
        {
            this.setup.DisposeView();
            this.setup = null;

            var model = new Model();
            this.view = new View(model);
            var controller = new Controller(model, view);

            view.ShowView();
        }
    }
}
