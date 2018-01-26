using System;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface IEditValueOriginPresenter : IPresenter<IEditValueOriginView>
   {
      void Edit(IWithValueOrigin withValueOrigin);
      Action<ValueOrigin> ValueOriginUpdated { get; set; }
      Func<bool> ValueOriginEditable { get; set; }
   }

   public class EditValueOriginPresenter : AbstractPresenter<IEditValueOriginView, IEditValueOriginPresenter>, IEditValueOriginPresenter
   {
      private ValueOriginDTO _valueOriginDTO;
      public Action<ValueOrigin> ValueOriginUpdated { get; set; } = x => { };
      public Func<bool> ValueOriginEditable { get; set; } = () => true;

      public EditValueOriginPresenter(IEditValueOriginView view) : base(view)
      {
      }

      public void Edit(IWithValueOrigin withValueOrigin)
      {
         if (withValueOrigin == null)
            return;

         _valueOriginDTO = new ValueOriginDTO(withValueOrigin);
         _view.BindTo(_valueOriginDTO);
      }
   }
}