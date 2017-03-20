using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ColorGradientGenerator : ContextSpecification<IColorGradientGenerator>
   {
      protected Color _startColor;
      protected Color _endColor;
      protected int _numberOfSteps;
      protected IReadOnlyList<Color> _gradients;

      protected override void Context()
      {
         sut = new ColorGradientGenerator();
         _startColor = Color.Blue;
         _endColor = Color.Red;
         _numberOfSteps = 2;
      }

      protected override void Because()
      {
         _gradients = sut.GenerateGradient(_startColor, _endColor, _numberOfSteps);
      }
   }

   public class When_generating_a_gradient_between_two_colors_and_zero_steps : concern_for_ColorGradientGenerator
   {
      protected override void Context()
      {
         base.Context();
         _numberOfSteps = 0;
      }

      [Observation]
      public void should_return_an_empty_gradient()
      {
         _gradients.ShouldBeEmpty();
      }
   }

   public class When_generating_a_gradient_between_two_colors_and_one_step : concern_for_ColorGradientGenerator
   {
      protected override void Context()
      {
         base.Context();
         _numberOfSteps = 1;
      }

      [Observation]
      public void should_return_a_gradient_containing_only_the_start_color()
      {
         _gradients.ShouldOnlyContain(_startColor);
      }
   }

   public class When_generating_a_gradient_between_two_colors_and_two_steps : concern_for_ColorGradientGenerator
   {
      protected override void Context()
      {
         base.Context();
         _numberOfSteps = 2;
      }

      [Observation]
      public void should_return_a_gradient_containing_only_the_start_and_end_color()
      {
         _gradients.ShouldOnlyContainInOrder(_startColor,_endColor);
      }
   }

   public class When_generating_a_gradient_between_two_colors_and_10_steps : concern_for_ColorGradientGenerator
   {
      protected override void Context()
      {
         base.Context();
         _numberOfSteps = 10;
      }

      [Observation]
      public void should_return_a_gradient_containing_10_colors()
      {
         _gradients.Count.ShouldBeEqualTo(10);
      }

      [Observation]
      public void the_first_color_should_be_the_start_color()
      {
         _gradients.First().ShouldBeEqualTo(_startColor);
      }

      [Observation]
      public void the_last_color_should_be_the_start_color()
      {
         _gradients.Last().ShouldBeEqualTo(_endColor);
      }
   }
}