﻿using System.Collections.Generic;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ClassificationContext
   {
      public IReadOnlyList<OSPSuite.Core.Domain.Classification> Classifications { get; set; }
   }
}