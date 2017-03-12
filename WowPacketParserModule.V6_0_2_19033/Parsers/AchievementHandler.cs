using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V6_0_2_19033.Parsers
{
    public static class AchievementHandler
    {
        [Parser(Opcode.SMSG_CRITERIA_UPDATE)]
        public static void HandleCriteriaPlayer(Packet packet)
        {
            packet.ReadInt32("Id");
            packet.ReadInt64("Quantity");
            packet.ReadPackedGuid128("Guid");
            packet.ReadInt32("Flags");
            packet.ReadPackedTime("Date");
            packet.ReadTime("TimeFromStart");
            packet.ReadTime("TimeFromCreate");
        }

        public static void ReadCriteriaProgress(Packet packet, params object[] idx)
        {
            packet.ReadInt32("Id", idx);
            packet.ReadUInt64("Quantity", idx);
            packet.ReadPackedGuid128("Player", idx);
            packet.ReadPackedTime("Date", idx);
            packet.ReadTime("TimeFromStart", idx);
            packet.ReadTime("TimeFromCreate", idx);

            packet.ResetBitReader();
            packet.ReadBits("Flags", 4, idx); // some flag... & 1 -> delete
        }

        public static void ReadEarnedAchievement(Packet packet, params object[] idx)
        {
            packet.ReadInt32("Id", idx);
            packet.ReadPackedTime("Date", idx);
            packet.ReadPackedGuid128("Owner", idx);
            packet.ReadInt32("VirtualRealmAddress", idx);
            packet.ReadInt32("NativeRealmAddress", idx);
        }

        public static void ReadAllAchievements(Packet packet, params object[] idx)
        {
            var earnedCount = packet.ReadUInt32("EarnedCount", idx);
            var progressCount = packet.ReadUInt32("ProgressCount", idx);

            for (var i = 0; i < earnedCount; ++i)
                ReadEarnedAchievement(packet, "Earned", i);

            for (var i = 0; i < progressCount; ++i)
                ReadCriteriaProgress(packet, "Progress", i);
        }

        [Parser(Opcode.SMSG_ALL_ACHIEVEMENT_DATA)]
        public static void HandleAllAchievementDataPlayer(Packet packet)
        {
            ReadAllAchievements(packet, "Data");
        }

        [Parser(Opcode.SMSG_RESPOND_INSPECT_ACHIEVEMENTS)]
        public static void HandleRespondInspectAchievements(Packet packet)
        {
            packet.ReadPackedGuid128("Player");
            ReadAllAchievements(packet, "Data");
        }
    }
}
