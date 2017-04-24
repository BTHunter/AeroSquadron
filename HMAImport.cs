using System;
using System.IO;
using System.Collections;
using System.Xml;

namespace AeroSquadron
{
    /// <summary>
    /// Summary description for ExtendedReader.
    /// </summary>
    public class ExtendedReader: System.IO.BinaryReader
    {
        public void SkipBytes(int iOffset)
        {
            this.BaseStream.Seek(iOffset,System.IO.SeekOrigin.Current);
        }

        public ExtendedReader(System.IO.Stream input): base(input)
        {
        }
    }

	/// <summary>
	/// Summary description for Import.
	/// </summary>
	public class HMAImport
	{
        SortedList slWeaponsClan, slWeaponsIS;

        public SortedList WeaponsClan
        {
            get 
            {
                return slWeaponsClan;
            }
            set
            {
                slWeaponsClan = value;
            }
        }

        public SortedList WeaponsIS
        {
            get 
            {
                return slWeaponsIS;
            }
            set
            {
                slWeaponsIS = value;
            }
        }

		public HMAImport()
		{
            slWeaponsClan = new SortedList();
            slWeaponsIS = new SortedList();
		}

        public FighterData ReadFile(string sFilename)
        {
            int iLength, iTemp;

            FileStream fDatafile = new FileStream(sFilename, FileMode.Open);
            ExtendedReader fReader = new ExtendedReader(fDatafile);
            fReader.BaseStream.Seek(0,System.IO.SeekOrigin.Begin);

            FighterData oFighter = new FighterData();

            fReader.SkipBytes(5);

            fReader.SkipBytes(2);
      
            //design type - fighter or not
            iTemp = fReader.ReadUInt16();
            if (iTemp != 1)
            {
                return null;
            }

            // some flags saying which Clans use this design
            fReader.SkipBytes(4);

            // some flags saying which Inner Sphere factions use this design
            fReader.SkipBytes(4);

            fReader.SkipBytes(2);
      
            // Gewicht, 64bit Int
            iTemp = (int) Math.Floor(fReader.ReadInt64() / 10000d);

            iLength = fReader.ReadUInt16();
            oFighter.Name = new String(fReader.ReadChars(iLength)).Trim();

            iLength = fReader.ReadUInt16();
            oFighter.Name = oFighter.Name + " " + new String(fReader.ReadChars(iLength)).Trim();

            //rules level
            iTemp = fReader.ReadUInt16();

            fReader.SkipBytes(2);

            oFighter.Cost = (int) Math.Floor(fReader.ReadInt64() / 10000d);

            oFighter.BV = (int) Math.Floor(fReader.ReadInt64() / 10000d);

            fReader.SkipBytes(24);

            //BF2 data
            iTemp = fReader.ReadInt16();
            fReader.SkipBytes(iTemp);

            //fReader.SkipBytes(36);

            fReader.SkipBytes(2);
            fReader.SkipBytes(2);

            iTemp = fReader.ReadInt16();
            fReader.SkipBytes(iTemp);

            iTemp = fReader.ReadInt16();
            fReader.SkipBytes(iTemp);

            iTemp = fReader.ReadInt16();
            fReader.SkipBytes(iTemp);

            iTemp = fReader.ReadInt16();
            fReader.SkipBytes(iTemp);

            //64bit number, factor 1000 or 10000
            fReader.SkipBytes(8);

            //heatsink type, add 1 to have the factor (1 or 2)
            iTemp = (fReader.ReadInt16() + 1);

            oFighter.Heatsinks = fReader.ReadInt32() * iTemp;
            oFighter.Fuel = (int) Math.Floor(fReader.ReadInt32() / 10000d);
            fReader.SkipBytes(4);

            //level (??)
            iTemp = fReader.ReadInt16();
            fReader.SkipBytes(2);

            //si
            iTemp = fReader.ReadInt16();
            fReader.SkipBytes(2);

            fReader.SkipBytes(12);

            //origin
            //0 = IS, 1 = Clan, 2 = Mixed
            iTemp = fReader.ReadUInt16();
            switch (iTemp)
            {
                case 0: oFighter.Tech = eTech.InnerSphere;
                        break;
                case 1: oFighter.Tech = eTech.Clan;
                        break;
                case 2: oFighter.Tech = eTech.Mixed;
                        break;
                default: oFighter.Tech = eTech.Undefined;
                        break;
            }

            if (iTemp == 2)
            {
                //origin at mixed-tech
                //0 = ID, 1 = Clan
                iTemp = fReader.ReadUInt16();
                switch (iTemp)
                {
                    case 0: oFighter.Tech = eTech.InnerSphere;
                            break;
                    case 1: oFighter.Tech = eTech.Clan;
                            break;
                    default: oFighter.Tech = eTech.Undefined;
                            break;
                }

                iTemp = fReader.ReadUInt16();
                iTemp = fReader.ReadUInt16();
                iTemp = fReader.ReadUInt16();
            }
            fReader.SkipBytes(4);

            //reactor value
            fReader.SkipBytes(2);
            //probably reactor type
            fReader.SkipBytes(2);
            //thrust
            oFighter.Thrust = fReader.ReadInt16();

            fReader.SkipBytes(4);

            //armor type
            fReader.SkipBytes(2);

            //nose armor
            oFighter.Armor = fReader.ReadInt16();
            fReader.SkipBytes(2); // oder int32?

            //wing
            oFighter.Armor += fReader.ReadInt16();
            fReader.SkipBytes(2); // oder int32?

            fReader.SkipBytes(4);

            //unused location
            iTemp = fReader.ReadInt16();
            fReader.SkipBytes(2); // oder int32?

            //aft
            oFighter.Armor += fReader.ReadInt16();
            fReader.SkipBytes(2); // oder int32?

            oFighter.Armor += fReader.ReadInt16();
            fReader.SkipBytes(2); // oder int32?

            fReader.SkipBytes(4);

            //unused location
            int frontlinkspanzerung = fReader.ReadInt16();
            fReader.SkipBytes(2); // oder int32?

            fReader.SkipBytes(2);

            int anzahlwaffen = fReader.ReadInt16();

            int iCount, iWeaponID, iLocation;
            eType oType = eType.Other; 
            eLocation oLocation;
            string sManufacturer;
            for (int i = 0; i < anzahlwaffen; i++)
            {
                iCount = fReader.ReadInt16();

                iWeaponID = fReader.ReadInt16();

                iLength = fReader.ReadInt16();
                sManufacturer = new String(fReader.ReadChars(iLength));

                iLocation = fReader.ReadInt16();

                switch (iLocation)
                {
                    case 1 : oLocation = eLocation.Nose;
                             break;
                    case 2 : oLocation = eLocation.Wing;
                             break;
                    case 5 : oLocation = eLocation.Aft;
                             break;
                    default: oLocation = eLocation.Nose;
                             break;
                }

                oType = eType.Other;
                if (oFighter.Tech == eTech.Clan)
                {
                    if (((tWeaponvalues) slWeaponsClan[iWeaponID]).Name.StartsWith("SRM"))
                    {
                        oType = eType.SRM;
                    }
                    else if (((tWeaponvalues) slWeaponsClan[iWeaponID]).Name.StartsWith("LRM"))
                    {
                        oType = eType.LRM;
                    }
                }
                else if (oFighter.Tech == eTech.InnerSphere) 
                {
                    if (((tWeaponvalues) slWeaponsIS[iWeaponID]).Name.StartsWith("SRM"))
                    {
                        oType = eType.SRM;
                    }
                    else if (((tWeaponvalues) slWeaponsIS[iWeaponID]).Name.StartsWith("LRM"))
                    {
                        oType = eType.LRM;
                    }
                }

                oFighter.AddWeapon(iWeaponID,iCount,oLocation,oType);
                fReader.SkipBytes(6);
            }

            int einzelgewicht;
            int gesamtgewicht;
            int sppunkte;
            int anzahl;
            int cost;
            string bezeichnung;

            //müsste Anzalh Elemente sein (Beginn bei 0)
            iTemp = fReader.ReadInt16();
            for (int i = 0; i <= iTemp; i++)
            {
                anzahl = fReader.ReadInt32();
                fReader.SkipBytes(2);

                einzelgewicht = (int) Math.Floor(fReader.ReadInt64() / 10000d);

                sppunkte = (int) Math.Floor(fReader.ReadInt64() / 10000d);

                gesamtgewicht = (int) Math.Floor(fReader.ReadInt64() / 10000d);

                fReader.SkipBytes(4);

                iLength = fReader.ReadInt16();
                bezeichnung = new String(fReader.ReadChars(iLength));
            }

            //müsste Anzalh Elemente sein (Beginn bei 0)
            iTemp = fReader.ReadInt16();
            for (int i = 0; i <= iTemp; i++)
            {
                anzahl = fReader.ReadInt32();
                fReader.SkipBytes(2); //durchmesser

                einzelgewicht = (int) Math.Floor(fReader.ReadInt64() / 10000d);

                cost = (int) Math.Floor(fReader.ReadInt64() / 10000d);

                gesamtgewicht = (int) Math.Floor(fReader.ReadInt64() / 10000d);

                fReader.SkipBytes(4);

                iLength = fReader.ReadInt16();
                bezeichnung = new String(fReader.ReadChars(iLength));
            }

            int schotten;
            int preis;
            while (fReader.ReadInt32() > -1) //FF FF FF FF
            {
                anzahl = fReader.ReadInt32();

                schotten = fReader.ReadInt16();

                einzelgewicht = (int) Math.Floor(fReader.ReadInt64() / 10000d);

                preis = (int) Math.Floor(fReader.ReadInt64() / 10000d);

                gesamtgewicht = (int) Math.Floor(fReader.ReadInt64() / 10000d);

                fReader.SkipBytes(2);
                fReader.SkipBytes(2);

                iLength = fReader.ReadInt16();
                bezeichnung = new String(fReader.ReadChars(iLength));
            }
            //seperator FF FF FF FF FF FF FF FF
            fReader.SkipBytes(8);
            //other fields
            fReader.SkipBytes(6);
            fReader.SkipBytes(8);

            //targeting computer
            if (fReader.ReadUInt16() != 0) 
            {
                oFighter.Targeting = true;
            }

            //Artemis
            switch (fReader.ReadUInt16())
            {
                case 1: oFighter.SRMArtemis = true;
                        break;
                case 2: oFighter.LRMArtemis = true;
                        break;
                case 3: oFighter.SRMArtemis = true;
                        oFighter.LRMArtemis = true;
                        break;
                default:break;
            }

            fReader.Close();
            fDatafile.Close();

            return oFighter;
        }

        public void ReadWeapons()
        {
            slWeaponsIS = ReadWeapondata(eTech.InnerSphere,"is-weapa.dat");
            slWeaponsClan = ReadWeapondata(eTech.Clan,"cl-weapa.dat");
        }

        private SortedList ReadWeapondata(eTech epTech, string spFilename)
        {
            int iWeaponID = 0;
            int iLength, iCount, iDamage;
            string sName;
            double dTemp;
            int iTemp;
            string sTemp;
            char[] cTemp;
            byte[] abTemp;
            tWeaponvalues oWeapon;
            SortedList slWeapons = new SortedList();

            FileStream fDatafile;
            ExtendedReader fReader;
            try
            {
                fDatafile = new FileStream(spFilename, FileMode.Open);
                fReader = new ExtendedReader(fDatafile);
                fReader.BaseStream.Seek(0,System.IO.SeekOrigin.Begin);
            }
            catch
            {
                fReader =  null;
                fDatafile = null;
            }

            if ((fReader != null) && (fDatafile != null)) 
            {
                iLength = fReader.ReadInt16();
                string sVersion = new String(fReader.ReadChars(iLength));

                iCount = fReader.ReadInt16();

                fReader.SkipBytes(2);

                try
                {
                    do
                    {
                        oWeapon = new tWeaponvalues();

                        fReader.SkipBytes(2);
                        fReader.SkipBytes(6);

                        //works only until #132...
                        if (iWeaponID == 0)
                        {
                            iWeaponID = fReader.ReadInt16();
                        }
                        else
                        {
                            fReader.SkipBytes(2);
                            iWeaponID++;
                        }

                        iLength = fReader.ReadInt16();
                        abTemp = new byte[iLength];
                        fReader.Read(abTemp,0,iLength);

                        cTemp = new char[iLength];
                        for (int j = 0; j < iLength; j++) cTemp[j] = (char) abTemp[j];
                        sName = new String(cTemp);
                        oWeapon.Name = sName.Trim();

                        //text for ammo
                        iLength = fReader.ReadInt16();
                        fReader.SkipBytes(iLength);

                        //heat as text
                        iLength = fReader.ReadInt16();
                        sTemp = new String(fReader.ReadChars(iLength));

                        try
                        {
                            oWeapon.Heat = Convert.ToInt32(sTemp);
                        }
                        catch (FormatException e)
                        {
                            oWeapon.Heat = 0;
                        }

                        //salvos per shot
                        iTemp = fReader.ReadInt16();

                        //damage per salvo (eg, single missles)
                        iTemp = fReader.ReadInt16();

                        //AT2 damage with ECM
                        iTemp = fReader.ReadInt16();

                        //AT2 damage without ECM or with special-attack (like Ultra-mode or cluster ammo)
                        iTemp = fReader.ReadInt16();
                        iDamage = iTemp;

                        //damage as text
                        iLength = fReader.ReadInt16();
                        sTemp = new String(fReader.ReadChars(iLength));

                        fReader.SkipBytes(10);

                        //range in hexes
                        fReader.SkipBytes(2);
                        fReader.SkipBytes(2);
                        fReader.SkipBytes(2);

                        //max AT2 range bracket
                        iTemp = fReader.ReadInt16();

                        if (iTemp > 0) oWeapon.SRV = iDamage;
                        if (iTemp > 1) oWeapon.MRV = iDamage;
                        if (iTemp > 2) oWeapon.LRV = iDamage;
                        if (iTemp > 3) oWeapon.ERV = iDamage;

                        //weight
                        dTemp = fReader.ReadDouble();

                        fReader.SkipBytes(2);

                        //ammo per ton
                        dTemp = fReader.ReadDouble();

                        //price
                        dTemp = fReader.ReadDouble();

                        fReader.SkipBytes(10);

                        //looks like some text-values
                        iLength = fReader.ReadInt16();
                        fReader.SkipBytes(iLength);

                        //Level
                        iTemp = fReader.ReadInt16();

                        //looks like the year
                        iLength = fReader.ReadInt16();
                        fReader.SkipBytes(iLength);

                        fReader.SkipBytes(2);

                        //BV
                        dTemp = fReader.ReadDouble();

                        fReader.SkipBytes(10);

                        slWeapons.Add(iWeaponID,oWeapon);
                    }
                    while (true);
                }
                catch (EndOfStreamException e)
                {
                    //add Artemis-Weapons
                    string sWeaponName;
                    int iAdditionalDamage = 0;
                    SortedList slArtemisWeapons = new SortedList();
                    tWeaponvalues oNewWeapon = new tWeaponvalues();
                    for (int i = 0; i < slWeapons.Count; i++)
                    {
                        oWeapon = ((tWeaponvalues) slWeapons.GetByIndex(i));
                        sWeaponName = oWeapon.Name.Substring(0,Math.Min(3,oWeapon.Name.Length));
                        if (oWeapon.Name.StartsWith("LRM") || oWeapon.Name.StartsWith("SRM"))
                        {
                            oNewWeapon = new tWeaponvalues();
                            oNewWeapon.Heat = oWeapon.Heat;
                            oNewWeapon.Name = oWeapon.Name + "+Artemis";
                            oNewWeapon.SRV = oWeapon.SRV;
                            oNewWeapon.MRV = oWeapon.MRV;
                            oNewWeapon.LRV = oWeapon.LRV;
                            oNewWeapon.ERV = oWeapon.ERV;
                            if (oWeapon.Name.StartsWith("LRM")) 
                            {
                                iAdditionalDamage = (int) Math.Round((decimal) oNewWeapon.SRV / 3);
                                //add only if there is a damage-value greater than 0
                                oNewWeapon.SRV += (int) Math.Min(oNewWeapon.SRV,iAdditionalDamage);
                                oNewWeapon.MRV += (int) Math.Min(oNewWeapon.MRV,iAdditionalDamage);
                                oNewWeapon.LRV += (int) Math.Min(oNewWeapon.LRV,iAdditionalDamage);
                                oNewWeapon.ERV += (int) Math.Min(oNewWeapon.ERV,iAdditionalDamage);
                            } 
                            else
                            {
                                switch (oNewWeapon.SRV)
                                {
                                    case 2 :
                                    case 8 :iAdditionalDamage = 2;
                                            break;
                                    case 6 :
                                    default:iAdditionalDamage = 0;
                                            break;
                                }
                                //add only if there is a damage-value greater than 0
                                oNewWeapon.SRV += (int) Math.Min(oNewWeapon.SRV,iAdditionalDamage);
                                oNewWeapon.MRV += (int) Math.Min(oNewWeapon.MRV,iAdditionalDamage);
                                oNewWeapon.LRV += (int) Math.Min(oNewWeapon.LRV,iAdditionalDamage);
                                oNewWeapon.ERV += (int) Math.Min(oNewWeapon.ERV,iAdditionalDamage);
                            }
                            slArtemisWeapons.Add(((int) slWeapons.GetKey(i)) + 0xffff,oNewWeapon);
                        }
                    }
                    //add artemis weapons to normal weapons
                    for (int i = 0; i < slArtemisWeapons.Count; i++) 
                    {
                        slWeapons.Add(slArtemisWeapons.GetKey(i),slArtemisWeapons.GetByIndex(i));
                    }
                    //EndOfStreamException is ok, but not other ones
                    return slWeapons;
                }
            }            
            return slWeapons;
        }

        public SortedList ReadRoster(string spFilename)
        {
            SortedList slFiles = new SortedList();
            string sFilename;
            int iLength, iCount;

            FileStream fDatafile;
            ExtendedReader fReader;
            try
            {
                fDatafile = new FileStream(spFilename, FileMode.Open);
                fReader = new ExtendedReader(fDatafile);
                fReader.BaseStream.Seek(0,System.IO.SeekOrigin.Begin);
            }
            catch
            {
                fReader =  null;
                fDatafile = null;
            }

            if ((fReader != null) && (fDatafile != null)) 
            {
                iLength = fReader.ReadInt16();
                string sFiletype = new String(fReader.ReadChars(iLength));
                if (!sFiletype.Equals("HeavyMetal Aero Roster File"))
                {
                    //Exception, "no HMA roster"
                    return slFiles;
                }

                iLength = fReader.ReadInt16();
                string sVersion = new String(fReader.ReadChars(iLength));
                /* disabled, makes probably far more problem than it prevents
                if (!sVersion.Equals("V1.02"))
                {
                    //Version not supported
                    return slFiles;
                }
                */

                //fighter-count
                iCount = fReader.ReadInt16(); 
                for (int i = 0; i < iCount; i++)
                {
                    fReader.SkipBytes(14);

                    iLength = fReader.ReadInt16();
                    sFilename = new String(fReader.ReadChars(iLength));
                    if (!sFilename.Equals(string.Empty))
                    {
                        if (slFiles.ContainsKey(sFilename))
                        {
                            slFiles[sFilename] = (int) slFiles[sFilename] + 1;
                        }
                        else
                        {
                            slFiles.Add(sFilename,1);
                        }
                    }
                
                    //filename without path
                    fReader.SkipBytes(fReader.ReadInt16());

                    fReader.SkipBytes(4);

                    //name of fighter
                    fReader.SkipBytes(fReader.ReadInt16());

                    //model of fighter
                    fReader.SkipBytes(fReader.ReadInt16());

                    //some info
                    fReader.SkipBytes(fReader.ReadInt16());

                    fReader.SkipBytes(28);

                    //some shorter name
                    fReader.SkipBytes(fReader.ReadInt16());

                    fReader.SkipBytes(2);

                    //AT2 Damage Values?
                    fReader.SkipBytes(fReader.ReadInt16());

                    //BF2 data?
                    fReader.SkipBytes(fReader.ReadInt16());
                }
            }

            return slFiles;
        }
	}
}
