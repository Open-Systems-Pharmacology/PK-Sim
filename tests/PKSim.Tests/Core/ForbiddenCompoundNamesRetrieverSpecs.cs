using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ForbiddenCompoundNamesRetriever : ContextSpecification<IForbiddenCompoundNamesRetriever>
   {
      protected IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new ForbiddenCompoundNamesRetriever(_representationInfoRepository);
      }

      protected void AddRepresentationInfos(RepresentationObjectType objectType, params string[] names)
      {
         var representationInfos = names.Select(name => new RepresentationInfo {ObjectType = objectType, Name = name}).ToList();
         A.CallTo(() => _representationInfoRepository.AllOfType(objectType)).Returns(representationInfos);
      }
   }

   public class When_retrieving_all_forbidden_compound_names : concern_for_ForbiddenCompoundNamesRetriever
   {
      private IReadOnlyList<string> _result;

      protected override void Context()
      {
         base.Context();
         AddRepresentationInfos(RepresentationObjectType.CONTAINER, "Liver", "Brain");
         AddRepresentationInfos(RepresentationObjectType.PARAMETER, "Volume");
         AddRepresentationInfos(RepresentationObjectType.MOLECULE, "CYP3A4");
         AddRepresentationInfos(RepresentationObjectType.OBSERVER, "Concentration");
         AddRepresentationInfos(RepresentationObjectType.PROCESS, "Liver", "Hepatic Clearance");
         AddRepresentationInfos(RepresentationObjectType.SPECIES, "Human");
         AddRepresentationInfos(RepresentationObjectType.EVENT, "Meal");
      }

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_the_names_of_all_containers_parameters_molecules_observers_and_processes_without_duplicates()
      {
         _result.ShouldContain("Liver", "Brain", "Volume", "CYP3A4", "Concentration", "Hepatic Clearance");
         _result.Count(x => x == "Liver").ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_return_the_names_of_the_molecules_created_at_runtime_for_systemic_processes()
      {
         _result.ShouldContain(CoreConstants.Molecule.UndefinedLiver, CoreConstants.Molecule.UndefinedLiverTransporter);
      }

      [Observation]
      public void should_not_return_the_names_of_other_object_types()
      {
         _result.Contains("Human").ShouldBeFalse();
         _result.Contains("Meal").ShouldBeFalse();
      }
   }
}
