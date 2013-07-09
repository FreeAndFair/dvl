// -----------------------------------------------------------------------
// <copyright file="ISubView.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Central.Central.Views
{
    using global::Central.Central.Models;

    /// <summary>
    /// Marker interface for sub-views.
    /// </summary>
    public interface ISubView
    {
        ISubModel GetModel();
    }
}
