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
        public Position position { get; set; }
        public int OVR { get; set; }
        public int POT { get; set; }
        public int RoomForGrowth { get { return POT - OVR; } }
        public int Age { get; set; }
        public int ContractExpiration { get; set; }
        public int SkillMoves { get; set; }
        public int WeakFoot { get; set; }
        public WorkRate OffensiveWorkrate { get; set; }
        public WorkRate DefensiveWorkrate { get; set; }
        public Foot StrongFoot { get; set; }
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
        SportingKC,
        BVB,
        BayernMunich
    }

    public enum Position
    {
        Striker,
        Goalkeeper,
        CenterBack
    }

    //Dan - 06/23/2018 - Each workrate level
    public enum WorkRate
    {
        High,
        Medium,
        Low
    }

    public enum Foot
    {
        Left,
        Right
    }
}