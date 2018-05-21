using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_Formulation : ContextSpecification<Formulation>
   {
      protected IParameter _particleSizeDistribution;
      protected IParameter _particleDisperseSystem;
      protected IParameter _particleRadiusMinSize;
      protected IParameter _particleLogVariationCoeff;
      protected IParameter _particleRadiusMean;

      protected override void Context()
      {
         _particleSizeDistribution = DomainHelperForSpecs.ConstantParameterWithValue(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_NORMAL).WithName(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION);
         _particleDisperseSystem = DomainHelperForSpecs.ConstantParameterWithValue(CoreConstants.Parameters.MONODISPERSE).WithName(CoreConstants.Parameters.PARTICLE_DISPERSE_SYSTEM);
         _particleRadiusMinSize = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.PARTICLE_RADIUS_MIN);
         _particleLogVariationCoeff = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.PARTICLE_LOG_VARIATION_COEFF);
         _particleRadiusMean = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.PARTICLE_RADIUS_MEAN);

         sut = new Formulation {_particleSizeDistribution, _particleDisperseSystem, _particleRadiusMinSize, _particleLogVariationCoeff, _particleRadiusMean };
      }
   }

   public class When_checking_if_a_formulation_is_monodisperse : concern_for_Formulation
   {
      [Observation]
      public void should_return_false_if_the_formulation_is_not_particle_dissolution()
      {
         sut.IsMonodisperse.ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_formulation_is_particle_polydisperse()
      {
         sut.FormulationType = CoreConstants.Formulation.Particles;
         _particleDisperseSystem.Value = CoreConstants.Parameters.POLYDISPERSE;
         sut.IsMonodisperse.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_formulation_is_particle_monodisperse()
      {
         sut.FormulationType = CoreConstants.Formulation.Particles;
         _particleDisperseSystem.Value = CoreConstants.Parameters.MONODISPERSE;
         sut.IsMonodisperse.ShouldBeTrue();
      }
   }

   public class When_checking_if_a_formulation_is_polydisperse : concern_for_Formulation
   {
      [Observation]
      public void should_return_false_if_the_formulation_is_not_particle_dissolution()
      {
         sut.IsPolydispserse.ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_formulation_is_particle_monodisperse()
      {
         sut.FormulationType = CoreConstants.Formulation.Particles;
         _particleDisperseSystem.Value = CoreConstants.Parameters.MONODISPERSE;
         sut.IsPolydispserse.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_formulation_is_particle_polydisperse()
      {
         sut.FormulationType = CoreConstants.Formulation.Particles;
         _particleDisperseSystem.Value = CoreConstants.Parameters.POLYDISPERSE;
         sut.IsPolydispserse.ShouldBeTrue();
      }
   }

   public class When_updating_the_visibility_and_default_state_of_particle_dissolution_parameters_for_a_monodisperse_formulation : concern_for_Formulation
   {
      protected override void Context()
      {
         base.Context();
         sut.FormulationType = CoreConstants.Formulation.Particles;
         _particleDisperseSystem.Value = CoreConstants.Parameters.MONODISPERSE;
      }

      protected override void Because()
      {
         sut.UpdateParticleParametersVisibility();
      }

      [Observation]
      public void should_hide_the_polydisperse_parameters()
      {
         _particleRadiusMinSize.Visible.ShouldBeFalse();
         _particleRadiusMinSize.IsDefault.ShouldBeTrue();
         _particleLogVariationCoeff.Visible.ShouldBeFalse();
         _particleLogVariationCoeff.IsDefault.ShouldBeTrue();
      }
   }

   public class When_updating_the_visibility_and_default_state_of_particle_dissolution_parameters_for_a_polydisperse_normal_formulation : concern_for_Formulation
   {
      protected override void Context()
      {
         base.Context();
         sut.FormulationType = CoreConstants.Formulation.Particles;
         _particleDisperseSystem.Value = CoreConstants.Parameters.POLYDISPERSE;
         _particleSizeDistribution.Value = CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_NORMAL;
      }

      protected override void Because()
      {
         sut.UpdateParticleParametersVisibility();
      }

      [Observation]
      public void should_hide_the_polydisperse_log_normal_parameters()
      {
         _particleLogVariationCoeff.Visible.ShouldBeFalse();
         _particleLogVariationCoeff.IsDefault.ShouldBeTrue();
      }

      [Observation]
      public void should_show_the_polydisperse_normal_parameters()
      {
         _particleRadiusMinSize.Visible.ShouldBeTrue();
         _particleRadiusMinSize.IsDefault.ShouldBeFalse();
      }
   }

   public class When_updating_the_visibility_and_default_state_of_particle_dissolution_parameters_for_a_polydisperse_lognormal_formulation : concern_for_Formulation
   {
      protected override void Context()
      {
         base.Context();
         sut.FormulationType = CoreConstants.Formulation.Particles;
         _particleDisperseSystem.Value = CoreConstants.Parameters.POLYDISPERSE;
         _particleSizeDistribution.Value = CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_LOG_NORMAL;
      }

      protected override void Because()
      {
         sut.UpdateParticleParametersVisibility();
      }

      [Observation]
      public void should_hide_the_polydisperse_normal_parameters()
      {
         _particleRadiusMean.Visible.ShouldBeFalse();
         _particleRadiusMean.IsDefault.ShouldBeTrue();
      }

      [Observation]
      public void should_show_the_polydisperse_lognormal_parameters()
      {
         _particleLogVariationCoeff.Visible.ShouldBeTrue();
         _particleLogVariationCoeff.IsDefault.ShouldBeFalse();
      }
   }
}