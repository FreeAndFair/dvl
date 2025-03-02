﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aegis_DVL.Database;

namespace Aegis_DVL.Data_Types {
  [Serializable]
  public class PollingPlace : IComparable {
    public string LocationName { get; set; }
    public string Address { get; set; }
    public string CityStateZIP { get; set; }

    public readonly List<string> PrecinctIds = new List<string>();

    public PollingPlace() {
      LocationName = "Vote Center";
      Address = "Vote Center";
      CityStateZIP = "Dallas, TX  75XXX";
    }

    public PollingPlace(Precinct p) {
      LocationName = p.LocationName;
      Address = p.Address;
      CityStateZIP = p.CityStateZIP;
      PrecinctIds.Add(p.PrecinctSplitId);
    }

    public int CompareTo(Object obj) {
      if (obj == null) return 1;
      PollingPlace otherPollingPlace = obj as PollingPlace;
      if (otherPollingPlace != null) {
        return LocationName.CompareTo(otherPollingPlace.LocationName);
      } else {
        throw new ArgumentException("Argument is not a PollingPlace.");
      }
    }
  }
}
