using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface IValueOriginPresenter : OSPSuite.Presentation.Presenters.IValueOriginPresenter
   {
      ValueOrigin UpdatedValueOrigin { get; }
      bool ValueOriginChanged { get;  }
   }

   public class ValueOriginPresenter : AbstractDisposablePresenter<IValueOriginView, OSPSuite.Presentation.Presenters.IValueOriginPresenter>, IValueOriginPresenter
   {
      private readonly ValueOrigin _valueOriginDTO;
      private readonly List<ValueOriginType> _allValueOrigins = new List<ValueOriginType>();
      private ValueOrigin _valueOrigin;

      public ValueOriginPresenter(IValueOriginView view) : base(view)
      {
         _valueOriginDTO = new ValueOrigin();
         _allValueOrigins.AddRange(ValueOriginTypes.AllValueOrigins.Except(new[] {ValueOriginTypes.Undefined}));
      }

      public void Edit(ValueOrigin valueOrigin)
      {
         _valueOrigin = valueOrigin;
         _valueOriginDTO.UpdateFrom(valueOrigin);
         _view.BindTo(_valueOriginDTO);
      }

      public void Save()
      {
         _view.Save();
         updateUndefinedValueOriginType();
      }

      private void updateUndefinedValueOriginType()
      {
         if (_valueOriginDTO.Type != ValueOriginTypes.Undefined)
            return;

         if (string.IsNullOrWhiteSpace(_valueOriginDTO.Description))
            return;

         _valueOriginDTO.Type = ValueOriginTypes.Unknown;
      }

      public IEnumerable<ValueOriginType> AllValueOrigins => _allValueOrigins;

      public ValueOrigin UpdatedValueOrigin
      {
         get
         {
            Save();
            return _valueOriginDTO;
         }
      }

      public bool ValueOriginChanged
      {
         get
         {
            Save();
            return !areEquivalent(_valueOrigin, _valueOriginDTO);
         }
      }

      private bool areEquivalent(ValueOrigin valueOrigin, ValueOrigin valueOriginDTO)
      {
         return sameDescription(valueOrigin.Description, valueOriginDTO.Description) &&
                valueOrigin.Type == valueOriginDTO.Type;
      }

      private bool sameDescription(string description1, string description2)
      {
         if (string.IsNullOrWhiteSpace(description1) && string.IsNullOrWhiteSpace(description2))
            return true;

         return string.Equals(description1, description2);
      }
   }
}