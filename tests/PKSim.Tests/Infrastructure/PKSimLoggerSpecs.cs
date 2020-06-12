using OSPSuite.BDDHelper;
using PKSim.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using DevExpress.Utils.VisualEffects;
using Microsoft.Extensions.Logging;
using DevExpress.Utils.Extensions;
using FluentNHibernate.Utils;
using FakeItEasy.Configuration;

namespace PKSim.Infrastructure
{
  public abstract class concern_for_PKSimLogger : ContextSpecification<PKSimLogger>
  {
    protected override void Context()
    {
      sut = new PKSimLogger();
    }
  }

  public class When_the_pk_sim_logger_is_configured : concern_for_PKSimLogger
  {
    [Observation]
    public void should_invoke_new_action()
    {
      var f1 = A.Fake<Func<ILoggingBuilder, ILoggingBuilder>>();
      sut.AddLoggingBuilderConfiguration(f1);
      A.CallTo(() => f1.Invoke(A<ILoggingBuilder>.Ignored)).MustHaveHappened();
    }

    [Observation]
    public void should_invoke_all_actions_in_order()
    {
      var iterations = 10;
      var functions = new List<Func<ILoggingBuilder, ILoggingBuilder>>();
      for (var i = 0; i < iterations; i++)
      {
        functions.Add(A.Fake<Func<ILoggingBuilder, ILoggingBuilder>>());
      }

      for (var i = 0; i < iterations; i++)
      {
        sut.AddLoggingBuilderConfiguration(functions[i]);
        IOrderableCallAssertion check = A.CallTo(() => functions[0].Invoke(A<ILoggingBuilder>.Ignored)).MustHaveHappened();
        for (var j = 1; j < i; j++)
        {
          check = check.Then(A.CallTo(() => functions[j].Invoke(A<ILoggingBuilder>.Ignored)).MustHaveHappened());
        }
        for (var j = i + 1; j < iterations; j++)
        {
          A.CallTo(() => functions[j].Invoke(A<ILoggingBuilder>.Ignored)).MustNotHaveHappened();
        }
        for (var j = 0; j < i; j++)
        {
          Fake.ClearConfiguration(functions[i]);
        }
      }
    }
  }
}
