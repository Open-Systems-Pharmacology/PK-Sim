using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatMoleculeToMoleculeBuilderMapper
   {
      MoleculeBuilder MapFrom(FlatMolecule flatMolecule, IFormulaCache formulaCache);
   }

   public class FlatMoleculeToMoleculeBuilderMapper : IFlatMoleculeToMoleculeBuilderMapper
   {
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IFormulaFactory _formulaFactory;

      public FlatMoleculeToMoleculeBuilderMapper(
         IParameterContainerTask parameterContainerTask,
         IObjectBaseFactory objectBaseFactory,
         IFormulaFactory formulaFactory)
      {
         _parameterContainerTask = parameterContainerTask;
         _objectBaseFactory = objectBaseFactory;
         _formulaFactory = formulaFactory;
      }

      public MoleculeBuilder MapFrom(FlatMolecule flatMolecule, IFormulaCache formulaCache)
      {
         var molecule = _objectBaseFactory.Create<MoleculeBuilder>();

         molecule.Name = flatMolecule.Id;
         molecule.IsFloating = flatMolecule.IsFloating;
         molecule.QuantityType = flatMolecule.MoleculeType;
         molecule.DefaultStartFormula = _formulaFactory.RateFor(flatMolecule, new FormulaCache());

         _parameterContainerTask.AddMoleculeParametersTo(molecule, formulaCache);

         return molecule;
      }
   }
}