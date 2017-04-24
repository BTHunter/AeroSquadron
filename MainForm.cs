using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Xml;
using System.Reflection;

namespace AeroSquadron
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MainMenu ASMainMenu;
        private System.Windows.Forms.MenuItem menuItem12;
        private System.Windows.Forms.PageSetupDialog MainPageSetupDialog;
        private System.Windows.Forms.PrintDialog MainPrintDialog;
        private System.Windows.Forms.ColumnHeader CountColumnHeader;
        private System.Windows.Forms.ColumnHeader NameColumnHeader;
        private System.Windows.Forms.ColumnHeader ArmorColumnHeader;
        private System.Windows.Forms.ListView SquadronListView;
        private System.Windows.Forms.ListView WeaponsListView;
        private System.Windows.Forms.ColumnHeader WTypeColumnHeader;
        private System.Windows.Forms.ColumnHeader WLocationColumnHeader;
        private System.Windows.Forms.ColumnHeader WHeatColumnHeader;
        private System.Windows.Forms.ColumnHeader WSRVColumnHeader;
        private System.Windows.Forms.ColumnHeader WMRVColumnHeader;
        private System.Windows.Forms.ColumnHeader WLRVColumnHeader;
        private System.Windows.Forms.ColumnHeader WERVColumnHeader;
        private System.Windows.Forms.ContextMenu SquadronContextMenu;
        private System.Windows.Forms.MenuItem MainFileMenuItem;
        private System.Windows.Forms.MenuItem MainLoadMenuItem;
        private System.Windows.Forms.MenuItem MainSaveMenuItem;
        private System.Windows.Forms.MenuItem MainPageSetupMenuItem;
        private System.Windows.Forms.MenuItem MainPrintMenuItem;
        private System.Windows.Forms.MenuItem MainExitMenuItem;
        private System.Windows.Forms.MenuItem MainSquadronMenuItem;
        private System.Windows.Forms.MenuItem MainNewMenuItem;
        private System.Windows.Forms.MenuItem MainAddMenuItem;
        private System.Windows.Forms.MenuItem MainInfoMenuItem;
        private System.Windows.Forms.MenuItem MainHelpTopicsMenuItem;
        private System.Windows.Forms.MenuItem MainAboutMenuItem;
        private System.Windows.Forms.MenuItem SquadronRemoveContextItem;
        private System.Windows.Forms.NumericUpDown FighterCountUpDown;
        private System.Windows.Forms.PrintPreviewDialog MainPrintPreviewDialog;
        private System.Windows.Forms.StatusBarPanel CountStatusBarPanel;
        private System.Windows.Forms.StatusBar MainStatusBar;
        private System.Windows.Forms.StatusBarPanel BVStatusBarPanel;
        private System.Windows.Forms.StatusBarPanel CostStatusBarPanel;
        private System.Drawing.Printing.PrintDocument MainPrintDocument;
        private System.Windows.Forms.Label CountLabel;
        private System.Windows.Forms.MenuItem MainOptionsMenuItem;
        private System.Windows.Forms.MenuItem LanguageEnglishMenuItem;
        private System.Windows.Forms.MenuItem LanguageGermanMenuItem;
        private System.Windows.Forms.MenuItem MainPreviewMenuItem;
        private System.Windows.Forms.OpenFileDialog MainAddFighterDialog;
        private System.Windows.Forms.OpenFileDialog MainOpenSquadronFileDialog;
        private System.Windows.Forms.MenuItem MainSaveAsMenuItem;
        private System.Windows.Forms.SaveFileDialog MainSaveSquadronFileDialog;
        private System.Windows.Forms.ComboBox AffiliationComboBox;
        private System.Windows.Forms.Label AffiliationLabel;
        private System.Windows.Forms.Label DesignationLabel;
        private System.Windows.Forms.TextBox DesignationTextBox;
        private System.Windows.Forms.Label GunneryLabel;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem MainExtendedPrintoutMenuItem;
        private System.Windows.Forms.Label PilotingLabel;
        private System.Windows.Forms.ComboBox GunneryComboBox;
        private System.Windows.Forms.ComboBox PilotingComboBox;
        private System.Windows.Forms.ColumnHeader HeatsinkColumnHeader;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private ResourceManager oResourceManager = new ResourceManager("AeroSquadron.AeroSquadron", System.Reflection.Assembly.GetExecutingAssembly());
		private ResourceManager oImageResourceManager = new ResourceManager("AeroSquadron.Images", System.Reflection.Assembly.GetExecutingAssembly());
        private CultureInfo oEnglishCulture = new CultureInfo("en-US");
        private CultureInfo oGermanCulture = new CultureInfo("de-DE");

        private bool bExtendedPrintout;
        private bool bFullDamageList;
        private bool bSquadronSaved;
        private string sCurrentSquadronFile;
        private string sAffiliation;
        private string sDesignation;
        private string sGunnery;
        private string sPiloting;

        private ArrayList alFighters;
        private SortedList slWeaponsIS, slWeaponsClan;
        private SortedList slWeaponsOriginalClan, slWeaponsOriginalIS;
        private SortedList slReplacements;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem ImportMenuItem;
        private System.Windows.Forms.OpenFileDialog MainImportRosterDialog;
        private System.Windows.Forms.MenuItem MainFullDamageListMenuItem;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem RulesAT2MenuItem;
		private System.Windows.Forms.MenuItem RulesSOMenuItem;

        private tSquadronData oSquadronData;

		enum RuleSet {SO, AT2};
		private RuleSet SelectedRules;

		public MainForm(string[] asParameters)
		{
			//initialise components
			InitializeComponent();
            //set strings accoring to culture
            LanguageEnglishMenuItem.Checked = false;
            LanguageGermanMenuItem.Checked = false;
            switch (System.Configuration.ConfigurationSettings.AppSettings["language"])
            {
                case "de":  Thread.CurrentThread.CurrentUICulture = oGermanCulture;
                            LanguageGermanMenuItem.Checked = true;
                            break;
                case "en":  Thread.CurrentThread.CurrentUICulture = oEnglishCulture;
                            LanguageEnglishMenuItem.Checked = true;
                            break;
                default:    Thread.CurrentThread.CurrentUICulture = oEnglishCulture;
                            LanguageEnglishMenuItem.Checked = true;
                            break;
            }
            //get replacement strings
            GetReplacements();
            //load weapon data
			if (!GetWeapondata()) 
			{
				Application.Exit();
				Environment.Exit(1);
			}
            //update UI
			TranslateWeaponLists();
            UpdateUI();
            //initialise components
            alFighters = new ArrayList();
            oSquadronData.Heat.Aft = 0;
            oSquadronData.Heat.LeftWing = 0;
            oSquadronData.Heat.RightWing = 0;
            oSquadronData.Heat.Nose = 0;
            oSquadronData.Heatsinks = 0;
            oSquadronData.Thrust = 0;
            oSquadronData.Cost = 0;
            oSquadronData.BV = 0;
            //initialise variables
            bSquadronSaved = false;
            try
            {
                bExtendedPrintout = bool.Parse(System.Configuration.ConfigurationSettings.AppSettings["extendedprintout"]);
                MainExtendedPrintoutMenuItem.Checked = bExtendedPrintout;
            }
            catch
            {
                bExtendedPrintout = false;
            }
            try
            {
                bFullDamageList = bool.Parse(System.Configuration.ConfigurationSettings.AppSettings["fulldamagelist"]);
                MainFullDamageListMenuItem.Checked = bFullDamageList;
                MainFullDamageListMenuItem.Enabled = bExtendedPrintout;
            }
            catch
            {
                bFullDamageList = false;
            }
            sCurrentSquadronFile = "";
            sAffiliation = "";
            sDesignation = "";
            sGunnery = "";
            sPiloting = "";

			SelectedRules = RuleSet.AT2;
			RulesSOMenuItem.Checked = false;
			RulesAT2MenuItem.Checked = true;

            //NumberFormatInfo oFormatInfo = CultureInfo.NumberFormat;
            foreach (string sValue in asParameters)
            {
                if (sValue.EndsWith(".asq"))
                {
                    LoadSquadron(sValue);
                    MainSaveMenuItem.Enabled = true;
                    break;
                }
            }
		}

        private bool GetWeapondata()
        {
            //load basic data
            HMAImport oImport = new HMAImport();
            oImport.ReadWeapons();
            slWeaponsOriginalIS = oImport.WeaponsIS;
            slWeaponsOriginalClan = oImport.WeaponsClan;
            if ((slWeaponsOriginalIS.Count == 0) || (slWeaponsOriginalClan.Count == 0))
            {
                MessageBox.Show("No weapon-files found.",oResourceManager.GetString("DialogError"),MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            } else return true;
        }

        private SortedList TranslateWeapons(SortedList slWeapons)
        {
            SortedList slResult = new SortedList();
            tWeaponvalues oWeapon;
            //do the translation
            for (int i = 0; i < slWeapons.Count; i++)
            {
                oWeapon = (tWeaponvalues) slWeapons.GetByIndex(i);
                for (int j = 0; j < slReplacements.Count; j++)
                {
                    oWeapon.Name = oWeapon.Name.Replace(slReplacements.GetKey(j).ToString(),slReplacements.GetByIndex(j).ToString());
                }
                slResult.Add(slWeapons.GetKey(i),oWeapon);
            }
            return slResult;
        }

        private void TranslateWeaponLists()
        {
            slWeaponsIS = TranslateWeapons(slWeaponsOriginalIS);
            slWeaponsClan = TranslateWeapons(slWeaponsOriginalClan);
        }

        private void GetReplacements()
        {
            //load replacements
            slReplacements = new SortedList();
            try
            {
                //load text for replacements
                XmlDocument xDocument = new XmlDocument();
                XmlTextReader xReader = new XmlTextReader(System.Environment.CurrentDirectory + "\\Replacements.xml");
                xReader.Read();      
                xDocument.Load(xReader);
                xReader.Close();

                //extract data
                XmlNodeList xNodes;
                XmlNode xNode;
                xNode = xDocument.DocumentElement.SelectSingleNode("language[@identifier='" + Thread.CurrentThread.CurrentUICulture.Name + "']");
                if (xNode != null)
                {
                    xNodes = xNode.SelectNodes("item");
                    for (int i = 0; i < xNodes.Count; i++)
                    {
                        slReplacements.Add(xNodes[i].Attributes["original"].Value,xNodes[i].Attributes["replacement"].Value);
                    }
                }
            }
            catch
            {
            }
        }

        private void UpdateUI()
        {
            this.Text = oResourceManager.GetString("MainFormTitle");
            MainFileMenuItem.Text = oResourceManager.GetString("MainMenuFile");
            MainLoadMenuItem.Text = oResourceManager.GetString("MainMenuLoad");
            MainSaveMenuItem.Text = oResourceManager.GetString("MainMenuSave");
            MainSaveAsMenuItem.Text = oResourceManager.GetString("MainMenuSaveAs");
            MainPageSetupMenuItem.Text = oResourceManager.GetString("MainMenuPageSetup");
            MainPreviewMenuItem.Text = oResourceManager.GetString("MainMenuPrintPreview");
            MainPrintMenuItem.Text = oResourceManager.GetString("MainMenuPrint");
            MainExitMenuItem.Text = oResourceManager.GetString("MainMenuExit");
            MainSquadronMenuItem.Text = oResourceManager.GetString("MainMenuSquadron");
            MainNewMenuItem.Text = oResourceManager.GetString("MainMenuNewSquadron");
            MainAddMenuItem.Text = oResourceManager.GetString("MainMenuAddFighter");
            MainInfoMenuItem.Text = oResourceManager.GetString("MainMenuInfo");
            MainHelpTopicsMenuItem.Text = oResourceManager.GetString("MainMenuHelp");
            MainAboutMenuItem.Text = oResourceManager.GetString("MainMenuAbout");
            CountColumnHeader.Text = oResourceManager.GetString("ListCount");
            NameColumnHeader.Text = oResourceManager.GetString("ListName");
            ArmorColumnHeader.Text = oResourceManager.GetString("ListArmor");
            SquadronRemoveContextItem.Text = oResourceManager.GetString("ContextMenuRemove");
            WTypeColumnHeader.Text = oResourceManager.GetString("ListCount");
            WLocationColumnHeader.Text = oResourceManager.GetString("ListLocation");
            WHeatColumnHeader.Text = oResourceManager.GetString("ListHeat");
            WSRVColumnHeader.Text = oResourceManager.GetString("ListSRV");
            WMRVColumnHeader.Text = oResourceManager.GetString("ListMRV");
            WLRVColumnHeader.Text = oResourceManager.GetString("ListLRV");
            WERVColumnHeader.Text = oResourceManager.GetString("ListERV");
            CountStatusBarPanel.Text = oResourceManager.GetString("StatusCount");
            BVStatusBarPanel.Text = oResourceManager.GetString("StatusBV");
            CostStatusBarPanel.Text = oResourceManager.GetString("StatusCost");
            MainOptionsMenuItem.Text = oResourceManager.GetString("MainMenuOptions");
            LanguageEnglishMenuItem.Text = oResourceManager.GetString("LanguageEnglish");
            LanguageGermanMenuItem.Text = oResourceManager.GetString("LanguageGerman");
            MainExtendedPrintoutMenuItem.Text = oResourceManager.GetString("MainMenuExtendedPrintout");
            MainFullDamageListMenuItem.Text = oResourceManager.GetString("MainFullDamageList");
            AffiliationLabel.Text = oResourceManager.GetString("DataAffiliation");
            DesignationLabel.Text = oResourceManager.GetString("DataDesignation");
            GunneryLabel.Text = oResourceManager.GetString("DataGunnery");
            PilotingLabel.Text = oResourceManager.GetString("DataPiloting");
            HeatsinkColumnHeader.Text = oResourceManager.GetString("ListHeatsinks");
            CountLabel.Text = oResourceManager.GetString("FighterCount");

            //clear affiliations
            AffiliationComboBox.Items.Clear();
            AffiliationComboBox.Items.Add("");
            try
            {
                //load text for affiliations
                XmlDocument xDocument = new XmlDocument();
                XmlTextReader xReader = new XmlTextReader(System.Environment.CurrentDirectory + "\\Affiliations.xml");
                xReader.Read();      
                xDocument.Load(xReader);
                xReader.Close();

                //extract data
                XmlNodeList xNodes;
                XmlNode xNode;
                xNode = xDocument.DocumentElement.SelectSingleNode("language[@identifier='" + Thread.CurrentThread.CurrentUICulture.Name + "']");
                if (xNode != null)
                {
                    xNodes = xNode.SelectNodes("affiliation");
                    for (int i = 0; i < xNodes.Count; i++)
                    {
                        AffiliationComboBox.Items.Add(xNodes[i].Attributes["value"].Value);
                    }
                }
            }
            catch
            {
            }
            //translate weapon-data
            TranslateWeaponLists();
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.ASMainMenu = new System.Windows.Forms.MainMenu();
			this.MainFileMenuItem = new System.Windows.Forms.MenuItem();
			this.MainLoadMenuItem = new System.Windows.Forms.MenuItem();
			this.MainSaveAsMenuItem = new System.Windows.Forms.MenuItem();
			this.MainSaveMenuItem = new System.Windows.Forms.MenuItem();
			this.ImportMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.MainPageSetupMenuItem = new System.Windows.Forms.MenuItem();
			this.MainPreviewMenuItem = new System.Windows.Forms.MenuItem();
			this.MainPrintMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.MainExitMenuItem = new System.Windows.Forms.MenuItem();
			this.MainSquadronMenuItem = new System.Windows.Forms.MenuItem();
			this.MainNewMenuItem = new System.Windows.Forms.MenuItem();
			this.MainAddMenuItem = new System.Windows.Forms.MenuItem();
			this.MainOptionsMenuItem = new System.Windows.Forms.MenuItem();
			this.LanguageEnglishMenuItem = new System.Windows.Forms.MenuItem();
			this.LanguageGermanMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.MainExtendedPrintoutMenuItem = new System.Windows.Forms.MenuItem();
			this.MainFullDamageListMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.RulesAT2MenuItem = new System.Windows.Forms.MenuItem();
			this.RulesSOMenuItem = new System.Windows.Forms.MenuItem();
			this.MainInfoMenuItem = new System.Windows.Forms.MenuItem();
			this.MainHelpTopicsMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.MainAboutMenuItem = new System.Windows.Forms.MenuItem();
			this.MainAddFighterDialog = new System.Windows.Forms.OpenFileDialog();
			this.MainSaveSquadronFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.MainPageSetupDialog = new System.Windows.Forms.PageSetupDialog();
			this.MainPrintDialog = new System.Windows.Forms.PrintDialog();
			this.SquadronListView = new System.Windows.Forms.ListView();
			this.CountColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.NameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.ArmorColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.HeatsinkColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.SquadronContextMenu = new System.Windows.Forms.ContextMenu();
			this.SquadronRemoveContextItem = new System.Windows.Forms.MenuItem();
			this.FighterCountUpDown = new System.Windows.Forms.NumericUpDown();
			this.WeaponsListView = new System.Windows.Forms.ListView();
			this.WTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.WLocationColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.WHeatColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.WSRVColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.WMRVColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.WLRVColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.WERVColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.MainPrintDocument = new System.Drawing.Printing.PrintDocument();
			this.MainPrintPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
			this.MainStatusBar = new System.Windows.Forms.StatusBar();
			this.CountStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.BVStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.CostStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			this.CountLabel = new System.Windows.Forms.Label();
			this.MainOpenSquadronFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.AffiliationComboBox = new System.Windows.Forms.ComboBox();
			this.AffiliationLabel = new System.Windows.Forms.Label();
			this.DesignationLabel = new System.Windows.Forms.Label();
			this.DesignationTextBox = new System.Windows.Forms.TextBox();
			this.GunneryLabel = new System.Windows.Forms.Label();
			this.PilotingLabel = new System.Windows.Forms.Label();
			this.GunneryComboBox = new System.Windows.Forms.ComboBox();
			this.PilotingComboBox = new System.Windows.Forms.ComboBox();
			this.MainImportRosterDialog = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.FighterCountUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.CountStatusBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.BVStatusBarPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.CostStatusBarPanel)).BeginInit();
			this.SuspendLayout();
			// 
			// ASMainMenu
			// 
			this.ASMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.MainFileMenuItem,
																					   this.MainSquadronMenuItem,
																					   this.MainOptionsMenuItem,
																					   this.MainInfoMenuItem});
			// 
			// MainFileMenuItem
			// 
			this.MainFileMenuItem.Index = 0;
			this.MainFileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.MainLoadMenuItem,
																							 this.MainSaveAsMenuItem,
																							 this.MainSaveMenuItem,
																							 this.ImportMenuItem,
																							 this.menuItem5,
																							 this.MainPageSetupMenuItem,
																							 this.MainPreviewMenuItem,
																							 this.MainPrintMenuItem,
																							 this.menuItem8,
																							 this.MainExitMenuItem});
			this.MainFileMenuItem.Text = "File";
			// 
			// MainLoadMenuItem
			// 
			this.MainLoadMenuItem.Index = 0;
			this.MainLoadMenuItem.Text = "Load";
			this.MainLoadMenuItem.Click += new System.EventHandler(this.MainLoadMenuItem_Click);
			// 
			// MainSaveAsMenuItem
			// 
			this.MainSaveAsMenuItem.Index = 1;
			this.MainSaveAsMenuItem.Text = "Save as...";
			this.MainSaveAsMenuItem.Click += new System.EventHandler(this.MainSaveAsMenuItem_Click);
			// 
			// MainSaveMenuItem
			// 
			this.MainSaveMenuItem.Enabled = false;
			this.MainSaveMenuItem.Index = 2;
			this.MainSaveMenuItem.Text = "Save";
			this.MainSaveMenuItem.Click += new System.EventHandler(this.MainSaveMenuItem_Click);
			// 
			// ImportMenuItem
			// 
			this.ImportMenuItem.Index = 3;
			this.ImportMenuItem.Text = "Import HMA Roster";
			this.ImportMenuItem.Click += new System.EventHandler(this.ImportMenuItem_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 4;
			this.menuItem5.Text = "-";
			// 
			// MainPageSetupMenuItem
			// 
			this.MainPageSetupMenuItem.Index = 5;
			this.MainPageSetupMenuItem.Text = "Setup";
			this.MainPageSetupMenuItem.Click += new System.EventHandler(this.MainPageSetupMenuItem_Click);
			// 
			// MainPreviewMenuItem
			// 
			this.MainPreviewMenuItem.Index = 6;
			this.MainPreviewMenuItem.Text = "Preview";
			this.MainPreviewMenuItem.Click += new System.EventHandler(this.MainPreviewMenuItem_Click);
			// 
			// MainPrintMenuItem
			// 
			this.MainPrintMenuItem.Index = 7;
			this.MainPrintMenuItem.Text = "Print";
			this.MainPrintMenuItem.Click += new System.EventHandler(this.MainPrintMenuItem_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 8;
			this.menuItem8.Text = "-";
			// 
			// MainExitMenuItem
			// 
			this.MainExitMenuItem.Index = 9;
			this.MainExitMenuItem.Text = "Exit";
			this.MainExitMenuItem.Click += new System.EventHandler(this.MainExitMenuItem_Click);
			// 
			// MainSquadronMenuItem
			// 
			this.MainSquadronMenuItem.Index = 1;
			this.MainSquadronMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								 this.MainNewMenuItem,
																								 this.MainAddMenuItem});
			this.MainSquadronMenuItem.Text = "Squadron";
			// 
			// MainNewMenuItem
			// 
			this.MainNewMenuItem.Index = 0;
			this.MainNewMenuItem.Text = "New Squadron";
			this.MainNewMenuItem.Click += new System.EventHandler(this.MainNewMenuItem_Click);
			// 
			// MainAddMenuItem
			// 
			this.MainAddMenuItem.Index = 1;
			this.MainAddMenuItem.Text = "Add Fighter";
			this.MainAddMenuItem.Click += new System.EventHandler(this.MainAddMenuItem_Click);
			// 
			// MainOptionsMenuItem
			// 
			this.MainOptionsMenuItem.Index = 2;
			this.MainOptionsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								this.LanguageEnglishMenuItem,
																								this.LanguageGermanMenuItem,
																								this.menuItem1,
																								this.MainExtendedPrintoutMenuItem,
																								this.MainFullDamageListMenuItem,
																								this.menuItem2,
																								this.RulesAT2MenuItem,
																								this.RulesSOMenuItem});
			this.MainOptionsMenuItem.Text = "Options";
			// 
			// LanguageEnglishMenuItem
			// 
			this.LanguageEnglishMenuItem.Checked = true;
			this.LanguageEnglishMenuItem.Index = 0;
			this.LanguageEnglishMenuItem.Text = "English";
			this.LanguageEnglishMenuItem.Click += new System.EventHandler(this.LanguageSelect);
			// 
			// LanguageGermanMenuItem
			// 
			this.LanguageGermanMenuItem.Index = 1;
			this.LanguageGermanMenuItem.Text = "German";
			this.LanguageGermanMenuItem.Click += new System.EventHandler(this.LanguageSelect);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// MainExtendedPrintoutMenuItem
			// 
			this.MainExtendedPrintoutMenuItem.Index = 3;
			this.MainExtendedPrintoutMenuItem.Text = "Extended Printout";
			this.MainExtendedPrintoutMenuItem.Click += new System.EventHandler(this.MainExtendedPrintoutMenuItem_Click);
			// 
			// MainFullDamageListMenuItem
			// 
			this.MainFullDamageListMenuItem.Index = 4;
			this.MainFullDamageListMenuItem.Text = "Full Damage List";
			this.MainFullDamageListMenuItem.Click += new System.EventHandler(this.FullDamageListMenuItem_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 5;
			this.menuItem2.Text = "-";
			// 
			// RulesAT2MenuItem
			// 
            this.RulesAT2MenuItem.Checked = true;
			this.RulesAT2MenuItem.Index = 6;
			this.RulesAT2MenuItem.Text = "AeroTech 2";
			this.RulesAT2MenuItem.Click += new System.EventHandler(this.SelectRules);
			// 
			// RulesSOMenuItem
			// 
            this.RulesSOMenuItem.Visible = false;
			this.RulesSOMenuItem.Index = 7;
			this.RulesSOMenuItem.Text = "Strategic Operations";
			this.RulesSOMenuItem.Click += new System.EventHandler(this.SelectRules);
			// 
			// MainInfoMenuItem
			// 
			this.MainInfoMenuItem.Index = 3;
			this.MainInfoMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.MainHelpTopicsMenuItem,
																							 this.menuItem12,
																							 this.MainAboutMenuItem});
			this.MainInfoMenuItem.Text = "Info";
			// 
			// MainHelpTopicsMenuItem
			// 
			this.MainHelpTopicsMenuItem.Enabled = false;
			this.MainHelpTopicsMenuItem.Index = 0;
			this.MainHelpTopicsMenuItem.Text = "Help";
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 1;
			this.menuItem12.Text = "-";
			// 
			// MainAboutMenuItem
			// 
			this.MainAboutMenuItem.Index = 2;
			this.MainAboutMenuItem.Text = "About";
			this.MainAboutMenuItem.Click += new System.EventHandler(this.MainAboutMenuItem_Click);
			// 
			// MainAddFighterDialog
			// 
			this.MainAddFighterDialog.Multiselect = true;
			// 
			// MainSaveSquadronFileDialog
			// 
			this.MainSaveSquadronFileDialog.FileName = "doc1";
			// 
			// SquadronListView
			// 
			this.SquadronListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							   this.CountColumnHeader,
																							   this.NameColumnHeader,
																							   this.ArmorColumnHeader,
																							   this.HeatsinkColumnHeader});
			this.SquadronListView.ContextMenu = this.SquadronContextMenu;
			this.SquadronListView.FullRowSelect = true;
			this.SquadronListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.SquadronListView.HideSelection = false;
			this.SquadronListView.Location = new System.Drawing.Point(8, 0);
			this.SquadronListView.Name = "SquadronListView";
			this.SquadronListView.Size = new System.Drawing.Size(320, 112);
			this.SquadronListView.TabIndex = 5;
			this.SquadronListView.View = System.Windows.Forms.View.Details;
			this.SquadronListView.SelectedIndexChanged += new System.EventHandler(this.SquadronListView_SelectedIndexChanged);
			// 
			// CountColumnHeader
			// 
			this.CountColumnHeader.Text = "Count";
			this.CountColumnHeader.Width = 50;
			// 
			// NameColumnHeader
			// 
			this.NameColumnHeader.Text = "Name";
			this.NameColumnHeader.Width = 150;
			// 
			// ArmorColumnHeader
			// 
			this.ArmorColumnHeader.Text = "Armor";
			this.ArmorColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.ArmorColumnHeader.Width = 55;
			// 
			// HeatsinkColumnHeader
			// 
			this.HeatsinkColumnHeader.Text = "Heatsinks";
			this.HeatsinkColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// SquadronContextMenu
			// 
			this.SquadronContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								this.SquadronRemoveContextItem});
			// 
			// SquadronRemoveContextItem
			// 
			this.SquadronRemoveContextItem.Index = 0;
			this.SquadronRemoveContextItem.Text = "remove";
			this.SquadronRemoveContextItem.Click += new System.EventHandler(this.SquadronRemoveContextItem_Click);
			// 
			// FighterCountUpDown
			// 
			this.FighterCountUpDown.Enabled = false;
			this.FighterCountUpDown.Location = new System.Drawing.Point(288, 112);
			this.FighterCountUpDown.Maximum = new System.Decimal(new int[] {
																			   12,
																			   0,
																			   0,
																			   0});
			this.FighterCountUpDown.Minimum = new System.Decimal(new int[] {
																			   1,
																			   0,
																			   0,
																			   0});
			this.FighterCountUpDown.Name = "FighterCountUpDown";
			this.FighterCountUpDown.Size = new System.Drawing.Size(40, 20);
			this.FighterCountUpDown.TabIndex = 6;
			this.FighterCountUpDown.Value = new System.Decimal(new int[] {
																			 1,
																			 0,
																			 0,
																			 0});
			this.FighterCountUpDown.ValueChanged += new System.EventHandler(this.FighterCountUpDown_ValueChanged);
			// 
			// WeaponsListView
			// 
			this.WeaponsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							  this.WTypeColumnHeader,
																							  this.WLocationColumnHeader,
																							  this.WHeatColumnHeader,
																							  this.WSRVColumnHeader,
																							  this.WMRVColumnHeader,
																							  this.WLRVColumnHeader,
																							  this.WERVColumnHeader});
			this.WeaponsListView.Location = new System.Drawing.Point(8, 136);
			this.WeaponsListView.Name = "WeaponsListView";
			this.WeaponsListView.Size = new System.Drawing.Size(320, 184);
			this.WeaponsListView.TabIndex = 7;
			this.WeaponsListView.View = System.Windows.Forms.View.Details;
			// 
			// WTypeColumnHeader
			// 
			this.WTypeColumnHeader.Text = "Count";
			this.WTypeColumnHeader.Width = 75;
			// 
			// WLocationColumnHeader
			// 
			this.WLocationColumnHeader.Text = "Location";
			this.WLocationColumnHeader.Width = 40;
			// 
			// WHeatColumnHeader
			// 
			this.WHeatColumnHeader.Text = "Heat";
			this.WHeatColumnHeader.Width = 40;
			// 
			// WSRVColumnHeader
			// 
			this.WSRVColumnHeader.Text = "SRV";
			this.WSRVColumnHeader.Width = 40;
			// 
			// WMRVColumnHeader
			// 
			this.WMRVColumnHeader.Text = "MRV";
			this.WMRVColumnHeader.Width = 40;
			// 
			// WLRVColumnHeader
			// 
			this.WLRVColumnHeader.Text = "LRV";
			this.WLRVColumnHeader.Width = 40;
			// 
			// WERVColumnHeader
			// 
			this.WERVColumnHeader.Text = "ERV";
			this.WERVColumnHeader.Width = 40;
			// 
			// MainPrintDocument
			// 
			this.MainPrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.MainPrintDocument_PrintPage);
			// 
			// MainPrintPreviewDialog
			// 
			this.MainPrintPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
			this.MainPrintPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
			this.MainPrintPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
			this.MainPrintPreviewDialog.Document = this.MainPrintDocument;
			this.MainPrintPreviewDialog.Enabled = true;
			this.MainPrintPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("MainPrintPreviewDialog.Icon")));
			this.MainPrintPreviewDialog.Location = new System.Drawing.Point(159, 21);
			this.MainPrintPreviewDialog.MinimumSize = new System.Drawing.Size(375, 250);
			this.MainPrintPreviewDialog.Name = "MainPrintPreviewDialog";
			this.MainPrintPreviewDialog.TransparencyKey = System.Drawing.Color.Empty;
			this.MainPrintPreviewDialog.UseAntiAlias = true;
			this.MainPrintPreviewDialog.Visible = false;
			// 
			// MainStatusBar
			// 
			this.MainStatusBar.Location = new System.Drawing.Point(0, 379);
			this.MainStatusBar.Name = "MainStatusBar";
			this.MainStatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																							 this.CountStatusBarPanel,
																							 this.BVStatusBarPanel,
																							 this.CostStatusBarPanel});
			this.MainStatusBar.ShowPanels = true;
			this.MainStatusBar.Size = new System.Drawing.Size(338, 22);
			this.MainStatusBar.TabIndex = 8;
			this.MainStatusBar.Text = "MainStatusBar";
			// 
			// CountStatusBarPanel
			// 
			this.CountStatusBarPanel.Text = "Count";
			this.CountStatusBarPanel.Width = 70;
			// 
			// BVStatusBarPanel
			// 
			this.BVStatusBarPanel.Text = "BV";
			this.BVStatusBarPanel.Width = 80;
			// 
			// CostStatusBarPanel
			// 
			this.CostStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.CostStatusBarPanel.Text = "Cost";
			this.CostStatusBarPanel.Width = 172;
			// 
			// CountLabel
			// 
			this.CountLabel.Location = new System.Drawing.Point(104, 120);
			this.CountLabel.Name = "CountLabel";
			this.CountLabel.Size = new System.Drawing.Size(176, 16);
			this.CountLabel.TabIndex = 9;
			this.CountLabel.Text = "aerospace fighter #";
			this.CountLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// AffiliationComboBox
			// 
			this.AffiliationComboBox.Items.AddRange(new object[] {
																	 "",
																	 "Federated Suns",
																	 "Lyran Alliance",
																	 "Draconis Combine",
																	 "Free Words League",
																	 "Capellan Confederation"});
			this.AffiliationComboBox.Location = new System.Drawing.Point(88, 328);
			this.AffiliationComboBox.Name = "AffiliationComboBox";
			this.AffiliationComboBox.Size = new System.Drawing.Size(120, 21);
			this.AffiliationComboBox.TabIndex = 10;
			// 
			// AffiliationLabel
			// 
			this.AffiliationLabel.AutoSize = true;
			this.AffiliationLabel.Location = new System.Drawing.Point(8, 336);
			this.AffiliationLabel.Name = "AffiliationLabel";
			this.AffiliationLabel.Size = new System.Drawing.Size(50, 16);
			this.AffiliationLabel.TabIndex = 11;
			this.AffiliationLabel.Text = "Affiliation";
			// 
			// DesignationLabel
			// 
			this.DesignationLabel.AutoSize = true;
			this.DesignationLabel.Location = new System.Drawing.Point(8, 360);
			this.DesignationLabel.Name = "DesignationLabel";
			this.DesignationLabel.Size = new System.Drawing.Size(64, 16);
			this.DesignationLabel.TabIndex = 12;
			this.DesignationLabel.Text = "Designation";
			// 
			// DesignationTextBox
			// 
			this.DesignationTextBox.Location = new System.Drawing.Point(88, 352);
			this.DesignationTextBox.Name = "DesignationTextBox";
			this.DesignationTextBox.Size = new System.Drawing.Size(120, 20);
			this.DesignationTextBox.TabIndex = 13;
			this.DesignationTextBox.Text = "";
			// 
			// GunneryLabel
			// 
			this.GunneryLabel.AutoSize = true;
			this.GunneryLabel.Location = new System.Drawing.Point(216, 336);
			this.GunneryLabel.Name = "GunneryLabel";
			this.GunneryLabel.Size = new System.Drawing.Size(48, 16);
			this.GunneryLabel.TabIndex = 16;
			this.GunneryLabel.Text = "Gunnery";
			// 
			// PilotingLabel
			// 
			this.PilotingLabel.AutoSize = true;
			this.PilotingLabel.Location = new System.Drawing.Point(216, 360);
			this.PilotingLabel.Name = "PilotingLabel";
			this.PilotingLabel.Size = new System.Drawing.Size(42, 16);
			this.PilotingLabel.TabIndex = 17;
			this.PilotingLabel.Text = "Piloting";
			// 
			// GunneryComboBox
			// 
			this.GunneryComboBox.Items.AddRange(new object[] {
																 "",
																 "0",
																 "1",
																 "2",
																 "3",
																 "4",
																 "5",
																 "6",
																 "7"});
			this.GunneryComboBox.Location = new System.Drawing.Point(288, 328);
			this.GunneryComboBox.MaxDropDownItems = 9;
			this.GunneryComboBox.Name = "GunneryComboBox";
			this.GunneryComboBox.Size = new System.Drawing.Size(40, 21);
			this.GunneryComboBox.TabIndex = 18;
			// 
			// PilotingComboBox
			// 
			this.PilotingComboBox.Items.AddRange(new object[] {
																  "",
																  "0",
																  "1",
																  "2",
																  "3",
																  "4",
																  "5",
																  "6",
																  "7"});
			this.PilotingComboBox.Location = new System.Drawing.Point(288, 352);
			this.PilotingComboBox.MaxDropDownItems = 9;
			this.PilotingComboBox.Name = "PilotingComboBox";
			this.PilotingComboBox.Size = new System.Drawing.Size(40, 21);
			this.PilotingComboBox.TabIndex = 19;
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(338, 401);
			this.Controls.Add(this.PilotingComboBox);
			this.Controls.Add(this.GunneryComboBox);
			this.Controls.Add(this.PilotingLabel);
			this.Controls.Add(this.GunneryLabel);
			this.Controls.Add(this.DesignationTextBox);
			this.Controls.Add(this.DesignationLabel);
			this.Controls.Add(this.AffiliationLabel);
			this.Controls.Add(this.AffiliationComboBox);
			this.Controls.Add(this.CountLabel);
			this.Controls.Add(this.MainStatusBar);
			this.Controls.Add(this.WeaponsListView);
			this.Controls.Add(this.FighterCountUpDown);
			this.Controls.Add(this.SquadronListView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Menu = this.ASMainMenu;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AeroSquadron";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
			((System.ComponentModel.ISupportInitialize)(this.FighterCountUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.CountStatusBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.BVStatusBarPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.CostStatusBarPanel)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
        [STAThread]
		static void Main(string[] asArgs) 
		{
			Application.Run(new MainForm(asArgs));
		}

        private void SquadronRemoveContextItem_Click(object sender, System.EventArgs e)
        {
            if (SquadronListView.SelectedItems.Count > 0)
            {
                bSquadronSaved = false;
                alFighters.RemoveAt(SquadronListView.Items.IndexOf(SquadronListView.SelectedItems[0]));
                SquadronListView.Items.Remove(SquadronListView.SelectedItems[0]);
                CalculateWeapons();
                CalculateStats();
            }
            
        }

        private void SquadronListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (SquadronListView.SelectedItems.Count > 0)
            {
                int iValue;
                iValue = ((FighterData) alFighters[SquadronListView.Items.IndexOf(SquadronListView.SelectedItems[0])]).Count;
                FighterCountUpDown.Value = new Decimal(iValue);
                FighterCountUpDown.Enabled = true;
            } 
            else
            {
                FighterCountUpDown.Enabled = false;
            }
        }

        private void FighterCountUpDown_ValueChanged(object sender, System.EventArgs e)
        {
            if (SquadronListView.SelectedItems.Count > 0)
            {
                bSquadronSaved = false;
                ((FighterData) alFighters[SquadronListView.Items.IndexOf(SquadronListView.SelectedItems[0])]).Count = (int) FighterCountUpDown.Value;
                SquadronListView.SelectedItems[0].SubItems[0].Text = FighterCountUpDown.Value.ToString();
                CalculateWeapons();
                CalculateStats();
            }
        }

        private void CalculateStats()
        {
            int iBV = 0;
            int iCost = 0;
            int iCount = 0;

            FighterData oFighter;

            for (int i = 0; i < alFighters.Count; i++)
            {
                oFighter = ((FighterData) alFighters[i]);

                iCount += oFighter.Count;
                iBV += oFighter.BV * oFighter.Count;
                iCost += oFighter.Cost * oFighter.Count;
            }

            CountStatusBarPanel.Text = oResourceManager.GetString("StatusCount") + ": " + iCount.ToString();
            BVStatusBarPanel.Text = oResourceManager.GetString("StatusBV") + ": " + iBV.ToString("0,0",Thread.CurrentThread.CurrentUICulture);
            CostStatusBarPanel.Text = oResourceManager.GetString("StatusCost") + ": " + iCost.ToString("0,0",Thread.CurrentThread.CurrentUICulture) + " C-Bills";
        }

        private void CalculateWeapons()
        {
            //tech-specific weapon-lists
            SortedList slISNoseWeapons = new SortedList();
            SortedList slISWingWeapons = new SortedList();
            SortedList slISAftWeapons = new SortedList();
            SortedList slClanNoseWeapons = new SortedList();
            SortedList slClanWingWeapons = new SortedList();
            SortedList slClanAftWeapons = new SortedList();
            //pointers to lists
            SortedList slISCurrentLocation;
            SortedList slClanCurrentLocation;
            SortedList slCurrentLocation;
            SortedList slWeapons;

            ListViewItem oItem;

            ArrayList alWeapons;
            tWeapondata oWeaponData;
            tWeaponvalues oWeaponValues;
            tDamageValues oDamages;

            FighterData oFighter;

            WeaponsListView.Items.Clear();
            ResetSquadronData();

            //default weapon-list
            slWeapons = slWeaponsIS;

            //add together weapons
            for (int i = 0; i < alFighters.Count; i++)
            {
                oFighter = (FighterData) alFighters[i];
                oSquadronData.Thrust = (int) Math.Min(oSquadronData.Thrust,oFighter.Thrust);
                oSquadronData.BV += oFighter.BV * oFighter.Count;
                oSquadronData.Cost += oFighter.Cost * oFighter.Count;
                oSquadronData.Heatsinks += oFighter.Heatsinks * oFighter.Count;

                alWeapons = oFighter.GetWeapons();
                if (oFighter.Tech == eTech.InnerSphere) 
                {
                    slWeapons = slWeaponsIS;
                }
                else
                {
                    slWeapons = slWeaponsClan;
                }
                for (int j = 0; j < alWeapons.Count; j++)
                {
                    oWeaponData = (tWeapondata) alWeapons[j];
                    //select dynamically correlative tech-specific lists
                    switch (oWeaponData.Location)
                    {
                        case eLocation.Nose: slISCurrentLocation = slISNoseWeapons;
                                             slClanCurrentLocation = slClanNoseWeapons;
                                             break;
                        case eLocation.Wing: slISCurrentLocation = slISWingWeapons;
                                             slClanCurrentLocation = slClanWingWeapons;
                                             break;
                        case eLocation.Aft : slISCurrentLocation = slISAftWeapons;
                                             slClanCurrentLocation = slClanAftWeapons;
                                             break;
                        default            : slISCurrentLocation = slISNoseWeapons;
                                             slClanCurrentLocation = slClanNoseWeapons;
                                             break;
                    }
                    //select dynamically correltive list
                    if (oFighter.Tech == eTech.InnerSphere) 
                    {
                        slCurrentLocation = slISCurrentLocation;
                    }
                    else
                    {
                        slCurrentLocation = slClanCurrentLocation;
                    }
                    //add weapons
                    if (slCurrentLocation.ContainsKey(oWeaponData.ID))
                    {
                        slCurrentLocation[oWeaponData.ID] = ((int) slCurrentLocation[oWeaponData.ID]) + (oFighter.Count * oWeaponData.Count);
                    } 
                    else
                    {
                        slCurrentLocation.Add(oWeaponData.ID,oWeaponData.Count * oFighter.Count);
                    }
                }
            }

            //show results
            string sLocation;
            int iCount, iHeat;
            for (int i = (int) eTech.InnerSphere; i <= (int) eTech.Clan; i++)
            {
                for (int j = (int) eLocation.Nose; j <= (int) eLocation.Aft; j++)
                {
                    //set current location
                    switch (j)
                    {
                        case (int) eLocation.Nose:  slISCurrentLocation = slISNoseWeapons;
                                                    slClanCurrentLocation = slClanNoseWeapons;
                                                    sLocation = oResourceManager.GetString("LocationShortNose");
                                                    break;
                        case (int) eLocation.Wing:  slISCurrentLocation = slISWingWeapons;
                                                    slClanCurrentLocation = slClanWingWeapons;
                                                    sLocation = oResourceManager.GetString("LocationShortWing");
                                                    break;
                        case (int) eLocation.Aft :  slISCurrentLocation = slISAftWeapons;
                                                    slClanCurrentLocation = slClanAftWeapons;
                                                    sLocation = oResourceManager.GetString("LocationShortAft");
                                                    break;
                        default                  :  slISCurrentLocation = slISNoseWeapons;
                                                    slClanCurrentLocation = slClanNoseWeapons;
                                                    sLocation = oResourceManager.GetString("LocationShortNose");
                                                    break;
                    }
                    //set tech-specific location-list
                    if (((eTech) i) == eTech.InnerSphere)
                    {
                        slCurrentLocation = slISCurrentLocation;
                        slWeapons = slWeaponsIS;
                    }
                    else
                    {
                        slCurrentLocation = slClanCurrentLocation;
                        slWeapons = slWeaponsClan;
                    }
                    //show weapons
                    iHeat = 0;
                    for (int k = 0; k < slCurrentLocation.Count; k++)
                    {
                        oWeaponValues = (tWeaponvalues) slWeapons[(int) slCurrentLocation.GetKey(k)];
                        iCount = (int) slCurrentLocation.GetByIndex(k);
                        if (oWeaponValues.SRV > 0)
                        {
                            oDamages.Short = (int) Math.Max(Math.Round((decimal) (iCount * oWeaponValues.SRV) / 10),1);
                        } else oDamages.Short = 0;
                        if (oWeaponValues.MRV > 0)
                        {
                            oDamages.Medium = (int) Math.Max(Math.Round((decimal) (iCount * oWeaponValues.MRV) / 10),1);
                        } else oDamages.Medium = 0;
                        if (oWeaponValues.LRV > 0)
                        {
                            oDamages.Long = (int) Math.Max(Math.Round((decimal) (iCount * oWeaponValues.LRV) / 10),1);
                        } else oDamages.Long = 0;
                        if (oWeaponValues.ERV > 0)
                        {
                            oDamages.Extreme = (int) Math.Max(Math.Round((decimal) (iCount * oWeaponValues.ERV) / 10),1);
                        } else oDamages.Extreme = 0;
                        
                        oItem = new ListViewItem(new String[] {oWeaponValues.Name, sLocation, (iCount * oWeaponValues.Heat).ToString(), oDamages.Short.ToString(), oDamages.Medium.ToString(), oDamages.Long.ToString(), oDamages.Extreme.ToString()});
                        WeaponsListView.Items.Add(oItem);
                        iHeat += iCount * oWeaponValues.Heat;
                    }
                    switch (j)
                    {
                        case (int) eLocation.Nose:  oSquadronData.Heat.Nose += iHeat;
                                                    break;
                        case (int) eLocation.Wing:  oSquadronData.Heat.RightWing += iHeat;
                                                    oSquadronData.Heat.LeftWing += iHeat;
                                                    break;
                        case (int) eLocation.Aft :  oSquadronData.Heat.Aft += iHeat;
                                                    break;
                        default                  :  oSquadronData.Heat.Nose += iHeat;
                                                    break;
                    }
                }
            }
        }

        private void MainExitMenuItem_Click(object sender, System.EventArgs e)
        {
            SaveSettings();
            Application.Exit();
			Environment.Exit(1);
        }

        private void ResetSquadronData()
        {
            oSquadronData.Heat.Aft = 0;
            oSquadronData.Heat.LeftWing = 0;
            oSquadronData.Heat.RightWing = 0;
            oSquadronData.Heat.Nose = 0;
            oSquadronData.Heatsinks = 0;
            oSquadronData.Thrust = 0xFF;
            oSquadronData.Cost = 0;
            oSquadronData.BV = 0;
        }

        private void ResetSquadron()
        {
            sCurrentSquadronFile = string.Empty;
            MainSaveMenuItem.Enabled = false;
            SquadronListView.Items.Clear();
            WeaponsListView.Items.Clear();
            alFighters.Clear();

            ResetSquadronData();
        }

        private void MainNewMenuItem_Click(object sender, System.EventArgs e)
        {
            if (SquadronListView.Items.Count == 0) 
            {
                ResetSquadron();
            }
            else
            {
                if (MessageBox.Show(oResourceManager.GetString("DialogClearSquadron"),oResourceManager.GetString("DialogQuestion"),MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes) 
                {
                    ResetSquadron();
                }
            }
            CalculateStats();
        }

        private bool AddFighter(string sFilename, int iCount)
        {
            HMAImport oImport = new HMAImport();
            oImport.WeaponsIS = slWeaponsIS;
            oImport.WeaponsClan = slWeaponsClan;
            FighterData oFighter = oImport.ReadFile(sFilename);
            if (oFighter != null)
            {
                bSquadronSaved = false;
                alFighters.Add(oFighter);
                oFighter.Count = iCount;
                SquadronListView.Items.Add(new ListViewItem(new String[] {oFighter.Count.ToString(), oFighter.Name, oFighter.Armor.ToString()}));
                CalculateWeapons();
                CalculateStats();
                return true;
            } 
            else 
            { 
                return false;
            }
        }

        private void MainAddMenuItem_Click(object sender, System.EventArgs e)
        {
            
            MainAddFighterDialog.Title = oResourceManager.GetString("DialogOpenTitle");
            MainAddFighterDialog.InitialDirectory = System.Configuration.ConfigurationSettings.AppSettings["addfighterpath"];
            MainAddFighterDialog.Filter = oResourceManager.GetString("DialogFighterFilterText") + " (*.hma)|*.hma";
            //MainAddFighterDialog.FilterIndex = 2;
            MainAddFighterDialog.RestoreDirectory = true;
            MainAddFighterDialog.Multiselect = true;
            if(MainAddFighterDialog.ShowDialog() == DialogResult.OK)
            {
                if (MainAddFighterDialog.FileNames.Length > 0)
                {
                    foreach (string sFilename in MainAddFighterDialog.FileNames)
                    {
                        if (!AddFighter(sFilename,1))
                        {
                            MessageBox.Show(oResourceManager.GetString("DialogInvalidFile"),oResourceManager.GetString("DialogError"),MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void MainPrintMenuItem_Click(object sender, System.EventArgs e)
        {
            if (CheckComposition())
            {
                MainPrintDialog.Document = MainPrintDocument;
                if (MainPrintDialog.ShowDialog() == DialogResult.OK)
                {
                    MainPrintDocument.Print();
                }
            }
        }

        private bool CheckComposition()
        {
            int iCount = 0;
            for (int i = 0; i < SquadronListView.Items.Count; i++)
            {
                iCount += ((FighterData) alFighters[i]).Count;
            }
            if (iCount <= 0)
            {
                MessageBox.Show(oResourceManager.GetString("DialogNoFighters"),oResourceManager.GetString("DialogError"),MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }
            if (iCount > 12)
            {
                MessageBox.Show(oResourceManager.GetString("DialogTooManyFighters"),oResourceManager.GetString("DialogError"),MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }

            if ((iCount != 2) && (iCount != 4) && (iCount != 5) && (iCount != 6) && (iCount != 10) && (iCount != 12))
            {
                if (MessageBox.Show(oResourceManager.GetString("DialogNonStandardNumberFighters"),oResourceManager.GetString("DialogQuestion"),MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No) 
                {
                    return false;
                }
            }

            if (WeaponsListView.Items.Count > 30) 
            {
                MessageBox.Show(oResourceManager.GetString("DialogTooManyWeapons"),oResourceManager.GetString("DialogError"),MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void DrawArmor(int iArmor, int iY, Pen oPen, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int iMaxX = (int) iArmor / 10;
            int iMaxY = iArmor % 10;

            //draws lines of 10
            if (iMaxX > 0)
            {
                for (int x = 0; x <= iMaxX; x++)
                {
                    e.Graphics.DrawLine(oPen, 30 + x * 6, iY, 30 + x * 6, iY + 60);   
                }
                for (int y = 0; y <= 10; y++)
                {
                    e.Graphics.DrawLine(oPen, 30, iY + y * 6, 30 + iMaxX * 6, iY + y * 6);
                }
            }
            //draw the rest
            if (iMaxY > 0)
            {
                e.Graphics.DrawLine(oPen,30 + iMaxX * 6,iY,30 + iMaxX * 6,iY + iMaxY * 6);
                e.Graphics.DrawLine(oPen,30 + (iMaxX + 1) * 6,iY,30 + (iMaxX + 1) * 6,iY + iMaxY * 6);
                for (int y = 0; y <= iMaxY; y++)
                {
                    e.Graphics.DrawLine(oPen, 30 + iMaxX * 6, iY + y * 6, 30 + (iMaxX + 1) * 6, iY + y * 6);
                }
            }
        }

		private void MainPrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) 
		{
			if (SelectedRules == RuleSet.AT2) 
			{
				PrintSquadronAT2(e);
			}
			else
			{
				PrintSquadronSO(e);
			}
		}

		private void PrintSquadronSO(System.Drawing.Printing.PrintPageEventArgs e)
		{
			int xPosition;
			int yPosition;

			SolidBrush oBrush = new SolidBrush(Color.Black);
			Pen oPen = new Pen(oBrush);

			Brush oGrayBrush = new SolidBrush(Color.Gray);
			Pen oGrayPen = new Pen(oGrayBrush);

			Font oSmallFont = new Font("Arial",6);
			Font oFont = new Font("Arial",8);
			Font oBoldFont = new Font("Arial",8,FontStyle.Bold);
			Font oCaptionFont = new Font("Arial",10,FontStyle.Bold);
			Font oBigCaptionFont = new Font("Arial",12,FontStyle.Bold);

			StringFormat oFormatCenter = new StringFormat();
			oFormatCenter.Alignment = StringAlignment.Center;
			StringFormat oFormatRight = new StringFormat();
			oFormatRight.Alignment = StringAlignment.Far;

			FighterData oFighter;

			e.Graphics.DrawRectangle(oPen, 25, 260, 720, 100);
			e.Graphics.DrawString("Ftr #1", oBoldFont, oBrush, 30, 265);

			xPosition = 30;
			yPosition = 275;
			e.Graphics.DrawString("Engine", oFont, oBrush, xPosition, yPosition);
			e.Graphics.DrawRectangle(oPen, xPosition + 70, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 90, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 110, yPosition, 12, 12);

			xPosition = 30;
			yPosition = 285;
			e.Graphics.DrawString("Avionics", oFont, oBrush, xPosition, yPosition);
			e.Graphics.DrawRectangle(oPen, xPosition + 70, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 90, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 110, yPosition, 12, 12);

			xPosition = 30;
			yPosition = 295;
			e.Graphics.DrawString("Sensors", oFont, oBrush, xPosition, yPosition);
			e.Graphics.DrawRectangle(oPen, xPosition + 70, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 90, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 110, yPosition, 12, 12);

			xPosition = 30;
			yPosition = 305;
			e.Graphics.DrawString("FCS", oFont, oBrush, xPosition, yPosition);
			e.Graphics.DrawRectangle(oPen, xPosition + 70, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 90, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 110, yPosition, 12, 12);

			xPosition = 30;
			yPosition = 315;
			e.Graphics.DrawString("Life Support", oFont, oBrush, xPosition, yPosition);
			e.Graphics.DrawRectangle(oPen, xPosition + 70, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 90, yPosition, 12, 12);
			e.Graphics.DrawRectangle(oPen, xPosition + 110, yPosition, 12, 12);

/*
			int iUnitCount = 0;
			int iOverallArmor = 0;
			int iOverallFuel = 0;
			int iTemp = 0;
			int i = 0, j = 0, k = 0, x = 0, y = 0;
			int iArmorOffset = 0;
			int iTopArmor, iTopCritical, iTopVelocity, iTopDamage, iTopMovement;
			eTech iTechBase = eTech.Undefined;

			//a hex field
			Point pEdge1 = new Point(0 , 0);
			Point pEdge2 = new Point(38, 0); //length: 38 (19 * 2)
			Point pEdge3 = new Point(57,33); //38 + 19
			Point pEdge4 = new Point(38,66); //width-offset: 19 (9,5 * 2)
			Point pEdge5 = new Point(0 ,66); //height-offset: 33 (16,5 * 2)
			Point pEdge6 = new Point(-19,33);
			Point[] apHex = {pEdge1, pEdge2, pEdge3, pEdge4, pEdge5, pEdge6};

			if (bExtendedPrintout)
			{
				iTopArmor = 270;
				iTopCritical = 405;
				iTopMovement = 560;
				iTopVelocity = 720;
				iTopDamage= 855;
			}
			else
			{
				iTopArmor = 350;
				iTopCritical = 550;
				iTopMovement = 740;
				iTopVelocity = 930;
				iTopDamage = 0;
			}

			Point[] aGrayRect = new Point[3];

			//fetch some data
			for (i = 0; i < SquadronListView.Items.Count; i++)
			{
				try
				{
					iTemp = Int32.Parse(SquadronListView.Items[i].SubItems[0].Text);
					iUnitCount += iTemp;
					iOverallArmor += Int32.Parse(SquadronListView.Items[i].SubItems[2].Text) * iTemp;
				}
				catch
				{
				}
			}

			//calculate capital-scale armor value
			iOverallArmor = (int) Math.Max(iOverallArmor / 10,1);

			//Logo
			//e.Graphics.DrawString("AeroTech 2", new Font("Arial",48,FontStyle.Bold), oBrush, 30, 70);
			e.Graphics.DrawString("AeroTech 2",new Font("Battletech New Italic",38,FontStyle.Bold),oBrush,new RectangleF(30,70,475,50),oFormatCenter);
			e.Graphics.DrawString(oResourceManager.GetString("PageSquadronRecordSheet"),new Font("Arial",18,FontStyle.Bold),oBrush,new RectangleF(30,130,475,50),oFormatCenter);

			//symbols for "active fighters"
			e.Graphics.DrawString(oResourceManager.GetString("PageActiveFighters"), oCaptionFont, oBrush,30,180);
			oPen.Width = 3;
			
			Image oImage = (Bitmap) oImageResourceManager.GetObject("Fighter.bmp");
			for (i = 0; i < iUnitCount; i++)
			{
				//e.Graphics.DrawRectangle(oPen,30 + i * 40, 200,35,50);
				e.Graphics.DrawImage(oImage,30+i*40,200,38,52);
				//e.Graphics.DrawString((i+1).ToString(),oFont,oBrush,45+i*40,230);
				e.Graphics.DrawString((i + 1).ToString(), oFont, oBrush, new RectangleF(30+i*40,230,38,20),oFormatCenter);
				//e.Graphics.DrawRectangle(oPen,new Rectangle(30+i*40,230,38,15));
			}

			//armor Texts
			oPen.Width = 2;
			e.Graphics.DrawString(oResourceManager.GetString("PageArmorDiagram"), oCaptionFont, oBrush,30,iTopArmor);
			e.Graphics.DrawString("(" + oResourceManager.GetString("PageCapitalScaleArmor") + ")", oFont, oBrush,200,iTopArmor);
			oPen.Width = 1;
			e.Graphics.DrawString(oResourceManager.GetString("PageArmorThreshold"), oFont, oBrush,40,iTopArmor + 20);
			e.Graphics.DrawString(oResourceManager.GetString("PageTotalArmor"), oFont, oBrush,40,iTopArmor + 35);
			e.Graphics.DrawString(((int) Math.Ceiling((double) (iOverallArmor / iUnitCount))).ToString(), oFont, oBrush,225,iTopArmor + 20);
			e.Graphics.DrawString(iOverallArmor.ToString(), oFont, oBrush,225,iTopArmor + 35);
			//armor diagram
			oPen.Width = 1;
			if (iOverallArmor > 800)
			{
				DrawArmor(800,iTopArmor + 55,oPen,e);
				iArmorOffset = 70;
				DrawArmor(iOverallArmor % 800,iTopArmor + 55 + iArmorOffset,oPen,e);
				if (!bExtendedPrintout)
				{
					iArmorOffset = 0;
				}
			} 
			else
			{
				DrawArmor(iOverallArmor,iTopArmor + 55,oPen,e);
				iArmorOffset = 0;
			}

			//criticals
			iTopCritical += iArmorOffset;
			oPen.Width = 5;
			e.Graphics.DrawRectangle(oPen,30,iTopCritical,480,170);

			oGrayPen.Width = 3;
			aGrayRect[0] = new Point(26,iTopCritical + 5);
			aGrayRect[1] = new Point(26,iTopCritical + 174);
			aGrayRect[2] = new Point(506,iTopCritical + 174);
			e.Graphics.DrawLines(oGrayPen,aGrayRect);

			e.Graphics.DrawString(oResourceManager.GetString("PageCriticalDamage"), oCaptionFont, oBrush,35,iTopCritical + 3);
			oPen.Width = 2;
			//avionics
			e.Graphics.DrawRectangle(oPen,150,iTopCritical + 20,20,20);
			e.Graphics.DrawRectangle(oPen,170,iTopCritical + 20,20,20);
			e.Graphics.DrawRectangle(oPen,190,iTopCritical + 20,20,20);
			e.Graphics.DrawString(oResourceManager.GetString("PageAvionics"), oFont, oBrush,50,iTopCritical + 20);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(150,iTopCritical + 25,20,20),oFormatCenter);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(170,iTopCritical + 25,20,20),oFormatCenter);
			e.Graphics.DrawString("+5", oFont, oBrush,new RectangleF(190,iTopCritical + 25,20,20),oFormatCenter);
			//cargo
			e.Graphics.DrawRectangle(oPen,150,iTopCritical + 50,20,20);
			e.Graphics.DrawString(oResourceManager.GetString("PageCargo"), oFont, oBrush,50,iTopCritical + 50);
			//engine
			e.Graphics.DrawRectangle(oPen,150,iTopCritical + 80,20,20);
			e.Graphics.DrawRectangle(oPen,170,iTopCritical + 80,20,20);
			e.Graphics.DrawRectangle(oPen,190,iTopCritical + 80,20,20);
			e.Graphics.DrawString(oResourceManager.GetString("PageEngine"), oFont, oBrush,50,iTopCritical + 80);
			e.Graphics.DrawString("2", oFont, oBrush,new RectangleF(150,iTopCritical + 85,20,20),oFormatCenter);
			e.Graphics.DrawString("4", oFont, oBrush,new RectangleF(170,iTopCritical + 85,20,20),oFormatCenter);
			e.Graphics.DrawString(oResourceManager.GetString("PageDestroyed"), oFont, oBrush,new RectangleF(190,iTopCritical + 85,20,20),oFormatCenter);
			//fcs
			e.Graphics.DrawRectangle(oPen,150,iTopCritical + 110,20,20);
			e.Graphics.DrawRectangle(oPen,170,iTopCritical + 110,20,20);
			e.Graphics.DrawRectangle(oPen,190,iTopCritical + 110,20,20);
			e.Graphics.DrawString(oResourceManager.GetString("PageFCS"), oFont, oBrush,50,iTopCritical + 110);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(150,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString("+4", oFont, oBrush,new RectangleF(170,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString(oResourceManager.GetString("PageDestroyed"), oFont, oBrush,new RectangleF(190,iTopCritical + 115,20,20),oFormatCenter);
			//gear
			e.Graphics.DrawRectangle(oPen,150,iTopCritical + 140,20,20);
			e.Graphics.DrawString(oResourceManager.GetString("PageGear"), oFont, oBrush,50,iTopCritical + 140);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(150,iTopCritical + 145,20,20),oFormatCenter);
			//life support
			e.Graphics.DrawRectangle(oPen,390,iTopCritical + 20,20,20);
			e.Graphics.DrawString(oResourceManager.GetString("PageLifeSupport"), oFont, oBrush,290,iTopCritical + 20);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(390,iTopCritical + 25,20,20),oFormatCenter);
			//sensors
			e.Graphics.DrawRectangle(oPen,390,iTopCritical + 50,20,20);
			e.Graphics.DrawRectangle(oPen,410,iTopCritical + 50,20,20);
			e.Graphics.DrawRectangle(oPen,430,iTopCritical + 50,20,20);
			e.Graphics.DrawString(oResourceManager.GetString("PageSensors"), oFont, oBrush,290,iTopCritical + 50);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(390,iTopCritical + 55,20,20),oFormatCenter);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(410,iTopCritical + 55,20,20),oFormatCenter);
			e.Graphics.DrawString("+5", oFont, oBrush,new RectangleF(430,iTopCritical + 55,20,20),oFormatCenter);
			//thruster caption
			e.Graphics.DrawString(oResourceManager.GetString("PageThrusters"), oFont, oBrush,290,iTopCritical + 80);
			//left thruster
			e.Graphics.DrawRectangle(oPen,390,iTopCritical + 110,20,20);
			e.Graphics.DrawRectangle(oPen,410,iTopCritical + 110,20,20);
			e.Graphics.DrawRectangle(oPen,430,iTopCritical + 110,20,20);
			e.Graphics.DrawRectangle(oPen,450,iTopCritical + 110,20,20);
			e.Graphics.DrawString(oResourceManager.GetString("PageLeft"), oFont, oBrush,290,iTopCritical + 110);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(390,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(410,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString("+3", oFont, oBrush,new RectangleF(430,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString(oResourceManager.GetString("PageDestroyed"), oFont, oBrush,new RectangleF(450,iTopCritical + 115,20,20),oFormatCenter);
			//right thruster
			e.Graphics.DrawRectangle(oPen,390,iTopCritical + 140,20,20);
			e.Graphics.DrawRectangle(oPen,410,iTopCritical + 140,20,20);
			e.Graphics.DrawRectangle(oPen,430,iTopCritical + 140,20,20);
			e.Graphics.DrawRectangle(oPen,450,iTopCritical + 140,20,20);
			e.Graphics.DrawString(oResourceManager.GetString("PageRight"), oFont, oBrush,290,iTopCritical + 140);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(390,iTopCritical + 145,20,20),oFormatCenter);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(410,iTopCritical + 145,20,20),oFormatCenter);
			e.Graphics.DrawString("+3", oFont, oBrush,new RectangleF(430,iTopCritical + 145,20,20),oFormatCenter);
			e.Graphics.DrawString(oResourceManager.GetString("PageDestroyed"), oFont, oBrush,new RectangleF(450,iTopCritical + 145,20,20),oFormatCenter);

			//advanced movement
			iTopMovement += iArmorOffset;
			//e.Graphics.DrawString(oResourceManager.GetString("PageAdvancedMovement"),oCaptionFont,oBrush,35,apHex[0].Y - 25);
			//Caption is always just below the crit-box, with the solution above it hangs in the air at standard printout
			e.Graphics.DrawString(oResourceManager.GetString("PageAdvancedMovement"),oCaptionFont,oBrush,35,iTopCritical + 180);
			//draw hex
			for (i = 0; i < apHex.GetLength(0); i++)
			{
				apHex[i].X += 250;
				apHex[i].Y += iTopMovement + 50;
			}
			e.Graphics.DrawPolygon(oPen,apHex);
			//draw hex for legend
			for (i = 0; i < apHex.GetLength(0); i++)
			{
				apHex[i].X += 150;
			}
			e.Graphics.DrawPolygon(oPen,apHex);
			e.Graphics.DrawString("A",oCaptionFont,oBrush,apHex[0].X + 13,apHex[0].Y - 25);
			e.Graphics.DrawString("B",oCaptionFont,oBrush,apHex[1].X + 17,apHex[1].Y - 5);
			e.Graphics.DrawString("C",oCaptionFont,oBrush,apHex[3].X + 17,apHex[3].Y - 15);
			e.Graphics.DrawString("D",oCaptionFont,oBrush,apHex[4].X + 13,apHex[4].Y + 10);
			e.Graphics.DrawString("E",oCaptionFont,oBrush,apHex[4].X - 30,apHex[4].Y - 15);
			e.Graphics.DrawString("F",oCaptionFont,oBrush,apHex[0].X - 30,apHex[0].Y - 5);
			//BV & cost
			e.Graphics.DrawString(oResourceManager.GetString("PageBV") + ":",oFont,oBrush,30,iTopMovement + 125);
			e.Graphics.DrawString(oResourceManager.GetString("PageCost") + ":",oFont,oBrush,30,iTopMovement + 140);
			e.Graphics.DrawString(oSquadronData.BV.ToString("0,0",Thread.CurrentThread.CurrentUICulture),oFont,oBrush,75,iTopMovement + 125);
			e.Graphics.DrawString(oSquadronData.Cost.ToString("0,0",Thread.CurrentThread.CurrentUICulture) + " " + oResourceManager.GetString("PageCurrency"),oFont,oBrush,75,iTopMovement + 140);

			//movement
			iTopVelocity += iArmorOffset;
			oPen.Width = 5;
			e.Graphics.DrawRectangle(oPen,30,iTopVelocity,480,120);

			oGrayPen.Width = 3;
			aGrayRect[0] = new Point(26,iTopVelocity + 5);
			aGrayRect[1] = new Point(26,iTopVelocity + 124);
			aGrayRect[2] = new Point(506,iTopVelocity + 124);
			e.Graphics.DrawLines(oGrayPen,aGrayRect);

			oPen.Width = 2;
			e.Graphics.DrawRectangle(oPen,100,iTopVelocity + 20,400,90);
			oPen.Width = 1;
			for (x = 1; x < 20; x++)
			{
				e.Graphics.DrawLine(oPen, 100 + x * 20, iTopVelocity + 20, 100 + x * 20, iTopVelocity + 110);
			}
			for (i = 0; i < 20; i++)
			{
				e.Graphics.DrawString((i + 1).ToString(), oFont, oBrush, new RectangleF(100 + i * 20,iTopVelocity + 22,20,15),oFormatCenter);
			}
			for (y = 1; y < 6; y++)
			{
				e.Graphics.DrawLine(oPen, 100, iTopVelocity + 20 + y * 15, 500, iTopVelocity + 20 + y * 15);
			}
			e.Graphics.DrawRectangle(oPen,70,iTopVelocity + 95,30,15);
			e.Graphics.DrawString(oResourceManager.GetString("PageVelocityRecord"),oBoldFont,oBrush,35,iTopVelocity + 5);
			e.Graphics.DrawString(oResourceManager.GetString("PageTurn") + " #",oFont,oBrush,40,iTopVelocity + 20 + 2);
			e.Graphics.DrawString(oResourceManager.GetString("PageThrust"),oFont,oBrush,40,iTopVelocity + 35 + 2);
			e.Graphics.DrawString(oResourceManager.GetString("PageVelocity"),oFont,oBrush,40,iTopVelocity + 50 + 2);
			e.Graphics.DrawString(oResourceManager.GetString("PageEffeciveVelocity"),oFont,oBrush,40,iTopVelocity + 65 + 2);
			e.Graphics.DrawString(oResourceManager.GetString("PageAltitude"),oFont,oBrush,40,iTopVelocity + 80 + 2);
			e.Graphics.DrawString(oResourceManager.GetString("PageFuel"),oFont,oBrush,40,iTopVelocity + 95 + 2);

			//Damage-reduction List
			if ((iArmorOffset == 0) && bExtendedPrintout)
			{
				ArrayList alDamageValues = new ArrayList();
				double dDamage;

				if (!bFullDamageList) 
				{
					for (i = 0; i < WeaponsListView.Items.Count; i++)
					{
						for (j = 3; j <=6; j++)
						{
							if (!alDamageValues.Contains(WeaponsListView.Items[i].SubItems[j].Text)) alDamageValues.Add(WeaponsListView.Items[i].SubItems[j].Text);
						}
					}
				}

				for (x = 1; x <= 30; x++)
				{
					if (bFullDamageList || alDamageValues.Contains(x.ToString()))
					{
						e.Graphics.DrawLine(oPen,45 + x * 15,iTopDamage,45 + x * 15,iTopDamage + (iUnitCount + 1) * 15);
						e.Graphics.DrawLine(oPen,45 + (x + 1) * 15,iTopDamage,45 + (x + 1) * 15,iTopDamage + (iUnitCount + 1) * 15);
						e.Graphics.DrawString(x.ToString(), oSmallFont, oBrush, new RectangleF(45 + x * 15,iTopDamage + 4,15,15),oFormatCenter);
						for (y = 1; y <= iUnitCount; y++)
						{
							dDamage = Math.Ceiling((double) (x * y) / (double) iUnitCount);
							e.Graphics.DrawString(((int) dDamage).ToString(), oSmallFont, oBrush, new RectangleF(45 + x * 15,iTopDamage + y * 15 + 3,15,15),oFormatCenter);
						}
					}
				}
				for (y = 1; y <= iUnitCount; y++)
				{
					e.Graphics.DrawLine(oPen,30,iTopDamage + y * 15,510,iTopDamage + y * 15);
					e.Graphics.DrawString("# " + y.ToString(), oSmallFont, oBrush, new RectangleF(35,iTopDamage + y * 15 + 3,25,15),oFormatCenter);
				}

				oPen.Width = 5;
				e.Graphics.DrawRectangle(oPen,30,iTopDamage,480,(iUnitCount + 1) * 15);

				oGrayPen.Width = 3;
				aGrayRect[0] = new Point(26,iTopDamage + 5);
				aGrayRect[1] = new Point(26,iTopDamage + (iUnitCount + 1) * 15 + 4);
				aGrayRect[2] = new Point(506,iTopDamage + (iUnitCount + 1) * 15 + 4);
				e.Graphics.DrawLines(oGrayPen,aGrayRect);

				oPen.Width = 2;
				e.Graphics.DrawLine(oPen,30,iTopDamage + 15,510,iTopDamage + 15);
				e.Graphics.DrawLine(oPen,60,iTopDamage,60,iTopDamage + (iUnitCount + 1) * 15);
				oPen.Width = 1;
				e.Graphics.DrawLine(oPen,30,iTopDamage,60,iTopDamage + 15);
			}

			//squardon details
			oPen.Width = 5;
			e.Graphics.DrawRectangle(oPen,520,80,240,970);

			oGrayPen.Width = 3;
			aGrayRect[0] = new Point(516,85);
			aGrayRect[1] = new Point(516,1054);
			aGrayRect[2] = new Point(755,1054);
			e.Graphics.DrawLines(oGrayPen,aGrayRect);

			oPen.Width = 2;
			e.Graphics.DrawString(oResourceManager.GetString("PageSquadronData"),oBigCaptionFont,oBrush,new RectangleF(520,85,240,30),oFormatCenter);
			oPen.Width = 1;
			e.Graphics.DrawString(oResourceManager.GetString("PageName") + ":",oFont,oBrush,525,105);
			e.Graphics.DrawString(oResourceManager.GetString("PageAffiliation") + ":",oFont,oBrush,525,120);
			e.Graphics.DrawString(oResourceManager.GetString("PageThrust"),oFont,oBrush,525,135);
			e.Graphics.DrawString(oResourceManager.GetString("PageTech"),oFont,oBrush,660,135);
			e.Graphics.DrawString(oResourceManager.GetString("PageSafeThrust") + ":",oFont,oBrush,540,150);
			e.Graphics.DrawString(oResourceManager.GetString("PageMaximumThrust") + ":",oFont,oBrush,540,165);
			e.Graphics.DrawString(DesignationTextBox.Text,oFont,oBrush,600,105);
			e.Graphics.DrawString(AffiliationComboBox.Text,oFont,oBrush,600,120);

			for (i = 0; i < iUnitCount; i++)
			{
				e.Graphics.DrawString(oResourceManager.GetString("PageFighter") + " #" + (i + 1).ToString() + ":",oFont,oBrush,525,180 + i * 15);
			}

			//print all fighters
			for (i = 0; i < alFighters.Count; i++)
			{
				oFighter = (FighterData) alFighters[i];
				for (j = 0; j < oFighter.Count; j++)
				{
					e.Graphics.DrawString(oFighter.Name,oFont,oBrush,600, 180 + k * 15);
					k++;
				}
				//calculate overall fuel
				iOverallFuel += oFighter.Fuel * oFighter.Count;
				//calculate tech base
				if (iTechBase != eTech.Mixed) 
				{
					if ((iTechBase != eTech.Undefined) && (iTechBase != oFighter.Tech))
					{
						iTechBase = eTech.Mixed;
					}
					else
					{
						iTechBase = oFighter.Tech;
					}
				}
			}
			//draw tech base
			e.Graphics.DrawString(oResourceManager.GetString("PageTechType" + ((int) iTechBase).ToString()),oFont,oBrush,680,150);

			//draw fuel
			e.Graphics.DrawString((iOverallFuel * 80).ToString(),oSmallFont,oBrush,new RectangleF(70,iTopVelocity + 98,30,15),oFormatCenter);

			//print thrust-values
			e.Graphics.DrawString(oSquadronData.Thrust.ToString(),oFont,oBrush,630,150);
			e.Graphics.DrawString(((int) Math.Floor((oSquadronData.Thrust * 1.5) + 0.6)).ToString(),oFont,oBrush,630,165);

			oPen.Width = 3;
			e.Graphics.DrawLine(oPen,new Point(525,365), new Point(755,365));

			//heat data
			oPen.Width = 1;
			e.Graphics.DrawString(oResourceManager.GetString("PageHeatCapacity") + ":",oBoldFont,oBrush,525,370);
			e.Graphics.DrawString(oResourceManager.GetString("PageHeatGeneration") + ":",oFont,oBrush,525,385);
			e.Graphics.DrawString(oResourceManager.GetString("PageNose") + ":",oFont,oBrush,525,400);
			e.Graphics.DrawString(oResourceManager.GetString("PageAft") + ":",oFont,oBrush,525,415);
			e.Graphics.DrawString(oResourceManager.GetString("PageLeftWing") + ":",oFont,oBrush,640,400);
			e.Graphics.DrawString(oResourceManager.GetString("PageRightWing") + ":",oFont,oBrush,640,415);

			e.Graphics.DrawString(oSquadronData.Heatsinks.ToString(),oFont,oBrush,660,370);
			e.Graphics.DrawString((oSquadronData.Heat.Nose + oSquadronData.Heat.RightWing + oSquadronData.Heat.LeftWing + oSquadronData.Heat.Aft).ToString(),oFont,oBrush,660,385);
			e.Graphics.DrawString(oSquadronData.Heat.Nose.ToString(),oFont,oBrush,575,400);
			e.Graphics.DrawString(oSquadronData.Heat.Aft.ToString(),oFont,oBrush,575,415);
			e.Graphics.DrawString(oSquadronData.Heat.LeftWing.ToString(),oFont,oBrush,725,400);
			e.Graphics.DrawString(oSquadronData.Heat.RightWing.ToString(),oFont,oBrush,725,415);

			oPen.Width = 3;
			e.Graphics.DrawLine(oPen,new Point(525,440), new Point(755,440));

			//weapon headers
			e.Graphics.DrawString(oResourceManager.GetString("PageWeaponHeader"),oBoldFont,oBrush,525,445);
			e.Graphics.DrawString(oResourceManager.GetString("PageWeaponType"),oSmallFont,oBrush,525,460);
			e.Graphics.DrawString(oResourceManager.GetString("PageWeaponLocation"),oSmallFont,oBrush,610,460);
			e.Graphics.DrawString(oResourceManager.GetString("PageWeaponHeat"),oSmallFont,oBrush,630,460);
			e.Graphics.DrawString(oResourceManager.GetString("PageWeaponSRV"),oSmallFont,oBrush,660,460);
			e.Graphics.DrawString(oResourceManager.GetString("PageWeaponMRV"),oSmallFont,oBrush,685,460);
			e.Graphics.DrawString(oResourceManager.GetString("PageWeaponLRV"),oSmallFont,oBrush,710,460);
			e.Graphics.DrawString(oResourceManager.GetString("PageWeaponERV"),oSmallFont,oBrush,735,460);
			//weapons
			oPen.Width = 1;
			for (i = 0; i < WeaponsListView.Items.Count; i++)
			{
				//520 + 5
				e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[0].Text,oFont,oBrush,525,475 + i * 15);
				//+ 80 + 5
				e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[1].Text,oFont,oBrush,610,475 + i * 15);
				//+ 15 + 5
				e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[2].Text,oFont,oBrush,630,475 + i * 15);
				//+ 25 + 5
				e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[3].Text,oFont,oBrush,660,475 + i * 15);
				//+ 20 + 5
				e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[4].Text,oFont,oBrush,685,475 + i * 15);
				//+ 20 + 5
				e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[5].Text,oFont,oBrush,710,475 + i * 15);
				//+ 20 + 5
				e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[6].Text,oFont,oBrush,735,475 + i * 15);
				//=760 - 5
			}

			oPen.Width = 3;
			e.Graphics.DrawLine(oPen,new Point(525,960), new Point(755,960));

			//pilot data
			oPen.Width = 2;
			e.Graphics.DrawString(oResourceManager.GetString("PagePilotData"),oCaptionFont,oBrush,new RectangleF(520,965,240,30),oFormatCenter);
			oPen.Width = 1;
			e.Graphics.DrawString(oResourceManager.GetString("PageGunnerySkill"),oFont,oBrush,525,985);
			e.Graphics.DrawString(oResourceManager.GetString("PagePilotingSkill"),oFont,oBrush,650,985);
			e.Graphics.DrawString(oResourceManager.GetString("PageHitsTaken") + ":",oFont,oBrush,525,1005);
			e.Graphics.DrawString(oResourceManager.GetString("PageModifier") + ":",oFont,oBrush,525,1025);
			e.Graphics.DrawString(GunneryComboBox.Text,oFont,oBrush,625,985);
			e.Graphics.DrawString(PilotingComboBox.Text,oFont,oBrush,725,985);

			e.Graphics.DrawRectangle(oPen,600,1000,120,40);
			e.Graphics.DrawLine(oPen,600,1020,720,1020);
			for (x = 1; x < 6; x++)
			{
				e.Graphics.DrawLine(oPen, 600 + x * 20, 1000, 600 + x * 20, 1040);
			}
			for (i = 0; i < 6; i++)
			{
				e.Graphics.DrawString((i + 1).ToString(), oFont, oBrush,new RectangleF(600 + i * 20,1005,20,20),oFormatCenter);
				if (i < 5)
				{
					e.Graphics.DrawString("+" + (i + 1).ToString(), oFont, oBrush,new RectangleF(600 + i * 20,1025,20,20),oFormatCenter);
				}
				else
				{
					e.Graphics.DrawString(oResourceManager.GetString("PagePilotIncapacitated"), oSmallFont, oBrush, new RectangleF(600 + i * 20,1025,22,20),oFormatCenter);
				}
			}
			e.Graphics.DrawString(oResourceManager.GetString("PageFooter"), oSmallFont, oBrush, 30, 1060);
			e.Graphics.DrawString(Assembly.GetExecutingAssembly().GetName().Version.ToString(),oSmallFont,oBrush,new RectangleF(520,1060,240,20),oFormatRight);
*/
			oBrush.Dispose();
			oPen.Dispose();
		}

		private void PrintSquadronAT2(System.Drawing.Printing.PrintPageEventArgs e)
        {
            int iUnitCount = 0;
            int iOverallArmor = 0;
            int iOverallFuel = 0;
            int iTemp = 0;
            int i = 0, j = 0, k = 0, x = 0, y = 0;
            int iArmorOffset = 0;
            int iTopArmor, iTopCritical, iTopVelocity, iTopDamage, iTopMovement;
            eTech iTechBase = eTech.Undefined;

            //a hex field
            Point pEdge1 = new Point(0 , 0);
            Point pEdge2 = new Point(38, 0); //length: 38 (19 * 2)
            Point pEdge3 = new Point(57,33); //38 + 19
            Point pEdge4 = new Point(38,66); //width-offset: 19 (9,5 * 2)
            Point pEdge5 = new Point(0 ,66); //height-offset: 33 (16,5 * 2)
            Point pEdge6 = new Point(-19,33);
            Point[] apHex = {pEdge1, pEdge2, pEdge3, pEdge4, pEdge5, pEdge6};

            if (bExtendedPrintout)
            {
                iTopArmor = 270;
                iTopCritical = 405;
                iTopMovement = 560;
                iTopVelocity = 720;
                iTopDamage= 855;
            }
            else
            {
                iTopArmor = 350;
                iTopCritical = 550;
                iTopMovement = 740;
                iTopVelocity = 930;
                iTopDamage = 0;
            }

            FighterData oFighter;

            SolidBrush oBrush = new SolidBrush(Color.Black);
            Pen oPen = new Pen(oBrush);

            Brush oGrayBrush = new SolidBrush(Color.Gray);
            Pen oGrayPen = new Pen(oGrayBrush);

            Point[] aGrayRect = new Point[3];

            Font oSmallFont = new Font("Arial",6);
            Font oFont = new Font("Arial",8);
            Font oBoldFont = new Font("Arial",8,FontStyle.Bold);
            Font oCaptionFont = new Font("Arial",10,FontStyle.Bold);
            Font oBigCaptionFont = new Font("Arial",12,FontStyle.Bold);

            StringFormat oFormatCenter = new StringFormat();
            oFormatCenter.Alignment = StringAlignment.Center;
            StringFormat oFormatRight = new StringFormat();
            oFormatRight.Alignment = StringAlignment.Far;

            //fetch some data
            for (i = 0; i < SquadronListView.Items.Count; i++)
            {
                try
                {
                    iTemp = Int32.Parse(SquadronListView.Items[i].SubItems[0].Text);
                    iUnitCount += iTemp;
                    iOverallArmor += Int32.Parse(SquadronListView.Items[i].SubItems[2].Text) * iTemp;
                }
                catch
                {
                }
            }

            //calculate capital-scale armor value
            iOverallArmor = (int) Math.Max(iOverallArmor / 10,1);

            //Logo
            //e.Graphics.DrawString("AeroTech 2", new Font("Arial",48,FontStyle.Bold), oBrush, 30, 70);
            e.Graphics.DrawString("AeroTech 2",new Font("Battletech New Italic",38,FontStyle.Bold),oBrush,new RectangleF(30,70,475,50),oFormatCenter);
            e.Graphics.DrawString(oResourceManager.GetString("PageSquadronRecordSheet"),new Font("Arial",18,FontStyle.Bold),oBrush,new RectangleF(30,130,475,50),oFormatCenter);

            //symbols for "active fighters"
            e.Graphics.DrawString(oResourceManager.GetString("PageActiveFighters"), oCaptionFont, oBrush,30,180);
            oPen.Width = 3;
			
			Image oImage = (Bitmap) oImageResourceManager.GetObject("Fighter.bmp");
            for (i = 0; i < iUnitCount; i++)
            {
                //e.Graphics.DrawRectangle(oPen,30 + i * 40, 200,35,50);
				e.Graphics.DrawImage(oImage,30+i*40,200,38,52);
				//e.Graphics.DrawString((i+1).ToString(),oFont,oBrush,45+i*40,230);
				e.Graphics.DrawString((i + 1).ToString(), oFont, oBrush, new RectangleF(30+i*40,230,38,20),oFormatCenter);
				//e.Graphics.DrawRectangle(oPen,new Rectangle(30+i*40,230,38,15));
            }

            //armor Texts
            oPen.Width = 2;
            e.Graphics.DrawString(oResourceManager.GetString("PageArmorDiagram"), oCaptionFont, oBrush,30,iTopArmor);
            e.Graphics.DrawString("(" + oResourceManager.GetString("PageCapitalScaleArmor") + ")", oFont, oBrush,200,iTopArmor);
            oPen.Width = 1;
            e.Graphics.DrawString(oResourceManager.GetString("PageArmorThreshold"), oFont, oBrush,40,iTopArmor + 20);
            e.Graphics.DrawString(oResourceManager.GetString("PageTotalArmor"), oFont, oBrush,40,iTopArmor + 35);
            e.Graphics.DrawString(((int) Math.Ceiling((double) (iOverallArmor / iUnitCount))).ToString(), oFont, oBrush,225,iTopArmor + 20);
            e.Graphics.DrawString(iOverallArmor.ToString(), oFont, oBrush,225,iTopArmor + 35);
            //armor diagram
            oPen.Width = 1;
            if (iOverallArmor > 800)
            {
                DrawArmor(800,iTopArmor + 55,oPen,e);
                iArmorOffset = 70;
                DrawArmor(iOverallArmor % 800,iTopArmor + 55 + iArmorOffset,oPen,e);
                if (!bExtendedPrintout)
                {
                    iArmorOffset = 0;
                }
            } 
            else
            {
                DrawArmor(iOverallArmor,iTopArmor + 55,oPen,e);
                iArmorOffset = 0;
            }

            //criticals
            iTopCritical += iArmorOffset;
            oPen.Width = 5;
            e.Graphics.DrawRectangle(oPen,30,iTopCritical,480,170);

            oGrayPen.Width = 3;
            aGrayRect[0] = new Point(26,iTopCritical + 5);
            aGrayRect[1] = new Point(26,iTopCritical + 174);
            aGrayRect[2] = new Point(506,iTopCritical + 174);
            e.Graphics.DrawLines(oGrayPen,aGrayRect);

            e.Graphics.DrawString(oResourceManager.GetString("PageCriticalDamage"), oCaptionFont, oBrush,35,iTopCritical + 3);
            oPen.Width = 2;
            //avionics
            e.Graphics.DrawRectangle(oPen,150,iTopCritical + 20,20,20);
            e.Graphics.DrawRectangle(oPen,170,iTopCritical + 20,20,20);
            e.Graphics.DrawRectangle(oPen,190,iTopCritical + 20,20,20);
            e.Graphics.DrawString(oResourceManager.GetString("PageAvionics"), oFont, oBrush,50,iTopCritical + 20);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(150,iTopCritical + 25,20,20),oFormatCenter);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(170,iTopCritical + 25,20,20),oFormatCenter);
			e.Graphics.DrawString("+5", oFont, oBrush,new RectangleF(190,iTopCritical + 25,20,20),oFormatCenter);
            //cargo
            e.Graphics.DrawRectangle(oPen,150,iTopCritical + 50,20,20);
            e.Graphics.DrawString(oResourceManager.GetString("PageCargo"), oFont, oBrush,50,iTopCritical + 50);
            //engine
            e.Graphics.DrawRectangle(oPen,150,iTopCritical + 80,20,20);
            e.Graphics.DrawRectangle(oPen,170,iTopCritical + 80,20,20);
            e.Graphics.DrawRectangle(oPen,190,iTopCritical + 80,20,20);
            e.Graphics.DrawString(oResourceManager.GetString("PageEngine"), oFont, oBrush,50,iTopCritical + 80);
			e.Graphics.DrawString("2", oFont, oBrush,new RectangleF(150,iTopCritical + 85,20,20),oFormatCenter);
			e.Graphics.DrawString("4", oFont, oBrush,new RectangleF(170,iTopCritical + 85,20,20),oFormatCenter);
			e.Graphics.DrawString(oResourceManager.GetString("PageDestroyed"), oFont, oBrush,new RectangleF(190,iTopCritical + 85,20,20),oFormatCenter);
            //fcs
            e.Graphics.DrawRectangle(oPen,150,iTopCritical + 110,20,20);
            e.Graphics.DrawRectangle(oPen,170,iTopCritical + 110,20,20);
            e.Graphics.DrawRectangle(oPen,190,iTopCritical + 110,20,20);
            e.Graphics.DrawString(oResourceManager.GetString("PageFCS"), oFont, oBrush,50,iTopCritical + 110);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(150,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString("+4", oFont, oBrush,new RectangleF(170,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString(oResourceManager.GetString("PageDestroyed"), oFont, oBrush,new RectangleF(190,iTopCritical + 115,20,20),oFormatCenter);
            //gear
            e.Graphics.DrawRectangle(oPen,150,iTopCritical + 140,20,20);
            e.Graphics.DrawString(oResourceManager.GetString("PageGear"), oFont, oBrush,50,iTopCritical + 140);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(150,iTopCritical + 145,20,20),oFormatCenter);
            //life support
            e.Graphics.DrawRectangle(oPen,390,iTopCritical + 20,20,20);
            e.Graphics.DrawString(oResourceManager.GetString("PageLifeSupport"), oFont, oBrush,290,iTopCritical + 20);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(390,iTopCritical + 25,20,20),oFormatCenter);
            //sensors
            e.Graphics.DrawRectangle(oPen,390,iTopCritical + 50,20,20);
            e.Graphics.DrawRectangle(oPen,410,iTopCritical + 50,20,20);
            e.Graphics.DrawRectangle(oPen,430,iTopCritical + 50,20,20);
            e.Graphics.DrawString(oResourceManager.GetString("PageSensors"), oFont, oBrush,290,iTopCritical + 50);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(390,iTopCritical + 55,20,20),oFormatCenter);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(410,iTopCritical + 55,20,20),oFormatCenter);
			e.Graphics.DrawString("+5", oFont, oBrush,new RectangleF(430,iTopCritical + 55,20,20),oFormatCenter);
            //thruster caption
            e.Graphics.DrawString(oResourceManager.GetString("PageThrusters"), oFont, oBrush,290,iTopCritical + 80);
            //left thruster
            e.Graphics.DrawRectangle(oPen,390,iTopCritical + 110,20,20);
            e.Graphics.DrawRectangle(oPen,410,iTopCritical + 110,20,20);
            e.Graphics.DrawRectangle(oPen,430,iTopCritical + 110,20,20);
            e.Graphics.DrawRectangle(oPen,450,iTopCritical + 110,20,20);
            e.Graphics.DrawString(oResourceManager.GetString("PageLeft"), oFont, oBrush,290,iTopCritical + 110);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(390,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(410,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString("+3", oFont, oBrush,new RectangleF(430,iTopCritical + 115,20,20),oFormatCenter);
			e.Graphics.DrawString(oResourceManager.GetString("PageDestroyed"), oFont, oBrush,new RectangleF(450,iTopCritical + 115,20,20),oFormatCenter);
            //right thruster
            e.Graphics.DrawRectangle(oPen,390,iTopCritical + 140,20,20);
            e.Graphics.DrawRectangle(oPen,410,iTopCritical + 140,20,20);
            e.Graphics.DrawRectangle(oPen,430,iTopCritical + 140,20,20);
            e.Graphics.DrawRectangle(oPen,450,iTopCritical + 140,20,20);
            e.Graphics.DrawString(oResourceManager.GetString("PageRight"), oFont, oBrush,290,iTopCritical + 140);
			e.Graphics.DrawString("+1", oFont, oBrush,new RectangleF(390,iTopCritical + 145,20,20),oFormatCenter);
			e.Graphics.DrawString("+2", oFont, oBrush,new RectangleF(410,iTopCritical + 145,20,20),oFormatCenter);
			e.Graphics.DrawString("+3", oFont, oBrush,new RectangleF(430,iTopCritical + 145,20,20),oFormatCenter);
			e.Graphics.DrawString(oResourceManager.GetString("PageDestroyed"), oFont, oBrush,new RectangleF(450,iTopCritical + 145,20,20),oFormatCenter);

            //advanced movement
            iTopMovement += iArmorOffset;
            //e.Graphics.DrawString(oResourceManager.GetString("PageAdvancedMovement"),oCaptionFont,oBrush,35,apHex[0].Y - 25);
            //Caption is always just below the crit-box, with the solution above it hangs in the air at standard printout
            e.Graphics.DrawString(oResourceManager.GetString("PageAdvancedMovement"),oCaptionFont,oBrush,35,iTopCritical + 180);
            //draw hex
            for (i = 0; i < apHex.GetLength(0); i++)
            {
                apHex[i].X += 250;
                apHex[i].Y += iTopMovement + 50;
            }
            e.Graphics.DrawPolygon(oPen,apHex);
            //draw hex for legend
            for (i = 0; i < apHex.GetLength(0); i++)
            {
                apHex[i].X += 150;
            }
            e.Graphics.DrawPolygon(oPen,apHex);
            e.Graphics.DrawString("A",oCaptionFont,oBrush,apHex[0].X + 13,apHex[0].Y - 25);
            e.Graphics.DrawString("B",oCaptionFont,oBrush,apHex[1].X + 17,apHex[1].Y - 5);
            e.Graphics.DrawString("C",oCaptionFont,oBrush,apHex[3].X + 17,apHex[3].Y - 15);
            e.Graphics.DrawString("D",oCaptionFont,oBrush,apHex[4].X + 13,apHex[4].Y + 10);
            e.Graphics.DrawString("E",oCaptionFont,oBrush,apHex[4].X - 30,apHex[4].Y - 15);
            e.Graphics.DrawString("F",oCaptionFont,oBrush,apHex[0].X - 30,apHex[0].Y - 5);
            //BV & cost
            e.Graphics.DrawString(oResourceManager.GetString("PageBV") + ":",oFont,oBrush,30,iTopMovement + 125);
            e.Graphics.DrawString(oResourceManager.GetString("PageCost") + ":",oFont,oBrush,30,iTopMovement + 140);
            e.Graphics.DrawString(oSquadronData.BV.ToString("0,0",Thread.CurrentThread.CurrentUICulture),oFont,oBrush,75,iTopMovement + 125);
            e.Graphics.DrawString(oSquadronData.Cost.ToString("0,0",Thread.CurrentThread.CurrentUICulture) + " " + oResourceManager.GetString("PageCurrency"),oFont,oBrush,75,iTopMovement + 140);

            //movement
            iTopVelocity += iArmorOffset;
            oPen.Width = 5;
            e.Graphics.DrawRectangle(oPen,30,iTopVelocity,480,120);

            oGrayPen.Width = 3;
            aGrayRect[0] = new Point(26,iTopVelocity + 5);
            aGrayRect[1] = new Point(26,iTopVelocity + 124);
            aGrayRect[2] = new Point(506,iTopVelocity + 124);
            e.Graphics.DrawLines(oGrayPen,aGrayRect);

            oPen.Width = 2;
            e.Graphics.DrawRectangle(oPen,100,iTopVelocity + 20,400,90);
            oPen.Width = 1;
            for (x = 1; x < 20; x++)
            {
                e.Graphics.DrawLine(oPen, 100 + x * 20, iTopVelocity + 20, 100 + x * 20, iTopVelocity + 110);
            }
            for (i = 0; i < 20; i++)
            {
                e.Graphics.DrawString((i + 1).ToString(), oFont, oBrush, new RectangleF(100 + i * 20,iTopVelocity + 22,20,15),oFormatCenter);
            }
            for (y = 1; y < 6; y++)
            {
                e.Graphics.DrawLine(oPen, 100, iTopVelocity + 20 + y * 15, 500, iTopVelocity + 20 + y * 15);
            }
            e.Graphics.DrawRectangle(oPen,70,iTopVelocity + 95,30,15);
            e.Graphics.DrawString(oResourceManager.GetString("PageVelocityRecord"),oBoldFont,oBrush,35,iTopVelocity + 5);
            e.Graphics.DrawString(oResourceManager.GetString("PageTurn") + " #",oFont,oBrush,40,iTopVelocity + 20 + 2);
            e.Graphics.DrawString(oResourceManager.GetString("PageThrust"),oFont,oBrush,40,iTopVelocity + 35 + 2);
            e.Graphics.DrawString(oResourceManager.GetString("PageVelocity"),oFont,oBrush,40,iTopVelocity + 50 + 2);
            e.Graphics.DrawString(oResourceManager.GetString("PageEffeciveVelocity"),oFont,oBrush,40,iTopVelocity + 65 + 2);
            e.Graphics.DrawString(oResourceManager.GetString("PageAltitude"),oFont,oBrush,40,iTopVelocity + 80 + 2);
            e.Graphics.DrawString(oResourceManager.GetString("PageFuel"),oFont,oBrush,40,iTopVelocity + 95 + 2);

            //Damage-reduction List
            if ((iArmorOffset == 0) && bExtendedPrintout)
            {
                ArrayList alDamageValues = new ArrayList();
                double dDamage;

                if (!bFullDamageList) 
                {
                    for (i = 0; i < WeaponsListView.Items.Count; i++)
                    {
                        for (j = 3; j <=6; j++)
                        {
                            if (!alDamageValues.Contains(WeaponsListView.Items[i].SubItems[j].Text)) alDamageValues.Add(WeaponsListView.Items[i].SubItems[j].Text);
                        }
                    }
                }

                for (x = 1; x <= 30; x++)
                {
                    if (bFullDamageList || alDamageValues.Contains(x.ToString()))
                    {
                        e.Graphics.DrawLine(oPen,45 + x * 15,iTopDamage,45 + x * 15,iTopDamage + (iUnitCount + 1) * 15);
                        e.Graphics.DrawLine(oPen,45 + (x + 1) * 15,iTopDamage,45 + (x + 1) * 15,iTopDamage + (iUnitCount + 1) * 15);
                        e.Graphics.DrawString(x.ToString(), oSmallFont, oBrush, new RectangleF(45 + x * 15,iTopDamage + 4,15,15),oFormatCenter);
                        for (y = 1; y <= iUnitCount; y++)
                        {
                            dDamage = Math.Ceiling((double) (x * y) / (double) iUnitCount);
                            e.Graphics.DrawString(((int) dDamage).ToString(), oSmallFont, oBrush, new RectangleF(45 + x * 15,iTopDamage + y * 15 + 3,15,15),oFormatCenter);
                        }
                    }
                }
                for (y = 1; y <= iUnitCount; y++)
                {
                    e.Graphics.DrawLine(oPen,30,iTopDamage + y * 15,510,iTopDamage + y * 15);
                    e.Graphics.DrawString("# " + y.ToString(), oSmallFont, oBrush, new RectangleF(35,iTopDamage + y * 15 + 3,25,15),oFormatCenter);
                }

                oPen.Width = 5;
                e.Graphics.DrawRectangle(oPen,30,iTopDamage,480,(iUnitCount + 1) * 15);

                oGrayPen.Width = 3;
                aGrayRect[0] = new Point(26,iTopDamage + 5);
                aGrayRect[1] = new Point(26,iTopDamage + (iUnitCount + 1) * 15 + 4);
                aGrayRect[2] = new Point(506,iTopDamage + (iUnitCount + 1) * 15 + 4);
                e.Graphics.DrawLines(oGrayPen,aGrayRect);

                oPen.Width = 2;
                e.Graphics.DrawLine(oPen,30,iTopDamage + 15,510,iTopDamage + 15);
                e.Graphics.DrawLine(oPen,60,iTopDamage,60,iTopDamage + (iUnitCount + 1) * 15);
                oPen.Width = 1;
                e.Graphics.DrawLine(oPen,30,iTopDamage,60,iTopDamage + 15);
            }

            //squardon details
            oPen.Width = 5;
            e.Graphics.DrawRectangle(oPen,520,80,240,970);

            oGrayPen.Width = 3;
            aGrayRect[0] = new Point(516,85);
            aGrayRect[1] = new Point(516,1054);
            aGrayRect[2] = new Point(755,1054);
            e.Graphics.DrawLines(oGrayPen,aGrayRect);

            oPen.Width = 2;
            e.Graphics.DrawString(oResourceManager.GetString("PageSquadronData"),oBigCaptionFont,oBrush,new RectangleF(520,85,240,30),oFormatCenter);
            oPen.Width = 1;
            e.Graphics.DrawString(oResourceManager.GetString("PageName") + ":",oFont,oBrush,525,105);
            e.Graphics.DrawString(oResourceManager.GetString("PageAffiliation") + ":",oFont,oBrush,525,120);
            e.Graphics.DrawString(oResourceManager.GetString("PageThrust"),oFont,oBrush,525,135);
            e.Graphics.DrawString(oResourceManager.GetString("PageTech"),oFont,oBrush,660,135);
            e.Graphics.DrawString(oResourceManager.GetString("PageSafeThrust") + ":",oFont,oBrush,540,150);
            e.Graphics.DrawString(oResourceManager.GetString("PageMaximumThrust") + ":",oFont,oBrush,540,165);
            e.Graphics.DrawString(DesignationTextBox.Text,oFont,oBrush,600,105);
            e.Graphics.DrawString(AffiliationComboBox.Text,oFont,oBrush,600,120);

            for (i = 0; i < iUnitCount; i++)
            {
                e.Graphics.DrawString(oResourceManager.GetString("PageFighter") + " #" + (i + 1).ToString() + ":",oFont,oBrush,525,180 + i * 15);
            }

            //print all fighters
            for (i = 0; i < alFighters.Count; i++)
            {
                oFighter = (FighterData) alFighters[i];
                for (j = 0; j < oFighter.Count; j++)
                {
                    e.Graphics.DrawString(oFighter.Name,oFont,oBrush,600, 180 + k * 15);
                    k++;
                }
                //calculate overall fuel
                iOverallFuel += oFighter.Fuel * oFighter.Count;
                //calculate tech base
                if (iTechBase != eTech.Mixed) 
                {
                    if ((iTechBase != eTech.Undefined) && (iTechBase != oFighter.Tech))
                    {
                        iTechBase = eTech.Mixed;
                    }
                    else
                    {
                        iTechBase = oFighter.Tech;
                    }
                }
            }
            //draw tech base
            e.Graphics.DrawString(oResourceManager.GetString("PageTechType" + ((int) iTechBase).ToString()),oFont,oBrush,680,150);

            //draw fuel
            e.Graphics.DrawString((iOverallFuel * 80).ToString(),oSmallFont,oBrush,new RectangleF(70,iTopVelocity + 98,30,15),oFormatCenter);

            //print thrust-values
            e.Graphics.DrawString(oSquadronData.Thrust.ToString(),oFont,oBrush,630,150);
            e.Graphics.DrawString(((int) Math.Floor((oSquadronData.Thrust * 1.5) + 0.6)).ToString(),oFont,oBrush,630,165);

            oPen.Width = 3;
            e.Graphics.DrawLine(oPen,new Point(525,365), new Point(755,365));

            //heat data
            oPen.Width = 1;
            e.Graphics.DrawString(oResourceManager.GetString("PageHeatCapacity") + ":",oBoldFont,oBrush,525,370);
            e.Graphics.DrawString(oResourceManager.GetString("PageHeatGeneration") + ":",oFont,oBrush,525,385);
            e.Graphics.DrawString(oResourceManager.GetString("PageNose") + ":",oFont,oBrush,525,400);
            e.Graphics.DrawString(oResourceManager.GetString("PageAft") + ":",oFont,oBrush,525,415);
            e.Graphics.DrawString(oResourceManager.GetString("PageLeftWing") + ":",oFont,oBrush,640,400);
            e.Graphics.DrawString(oResourceManager.GetString("PageRightWing") + ":",oFont,oBrush,640,415);

            e.Graphics.DrawString(oSquadronData.Heatsinks.ToString(),oFont,oBrush,660,370);
            e.Graphics.DrawString((oSquadronData.Heat.Nose + oSquadronData.Heat.RightWing + oSquadronData.Heat.LeftWing + oSquadronData.Heat.Aft).ToString(),oFont,oBrush,660,385);
            e.Graphics.DrawString(oSquadronData.Heat.Nose.ToString(),oFont,oBrush,575,400);
            e.Graphics.DrawString(oSquadronData.Heat.Aft.ToString(),oFont,oBrush,575,415);
            e.Graphics.DrawString(oSquadronData.Heat.LeftWing.ToString(),oFont,oBrush,725,400);
            e.Graphics.DrawString(oSquadronData.Heat.RightWing.ToString(),oFont,oBrush,725,415);

            oPen.Width = 3;
            e.Graphics.DrawLine(oPen,new Point(525,440), new Point(755,440));

            //weapon headers
            e.Graphics.DrawString(oResourceManager.GetString("PageWeaponHeader"),oBoldFont,oBrush,525,445);
            e.Graphics.DrawString(oResourceManager.GetString("PageWeaponType"),oSmallFont,oBrush,525,460);
            e.Graphics.DrawString(oResourceManager.GetString("PageWeaponLocation"),oSmallFont,oBrush,610,460);
            e.Graphics.DrawString(oResourceManager.GetString("PageWeaponHeat"),oSmallFont,oBrush,630,460);
            e.Graphics.DrawString(oResourceManager.GetString("PageWeaponSRV"),oSmallFont,oBrush,660,460);
            e.Graphics.DrawString(oResourceManager.GetString("PageWeaponMRV"),oSmallFont,oBrush,685,460);
            e.Graphics.DrawString(oResourceManager.GetString("PageWeaponLRV"),oSmallFont,oBrush,710,460);
            e.Graphics.DrawString(oResourceManager.GetString("PageWeaponERV"),oSmallFont,oBrush,735,460);
            //weapons
            oPen.Width = 1;
            for (i = 0; i < WeaponsListView.Items.Count; i++)
            {
                //520 + 5
                e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[0].Text,oFont,oBrush,525,475 + i * 15);
                //+ 80 + 5
                e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[1].Text,oFont,oBrush,610,475 + i * 15);
                //+ 15 + 5
                e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[2].Text,oFont,oBrush,630,475 + i * 15);
                //+ 25 + 5
                e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[3].Text,oFont,oBrush,660,475 + i * 15);
                //+ 20 + 5
                e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[4].Text,oFont,oBrush,685,475 + i * 15);
                //+ 20 + 5
                e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[5].Text,oFont,oBrush,710,475 + i * 15);
                //+ 20 + 5
                e.Graphics.DrawString(WeaponsListView.Items[i].SubItems[6].Text,oFont,oBrush,735,475 + i * 15);
                //=760 - 5
            }

            oPen.Width = 3;
            e.Graphics.DrawLine(oPen,new Point(525,960), new Point(755,960));

            //pilot data
            oPen.Width = 2;
            e.Graphics.DrawString(oResourceManager.GetString("PagePilotData"),oCaptionFont,oBrush,new RectangleF(520,965,240,30),oFormatCenter);
            oPen.Width = 1;
            e.Graphics.DrawString(oResourceManager.GetString("PageGunnerySkill"),oFont,oBrush,525,985);
            e.Graphics.DrawString(oResourceManager.GetString("PagePilotingSkill"),oFont,oBrush,650,985);
            e.Graphics.DrawString(oResourceManager.GetString("PageHitsTaken") + ":",oFont,oBrush,525,1005);
            e.Graphics.DrawString(oResourceManager.GetString("PageModifier") + ":",oFont,oBrush,525,1025);
            e.Graphics.DrawString(GunneryComboBox.Text,oFont,oBrush,625,985);
            e.Graphics.DrawString(PilotingComboBox.Text,oFont,oBrush,725,985);

            e.Graphics.DrawRectangle(oPen,600,1000,120,40);
            e.Graphics.DrawLine(oPen,600,1020,720,1020);
            for (x = 1; x < 6; x++)
            {
                e.Graphics.DrawLine(oPen, 600 + x * 20, 1000, 600 + x * 20, 1040);
            }
            for (i = 0; i < 6; i++)
            {
                e.Graphics.DrawString((i + 1).ToString(), oFont, oBrush,new RectangleF(600 + i * 20,1005,20,20),oFormatCenter);
                if (i < 5)
                {
                    e.Graphics.DrawString("+" + (i + 1).ToString(), oFont, oBrush,new RectangleF(600 + i * 20,1025,20,20),oFormatCenter);
                }
                else
                {
                    e.Graphics.DrawString(oResourceManager.GetString("PagePilotIncapacitated"), oSmallFont, oBrush, new RectangleF(600 + i * 20,1025,22,20),oFormatCenter);
                }
            }
            e.Graphics.DrawString(oResourceManager.GetString("PageFooter"), oSmallFont, oBrush, 30, 1060);
            e.Graphics.DrawString(Assembly.GetExecutingAssembly().GetName().Version.ToString(),oSmallFont,oBrush,new RectangleF(520,1060,240,20),oFormatRight);

            oBrush.Dispose();
            oPen.Dispose();
        }

        private void MainPageSetupMenuItem_Click(object sender, System.EventArgs e)
        {
            MainPrintDocument.DocumentName = "AeroSquadron";
            MainPageSetupDialog.Document = MainPrintDocument;
            MainPageSetupDialog.ShowDialog();
        }

        private void MainPreviewMenuItem_Click(object sender, System.EventArgs e)
        {
            if (CheckComposition())
            {
                MainPrintPreviewDialog.SetBounds(0,0,300,550);
                MainPrintPreviewDialog.StartPosition = FormStartPosition.CenterScreen;
                MainPrintPreviewDialog.PrintPreviewControl.AutoZoom = true;
                MainPrintPreviewDialog.ShowDialog();
            }
        }

        private void MainAboutMenuItem_Click(object sender, System.EventArgs e)
        {
            InfoForm oInfoForm = new InfoForm();
            oInfoForm.LocalisationResourceManager = oResourceManager;
            oInfoForm.Show();
        }

        private void LanguageSelect(object sender, System.EventArgs e)
        {
            MenuItem oSender = (MenuItem) sender;

            LanguageEnglishMenuItem.Checked = false;
            LanguageGermanMenuItem.Checked = false;
            oSender.Checked = true;

            if (oSender.Text.Equals(oResourceManager.GetString("LanguageGerman")))
            {
                Thread.CurrentThread.CurrentUICulture = oGermanCulture;
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = oEnglishCulture;
            }
            UpdateUI();
            //update names of weapons
            GetReplacements();
            TranslateWeaponLists();
            CalculateWeapons();
            CalculateStats();
        }

        private void MainForm_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void MainForm_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] sFilenames = (string[]) e.Data.GetData(DataFormats.FileDrop);
            foreach (string sFilename in sFilenames)
            {
                if (sFilename.ToLower().EndsWith(".hma"))
                {
                    AddFighter(sFilename,1);
                } 
                else if (sFilename.ToLower().EndsWith(".asq"))
                {
                    LoadSquadron(sFilename);
                    return;
                }
            }
        }

        private void MainLoadMenuItem_Click(object sender, System.EventArgs e)
        {
            MainOpenSquadronFileDialog.Title = oResourceManager.GetString("DialogOpenTitle");
            MainOpenSquadronFileDialog.InitialDirectory = System.Configuration.ConfigurationSettings.AppSettings["loadsquadronpath"];
            MainOpenSquadronFileDialog.Filter = oResourceManager.GetString("DialogSquadronFilterText") + " (*.asq)|*.asq";
            MainOpenSquadronFileDialog.FilterIndex = 2;
            MainOpenSquadronFileDialog.RestoreDirectory = true;
            if (MainOpenSquadronFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (MainOpenSquadronFileDialog.FileName != String.Empty) 
                {
                    if (LoadSquadron(MainOpenSquadronFileDialog.FileName))
                    {
                        sCurrentSquadronFile = MainOpenSquadronFileDialog.FileName;
                        MainSaveMenuItem.Enabled = true;
                    }
                    else
                    {
                        //show errormessage   
                    }
                }
            }
        }

        private bool LoadSquadron(string sFilename)
        {
            //open saved squadron file
            if ((alFighters.Count > 0) && (!bSquadronSaved))
            {
                if (MessageBox.Show("Squadron not saved, overwrite?",oResourceManager.GetString("DialogQuestion"),MessageBoxButtons.YesNo,MessageBoxIcon.Error) != DialogResult.Yes)
                {
                    return false;
                }
            }
            ResetSquadron();

            XmlDocument xDocument = new XmlDocument();
            XmlTextReader xReader = new XmlTextReader(sFilename);
            xReader.Read();      
            xDocument.Load(xReader);
            xReader.Close();

            XmlNodeList xFighterNodes, xWeaponNodes;
			XmlNode xNode;
            FighterData oFighter;

            xNode = xDocument.DocumentElement.SelectSingleNode("info");
            sAffiliation = xNode.Attributes["affiliation"].Value;
            sDesignation = xNode.Attributes["designation"].Value;

            xNode = xDocument.DocumentElement.SelectSingleNode("pilot");
            sGunnery = xNode.Attributes["gunnery"].Value;
            sPiloting = xNode.Attributes["piloting"].Value;

			xNode = xDocument.DocumentElement.SelectSingleNode("fighters");
			if (xNode != null)
			{
				xFighterNodes = xNode.SelectNodes("fighter");
				if ((xFighterNodes != null) && (xFighterNodes.Count > 0))
				{
					for (int i = 0; i < xFighterNodes.Count; i++)
					{
                        oFighter = new FighterData();
                        oFighter.Armor = Int32.Parse(xFighterNodes[i].Attributes["armor"].Value);
                        oFighter.BV = Int32.Parse(xFighterNodes[i].Attributes["bv"].Value);
                        oFighter.Cost = Int32.Parse(xFighterNodes[i].Attributes["cost"].Value);
                        oFighter.Count = Int32.Parse(xFighterNodes[i].Attributes["count"].Value);
                        oFighter.Fuel = Int32.Parse(xFighterNodes[i].Attributes["fuel"].Value);
                        //oFighter.Heat = 0;
                        oFighter.Heatsinks = Int32.Parse(xFighterNodes[i].Attributes["heatsinks"].Value);
                        oFighter.LRMArtemis = Boolean.Parse(xFighterNodes[i].Attributes["lrmartemis"].Value);
                        oFighter.SRMArtemis = Boolean.Parse(xFighterNodes[i].Attributes["srmartemis"].Value);
                        oFighter.Targeting = Boolean.Parse(xFighterNodes[i].Attributes["targeting"].Value);
                        oFighter.Name = xFighterNodes[i].Attributes["name"].Value;
                        oFighter.Tech = (eTech) Int32.Parse(xFighterNodes[i].Attributes["tech"].Value);
                        oFighter.Thrust = Int32.Parse(xFighterNodes[i].Attributes["thrust"].Value);
						xWeaponNodes = xFighterNodes[i].SelectNodes("weapons/weapon");
						if ((xWeaponNodes != null) && (xWeaponNodes.Count > 0))
                        {
                            for (int j = 0; j < xWeaponNodes.Count; j++)
                            {
                                //(eTech) Int32.Parse(xFighterNodes[i].Attributes["tech"].Value)
                                oFighter.AddWeapon(Int32.Parse(xWeaponNodes[j].Attributes["id"].Value),Int32.Parse(xWeaponNodes[j].Attributes["count"].Value),(eLocation) Int32.Parse(xWeaponNodes[j].Attributes["location"].Value),(eType) Int32.Parse(xWeaponNodes[j].Attributes["type"].Value));
                            }
                        }
                        alFighters.Add(oFighter);
                        SquadronListView.Items.Add(new ListViewItem(new String[] {oFighter.Count.ToString(), oFighter.Name, oFighter.Armor.ToString()}));
					}
				}
			}

            CalculateWeapons();
            CalculateStats();
            bSquadronSaved = true;
            return true;
        }

        private void MainSaveMenuItem_Click(object sender, System.EventArgs e)
        {
            if (!SaveSquadron(sCurrentSquadronFile))
            {
                //error message
                bSquadronSaved = true;
            }
        }

        private bool SaveSquadron(string sFilename)
        {
            FighterData oFighter;
            tWeapondata oWeapon;
            ArrayList alFighterWeapons;

            XmlTextWriter xWriter = new XmlTextWriter(sFilename,System.Text.Encoding.UTF8);
            xWriter.WriteStartDocument();

            xWriter.WriteStartElement("squadron");

            xWriter.WriteStartElement("info");
            xWriter.WriteAttributeString("affiliation",AffiliationComboBox.Text);
            xWriter.WriteAttributeString("designation",DesignationTextBox.Text);
            xWriter.WriteEndElement();

            xWriter.WriteStartElement("pilot");
            xWriter.WriteAttributeString("gunnery",GunneryComboBox.Text);
            xWriter.WriteAttributeString("piloting",PilotingComboBox.Text);
            xWriter.WriteEndElement();

            xWriter.WriteStartElement("fighters");
            for (int i = 0; i < alFighters.Count; i++)
            {
                oFighter = (FighterData) alFighters[i];
                xWriter.WriteStartElement("fighter");
                xWriter.WriteAttributeString("name",oFighter.Name);
                xWriter.WriteAttributeString("armor",oFighter.Armor.ToString());
                xWriter.WriteAttributeString("bv",oFighter.BV.ToString());
                xWriter.WriteAttributeString("thrust",oFighter.Thrust.ToString());
                xWriter.WriteAttributeString("cost",oFighter.Cost.ToString());
                xWriter.WriteAttributeString("count",oFighter.Count.ToString());
                xWriter.WriteAttributeString("fuel",oFighter.Fuel.ToString());
                xWriter.WriteAttributeString("heatsinks",oFighter.Heatsinks.ToString());
                xWriter.WriteAttributeString("lrmartemis",oFighter.LRMArtemis.ToString());
                xWriter.WriteAttributeString("srmartemis",oFighter.SRMArtemis.ToString());
                xWriter.WriteAttributeString("targeting",oFighter.Targeting.ToString());
                xWriter.WriteAttributeString("tech",((int) oFighter.Tech).ToString());

                alFighterWeapons = oFighter.GetWeapons();
                xWriter.WriteStartElement("weapons");
                for (int j = 0; j < alFighterWeapons.Count; j++)
                {
                    oWeapon = (tWeapondata) alFighterWeapons[j];
                    xWriter.WriteStartElement("weapon");
                    xWriter.WriteAttributeString("id",oWeapon.ID.ToString());
                    xWriter.WriteAttributeString("count",oWeapon.Count.ToString());
                    xWriter.WriteAttributeString("location",((int) oWeapon.Location).ToString());
                    xWriter.WriteAttributeString("tech",((int) oWeapon.Tech).ToString());
                    xWriter.WriteAttributeString("type",((int) oWeapon.Type).ToString());
                    xWriter.WriteEndElement();
                }
                xWriter.WriteEndElement();

                xWriter.WriteEndElement();
            }
            xWriter.WriteEndElement();

            xWriter.WriteEndElement();

            xWriter.WriteEndDocument();
            xWriter.Close();

            return true;
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            string sFilename = System.Environment.CurrentDirectory + @"\AeroSquadron.exe.config";
            string sValue;
            bool bChanged = false;

            try
            {
                XmlDocument xDocument = new XmlDocument();
                XmlTextReader xReader = new XmlTextReader(sFilename);
                xReader.Read();      
                xDocument.Load(xReader);
                xReader.Close();

                if (Thread.CurrentThread.CurrentUICulture.Equals(oGermanCulture)) sValue = "de";
                else sValue = "en";

                if (!xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='language']").Attributes["value"].Value.Equals(sValue))
                {
                    xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='language']").Attributes["value"].Value = sValue;
                    bChanged = true;
                }
                if ((MainSaveSquadronFileDialog.FileName != string.Empty) && (!xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='savesquadronpath']").Attributes["value"].Value.Equals(MainSaveSquadronFileDialog.FileName.Substring(0,MainSaveSquadronFileDialog.FileName.LastIndexOf("\\") + 1))))
                {
                    xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='savesquadronpath']").Attributes["value"].Value = MainSaveSquadronFileDialog.FileName.Substring(0,MainSaveSquadronFileDialog.FileName.LastIndexOf("\\") + 1);
                    bChanged = true;
                }
                if ((MainOpenSquadronFileDialog.FileName != string.Empty) && (!xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='loadsquadronpath']").Attributes["value"].Value.Equals(MainOpenSquadronFileDialog.FileName.Substring(0,MainOpenSquadronFileDialog.FileName.LastIndexOf("\\") + 1))))
                {
                    xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='loadsquadronpath']").Attributes["value"].Value = MainOpenSquadronFileDialog.FileName.Substring(0,MainOpenSquadronFileDialog.FileName.LastIndexOf("\\") + 1);
                    bChanged = true;
                }
                if ((MainAddFighterDialog.FileName != string.Empty) && (!xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='addfighterpath']").Attributes["value"].Value.Equals(MainAddFighterDialog.FileName.Substring(0,MainAddFighterDialog.FileName.LastIndexOf("\\") + 1))))
                {
                    xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='addfighterpath']").Attributes["value"].Value = MainAddFighterDialog.FileName.Substring(0,MainAddFighterDialog.FileName.LastIndexOf("\\") + 1);
                    bChanged = true;
                }
                if ((MainImportRosterDialog.FileName != string.Empty) && (!xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='importrosterpath']").Attributes["value"].Value.Equals(MainImportRosterDialog.FileName.Substring(0,MainImportRosterDialog.FileName.LastIndexOf("\\") + 1))))
                {
                    xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='importrosterpath']").Attributes["value"].Value = MainImportRosterDialog.FileName.Substring(0,MainImportRosterDialog.FileName.LastIndexOf("\\") + 1);
                    bChanged = true;
                }
                if (bExtendedPrintout != bool.Parse(System.Configuration.ConfigurationSettings.AppSettings["extendedprintout"]))
                {
                    xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='extendedprintout']").Attributes["value"].Value = bExtendedPrintout.ToString();
                    bChanged = true;
                }
                if (bFullDamageList != bool.Parse(System.Configuration.ConfigurationSettings.AppSettings["fulldamagelist"]))
                {
                    xDocument.DocumentElement.SelectSingleNode("appSettings/add[@key='fulldamagelist']").Attributes["value"].Value = bFullDamageList.ToString();
                    bChanged = true;
                }
            
                if (bChanged)
                {
                    xDocument.Save(sFilename);
                }
                xReader.Close();
            }
            catch
            {
            }
        }

        private void MainSaveAsMenuItem_Click(object sender, System.EventArgs e)
        {
            if (SquadronListView.Items.Count > 0)
            {
                MainSaveSquadronFileDialog.Title = oResourceManager.GetString("DialogSaveAsTitle");
                MainSaveSquadronFileDialog.InitialDirectory = System.Configuration.ConfigurationSettings.AppSettings["savesquadronpath"];
                MainSaveSquadronFileDialog.Filter = oResourceManager.GetString("DialogSquadronFilterText") + " (*.asq)|*.asq";
                MainSaveSquadronFileDialog.FilterIndex = 2;
                MainSaveSquadronFileDialog.FileName = string.Empty;
                MainSaveSquadronFileDialog.RestoreDirectory = true;
                if (MainSaveSquadronFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (MainSaveSquadronFileDialog.FileName != String.Empty) 
                    {
                        if (!SaveSquadron(MainSaveSquadronFileDialog.FileName))
                        {
                            sCurrentSquadronFile = MainOpenSquadronFileDialog.FileName;
                            MainSaveMenuItem.Enabled = true;
                        }
                        else
                        {
                            //show errormessage
                        }
                    }
                }
            }
        }

        private void MainExtendedPrintoutMenuItem_Click(object sender, System.EventArgs e)
        {
            MainExtendedPrintoutMenuItem.Checked = !MainExtendedPrintoutMenuItem.Checked;
            bExtendedPrintout = MainExtendedPrintoutMenuItem.Checked;
            MainFullDamageListMenuItem.Enabled = MainExtendedPrintoutMenuItem.Checked;
        }

        private void FullDamageListMenuItem_Click(object sender, System.EventArgs e)
        {
            MainFullDamageListMenuItem.Checked = !MainFullDamageListMenuItem.Checked;
            bFullDamageList = MainFullDamageListMenuItem.Checked;
        }

        private void ImportMenuItem_Click(object sender, System.EventArgs e)
        {
            MainImportRosterDialog.Title = oResourceManager.GetString("DialogImportTitle");
            MainImportRosterDialog.InitialDirectory = System.Configuration.ConfigurationSettings.AppSettings["importrosterpath"];
            MainImportRosterDialog.Filter = oResourceManager.GetString("DialogRosterFilterText") + " (*.roa)|*.roa";
            MainImportRosterDialog.FilterIndex = 2;
            MainImportRosterDialog.RestoreDirectory = true;
            if (MainImportRosterDialog.ShowDialog() == DialogResult.OK)
            {
                if (!MainImportRosterDialog.FileName.Equals(String.Empty)) 
                {

                    if ((SquadronListView.Items.Count == 0) || (MessageBox.Show(oResourceManager.GetString("DialogClearSquadron"),oResourceManager.GetString("DialogQuestion"),MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes))
                    {
                        ResetSquadron();
                        ResetSquadron();
                        HMAImport oImport = new HMAImport();
                        SortedList slFiles = oImport.ReadRoster(MainImportRosterDialog.FileName);
                        for (int i = 0; i < slFiles.Count; i++)
                        {
                            if (!AddFighter((string) slFiles.GetKey(i),(int) slFiles.GetByIndex(i)))
                            {
                                MessageBox.Show(oResourceManager.GetString("DialogInvalidFile"),oResourceManager.GetString("DialogError"),MessageBoxButtons.OK,MessageBoxIcon.Error);
                            }
                        }
                    }
                    CalculateStats();
                }
            }
        }

		private void SelectRules(object sender, System.EventArgs e)
		{
			MenuItem oSender = (MenuItem) sender;

			RulesSOMenuItem.Checked = false;
			RulesAT2MenuItem.Checked = false;
			oSender.Checked = true;

			if (oSender.Text.Equals("AeroTech 2"))
			{
				SelectedRules = RuleSet.AT2;
			}
			else
			{
				SelectedRules = RuleSet.SO;
			}
		}

	}
}
