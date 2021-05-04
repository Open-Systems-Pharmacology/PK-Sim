using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualExpressionsUpdater : ContextSpecification<IIndividualExpressionsUpdater>
   {
      protected Individual _targetIndividual;
      protected Individual _sourceIndividual;
      protected IOntogenyTask<Individual> _ontogenyTask;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      private IEntityPathResolver _entityPathResolver;
      private ICloner _cloner;
      protected IParameterUpdater _parameterUpdater;

      protected override void Context()
      {
         _sourceIndividual = DomainHelperForSpecs.CreateIndividual();
         _ontogenyTask = A.Fake<IOntogenyTask<Individual>>();
         _targetIndividual = DomainHelperForSpecs.CreateIndividual();
         _moleculeExpressionTask= A.Fake<IMoleculeExpressionTask<Individual>>();
         _entityPathResolver=new EntityPathResolverForSpecs();
         _cloner= A.Fake<ICloner>();
         _parameterUpdater= A.Fake<IParameterUpdater>();

         sut = new IndividualExpressionsUpdater(_ontogenyTask,_moleculeExpressionTask,_entityPathResolver,_cloner,_parameterUpdater );
      }
   }

   public class When_updating_the_enzyme_expression_from_one_individual_to_another : concern_for_IndividualExpressionsUpdater
   {
      private IndividualMolecule _enzyme1;
      private IndividualMolecule _enzyme2;
      private IndividualMolecule _newEnzyme1;
      private IndividualMolecule _newEnzyme2;
      private IParameter _userDefinedParameter;
      private double _defaultValue = 20;
      private TransporterExpressionContainer _expressionContainer1;
      private TransporterExpressionContainer _expressionContainer2;
      private IParameter _expressionParameter1;
      private IParameter _expressionParameter2;

      protected override void Context()
      {
         base.Context();
         _enzyme1 = new IndividualEnzyme().WithName("meta1");
         _enzyme2 = new IndividualEnzyme().WithName("meta2");
         _newEnzyme1 = new IndividualEnzyme().WithName("meta1");
         _newEnzyme2 = new IndividualEnzyme().WithName("meta2");

         //Create a parameter that changed!
         _userDefinedParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _userDefinedParameter.DefaultValue = _defaultValue;
         _newEnzyme1.Add(_userDefinedParameter);

         _newEnzyme1.Ontogeny = new DatabaseOntogeny();
         _newEnzyme2.Ontogeny = new DatabaseOntogeny();
         _sourceIndividual.AddMolecule(_enzyme1);
         _sourceIndividual.AddMolecule(_enzyme2);

         //Simulate creating a new molecule in the target individual 
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo(_targetIndividual, _enzyme1))
            .Invokes(x => _targetIndividual.AddMolecule(_newEnzyme1));

         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo(_targetIndividual, _enzyme2))
            .Invokes(x => _targetIndividual.AddMolecule(_newEnzyme2));

         _expressionContainer1 = new TransporterExpressionContainer().WithName(_enzyme1.Name);
         _expressionContainer1.TransportDirection = TransportDirectionId.BiDirectionalBrainPlasmaInterstitial;
         _expressionParameter1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1").WithParentContainer(_expressionContainer1);
         _enzyme1.Add(_expressionContainer1);

         _expressionContainer2 = new TransporterExpressionContainer().WithName(_newEnzyme1.Name);
         _expressionParameter2 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1").WithParentContainer(_expressionContainer2);

         //set to sthg else to ensure it is updated
         _expressionContainer2.TransportDirection = TransportDirectionId.EffluxBloodCellsToPlasma;
         _newEnzyme1.Add(_expressionContainer2);
      }

      protected override void Because()
      {
         sut.Update(_sourceIndividual, _targetIndividual);
      }

      [Observation]
      public void should_add_a_clone_of_the_source_protein_in_the_target_individual()
      {
         _targetIndividual.MoleculeByName<IndividualMolecule>("meta1").ShouldBeEqualTo(_newEnzyme1);
         _targetIndividual.MoleculeByName<IndividualMolecule>("meta2").ShouldBeEqualTo(_newEnzyme2);
      }

      [Observation]
      public void should_have_updated_the_ontogeny_of_the_new_molecule()
      {
         A.CallTo(() => _ontogenyTask.SetOntogenyForMolecule(_newEnzyme1, _newEnzyme1.Ontogeny, _targetIndividual)).MustHaveHappened();
         A.CallTo(() => _ontogenyTask.SetOntogenyForMolecule(_newEnzyme2, _newEnzyme2.Ontogeny, _targetIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_enzyme_parameters_from_source_to_target()
      {
         A.CallTo(() => _parameterUpdater.UpdateValue(_expressionParameter1, _expressionParameter2)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_molecule_expression_container()
      {
         _expressionContainer2.TransportDirection.ShouldBeEqualTo(TransportDirectionId.BiDirectionalBrainPlasmaInterstitial);
      }


      [Observation]
      public void should_reset_all_molecule_user_defined_parameters_to_their_default_value()
      {
         _userDefinedParameter.Value.ShouldBeEqualTo(_defaultValue);
      }
   }
}