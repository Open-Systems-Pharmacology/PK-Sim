using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundVSSPresenter : ContextSpecification<ICompoundVSSPresenter>
   {
      protected IRepresentationInfoRepository _infoRepository;
      private IHeavyWorkManager _heavyWorkManager;
      protected IVSSCalculator _vssCalculator;
      protected ICompoundVSSView _view;
      protected Compound _compound;

      protected override void Context()
      {
         _view = A.Fake<ICompoundVSSView>();
         _infoRepository = A.Fake<IRepresentationInfoRepository>();
         _heavyWorkManager = new HeavyWorkManagerForSpecs();
         _vssCalculator = A.Fake<IVSSCalculator>();
         sut = new CompoundVSSPresenter(_view, _vssCalculator, _heavyWorkManager, _infoRepository);

         _compound = A.Fake<Compound>();

         sut.EditCompound(_compound);
      }
   }

   public class When_editing_the_species_dependant_vss_presenter_for_a_given_compound : concern_for_CompoundVSSPresenter
   {
      [Observation]
      public void should_update_the_view_height_to_trigger_a_display_refresh()
      {
         A.CallTo(() => _view.AdjustHeight()).MustHaveHappened();
      }
   }

   public class When_calculating_the_different_vss_values_for_the_available_species : concern_for_CompoundVSSPresenter
   {
      private Species _species1;
      private Species _species2;
      private List<VSSValueDTO> _allVSSValueDTO;

      protected override void Context()
      {
         base.Context();
         _species1 = new Species().WithId("Dog").WithName("Dog");
         _species1.DisplayName = "MY DOG";
         _species2 = new Species().WithId("Human").WithName("Human");
         _species2.DisplayName = "MY HUMAN";
         var cache = new Cache<Species, IParameter>();
         var vss1 = A.Fake<IParameter>();
         var vss2 = A.Fake<IParameter>();
         vss1.ValueInDisplayUnit = 10;
         vss1.DisplayUnit = new Unit("XXX", 1, 0);
         vss2.ValueInDisplayUnit = 20;
         cache[_species1] = vss1;
         cache[_species2] = vss2;
         A.CallTo(() => _vssCalculator.VSSPhysChemFor(_compound)).Returns(cache);

         A.CallTo(() => _view.BindTo(A<IEnumerable<VSSValueDTO>>._))
            .Invokes(x => _allVSSValueDTO = x.GetArgument<IEnumerable<VSSValueDTO>>(0).ToList());
      }

      protected override void Because()
      {
         sut.CalculateVSS();
      }

      [Observation]
      public void should_create_one_entry_for_each_available_vss_values()
      {
         _allVSSValueDTO.Count.ShouldBeEqualTo(2);
         _allVSSValueDTO[0].Species.ShouldBeEqualTo(_species1.DisplayName);
         _allVSSValueDTO[0].VSS.ShouldBeEqualTo(10);
         _allVSSValueDTO[1].Species.ShouldBeEqualTo(_species2.DisplayName);
         _allVSSValueDTO[1].VSS.ShouldBeEqualTo(20);
      }
   }

   public class When_retrieving_the_vss_caption : concern_for_CompoundVSSPresenter
   {
      protected override void Context()
      {
         base.Context();
         var vss = A.Fake<IParameter>();
         var species = new Species().WithId("Dog").WithName("Dog");

         vss.DisplayUnit = new Unit("XXX", 1, 0);
         A.CallTo(() => _infoRepository.DisplayNameFor(vss)).Returns("MY VSS");
         var cache = new Cache<Species, IParameter>();
         cache[species] = vss;

         A.CallTo(() => _vssCalculator.VSSPhysChemFor(_compound)).Returns(cache);
         sut.CalculateVSS();
      }

      [Observation]
      public void should_use_the_display_name_and_unit_of_the_vss_parameter()
      {
         sut.VSSCaption.ShouldBeEqualTo("MY VSS [XXX]");
      }
   }
}