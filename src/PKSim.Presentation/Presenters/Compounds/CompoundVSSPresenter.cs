using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundVSSPresenter : IPresenter<ICompoundVSSView>, ISubPresenter, IUnitsInColumnPresenter<string>
   {
      void EditCompound(Compound compound);

      /// <summary>
      ///    Starts the calculation of VSS for all available <see cref="Species" />
      /// </summary>
      void CalculateVSS();

      /// <summary>
      ///    Returns the formatted caption to be displayed as header (with display unit)
      /// </summary>
      string VSSCaption { get; }
   }

   public class CompoundVSSPresenter : AbstractSubPresenter<ICompoundVSSView, ICompoundVSSPresenter>, ICompoundVSSPresenter
   {
      private readonly IVSSCalculator _vssCalculator;
      private readonly IHeavyWorkManager _heavyWorkManager;
      private readonly IRepresentationInfoRepository _infoRepository;
      private Compound _compound;
      private ICache<Species, IParameter> _allVSSValues;

      public CompoundVSSPresenter(ICompoundVSSView view, IVSSCalculator vssCalculator, IHeavyWorkManager heavyWorkManager, IRepresentationInfoRepository infoRepository) : base(view)
      {
         _vssCalculator = vssCalculator;
         _heavyWorkManager = heavyWorkManager;
         _infoRepository = infoRepository;
      }

      public void CalculateVSS()
      {
         _heavyWorkManager.Start(startVssCalculations);
         refreshView();
      }

      public string VSSCaption
      {
         get
         {
            var displayName = _infoRepository.DisplayNameFor(vssParameter);
            return Constants.NameWithUnitFor(displayName, vssParameter.DisplayUnit);
         }
      }

      private void startVssCalculations()
      {
         _allVSSValues = _vssCalculator.VSSPhysChemFor(_compound);
      }

      private void refreshView()
      {
         var allVSSValuesDTO = _allVSSValues.KeyValues.Select(vss => new VSSValueDTO
         {
            Species = vss.Key.DisplayName,
            VSS = vss.Value.ValueInDisplayUnit
         }).ToList();
         _view.BindTo(allVSSValuesDTO);
      }

      public void EditCompound(Compound compound)
      {
         _compound = compound;
         _view.AdjustHeight();
      }

      public void ChangeUnit(string parameterName, Unit newUnit)
      {
         _allVSSValues.Each(p => p.DisplayUnit = newUnit);
         refreshView();
      }

      public Unit DisplayUnitFor(string parameterName)
      {
         return vssParameter.DisplayUnit;
      }

      public IEnumerable<Unit> AvailableUnitsFor(string columnIndentifier)
      {
         return vssParameter.Dimension.Units;
      }

      private IParameter vssParameter
      {
         get { return _allVSSValues.FirstOrDefault() ?? _vssCalculator.VSSParameterWithValue(double.NaN); }
      }
   }
}