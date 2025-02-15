START ../../Server_Project/Server/PacketGenerator/bin/PacketGenerator.exe ../../Server_Project/Server/PacketGenerator/PDL.xml
XCOPY /Y GenPackets.cs "../../Server_Project/Server/DummyClient/Packet"
XCOPY /Y GenPackets.cs "../../Server_Project/Server/Server/Packet"