// -----------------------------------------------------------------------
// <copyright file="LogController.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace PollingTable.PollingTable.Log
{
    using System;

    using Timer = System.Threading.Timer;

    /// <summary>
    /// Responsible for reacting to the user's clicks and periodically updating the underlying model.
    /// </summary>
    public class LogController
    {
        private LogWindow view;
        private LogModel model;

        public LogController(LogWindow view, LogModel model)
        {
            this.view = view;
            this.model = model;

            this.view.AddRefreshButtonClicked(this.RefreshClicked);
            this.view.AddResetButtonClicked(this.RefreshClicked);

            // Periodically ask the view to update.
            Timer t = new Timer(Update, null, 10000, 60000);

            this.view.ResetFields();

            view.Show();

            view.SetDataSource(model.Logs);
        }

        /// <summary>
        /// Ask the model to update and update the text in the view.
        /// </summary>
        /// <param name="o"></param>
        public void Update(object o)
        {
            model.Update();
            view.VotersText = model.VotedVoters + " out of " + model.TotalVoters + " have voted thus far";
        }

        /// <summary>
        /// React to the GUI's refresh button being clicked by fetching data from the view 
        /// and creating a new filter.
        /// </summary>
        public void RefreshClicked()
        {
            uint cprInt;
            uint.TryParse(view.Cpr, out cprInt);

            uint tableInt;
            uint.TryParse(view.Table, out tableInt);

            LogFilter l = new LogFilter()
                {
                    Activity = view.Activity,
                    Cpr = cprInt != 0 ? cprInt : (uint?)null,
                    From = view.From != view.To ? view.From : (DateTime?)null,
                    Table = tableInt != 0 ? tableInt : (uint?)null,
                    To = view.To != view.From ? view.To : (DateTime?)null
                };

            this.model.UpdateFilter(l);
        }
    }
}