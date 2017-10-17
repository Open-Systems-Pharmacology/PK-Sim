﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PKSim.Infrastructure.Serialization.Json
{
   public class WritablePropertiesOnlyResolver : DefaultContractResolver
   {
      protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
      {
         var props = base.CreateProperties(type, memberSerialization);
         return props.Where(p => p.Writable).ToList();
      }
   }
   }