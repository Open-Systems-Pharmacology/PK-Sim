using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterDefaultStateUpdater : ContextSpecification<IParameterDefaultStateUpdater>
   {
      protected override void Context()
      {
         sut = new ParameterDefaultStateUpdater();
      }
   }
}	