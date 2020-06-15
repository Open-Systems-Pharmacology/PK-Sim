using FluentNHibernate.Utils;
using OSPSuite.BDDHelper;
using PKSim.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PKSim.Infrastructure
{
  public abstract class concern_for_FunctioExtensions : ContextSpecification<Func<int, int>>
  {
    protected Func<int, int> inc;
    protected Func<int, int> dup;
    protected override void Context()
    {
      inc = x => x + 1;
      dup = x => x * 2;
    }
  }

  public class FunctionComposition_inc_dup : concern_for_FunctioExtensions
  {
    protected override void Because()
    {
      sut = x => inc.Compose(dup, 5);
    }

    [Observation]
    public void should_increment_then_duplicate()
    {
      Assert.AreEqual(sut.Invoke(5), 12);
    }
  }

  public class FunctionComposition_dup_inc : concern_for_FunctioExtensions
  {
    protected override void Because()
    {
      sut = x => dup.Compose(inc, 5);
    }

    [Observation]
    public void should_duplicate_then_increment()
    {
      Assert.AreEqual(sut.Invoke(5), 11);
    }
  }
}
