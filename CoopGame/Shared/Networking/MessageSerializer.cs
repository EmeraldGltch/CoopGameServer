using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using CoopGame.Shared.Networking.Messages;

namespace CoopGame.Shared.Networking;

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
            throw new Exception("Message type not registered: {message.GetType().Name}");
        }

        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        // Write the message type
        writer.Write((byte)type);

        // Dynamic registry (Even adds modded messages!)
        foreach (var prop in message.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
            writeValue(writer, prop.GetValue(message));
        }

        foreach (var field in message.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
            writeValue(writer, field.GetValue(message));
        }
    }

    public static IMessage read(Stream stream) {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        byte typeByte = reader.ReadByte();
        MessageType type = (MessageType)typeByte;

        if (!typeMap.TryGetValue(type, out var messageType)) {
            throw new Exception($"Unknown message type: {type}");
        }

        IMessage message = (IMessage)Activator.CreateInstance(messageType);

        // Dynamic registry (Even adds modded messages!)
        foreach (var prop in messageType.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
            prop.SetValue(message, readValue(reader, prop.PropertyType));
        }

        foreach (var field in messageType.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
            field.SetValue(message, readValue(reader, field.FieldType));
        }

        return message;
    }

    //////////////////////
    // Helper Functions //
    //////////////////////

    private static void writeValue(BinaryWriter writer, object value) {
        switch (value) {
            case string s:
                writer.Write(s);
                break;
            case int i:
                writer.Write(i);
                break;
            case float f:
                writer.Write(f);
                break;
            case double d:
                writer.Write(d);
                break;
            case bool b:
                writer.Write(b);
                break;
            case int[] arr:
                writer.Write(arr.Length);

                foreach (var v in arr) {
                    writer.Write(v);
                }

                break;
            case float[] arr:
                writer.Write(arr.Length);

                foreach (var v in arr) {
                    writer.Write(v);
                }

                break;
            default:
                throw new Exception($"Unsupported type in message serialization: {value?.GetType().Name}");
        }
    }

    private static object readValue(BinaryReader reader, Type type) {
        if (type == typeof(string)) {
            return reader.ReadString();
        } else if (type == typeof(int)) {
            return reader.ReadInt32();
        } else if (type == typeof(float)) {
            return reader.ReadSingle();
        } else if (type == typeof(double)) {
            return reader.ReadDouble();
        } else if (type == typeof(bool)) {
            return reader.ReadBoolean();
        } else if (type == typeof(int[])) {
            int length = reader.ReadInt32();
            int[] arr = new int[length];

            for(int i = 0; i < length; i++) {
                arr[i] = reader.ReadInt32();
            }

            return arr;
        } else if (type == typeof(float[])) {
            int length = reader.ReadInt32();
            float[] arr = new float[length];

            for (int i = 0; i < length; i++) {
                arr[i] = reader.ReadSingle();
            }

            return arr;
        } else {
            throw new Exception($"Unsupported type in message deserialization: {type.Name}");
        }
    }
}
