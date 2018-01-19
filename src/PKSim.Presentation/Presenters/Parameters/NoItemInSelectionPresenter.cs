using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface INoItemInSelectionPresenter : ICustomParametersPresenter
   {
      INoItemInSelectionPresenter WithDescription(string description);
   }

   public class NoItemInSelectionPresenter : AbstractCommandCollectorPresenter<INoItemInSelectionView, INoItemInSelectionPresenter>, INoItemInSelectionPresenter
   {
      public NoItemInSelectionPresenter(INoItemInSelectionView view) : base(view)
      {
      }

      public string Description
      {
         set => _view.Description = value;
         get => _view.Description;
      }

      public void Edit(IEnumerable<IParameter> parameters)
      {
         /*nothing to do here*/
      }

      public bool ForcesDisplay { get; } = false;
      public bool AlwaysRefresh { get; } = false;

      public IEnumerable<IParameter> EditedParameters => Enumerable.Empty<IParameter>();

      public INoItemInSelectionPresenter WithDescription(string description)
      {
         Description = description;
         return this;
      }
   }
}