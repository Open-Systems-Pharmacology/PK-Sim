using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Model
{
   public interface ICompoundFactory
   {
      Compound Create();
   }

   public class CompoundFactory : ICompoundFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IParameterAlternativeFactory _parameterAlternativeFactory;
      private readonly IParameterGroupTask _parameterGroupTask;
      private readonly ICompoundCalculationMethodCategoryRepository _compoundCalculationMethodCategoryRepository;

      public CompoundFactory(
         IObjectBaseFactory objectBaseFactory,
         IParameterContainerTask parameterContainerTask,
         IParameterAlternativeFactory parameterAlternativeFactory,
         IParameterGroupTask parameterGroupTask,
         ICompoundCalculationMethodCategoryRepository compoundCalculationMethodCategoryRepository)
      {
         _objectBaseFactory = objectBaseFactory;
         _parameterContainerTask = parameterContainerTask;
         _parameterAlternativeFactory = parameterAlternativeFactory;
         _parameterGroupTask = parameterGroupTask;
         _compoundCalculationMethodCategoryRepository = compoundCalculationMethodCategoryRepository;
      }

      public Compound Create()
      {
         var compound = _objectBaseFactory.Create<Compound>();
         compound.Root = _objectBaseFactory.Create<IRootContainer>();
         readCompoundFromTemplate(compound);
         initializeCalculationMethods(compound);
         compound.IsLoaded = true;
         return compound;
      }

      private void initializeCalculationMethods(Compound compound)
      {
         _compoundCalculationMethodCategoryRepository.All().Each(category => compound.CalculationMethodCache.AddCalculationMethod(category.DefaultItem));
      }

      private void readCompoundFromTemplate(Compound compound)
      {
         //STEP1: Add all parameters defined for the compound from the database
         //need to set the name of the compound to drug in order to load parameter from database
         _parameterContainerTask.AddCompoundParametersTo(compound);

         foreach (var group in _parameterGroupTask.GroupsUsedBy(compound.AllParameters().ToList()).Where(groupNeedsAlterntive))
         {
            // create and add new compound parameter group
            var compoundParamGroup = _objectBaseFactory.Create<ParameterAlternativeGroup>().WithName(group.Name);
            compound.AddParameterAlternativeGroup(compoundParamGroup);

            // create a default alternative
            compoundParamGroup.AddAlternative(_parameterAlternativeFactory.CreateDefaultAlternativeFor(compoundParamGroup));
         }
      }

      private bool groupNeedsAlterntive(IGroup group)
      {
         return CoreConstants.Groups.GroupsWithAlternative.Contains(group.Name);
      }
   }
}