using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_molecule_with_a_user_defined_ontogeny : ContextForSerialization<IndividualEnzyme>
   {
      private IndividualEnzyme _enzyme;
      private IndividualEnzyme _deserializedEnzyme;
      private DistributedTableFormula _table;

      protected override void Context()
      {
         base.Context();
         _enzyme = new IndividualEnzyme();
         _table = new DistributedTableFormula();
         _table.AddPoint(1, 10, new DistributionMetaData {Mean = 10, Deviation = 100, Distribution = DistributionType.Normal});
         _table.AddPoint(2, 20, new DistributionMetaData {Mean = 20, Deviation = 200, Distribution = DistributionType.Normal});
         _enzyme.Ontogeny = new UserDefinedOntogeny {Table = _table};
      }

      protected override void Because()
      {
         _deserializedEnzyme = SerializeAndDeserialize(_enzyme);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_user_defined_ontogeny()
      {
         _deserializedEnzyme.ShouldNotBeNull();
         _deserializedEnzyme.Ontogeny.IsUserDefined().ShouldBeTrue();
         var userDefinedOntogeny = _deserializedEnzyme.Ontogeny.DowncastTo<UserDefinedOntogeny>();
         userDefinedOntogeny.PostmenstrualAges().ShouldOnlyContain(1, 2);
         userDefinedOntogeny.OntogenyFactors().ShouldOnlyContain(10, 20);
         userDefinedOntogeny.Deviations().ShouldOnlyContain(100, 200);
      }
   }

   public class When_serializing_a_molecule_with_a_database_ontogeny : ContextForSerialization<IndividualEnzyme>
   {
      private IndividualEnzyme _enzyme;
      private IndividualEnzyme _deserializedEnzyme;

      protected override void Context()
      {
         base.Context();
         _enzyme = new IndividualEnzyme();
         var ontogenyRepo = IoC.Resolve<IOntogenyRepository>();
         _enzyme.Ontogeny = ontogenyRepo.AllFor(CoreConstants.Species.HUMAN).First();
      }

      protected override void Because()
      {
         _deserializedEnzyme = SerializeAndDeserialize(_enzyme);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_database_ontogeny()
      {
         _deserializedEnzyme.ShouldNotBeNull();
         _deserializedEnzyme.Ontogeny.IsUserDefined().ShouldBeFalse();
         _deserializedEnzyme.Ontogeny.ShouldBeEqualTo(_enzyme.Ontogeny);
      }
   }
}