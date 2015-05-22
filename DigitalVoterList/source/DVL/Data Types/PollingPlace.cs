using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aegis_DVL.Database;

namespace Aegis_DVL.Data_Types {
  public class PollingPlace : IComparable {
    public string LocationName { get; set; }

    public readonly List<string> PrecinctIds = new List<string>();

    public PollingPlace(Precinct p) {
      LocationName = p.LocationName;
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
