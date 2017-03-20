using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatMoleculeToMoleculeBuilderMapper
   {
      IMoleculeBuilder MapFrom(FlatMolecule flatMolecule, IFormulaCache formulaCache);
   }

   public class FlatMoleculeToMoleculeBuilderMapper : IFlatMoleculeToMoleculeBuilderMapper
   {
      private readonly IParameterContainerTask _paramContainerTask;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IFormulaFactory _formulaFactory;

      public FlatMoleculeToMoleculeBuilderMapper(IParameterContainerTask paramContainerTask, IObjectBaseFactory objectBaseFactory,
         IFormulaFactory formulaFactory)
      {
         _paramContainerTask = paramContainerTask;
         _objectBaseFactory = objectBaseFactory;
         _formulaFactory = formulaFactory;
      }

      public IMoleculeBuilder MapFrom(FlatMolecule flatMolecule, IFormulaCache formulaCache)
      {
         var molecule = _objectBaseFactory.Create<IMoleculeBuilder>();

         molecule.Name = flatMolecule.Id;
         molecule.IsFloating = flatMolecule.IsFloating;
         molecule.QuantityType = flatMolecule.MoleculeType;
         molecule.DefaultStartFormula = _formulaFactory.RateFor(flatMolecule, new FormulaCache());

         _paramContainerTask.AddMoleculeParametersTo(molecule, formulaCache);

         return molecule;
      }
   }
}