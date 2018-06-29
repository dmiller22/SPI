using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPI.Models
{
    //Dan - 06/23/2018 - Basic info for each player
    public class Player_Basic
    {
        public int PlayerID { get; set; }
        public string Name { get; set; }
        public int NationalTeamID { get; set; }
        public NationalTeam NationalTeam { get; set; }
        public int ClubTeamID { get; set; }
        public ClubTeam ClubTeam { get; set; }
        public string position { get; set; }
        public int OVR { get; set; }
        public int POT { get; set; }
        public int RoomForGrowth { get { return POT - OVR; } }
        public int Age { get; set; }
        public int ContractExpiration { get; set; }
        public int SkillMoves { get; set; }
        public int WeakFoot { get; set; }
        public string OffensiveWorkrate { get; set; }
        public string DefensiveWorkrate { get; set; }
        public string StrongFoot { get; set; }
    }

    //Dan - {Insert Date} - NationalTeams and their ID
    public enum NationalTeam
    {
        UnitedStates,
        Mexico,
        Germany = 21,
        CostaRica,
        Poland = 37
    }

    //Dan - {Insert Date} - ClubTeams and their ID
    public enum ClubTeam
    {
        BayernMunich = 21,
        BVB = 22,
        MonchenGladbach = 23,
        Freiburg = 25,
        Hamburg = 28,
        FCKoln = 31,
        Leverkusen = 32,
        Schalke = 34,
        Stuttgart = 36,
        WerderBremen = 38,
        HerthaBerlin = 166,
        Mainz = 169,
        Wolfsburg = 175,
        Hannover = 485,
        ColumbusCrew = 687,
        DCUnited = 688,
        NewYorkRedBulls = 689,
        NewEnglandRevolution = 691,
        ChicagoFire = 693,
        ColoradoRapids = 694,
        FCDallas = 695,
        SportingKC = 696,
        LAGalaxy = 697,
        HoustonDynamo = 698,
        EintrachtFrankfurt = 1824,
        Hoffenheim = 10029,
        Augsburg = 100409,
        VancouverWhitecaps = 101112,
        RealSaltLake = 111065,
        MinnesotaUnited = 111138,
        MontralImpact = 111139,
        PortlandTimbers = 111140,
        SeattleSounders = 111144,
        TorontoFC = 111651,
        SanJoseEarthquake = 111928,
        PhiladelphiaUnion = 112134,
        RBLeipzig = 112172,
        OrlandoCity = 112606,
        NewYorkCityFC = 112828,
        AtlantaUnited = 112885,
        LosAngelesFC = 112996
    }

    public enum Leagues
    {
        Bundesliga = 19,
        MLS = 39
    }
}