namespace CoopGame.Shared.Networking;

using System;
using System.Collections.Generic;
using System.IO;

using CoopGame.Shared.Networking.Messages;

public static class MessageSerializer {
    // Maps the message type to respective class
    private static readonly Dictionary<MessageType, Type> typeMap = new();
    private static readonly Dictionary<Type, MessageType> typeReverseMap = new();

    public static void register<T>(MessageType type) where T : IMessage {
        typeMap[type] = typeof(T);
        typeReverseMap[typeof(T)] = type;
    }

    // Serialize and Send
    public static void send(Stream stream, IMessage message) {
        if (!typeReverseMap.TryGetValue(message.GetType(), out var type)) {
            throw new Exception("Message type not registered");
        }

        using (BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true)) {
            writer.Write((byte)type);

            switch (message) {
                case ClientJoinMessage join:
                    writer.Write(join.playerName);
                    break;
                case ServerWelcomeMessage welcome:
                    writer.Write(welcome.welcomeText);
                    break;
                default:
                    throw new Exception("Unknown message type");
            }
        }
    }

    public static IMessage read(Stream stream) {
        using (BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true)) {
            byte typeByte = reader.ReadByte();
            MessageType type = (MessageType)typeByte;

            switch (type) {
                case MessageType.ClientJoin:
                    return new ClientJoinMessage {
                        playerName = reader.ReadString()
                    };
                case MessageType.ServerWelcome:
                    return new ServerWelcomeMessage {
                        welcomeText = reader.ReadString()
                    };
                default:
                    throw new Exception($"Unknown message type: {type}");
            }
        }
    }
}
