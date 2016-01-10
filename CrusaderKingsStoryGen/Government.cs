using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    class Government
    {
        public string type = "nomadic";
        public List<String> preferred_holdings = new List<string>() {"NOMAD"};

        public List<String> builds_with_prestige = new List<string>();
        public List<String> builds_with_piety = new List<string>();
        public List<String> allowed_holdings = new List<string>() { "NOMAD"};
        public List<String> allowed_holdings_culture = new List<string>();
        public List<String> allowed_holdings_religion = new List<string>();
        public List<String> allowed_holdings_culture_and_religion = new List<string>();
        public List<String> accepts_liege_governments = new List<string>();
        public List<String> free_revoke_on_governments_religion = new List<string>();

        public bool allowReligionHead = false;

        public List<String> cultureGroupAllow = new List<string>();
        public List<String> cultureAllow = new List<string>();
        public bool is_patrician = false;

        public Color color;

  
        public List<String> ignore_in_vassal_limit_calculation = new List<string>();
        public bool allows_matrilineal_marriage = true;
        
        public String title_prefix = null;
        public String frame_suffix = null;
        public bool merchant_republic = false;
        public bool uses_decadence = false;
        public bool uses_jizya_tax = false;
        public bool uses_piety_for_law_change = false;
        public bool uses_prestige_for_law_change = false;
        public bool allow_title_revokation = true;
        public bool allow_looting = true;
        public bool can_imprison_without_reason = true;
        public bool can_revoke_without_reason = true;
        public bool ignores_de_jure_laws = false;
        public bool dukes_called_kings = true;
        public bool barons_need_dynasty = false;
        public bool can_create_kingdoms = true;
        public bool can_usurp_kingdoms_and_empires = true;
        public bool have_gender_laws = true;
        public bool can_build_holdings = true;
        public bool can_build_forts = false;
        public bool can_build_castle = false;
        public bool can_build_city = false;
        public bool can_build_temple = true;
        public bool can_build_tribal = true;
        public bool can_grant_kingdoms_and_empires_to_other_government = true;
        public bool can_be_granted_kingdoms_and_empires_by_other_government = true;
        public bool free_retract_vassalage = true;
        public int max_consorts = 3 ;

        public double aggression = 4;
        public string name;

        static HashSet<String> done = new HashSet<string>();
        public static List<String> cultureDone = new List<string>();
        private bool can_change_to_nomad_on_start = false;

        private string GetWordList(List<string> strs)
        {
            done.Clear();
            String re = "";
            foreach (var str in strs)
            {
                if (!done.Contains(str))
                    re += str + " ";

                done.Add(str);                
                
            }

            return re;
        }

        public void Save(ScriptScope scriptScope)
        {
            String cultureBlock = "";

            foreach (var cultureParser in cultureAllow)
            {
                cultureBlock += "culture = " + cultureParser + @"
                ";
            }

            if (cultureAllow.Count == 0)
                return;

            int r = Rand.Next(255);
            int g = Rand.Next(255);
            int b = Rand.Next(255);

            scriptScope.Do(@"
                color = { " + r + " " + g + " " + b + @" }
                allow_looting = " + (allow_looting ? "yes" : "no") + @"
                allow_title_revokation = " + (allow_title_revokation ? "yes" : "no") + @"
                allows_matrilineal_marriage = " + (allows_matrilineal_marriage ? "yes" : "no") + @"
                barons_need_dynasty = " + (barons_need_dynasty ? "yes" : "no") + @"
                can_be_granted_kingdoms_and_empires_by_other_government = " + (can_be_granted_kingdoms_and_empires_by_other_government ? "yes" : "no") + @"
                can_build_castle = " + (can_build_castle ? "yes" : "no") + @"
                can_build_city = " + (can_build_city ? "yes" : "no") + @"
                can_build_forts = " + (can_build_forts ? "yes" : "no") + @"
                can_build_holdings = " + (can_build_holdings ? "yes" : "no") + @"
                can_build_temple = " + (can_build_temple ? "yes" : "no") + @"
                can_build_tribal = " + (can_build_tribal ? "yes" : "no") + @"
                can_create_kingdoms = " + (can_create_kingdoms ? "yes" : "no") + @"
                can_grant_kingdoms_and_empires_to_other_government = " + (can_grant_kingdoms_and_empires_to_other_government ? "yes" : "no") + @"
                can_imprison_without_reason = " + (can_imprison_without_reason ? "yes" : "no") + @"
                can_revoke_without_reason = " + (can_revoke_without_reason ? "yes" : "no") + @"
                can_usurp_kingdoms_and_empires = " + (can_usurp_kingdoms_and_empires ? "yes" : "no") + @"
                dukes_called_kings = " + (dukes_called_kings ? "yes" : "no") + @"
                free_retract_vassalage = " + (free_retract_vassalage ? "yes" : "no") + @"
                ignores_de_jure_laws = " + (ignores_de_jure_laws ? "yes" : "no") + @"
                can_change_to_nomad_on_start = " + (can_change_to_nomad_on_start ? "yes" : "no") + @"
          
                " + (frame_suffix != null ? @"
			        frame_suffix = " + frame_suffix : "") + @"
                " + (title_prefix != null ? @"
			        title_prefix = " + title_prefix : "") + @"
            
                merchant_republic = " + (type=="republic" ? "yes" : "no") + @"
                uses_decadence = " + (uses_decadence ? "yes" : "no") + @"
                uses_piety_for_law_change = " + (uses_piety_for_law_change ? "yes" : "no") + @"
                uses_prestige_for_law_change = " + (uses_prestige_for_law_change ? "yes" : "no") + @"
                " + (preferred_holdings.Count > 0 ? @"
                preferred_holdings = { 
                    " + GetWordList(preferred_holdings) + @" 
                }" : "" ) + @"
                " + (allowed_holdings.Count > 0 ? @"
                allowed_holdings = { 
                    " + GetWordList(allowed_holdings) + @" 
                }" : "") + @"
                " + (allowed_holdings_culture.Count > 0 ? @"
                allowed_holdings_culture = { 
                    " + GetWordList(allowed_holdings_culture) + @" 
                }" : "") + @"
                " + (allowed_holdings_culture_and_religion.Count > 0 ? @"
                allowed_holdings_culture_and_religion = { 
                    " + GetWordList(allowed_holdings_culture_and_religion) + @" 
                }" : "") + @"
                " + (builds_with_prestige.Count > 0 ? @"
                builds_with_prestige = { 
                    " + GetWordList(builds_with_prestige) + @" 
                }" : "") + @"
                " + (builds_with_piety.Count > 0 ? @"
                builds_with_piety = { 
                    " + GetWordList(builds_with_piety) + @" 
                }" : "") + @"
             
                " + (accepts_liege_governments.Count > 0 ? @"
                accepts_liege_governments = { 
                    " + GetWordList(accepts_liege_governments) + @" 
                }" : "") + @"
    
                potential = {
                    " + (cultureAllow.Count > 1 ? @"
			        OR = {
                        " + cultureBlock + @"
                    } " : cultureBlock ) + @"
			        is_patrician = " + (type == "republic" ? "yes" : "no") + @"
		            mercenary = no
			        holy_order = no
                }
                ");
        }
        public Government Mutate(int numChanges)
        {
          
            Government g = new Government();

            g.frame_suffix = frame_suffix;
            g.title_prefix = title_prefix;
            g.aggression = aggression;
            g.allowReligionHead = allowReligionHead;
            g.allow_looting = allow_looting;
            g.allow_title_revokation = allow_title_revokation;
            g.allows_matrilineal_marriage = allows_matrilineal_marriage;
            g.barons_need_dynasty = barons_need_dynasty;
            g.can_be_granted_kingdoms_and_empires_by_other_government = can_be_granted_kingdoms_and_empires_by_other_government;
            g.can_build_castle = can_build_castle;
            g.can_build_city = can_build_city;
            g.can_build_forts = can_build_forts;
            g.can_build_holdings = can_build_holdings;
            g.can_build_temple = can_build_temple;
            g.can_build_tribal = can_build_tribal;
            g.can_create_kingdoms = can_create_kingdoms;
            g.can_grant_kingdoms_and_empires_to_other_government = can_grant_kingdoms_and_empires_to_other_government;
            g.can_imprison_without_reason = can_imprison_without_reason;
            g.can_revoke_without_reason = can_revoke_without_reason;
            g.can_usurp_kingdoms_and_empires = can_usurp_kingdoms_and_empires;
            g.dukes_called_kings = dukes_called_kings;
            g.free_retract_vassalage = free_retract_vassalage;
            g.have_gender_laws = have_gender_laws;
            g.ignores_de_jure_laws = ignores_de_jure_laws;
            g.is_patrician = is_patrician;
            g.merchant_republic = merchant_republic;
            g.uses_decadence = uses_decadence;
            g.uses_jizya_tax = uses_jizya_tax;
            g.uses_piety_for_law_change = uses_piety_for_law_change;
            g.uses_prestige_for_law_change = uses_prestige_for_law_change;

            g.accepts_liege_governments.AddRange(accepts_liege_governments);
            g.preferred_holdings.AddRange(preferred_holdings);
            g.allowed_holdings.AddRange(allowed_holdings);
            g.allowed_holdings_culture.AddRange(allowed_holdings_culture);
            g.allowed_holdings_culture_and_religion.AddRange(allowed_holdings_culture_and_religion);
//            g.cultureAllow.AddRange(cultureAllow);
            g.builds_with_prestige.AddRange(builds_with_prestige);
            g.builds_with_piety.AddRange(builds_with_piety);
            g.color = color;
            g.type = type;
            g.SetType(type);
            if (type == "feudal")
            {
                if (Rand.Next(5) == 0)
                {
                    switch (Rand.Next(2))
                    {
                        case 0:
                            g.type = "theocracy";
                            break;
                        case 1:
                            g.type = "republic";
                            break;

                    }

                    SetType(g.type);
                }
            }
            else if (type == "tribal")
            {
                if (Rand.Next(3) == 0)
                {
                    g.type = "feudal";
                     
                    SetType(g.type);
                }
            }
            else if (type == "theocracy")
            {
                if (Rand.Next(5) == 0)
                {
                    switch (Rand.Next(2))
                    {
                        case 0:
                            g.type = "republic";
                            break;
                        case 1:
                            g.type = "feudal";
                            break;

                    }

                    SetType(g.type);
                }
            }

            
            if (type == "nomadic")
            {
                if (Rand.Next(5) == 0 || GovernmentManager.instance.numNomadic > 10)
                {
                    g.type = "tribal";
           
                    SetType(g.type);
                }
            }

            if (type == "tribal" && GovernmentManager.instance.numTribal > 20)
            {
                g.type = "feudal";

                SetType(g.type);
            }

            if (type == "tribal")
                GovernmentManager.instance.numTribal++;

            if (type == "nomadic")
                GovernmentManager.instance.numNomadic++;

/*
            if (Rand.Next(5) == 0)
            {
              
                switch (Rand.Next(5))
                {
                    case 0:
                        g.type = "feudal";
                        g.can_build_castle = true;
                        if (!g.allowed_holdings.Contains("CASTLE"))
                            g.allowed_holdings.Add("CASTLE");
                        if (!g.preferred_holdings.Contains("CASTLE"))
                            g.preferred_holdings.Add("CASTLE");
                       break;
                    case 1:
                        g.type = "tribal";
                         g.preferred_holdings.Clear();
                            g.allowed_holdings.Clear();
                          g.allowed_holdings.Add("TRIBAL");
                          g.preferred_holdings.Add("TRIBAL");
                          g.can_build_tribal = true;
                        break;
                    case 2:
                        g.type = "nomadic";
                        g.preferred_holdings.Clear();
                        g.allowed_holdings.Clear();
                        g.allowed_holdings.Add("NOMAD");
                        g.preferred_holdings.Add("NOMAD");
                        break;
                    case 3:
                        g.type = "theocracy";
                        if (!g.allowed_holdings.Contains("TEMPLE"))
                            g.allowed_holdings.Add("TEMPLE");
                        if (!g.preferred_holdings.Contains("TEMPLE"))
                            g.preferred_holdings.Add("TEMPLE");
                        g.can_build_temple = true;
                        break;
                    case 4:
                        g.type = "republic";
                        break;
                }
            }
            */


            for (int n = 0; n < numChanges; n++)
            {
                g.DoChange();
            }
            if(!GovernmentManager.instance.governments.Contains(g))
                GovernmentManager.instance.governments.Add(g);
            return g;
        }

        public void SetType(string type)
        {
            allowed_holdings.Clear();
            preferred_holdings.Clear();
            allowed_holdings_culture.Clear();
            accepts_liege_governments.Clear();
            this.type = type;
            accepts_liege_governments.Add(name);
            switch (type)
            {
                case "nomadic":
                    allowed_holdings.Add("NOMAD");
                    preferred_holdings.Add("NOMAD");
                    can_build_castle = false;
                    can_build_city = false;
                    can_build_forts = false;
                    can_build_holdings = false;
                    can_build_temple = true;
                    can_build_tribal = false;
                    can_create_kingdoms = false;
                    frame_suffix = "_nomadic";
                    title_prefix = "nomadic_";
                    is_patrician = false;
                    break;
                case "tribal":
                    allowed_holdings.Add("TRIBAL");
                    allowed_holdings.Add("FORT");
                    preferred_holdings.Add("TRIBAL");
                    can_build_castle = false;
                    can_build_city = false;
                    can_build_forts = false;
                    can_build_holdings = true;
                    can_build_temple = true;
                    can_build_tribal = true;
                    can_create_kingdoms = true;
                      frame_suffix = "_tribal";
                    title_prefix = "tribal_";
                    is_patrician = false;
                    can_change_to_nomad_on_start = true;
                    break;
                case "feudal":
                    allowed_holdings.Add("CASTLE");
                    allowed_holdings.Add("FORT");
                    preferred_holdings.Add("CASTLE");
                    can_build_castle = true;
                    can_build_city = true;
                    can_build_forts = true;
                    can_build_holdings = true;
                    can_build_temple = true;
                    can_build_tribal = false;
                    frame_suffix = null;
                    title_prefix = null;
                    is_patrician = false;
                    can_create_kingdoms = true;
                    allowed_holdings_culture.Add("TRIBAL");
                    break;
                case "theocracy":
                    allowed_holdings.Add("TEMPLE");
                    allowed_holdings.Add("CASTLE");
                    allowed_holdings.Add("FORT");
                    preferred_holdings.Add("TEMPLE");
                    frame_suffix = "_theocracy";
                    title_prefix = "temple_";
                    can_build_castle = true;
                    can_build_city = true;
                    can_build_forts = true;
                    can_build_holdings = true;
                    can_build_temple = true;
                    can_build_tribal = false;
                    can_create_kingdoms = true;
                    is_patrician = false;
                    allowed_holdings_culture.Add("TRIBAL");
                    break;
                case "republic":
                    allowed_holdings.Add("TRADE_POST");
                    allowed_holdings.Add("CITY");
                    allowed_holdings.Add("CASTLE");
                    allowed_holdings.Add("FAMILY_PALACE");
                    allowed_holdings.Add("FORT");
                    preferred_holdings.Add("CITY");
                    frame_suffix = "_merchantrepublic";
                    title_prefix = "city_";
                    can_build_castle = true;
                    can_build_city = true;
                    can_build_forts = true;
                    can_build_holdings = true;
                    can_build_temple = true;
                    can_build_tribal = false;
                    can_create_kingdoms = true;
                    is_patrician = true;
                    allowed_holdings_culture.Add("TRIBAL");
                    break;
            }
        }

        private void DoChange()
        {
            switch (Rand.Next(15))
            {
                case 0:
                    aggression = Rand.Next(5);
                    if (aggression > 3)
                    {
                        DoWarryChanges();
                    }
                    if (aggression < 2)
                    {
                        DoPeaceyChanges();
                    }
                    break;
                case 1:
                    allow_title_revokation = !allow_title_revokation;
                    break;
                case 2:
                    can_revoke_without_reason = !can_revoke_without_reason;
                    if (can_revoke_without_reason)
                    {
                        allow_title_revokation = true;
                        can_usurp_kingdoms_and_empires = true;
                    }
                    break;
                case 3:
                    can_imprison_without_reason = !can_imprison_without_reason;
                    if (can_imprison_without_reason)
                    {
                        can_revoke_without_reason = true;
                        allow_title_revokation = true;
                        can_usurp_kingdoms_and_empires = true;
                    }
                    break;
     
                case 4:
                    can_create_kingdoms = !can_create_kingdoms;
                    break;
                case 5:
                    can_grant_kingdoms_and_empires_to_other_government = !can_grant_kingdoms_and_empires_to_other_government;
                    break;
                case 6:
                    dukes_called_kings = !dukes_called_kings;
                    break;
                case 7:
                    free_retract_vassalage = !free_retract_vassalage;
                    break;
                 case 8:
                    ignores_de_jure_laws = !ignores_de_jure_laws;
                    break;
                case 9:
                    is_patrician = !is_patrician;
                    break;
                case 10:
                    merchant_republic = !merchant_republic;
                    break;
                case 11:
                    uses_decadence = !uses_decadence;
                    break;
                case 12:
                    uses_jizya_tax = !uses_jizya_tax;
                    break;
                case 13:
                    uses_piety_for_law_change = !uses_piety_for_law_change;
                    if (uses_piety_for_law_change)
                        uses_prestige_for_law_change = false;
                    break;
                case 14:
                    uses_prestige_for_law_change = !uses_prestige_for_law_change;
                    if (uses_prestige_for_law_change)
                        uses_piety_for_law_change = false;
                    break;
              
            }

        }

        private void DoPeaceyChanges()
        {
            allow_looting = false;
        }

        private void DoWarryChanges()
        {
            allow_looting = true;
        }

    }
}
