using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundInSimulationPresenter : IPresenter<ICompoundInSimulationView>, ICustomParametersPresenter
   {
   }

   public class CompoundInSimulationPresenter : AbstractCommandCollectorPresenter<ICompoundInSimulationView, ICompoundInSimulationPresenter>, ICompoundInSimulationPresenter
   {
      private readonly IMolWeightGroupPresenter _molWeightGroupPresenter;
      private readonly ICompoundTypeGroupPresenter _compoundTypeGroupPresenter;
      private readonly IMultiParameterEditPresenter _simpleParameterPresenter;
      private readonly IMultiParameterEditPresenter _advancedParameterPresenter;
      private readonly ICalculationMethodSelectionPresenterForSimulation _calculationMethodsPresenter;
      private readonly IWithIdRepository _withIdRepository;
      private List<IParameter> _allParameters;
      public string Description { get; set; }
      public bool ForcesDisplay => true;
      public bool AlwaysRefresh { get; } = false;

      public IEnumerable<IParameter> EditedParameters => _allParameters;

      public CompoundInSimulationPresenter(ICompoundInSimulationView view, IMolWeightGroupPresenter molWeightGroupPresenter,
         ICompoundTypeGroupPresenter compoundTypeGroupPresenter, IMultiParameterEditPresenter simpleParameterPresenter,
         IMultiParameterEditPresenter advancedParameterPresenter, ICalculationMethodSelectionPresenterForSimulation calculationMethodsPresenter,
         IWithIdRepository withIdRepository)
         : base(view)
      {
         _molWeightGroupPresenter = molWeightGroupPresenter;
         _compoundTypeGroupPresenter = compoundTypeGroupPresenter;
         _simpleParameterPresenter = simpleParameterPresenter;
         _advancedParameterPresenter = advancedParameterPresenter;
         _calculationMethodsPresenter = calculationMethodsPresenter;
         _withIdRepository = withIdRepository;
         AddSubPresenters(_calculationMethodsPresenter, _molWeightGroupPresenter, _compoundTypeGroupPresenter, _simpleParameterPresenter, _advancedParameterPresenter);
         _calculationMethodsPresenter.ReadOnly = true;
         _compoundTypeGroupPresenter.ShowFavorites = true;
         initializeParametersPresenter(_simpleParameterPresenter);
         initializeParametersPresenter(_advancedParameterPresenter);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _simpleParameterPresenter.Caption = PKSimConstants.UI.CompoundParameterInSimulationSimple;
         _advancedParameterPresenter.Caption = PKSimConstants.UI.CompoundParameterInSimulationAdvanced;
         _view.AddView(_simpleParameterPresenter.View);
         _view.AddView(_molWeightGroupPresenter.BaseView);
         _view.AddView(_compoundTypeGroupPresenter.BaseView);
         _view.AddView(_advancedParameterPresenter.View);
         _view.AddCachedView(_calculationMethodsPresenter.BaseView);
      }

      public void Edit(IEnumerable<IParameter> parameters)
      {
         _allParameters = parameters.ToList();
         _molWeightGroupPresenter.EditCompoundParameters(_allParameters);
         _compoundTypeGroupPresenter.EditCompoundParameters(_allParameters);
         _advancedParameterPresenter.Edit(_allParameters.Where(parameterIsAdvancedParameter));
         _simpleParameterPresenter.Edit(_allParameters.Where(parameterIsSimpleParameter));

         var compoundProperties = compoundPropertiesFor(_allParameters);
         if (compoundProperties != null)
            _calculationMethodsPresenter.Edit(compoundProperties);
         else
            hideCalculationMethodView();
      }

      private void initializeParametersPresenter(IMultiParameterEditPresenter multiParameterEditPresenter)
      {
         multiParameterEditPresenter.IsSimpleEditor = true;
         multiParameterEditPresenter.ShowFavorites = true;
      }

      private void hideCalculationMethodView()
      {
         _view.HideCachedView(_calculationMethodsPresenter.BaseView);
      }

      private CompoundProperties compoundPropertiesFor(IReadOnlyList<IParameter> allParameters)
      {
         if (!allParameters.Any())
            return null;

         var firstParameter = allParameters[0];
         var compoundName = firstParameter.ParentContainer.Name;

         var simulation = _withIdRepository.Get<Simulation>(firstParameter.Origin.SimulationId);

         return simulation?.CompoundPropertiesList.FirstOrDefault(x => x.Compound.IsNamed(compoundName));
      }

      private bool parameterIsAdvancedParameter(IParameter parameter)
      {
         return parameter.GroupName.Equals(CoreConstants.Groups.COMPOUND_TWO_PORE) ||
                parameter.GroupName.Equals(CoreConstants.Groups.COMPOUND_DISSOLUTION);
      }

      private bool parameterIsSimpleParameter(IParameter parameter)
      {
         if (parameterIsAdvancedParameter(parameter))
            return false;

         return !parameter.GroupName.Equals(CoreConstants.Groups.COMPOUND_MW) &&
                !parameter.GroupName.Equals(CoreConstants.Groups.COMPOUND_PKA);
      }
   }
}