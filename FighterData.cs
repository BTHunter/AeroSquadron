using System;
using System.Collections;

namespace AeroSquadron
{
    public enum eLocation {Nose, Wing, Aft};
    public enum eTech {Undefined,InnerSphere, Clan, Mixed};
    public enum eType {Other,SRM,LRM,Direct};

    public struct tLocations
    {
        public int Nose;
        public int RightWing;
        public int LeftWing;
        public int Aft;
    }

    public struct tDamageValues
    {
        public int Short;
        public int Medium;
        public int Long;
        public int Extreme;
    }

    public struct tWeapondata
    {
        public int ID;
        public eTech Tech;
        public int Count;
        public eLocation Location;
        public eType Type;
    }

    public struct tWeaponvalues
    {
        public string Name;
        public int Heat;
        public int SRV, MRV, LRV, ERV;
    }

    public struct tSquadronData
    {
        public int Heatsinks;
        public tLocations Heat;
        public int Thrust;
        public int BV;
        public int Cost;
    }

    /// <summary>
    /// Summary description for Data.
    /// </summary>
    public class FighterData
    {
        private string sName;
        private eTech iTech;

        private int iThrust;
        private int iHeatsinks;
        private int iBV;
        private int iCost;
        private int iFuel;
        private int iArmor;
        private int iCount;
        private bool bSRMArtemis;
        private bool bLRMArtemis;
        private bool bTargeting;

        private tLocations oHeat;

        private ArrayList alWeapons;

        public FighterData()
        {
            sName = "";
            iTech = eTech.Undefined;
            iThrust = 3;
            iHeatsinks = 10;
            iBV = 0;
            iCost = 0;
            iFuel = 0;
            iCount = 1;
            iArmor = 0;

            oHeat.Nose = 0;
            oHeat.RightWing = 0;
            oHeat.LeftWing = 0;
            oHeat.Aft = 0;

            bSRMArtemis = false;
            bLRMArtemis = false;
            bTargeting = false;
			
            alWeapons = new ArrayList();
        }

        public void AddWeapon(int ipID, int ipCount, eLocation epLocation, eType epType)
        {
            tWeapondata oWeaponData;

            oWeaponData.ID = ipID;
            oWeaponData.Count = ipCount;
            //oWeaponData.Heat = ipHeat;
            oWeaponData.Location = epLocation;
            oWeaponData.Tech = eTech.Undefined;
            oWeaponData.Type = epType;

            alWeapons.Add(oWeaponData);
        }

        public ArrayList GetWeapons()
        {
            return alWeapons;
        }

        public string Name
        {
            get
            {
                return sName;
            }
            set
            {
                sName = value;
            }
        }

        public int Thrust
        {
            get
            {
                return iThrust;
            }
            set
            {
                iThrust = value;
            }
        }

        public int Cost
        {
            get
            {
                return iCost;
            }
            set
            {
                iCost = value;
            }
        }

        public int BV
        {
            get
            {
                return iBV;
            }
            set
            {
                iBV = value;
            }
        }

        public int Fuel
        {
            get
            {
                return iFuel;
            }
            set
            {
                iFuel = value;
            }
        }

        public int Heatsinks
        {
            get
            {
                return iHeatsinks;
            }
            set
            {
                iHeatsinks = value;
            }
        }

        public int Armor
        {
            get
            {
                return iArmor;
            }
            set
            {
                iArmor = value;
            }
        }

        public int Count
        {
            get
            {
                return iCount;
            }
            set
            {
                iCount = value;
            }
        }

        public bool SRMArtemis
        {
            get
            {
                return bSRMArtemis;
            }
            set
            {
                bSRMArtemis = value;
                tWeapondata oWeapon;
                for (int i = 0; i < alWeapons.Count; i++)
                {
                    oWeapon = (tWeapondata) alWeapons[i];
                    if (oWeapon.Type == eType.SRM)
                    {
                        if (bSRMArtemis && (oWeapon.ID < 0xffff))
                        {
                            oWeapon.ID += 0xffff;
                            alWeapons[i] = oWeapon;
                        } 
                        else if (!bSRMArtemis && (oWeapon.ID > 0xffff))
                        {
                            oWeapon.ID -= 0xffff;
                            alWeapons[i] = oWeapon;
                        }
                    }
                }
            }
        }

        public bool LRMArtemis
        {
            get
            {
                return bLRMArtemis;
            }
            set
            {
                bLRMArtemis = value;
                tWeapondata oWeapon;
                for (int i = 0; i < alWeapons.Count; i++)
                {
                    oWeapon = (tWeapondata) alWeapons[i];
                    if (oWeapon.Type == eType.LRM)
                    {
                        if (bLRMArtemis && (oWeapon.ID < 0xffff))
                        {
                            oWeapon.ID += 0xffff;
                            alWeapons[i] = oWeapon;
                        } 
                        else if (!bLRMArtemis && (oWeapon.ID > 0xffff))
                        {
                            oWeapon.ID -= 0xffff;
                            alWeapons[i] = oWeapon;
                        }
                    }
                }
            }
        }

        public bool Targeting
        {
            get
            {
                return bTargeting;
            }
            set
            {
                bTargeting = value;
            }
        }

        public eTech Tech
        {
            get
            {
                return iTech;
            }
            set
            {
                iTech = value;
            }
        }

        public tLocations Heat
        {
            get
            {
                return oHeat;
            }
            set
            {
                oHeat = value;
            }
        }
    }
}
