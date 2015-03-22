using ColossalFramework;
using System.Collections.Generic;
using UnityEngine;

namespace EPTUI
{
    public static class TransportUtil
    {
        private static readonly TransportManager Tm = Singleton<TransportManager>.instance;

        public static bool IsTransportLineHidden(ushort lineID)
        {
            if ((Tm.m_lines.m_buffer[lineID].m_flags & TransportLine.Flags.Hidden) != TransportLine.Flags.None)
                return true;
            return false;
        }

        public static void HideTransportLine(ushort lineID)
        {
            Tm.m_lines.m_buffer[lineID].m_flags |= TransportLine.Flags.Hidden;
			Tm.m_lines.m_buffer[lineID].m_flags &= ~TransportLine.Flags.Created;
        }
        public static void ShowTransportLine(ushort lineID)
        {
            Tm.m_lines.m_buffer[lineID].m_flags &= ~TransportLine.Flags.Hidden;
			Tm.m_lines.m_buffer[lineID].m_flags |= TransportLine.Flags.Created;
        }

        public static void SelectTransportLine(ushort lineID)
        {
            Tm.m_lines.m_buffer[lineID].m_flags |= TransportLine.Flags.Selected;
        }

        public static void DeselectTransportLine(ushort lineID)
        {
            Tm.m_lines.m_buffer[lineID].m_flags &= ~TransportLine.Flags.Selected;
        }
        public static TransportInfo.TransportType GetTransportLineType(ushort lineID)
        {
            return Tm.m_lines.m_buffer[lineID].Info.m_transportType;
        }

        public static string GetLineName(ushort lineID)
        {
            return Tm.GetLineName(lineID);
        }

        public static Color GetLineColor(ushort lineID)
        {
            return Tm.GetLineColor(lineID);
        }

        public static uint GetStopCount(ushort lineID)
        {
            return (uint)Tm.m_lines.m_buffer[lineID].CountStops(lineID);
        }

        public static uint GetTouristPassengerCount(ushort lineID)
        {
            return Tm.m_lines.m_buffer[lineID].m_passengers.m_touristPassengers.m_averageCount;
        }

        public static uint GetResidentPassengerCount(ushort lineID)
        {
            return Tm.m_lines.m_buffer[lineID].m_passengers.m_residentPassengers.m_averageCount;
        }

        public static uint GetVehicleCount(ushort lineID)
        {
            return (uint)Tm.m_lines.m_buffer[lineID].CountVehicles(lineID);
        }

        public static uint GetTripsSaved(ushort lineID)
        {
            // formula lifted straight from decompiled source of PublicTransportWorldInfoPanel
            // just slightly deobfuscated
            var residents = GetResidentPassengerCount(lineID);
            var tourists = GetTouristPassengerCount(lineID);
            var teens = Tm.m_lines.m_buffer[lineID].m_passengers.m_teenPassengers.m_averageCount;
            var young = Tm.m_lines.m_buffer[lineID].m_passengers.m_youngPassengers.m_averageCount;
            var adult = Tm.m_lines.m_buffer[lineID].m_passengers.m_adultPassengers.m_averageCount;
            var senior = Tm.m_lines.m_buffer[lineID].m_passengers.m_seniorPassengers.m_averageCount;
            var carOwners = Tm.m_lines.m_buffer[lineID].m_passengers.m_carOwningPassengers.m_averageCount;

            uint result = 0;
            if (residents + tourists != 0)
            {
                result = teens*5 +
                         young*((15*residents + 20*tourists + ((residents + tourists)/2))/(residents + tourists)) +
                         adult*((20*residents + 20*tourists + ((residents + tourists)/2))/(residents + tourists)) +
                         senior*((10*residents + 20*tourists + ((residents + tourists)/2))/(residents + tourists));
            }

            int tripsSaved = 0;
            if (result != 0)
            {
                var tmp = (carOwners*10000L + result/2) / result;
                tripsSaved = Mathf.Clamp((int)tmp, 0, 100);
            }

            return (uint)tripsSaved;
        }

        public static int GetTransportLineCount()
        {
            return Tm.m_lineCount;
        }

        public static Vector3 GetFirstLineStop(ushort lineID)
        {
            var segments = Tm.m_lineSegments[lineID];
            return segments[0].m_bounds.center;
        }

        public static HashSet<ushort> GetUsedTransportLineIndices()
        {
            // Hack - Since it seems there is no way to get a List of
            // the used indices in an Array16 we need to check the whole
            // buffer and save them ourselves.
            var indices = new HashSet<ushort>();
            for (ushort i = 0; i < Tm.m_lines.m_size; ++i)
            {
                // Get only completed transport lines
                var flags = Tm.m_lines.m_buffer[i].m_flags;

                if ((flags & TransportLine.Flags.Complete) != TransportLine.Flags.None &&
                    (flags & TransportLine.Flags.Temporary) == TransportLine.Flags.None)
                {
                    indices.Add(i);
                }
            }
            return indices;
        }
    }
}