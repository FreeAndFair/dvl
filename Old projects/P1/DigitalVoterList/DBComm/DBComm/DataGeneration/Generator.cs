// -----------------------------------------------------------------------
// <copyright file="Generator.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace DBComm.DBComm.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using global::DBComm.DBComm.DAO;
    using global::DBComm.DBComm.DO;

    /// <summary>
    /// A class to generate test data.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "*", Justification = "Generator code")]
    public class Generator
    {
        private readonly Data data = new Data();

        private readonly Random r = new Random();

        private readonly DigitalVoterList dvl;

        public Generator(DigitalVoterList dvl)
        {
            this.dvl = dvl;
        }

        public void Generate(int municipalities, int stations, int voters)
        {
            this.GenerateMunicipalities(municipalities);
            this.GeneratePollingStations(stations, municipalities);
            this.GenerateVoters(voters, stations);
        }

        public void GenerateMunicipalities(int municipalities)
        {
            var m = new HashSet<MunicipalityDO>();

            for (uint i = 0; i < municipalities; i++)
            {
                var municipality = new MunicipalityDO(i, this.data.GetRoadname() + " " + this.r.Next(1000), this.data.GetCityname(), this.data.GetMunicipalityname());
                m.Add(municipality);
            }

            var dao = new MunicipalityDAO(dvl);
            dao.Create(m);
        }

        public void GeneratePollingStations(int stations, int municipalities)
        {
            var p = new HashSet<PollingStationDO>();

            for (uint i = 0; i < stations; i++)
            {
                var pollingstation = new PollingStationDO(
                    (uint?)this.r.Next(municipalities), i, this.data.GetStationName(), this.data.GetRoadname() + " " + this.r.Next(1000));

                p.Add(pollingstation);
            }
            var dao = new PollingStationDAO(dvl);
            dao.Create(p);
        }

        public void GenerateVoters(int voters, int pollingstations)
        {
            var v = new HashSet<VoterDO>();

            for (uint i = 0; i < voters; i++)
            {
                uint cpr = data.GetCPR();

                var voter = new VoterDO((uint)this.r.Next(pollingstations), cpr, data.GetFirstName(cpr) + " " + data.GetLastname(), data.GetRoadname() + " " + r.Next(1000), data.GetCityname(), false, false);

                v.Add(voter);
            }
            var dao = new VoterDAO(dvl);
            dao.Create(v);
        }
    }
}