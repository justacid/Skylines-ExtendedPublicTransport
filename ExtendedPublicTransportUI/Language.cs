using ICities;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.Globalization;
using System;
//using System.Collections;
using System.Collections.Generic;

namespace EPTUI
{
	public class EPTUILang : MonoBehaviour {

		public static string GameLanguage;
		public static int[] ColumnWidths = {43,160,207,294,380};
		public static Dictionary<string, string> nl = new Dictionary<string, string>();
		public static Dictionary<string, string> de = new Dictionary<string, string>();
		public static Dictionary<string, string> it = new Dictionary<string, string>();
		public static Dictionary<string, string> en = new Dictionary<string, string> ();

		public static string text (string index) {

			ColossalFramework.SavedString lang_setting = new ColossalFramework.SavedString("localeID", "gameSettings");
			if (GameLanguage == null || GameLanguage != lang_setting.value) {

				try {
					GameLanguage = lang_setting.value;
					if (GameLanguage == "nl") {
						ColumnWidths = new int[] {43,160,207,276,380};
						nl = new Dictionary<string, string> ();

						nl ["SELECT_ALL"] = "Selecteer alles";
						nl ["SELECT_NONE"] = "Selecteer niets";
						nl ["NAME"] = "Naam";
						nl ["STOPS"] = "Haltes";
						nl ["PASSENGERS"] = "Passagiers";
						nl ["TRIPS_SAVED"] = "Ritten bespaard";
						nl ["VEHICLES"] = "Voertuigen";
						nl ["DELETE_LINE"] = "Hef lijn op";
						nl ["NONE"] = "(Geen)";
						nl ["BUS_LINES"] = "Buslijnen ({0})";
						nl ["METRO_LINES"] = "Metrolijnen ({0})";
						nl ["TRAIN_LINES"] = "Treinlijnen ({0})";
						return nl [index];
					} else if (GameLanguage == "de") {
						ColumnWidths = new int[] {43,150,225,300,380};
						de = new Dictionary<string, string> ();

						de ["SELECT_ALL"] = "Alle wählen";
						de ["SELECT_NONE"] = "Nichts wählen ";
						de ["NAME"] = "Name";
						de ["STOPS"] = "Haltestellen";
						de ["PASSENGERS"] = "Passagiere";
						de ["TRIPS_SAVED"] = "Ritte erspart";
						de ["VEHICLES"] = "Fahrzeuge";
						de ["DELETE_LINE"] = "Linie aufheben";
						de ["NONE"] = "(Keine)";
						de ["BUS_LINES"] = "Buslinien ({0})";
						de ["METRO_LINES"] = "Metrolinien ({0})";
						de ["TRAIN_LINES"] = "Bahnlinien ({0})";
						return de [index];
					} else if (GameLanguage == "it") {
						//ColumnWidths = new int[] {43,160,207,276,380};
						it = new Dictionary<string, string> ();

						it ["SELECT_ALL"] = "Seleziona tutto";
						it ["SELECT_NONE"] = "Seleziona niente ";
						it ["NAME"] = "Nome";
						it ["STOPS"] = "Fermate";
						it ["PASSENGERS"] = "Viaggiatori";
						it ["TRIPS_SAVED"] = "Corse risparmiate";
						it ["VEHICLES"] = "Veicolo";
						it ["DELETE_LINE"] = "Sollevamento linea";
						it ["NONE"] = "(Niente)";
						it ["BUS_LINES"] = "Linee autobus ({0})";
						it ["METRO_LINES"] = "Linee metro ({0})";
						it ["TRAIN_LINES"] = "Linee treni ({0})";
						return it [index];
					} else {
						en = new Dictionary<string, string> ();

						en ["SELECT_ALL"] = "Select All";
						en ["SELECT_NONE"] = "Select None";
						en ["NAME"] = "Name";
						en ["STOPS"] = "Stops";
						en ["PASSENGERS"] = "Passengers";
						en ["TRIPS_SAVED"] = "Trips Saved";
						en ["VEHICLES"] = "Vehicles";
						en ["DELETE_LINE"] = "Delete Line";
						en ["NONE"] = "(None)";
						en ["BUS_LINES"] = "Bus Lines ({0})";
						en ["METRO_LINES"] = "Metro Lines ({0})";
						en ["TRAIN_LINES"] = "Train Lines ({0})";
						return en [index];

					}
				} catch (Exception e) {
//					Debug.Error ("EPTUI Language File Error " + e.ToString ());
					DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, "EPTUI Language File Error " + e.ToString ());
					return "Error";
				}
			
			} else {
				if (GameLanguage == "nl") {
					return nl [index];
				}
				if (GameLanguage == "it") {
					return it [index];
				}
				if (GameLanguage == "de") {
					return de [index];
				}
				return en [index];
			}
		}
	}
}
